namespace PngifyMe.Services.Twitch;

public class CustomPowerUp
{
    public string broadcaster_user_id { get; set; }
    public string broadcaster_user_login { get; set; }
    public string broadcaster_user_name { get; set; }
    public string id { get; set; }
    public string user_id { get; set; }
    public string user_login { get; set; }
    public string user_name { get; set; }
    public string user_input { get; set; }
    public string status { get; set; }
    public CustomPowerUpData custom_power_up { get; set; }
    public string redeemed_at { get; set; }
}

public class CustomPowerUpData
{
    public string id { get; set; }
    public string title { get; set; }
    public int bits { get; set; }
    public string prompt { get; set; }
}