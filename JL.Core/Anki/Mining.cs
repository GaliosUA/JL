using JL.Core.Dicts;
using JL.Core.Utilities;

namespace JL.Core.Anki;

public static class Mining
{
    public static async Task<bool> Mine(Dictionary<JLField, string> miningParams)
    {
        string primarySpelling = miningParams[JLField.PrimarySpelling];

        Dictionary<MineType, AnkiConfig>? ankiConfigDict = await AnkiConfig.ReadAnkiConfig().ConfigureAwait(false);

        if (ankiConfigDict is null)
        {
            Storage.Frontend.Alert(AlertLevel.Error, "Please setup mining first in the preferences");
            return false;
        }

        AnkiConfig? ankiConfig;

        DictType? dictType = null;

        if (miningParams.TryGetValue(JLField.DictionaryName, out string? dictionaryName))
        {
            if (Storage.Dicts.TryGetValue(dictionaryName, out Dict? dict))
            {
                dictType = dict.Type;
            }
        }

        if (dictType is null)
        {
            Storage.Frontend.Alert(AlertLevel.Error, $"Mining failed for {primarySpelling}. Cannot find the type of {JLField.DictionaryName.GetDescription()}");
            Utils.Logger.Error("Mining failed for {FoundSpelling}. Cannot find the type of {DictionaryName}", primarySpelling, JLField.DictionaryName.GetDescription());
            return false;
        }


        if (Storage.s_wordDictTypes.Contains(dictType.Value))
        {
            _ = ankiConfigDict.TryGetValue(MineType.Word, out ankiConfig);
        }

        else if (Storage.s_kanjiDictTypes.Contains(dictType.Value))
        {
            _ = ankiConfigDict.TryGetValue(MineType.Kanji, out ankiConfig);
        }

        else if (Storage.s_nameDictTypes.Contains(dictType.Value))
        {
            _ = ankiConfigDict.TryGetValue(MineType.Name, out ankiConfig);
        }

        else
        {
            _ = ankiConfigDict.TryGetValue(MineType.Other, out ankiConfig);
        }

        if (ankiConfig is null)
        {
            Storage.Frontend.Alert(AlertLevel.Error, "Please setup mining first in the preferences");
            return false;
        }

        // idk if this gets the right audio for every word
        string? reading = miningParams.GetValueOrDefault(JLField.Readings)?.Split(",")[0];
        if (string.IsNullOrEmpty(reading))
        {
            reading = primarySpelling;
        }

        Dictionary<string, JLField> userFields = ankiConfig.Fields;
        Dictionary<string, object> fields = ConvertFields(userFields, miningParams);

        byte[]? audioRes = null;
        bool needsAudio = userFields.Values.Any(static jlField => jlField is JLField.Audio);
        if (needsAudio)
        {
            audioRes = await Audio.AudioUtils.GetAudioPrioritizedAudio(primarySpelling, reading).ConfigureAwait(false);
        }

        Dictionary<string, object?>[] audio =
        {
                new()
                {
                    { "data", audioRes },
                    { "filename", $"JL_audio_{reading}_{primarySpelling}.mp3" },
                    { "skipHash", Storage.Jpod101NoAudioMd5Hash },
                    { "fields", FindAudioFields(userFields) }
                }
            };

        Dictionary<string, object> options = new()
            {
                { "allowDuplicate", Storage.Frontend.CoreConfig.AllowDuplicateCards }
            };

        Note note = new(ankiConfig.DeckName, ankiConfig.ModelName, fields, options, ankiConfig.Tags, audio, video: null, picture: null);
        Response? response = await AnkiConnect.AddNoteToDeck(note).ConfigureAwait(false);

        if (response is null)
        {
            Storage.Frontend.Alert(AlertLevel.Error, $"Mining failed for {primarySpelling}");
            Utils.Logger.Error("Mining failed for {FoundSpelling}", primarySpelling);
            return false;
        }

        if (needsAudio && (audioRes is null || Utils.GetMd5String(audioRes) is Storage.Jpod101NoAudioMd5Hash))
        {
            Storage.Frontend.Alert(AlertLevel.Warning, $"Mined {primarySpelling} (no audio)");
            Utils.Logger.Information("Mined {FoundSpelling} (no audio)", primarySpelling);
        }

        else
        {
            Storage.Frontend.Alert(AlertLevel.Success, $"Mined {primarySpelling}");
            Utils.Logger.Information("Mined {FoundSpelling}", primarySpelling);
        }

        if (Storage.Frontend.CoreConfig.ForceSyncAnki)
        {
            await AnkiConnect.Sync().ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    /// Converts JLField,Value pairs to UserField,Value pairs <br/>
    /// JLField is our internal name of a mining field <br/>
    /// Value is the actual content of a mining field (e.g. if the field name is LocalTime, then it should contain the current time) <br/>
    /// UserField is the name of the user's field in Anki (e.g. Expression) <br/>
    /// </summary>
    private static Dictionary<string, object> ConvertFields(Dictionary<string, JLField> userFields,
        IReadOnlyDictionary<JLField, string> miningParams)
    {
        Dictionary<string, object> dict = new();
        foreach ((string key, JLField value) in userFields)
        {
            string? fieldName = miningParams.GetValueOrDefault(value);
            if (!string.IsNullOrEmpty(fieldName))
            {
                dict.Add(key, fieldName);
            }
        }

        return dict;
    }

    private static List<string> FindAudioFields(Dictionary<string, JLField> userFields)
    {
        return userFields.Keys.Where(key => userFields[key] is JLField.Audio).ToList();
    }
}
