using Rayer.Core.Common;
using Rayer.Core.Framework.Settings.Abstractions;
using Rayer.Core.Models;
using System.Windows.Input;
using Wpf.Ui.Appearance;

namespace Rayer.Core.Framework.Settings;

internal class ISettingsJsonConverter : SettingsJsonConverter<ISettings>
{

}

internal class SettingsJsonConverter : SettingsJsonConverter<Impl.Settings>
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
                    case nameof(settings.PitchProvider):
                        settings.PitchProvider = (PitchProvider)reader.GetInt32();
                        break;
                    case nameof(settings.LyricSearcher):
                        settings.LyricSearcher = (LyricSearcher)reader.GetInt32();
                        break;
                    case nameof(settings.DefaultSearcher):
                        settings.DefaultSearcher = (SearcherType)reader.GetInt32();
                        break;
                    case nameof(settings.Volume):
                        settings.Volume = (float)reader.GetDouble();
                        break;
                    case nameof(settings.Pitch):
                        settings.Pitch = (float)reader.GetDouble();
                        break;
                    case nameof(settings.Speed):
                        settings.Speed = (float)reader.GetDouble();
                        break;
                    case nameof(settings.PlaybackRecord):
                        settings.PlaybackRecord = ReadPlaybackRecord(ref reader);
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
                    default:
                        {
                            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray && reader.TokenType != JsonTokenType.EndObject)
                            {

                            }
                        }

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
        writer.WriteNumber(nameof(value.PitchProvider), Convert.ToDecimal(value.PitchProvider));
        writer.WriteNumber(nameof(value.LyricSearcher), Convert.ToDecimal(value.LyricSearcher));
        writer.WriteNumber(nameof(value.DefaultSearcher), Convert.ToDecimal(value.DefaultSearcher));
        writer.WriteNumber(nameof(value.Volume), Convert.ToDecimal(value.Volume));
        writer.WriteNumber(nameof(value.Pitch), Convert.ToDecimal(value.Pitch));
        writer.WriteNumber(nameof(value.Speed), Convert.ToDecimal(value.Speed));
        writer.WriteStartObject(nameof(value.PlaybackRecord));
        writer.WriteString(nameof(value.PlaybackRecord.Id), value.PlaybackRecord.Id);
        writer.WriteString(nameof(value.PlaybackRecord.Offset), value.PlaybackRecord.Offset.ToString());
        writer.WriteEndObject();

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
                    case nameof(KeyBinding.Key):
                        key = (Key)reader.GetInt32();
                        break;
                    case nameof(KeyBinding.Modifiers):
                        modifiers = (ModifierKeys)reader.GetInt32();
                        break;
                }
            }
        }

        return new KeyBinding() { Key = key, Modifiers = modifiers };
    }

    private static PlaybackRecord ReadPlaybackRecord(ref Utf8JsonReader reader)
    {
        string? id = null;
        TimeSpan offset = TimeSpan.Zero;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new PlaybackRecord() { Id = id, Offset = offset };
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();

                reader.Read();
                switch (propertyName)
                {
                    case nameof(PlaybackRecord.Id):
                        id = reader.GetString();
                        break;
                    case nameof(PlaybackRecord.Offset):
                        offset = TimeSpan.TryParse(reader.GetString(), out var timespan) ? timespan : TimeSpan.Zero;
                        break;
                }
            }
        }

        return new PlaybackRecord() { Id = id, Offset = offset };
    }
}