using PngifyMe.Services.Secrets;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace PngifyMe.Services.Twitch
{
    public class TwitchApi
    {
        public TwitchAPI Api { get; private set; }
        public string FilePath { get; }
        public string Redirect { get; } = "http://localhost:9797/";
        public string UserId { get; private set; }
        public string UserName { get; private set; }

        //https://dev.twitch.tv/docs/pubsub/#introduction
        private static List<string> scopes = new List<string>
        {
            "chat:read",
            "user:read:chat",
            "user:bot",
            "channel:bot",
            "user:read:follows",
            "moderator:read:followers",
            "channel:read:subscriptions",
            "bits:read",
            "channel:read:redemptions",
            "channel:read:predictions",
            "channel:read:hype_train",
        };
        public TwitchAuth? Auth { get; private set; }

        public TwitchApi()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder) + "\\PngifyMe";
            Directory.CreateDirectory(path);
            FilePath = Path.Join(path, "twitch.json");
        }

        public async Task Connect()
        {
            Api = new TwitchAPI();
            Api.Settings.ClientId = SecretsManager.TwitchClientId;
            LoadAuth();
            if (Auth == null || DateTime.Now > Auth.Expiration)
            {
                Auth = new TwitchAuth();

                // open browser to let user verify here
                Process.Start(new ProcessStartInfo
                {
                    FileName = GetAuthorizationCodeUrl(SecretsManager.TwitchClientId, Redirect, scopes),
                    UseShellExecute = true
                });

                string token = await StartHttpListener(Redirect);

                Auth.AccessToken = token;
                Api.Settings.AccessToken = token;
            }

            var validation = await Api.Auth.ValidateAccessTokenAsync();
            Auth.Expiration = DateTime.Now.AddSeconds(validation.ExpiresIn);
            UserId = validation.UserId;
            UserName = validation.Login;
            Log.Debug($"Twitch id: {UserId}, Name: {UserName}");

            Save();
        }

        public TwitchClient ConnectClient()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(UserName, Api.Settings.AccessToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            var client = new TwitchClient(customClient);
            client.Initialize(credentials, UserName);
            client.Connect();
            return client;
        }

        public async Task<TwitchLib.Api.Helix.Models.Chat.Emotes.ChannelEmote[]> GetEmotes()
        {
            var resp = await Api.Helix.Chat.GetChannelEmotesAsync(UserId);
            return resp.ChannelEmotes;
        }

        private void LoadAuth()
        {
            if (!File.Exists(FilePath)) return;
            Auth = JsonSerializer.Deserialize<TwitchAuth>(File.ReadAllText(FilePath));
            Api.Settings.AccessToken = Auth.AccessToken;
        }

        private void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(Auth));
        }

        public void DeleteAuth()
        {
            if (!File.Exists(FilePath)) return;
            File.Delete(FilePath);
        }

        private static string GetAuthorizationCodeUrl(string clientId, string redirectUri, List<string> scopes)
        {
            var scopesStr = string.Join('+', scopes);

            return "https://id.twitch.tv/oauth2/authorize?" +
                   $"client_id={clientId}&" +
                   $"redirect_uri={System.Web.HttpUtility.UrlEncode(redirectUri)}&" +
                   "response_type=token&" +
                   $"scope={scopesStr}";
        }


        private async Task<string> ListenForRedirect(string redirectUri)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(redirectUri); // Set the prefix for localhost:port

            listener.Start();

            Debug.WriteLine($"Listening for redirect on {redirectUri}...");

            // Wait for the incoming connection from the browser (the redirect)
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;

            // Capture the data from the redirected request (this can contain tokens or auth data)
            string responseText = "Authentication successful! You may now close this window.";
            using (HttpListenerResponse response = context.Response)
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }

            // Handle the received data, for example extracting tokens or session info
            string queryParams = request.Url.Query;
            Debug.WriteLine($"Received query parameters: {queryParams}");

            var queryDictionary = System.Web.HttpUtility.ParseQueryString(queryParams);
            string code = queryDictionary.Get("access_token");

            listener.Stop();

            return code;
        }

        private async Task<string> StartHttpListener(string redirectUri)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(redirectUri);
            listener.Start();

            Debug.WriteLine("Listening for redirect...");

            // Wait for the redirect request
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;

            // Serve a simple HTML page to capture the access token
            string responseHtml = $@"
    <html>
    <head>
        <title>Authentication Complete</title>
    </head>
    <body>
        <h1>Authentication Successful</h1>
        <script>
            // Extract the access token from the URL fragment
            var token = window.location.hash.match(/access_token=([^&]*)/)[1];
            
            // Send the token to the C# backend via HTTP request
            fetch('{redirectUri}token', {{
                method: 'POST',
                headers: {{ 'Content-Type': 'application/json' }},
                body: JSON.stringify({{ access_token: token }})
            }}).then(function() {{
                document.body.innerHTML = '<h2>Token Received. You can close this window.</h2>';
            }}).catch(function(error) {{
                console.error('Error:', error);
            }});
        </script>
    </body>
    </html>";

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseHtml);
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();

            // Handle the token POST request
            string token = string.Empty;
            HttpListenerContext tokenContext = await listener.GetContextAsync();
            if (tokenContext.Request.HttpMethod == "POST")
            {
                using (var reader = new StreamReader(tokenContext.Request.InputStream))
                {
                    string json = await reader.ReadToEndAsync();
                    // Extract the token from the JSON data
                    token = json.Split(new string[] { "\"access_token\":\"", "\"" }, StringSplitOptions.None)[1];
                    Debug.WriteLine($"Access Token: {token}");
                }

                // Send a response back to the browser
                tokenContext.Response.StatusCode = (int)HttpStatusCode.OK;
                tokenContext.Response.Close();
            }

            listener.Stop();
            return token;
        }
    }

    public class TwitchAuth
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
