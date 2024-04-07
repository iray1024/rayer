using NAudio.Extras;
using Rayer.Core.Abstractions;
using Rayer.Core.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;
using Wpf.Ui.Appearance;

namespace Rayer.Core.Common;

internal class ISettingsJsonConverter : SettingsJsonConverter<ISettings>
{

}

internal class SettingsJsonConverter : SettingsJsonConverter<Settings>
{

}

internal class SettingsJsonConverter<T> : JsonConverter<T>
    where T : ISettings
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var settings = Activator.CreateInstance<T>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return settings;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();

                reader.Read();
                switch (propertyName)
                {
                    case nameof(settings.AudioLibrary):
                        {
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                            {
                                if (reader.GetString() is string lib)
                                {
                                    settings.AudioLibrary.Add(lib);
                                }
                            }
                        }
                        break;
                    case nameof(settings.Theme):
                        settings.Theme = (ApplicationTheme)reader.GetInt32();
                        break;
                    case nameof(settings.PlaySingleAudioStrategy):
                        settings.PlaySingleAudioStrategy = (PlaySingleAudioStrategy)reader.GetInt32();
                        break;
                    case nameof(settings.PlayloopMode):
                        settings.PlayloopMode = (PlayloopMode)reader.GetInt32();
                        break;
                    case nameof(settings.ImmersiveMode):
                        settings.ImmersiveMode = (ImmersiveMode)reader.GetInt32();
                        break;
                    case nameof(settings.EqualizerMode):
                        settings.EqualizerMode = (EqualizerMode)reader.GetInt32();
                        break;
                    case nameof(settings.Volume):
                        settings.Volume = (float)reader.GetDouble();
                        break;
                    case nameof(settings.Pitch):
                        settings.Pitch = (float)reader.GetDouble();
                        break;                    
                    case nameof(settings.KeyPlayOrPause):
                        settings.KeyPlayOrPause = ReadKeyBinding(ref reader);
                        break;
                    case nameof(settings.KeyPrevious):
                        settings.KeyPrevious = ReadKeyBinding(ref reader);
                        break;
                    case nameof(settings.KeyNext):
                        settings.KeyNext = ReadKeyBinding(ref reader);
                        break;
                    case nameof(settings.KeyPitchUp):
                        settings.KeyPitchUp = ReadKeyBinding(ref reader);
                        break;
                    case nameof(settings.KeyPitchDown):
                        settings.KeyPitchDown = ReadKeyBinding(ref reader);
                        break;
                    case nameof(settings.KeyForward):
                        settings.KeyForward = ReadKeyBinding(ref reader);
                        break;
                    case nameof(settings.KeyRewind):
                        settings.KeyRewind = ReadKeyBinding(ref reader);
                        break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteStartArray(nameof(value.AudioLibrary));
        foreach (var item in value.AudioLibrary)
        {
            writer.WriteStringValue(item);
        }
        writer.WriteEndArray();

        writer.WriteNumber(nameof(value.Theme), Convert.ToDecimal(value.Theme));
        writer.WriteNumber(nameof(value.PlaySingleAudioStrategy), Convert.ToDecimal(value.PlaySingleAudioStrategy));
        writer.WriteNumber(nameof(value.PlayloopMode), Convert.ToDecimal(value.PlayloopMode));
        writer.WriteNumber(nameof(value.ImmersiveMode), Convert.ToDecimal(value.ImmersiveMode));
        writer.WriteNumber(nameof(value.EqualizerMode), Convert.ToDecimal(value.EqualizerMode));
        writer.WriteNumber(nameof(value.Volume), Convert.ToDecimal(value.Volume));
        writer.WriteNumber(nameof(value.Pitch), Convert.ToDecimal(value.Pitch));
        
        writer.WriteStartObject(nameof(value.KeyPlayOrPause));
        writer.WriteNumber(nameof(value.KeyPlayOrPause.Modifiers), Convert.ToDecimal(value.KeyPlayOrPause.Modifiers));
        writer.WriteNumber(nameof(value.KeyPlayOrPause.Key), Convert.ToDecimal(value.KeyPlayOrPause.Key));
        writer.WriteEndObject();

        writer.WriteStartObject(nameof(value.KeyPrevious));
        writer.WriteNumber(nameof(value.KeyPrevious.Modifiers), Convert.ToDecimal(value.KeyPrevious.Modifiers));
        writer.WriteNumber(nameof(value.KeyPrevious.Key), Convert.ToDecimal(value.KeyPrevious.Key));
        writer.WriteEndObject();

        writer.WriteStartObject(nameof(value.KeyNext));
        writer.WriteNumber(nameof(value.KeyNext.Modifiers), Convert.ToDecimal(value.KeyNext.Modifiers));
        writer.WriteNumber(nameof(value.KeyNext.Key), Convert.ToDecimal(value.KeyNext.Key));
        writer.WriteEndObject();

        writer.WriteStartObject(nameof(value.KeyPitchUp));
        writer.WriteNumber(nameof(value.KeyPitchUp.Modifiers), Convert.ToDecimal(value.KeyPitchUp.Modifiers));
        writer.WriteNumber(nameof(value.KeyPitchUp.Key), Convert.ToDecimal(value.KeyPitchUp.Key));
        writer.WriteEndObject();

        writer.WriteStartObject(nameof(value.KeyPitchDown));
        writer.WriteNumber(nameof(value.KeyPitchDown.Modifiers), Convert.ToDecimal(value.KeyPitchDown.Modifiers));
        writer.WriteNumber(nameof(value.KeyPitchDown.Key), Convert.ToDecimal(value.KeyPitchDown.Key));
        writer.WriteEndObject();

        writer.WriteStartObject(nameof(value.KeyForward));
        writer.WriteNumber(nameof(value.KeyForward.Modifiers), Convert.ToDecimal(value.KeyForward.Modifiers));
        writer.WriteNumber(nameof(value.KeyForward.Key), Convert.ToDecimal(value.KeyForward.Key));
        writer.WriteEndObject();

        writer.WriteStartObject(nameof(value.KeyRewind));
        writer.WriteNumber(nameof(value.KeyRewind.Modifiers), Convert.ToDecimal(value.KeyRewind.Modifiers));
        writer.WriteNumber(nameof(value.KeyRewind.Key), Convert.ToDecimal(value.KeyRewind.Key));
        writer.WriteEndObject();

        writer.WriteEndObject();
    }

    private static KeyBinding ReadKeyBinding(ref Utf8JsonReader reader)
    {
        var key = Key.None;
        var modifiers = ModifierKeys.None;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new KeyBinding() { Key = key, Modifiers = modifiers };
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();

                reader.Read();
                switch (propertyName)
                {
                    case "Key":
                        key = (Key)reader.GetInt32();
                        break;
                    case "Modifiers":
                        modifiers = (ModifierKeys)reader.GetInt32();
                        break;
                }
            }
        }

        return new KeyBinding() { Key = key, Modifiers = modifiers };
    }
}