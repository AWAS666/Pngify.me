using PngifyMe.Services.Settings;
using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PngifyMe.Layers.Helper
{
    // Custom JsonConverter for BaseLayer and its derived types
    public class BaseLayerJsonConverter : JsonConverter<BaseLayer>
    {
        public override BaseLayer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var jsonObject = doc.RootElement;

                if (!jsonObject.TryGetProperty("$type", out JsonElement typeElement))
                {
                    throw new JsonException("Missing $type discriminator");
                }

                string typeName = typeElement.GetString();
                Type actualType = Type.GetType(typeName);

                if (actualType == null || !typeof(BaseLayer).IsAssignableFrom(actualType))
                {
                    throw new JsonException($"Unknown type '{typeName}' or type does not inherit from BaseLayer.");
                }

                return (BaseLayer)JsonSerializer.Deserialize(jsonObject.GetRawText(), actualType, options);
            }
        }

        public override void Write(Utf8JsonWriter writer, BaseLayer value, JsonSerializerOptions options)
        {
            Type actualType = value.GetType();

            writer.WriteStartObject();

            // Write type discriminator
            writer.WriteString("$type", actualType.Namespace + "." + actualType.Name);

            // Serialize all properties of the actual derived type, including inherited properties
            foreach (PropertyInfo property in actualType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanRead && property.GetCustomAttribute(typeof(JsonIgnoreAttribute)) == null)
                {
                    object propValue = property.GetValue(value);
                    writer.WritePropertyName(property.Name);
                    JsonSerializer.Serialize(writer, propValue, property.PropertyType, options);
                }
            }

            writer.WriteEndObject();
        }
    }

    public class TriggerJsonConverter : JsonConverter<Trigger>
    {
        public override Trigger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var jsonObject = doc.RootElement;

                if (!jsonObject.TryGetProperty("$type", out JsonElement typeElement))
                {
                    throw new JsonException("Missing $type discriminator");
                }

                string typeName = typeElement.GetString();
                Type actualType = Type.GetType(typeName);

                if (actualType == null || !typeof(Trigger).IsAssignableFrom(actualType))
                {
                    throw new JsonException($"Unknown type '{typeName}' or type does not inherit from BaseLayer.");
                }

                return (Trigger)JsonSerializer.Deserialize(jsonObject.GetRawText(), actualType, options);
            }
        }

        public override void Write(Utf8JsonWriter writer, Trigger value, JsonSerializerOptions options)
        {
            Type actualType = value.GetType();

            writer.WriteStartObject();

            // Write type discriminator
            writer.WriteString("$type", actualType.Namespace + "." + actualType.Name);

            // Serialize all properties of the actual derived type, including inherited properties
            foreach (PropertyInfo property in actualType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanRead && property.GetCustomAttribute(typeof(JsonIgnoreAttribute)) == null)
                {
                    object propValue = property.GetValue(value);
                    writer.WritePropertyName(property.Name);
                    JsonSerializer.Serialize(writer, propValue, property.PropertyType, options);
                }
            }

            writer.WriteEndObject();
        }
    }
}
