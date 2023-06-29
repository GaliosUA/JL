using System.Xml;
using JL.Core.Utilities;

namespace JL.Core.Dicts.EDICT.KANJIDIC;

internal static class KanjidicLoader
{
    public static async Task Load(Dict dict)
    {
        if (File.Exists(dict.Path))
        {
            XmlReaderSettings xmlReaderSettings = new()
            {
                Async = true,
                DtdProcessing = DtdProcessing.Parse,
                IgnoreWhitespace = true
            };

            using (XmlReader xmlReader = XmlReader.Create(dict.Path, xmlReaderSettings))
            {
                while (xmlReader.ReadToFollowing("literal"))
                {
                    await ReadCharacter(xmlReader, dict).ConfigureAwait(false);
                }
            }

            dict.Contents.TrimExcess();
        }

        else if (Utils.Frontend.ShowYesNoDialog(
                     "Couldn't find kanjidic2.xml. Would you like to download it now?",
                     "Download KANJIDIC2?"))
        {
            bool downloaded = await ResourceUpdater.UpdateResource(dict.Path,
                DictUtils.s_kanjidicUrl,
                DictType.Kanjidic.ToString(), false, false).ConfigureAwait(false);

            if (downloaded)
            {
                await Load(dict).ConfigureAwait(false);
            }
        }

        else
        {
            dict.Active = false;
        }
    }

    private static async Task ReadCharacter(XmlReader xmlReader, Dict dict)
    {
        string key = await xmlReader.ReadElementContentAsStringAsync().ConfigureAwait(false);

        KanjidicRecord entry = new();

        while (!xmlReader.EOF)
        {
            if (xmlReader is { Name: "character", NodeType: XmlNodeType.EndElement })
            {
                break;
            }

            if (xmlReader.NodeType is XmlNodeType.Element)
            {
                switch (xmlReader.Name)
                {
                    case "grade":
                        entry.Grade = xmlReader.ReadElementContentAsInt();
                        break;

                    case "stroke_count":
                        entry.StrokeCount = xmlReader.ReadElementContentAsInt();
                        break;

                    case "freq":
                        entry.Frequency = xmlReader.ReadElementContentAsInt();
                        break;

                    case "meaning":
                        // English definition
                        if (!xmlReader.HasAttributes)
                        {
                            entry.Definitions!.Add(await xmlReader.ReadElementContentAsStringAsync().ConfigureAwait(false));
                        }
                        else
                        {
                            _ = await xmlReader.ReadAsync().ConfigureAwait(false);
                        }

                        break;

                    case "nanori":
                        entry.NanoriReadings!.Add(await xmlReader.ReadElementContentAsStringAsync().ConfigureAwait(false));
                        break;

                    case "reading":
                        switch (xmlReader.GetAttribute("r_type"))
                        {
                            case "ja_on":
                                entry.OnReadings!.Add(await xmlReader.ReadElementContentAsStringAsync().ConfigureAwait(false));
                                break;

                            case "ja_kun":
                                entry.KunReadings!.Add(await xmlReader.ReadElementContentAsStringAsync().ConfigureAwait(false));
                                break;

                            default:
                                _ = await xmlReader.ReadAsync().ConfigureAwait(false);
                                break;
                        }

                        break;

                    default:
                        _ = await xmlReader.ReadAsync().ConfigureAwait(false);
                        break;
                }
            }

            else
            {
                _ = await xmlReader.ReadAsync().ConfigureAwait(false);
            }
        }

        entry.NanoriReadings = Utils.TrimStringList(entry.NanoriReadings!);
        entry.Definitions = Utils.TrimStringList(entry.Definitions!);
        entry.OnReadings = Utils.TrimStringList(entry.OnReadings!);
        entry.KunReadings = Utils.TrimStringList(entry.KunReadings!);

        dict.Contents.Add(key, new List<IDictRecord> { entry });
    }
}
