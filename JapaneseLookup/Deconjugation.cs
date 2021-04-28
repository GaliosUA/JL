﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace JapaneseLookup
{
    // translated from https://github.com/wareya/nazeka/blob/master/background-script.js
    public static class Deconjugation
    {
        private static readonly string File =
            System.IO.File.ReadAllText("../net5.0-windows/Resources/deconjugator_edited_arrays.json");

        private static readonly Rule[] Rules = JsonSerializer.Deserialize<Rule[]>(File);

        internal class Form
        {
            public Form(string text, string originalText, List<string> tags, HashSet<string> seentext,
                List<string> process)
            {
                Text = text;
                OriginalText = originalText;
                Tags = tags;
                Seentext = seentext;
                Process = process;
            }

            public string Text { get; }
            public string OriginalText { get; }
            public List<string> Tags { get; }
            public HashSet<string> Seentext { get; }
            public List<string> Process { get; }
        }

        private class Rule
        {
            public Rule(string type, List<string> decEnd, List<string> conEnd, List<string> decTag,
                List<string> conTag, string detail)
            {
                Type = type;
                DecEnd = decEnd;
                ConEnd = conEnd;
                DecTag = decTag;
                ConTag = conTag;
                Detail = detail;
            }

            [JsonPropertyName("type")] public string Type { get; }
            [JsonPropertyName("dec_end")] public List<string> DecEnd { get; }
            [JsonPropertyName("con_end")] public List<string> ConEnd { get; }
            [JsonPropertyName("dec_tag")] public List<string> DecTag { get; }
            [JsonPropertyName("con_tag")] public List<string> ConTag { get; }
            [JsonPropertyName("detail")] public string Detail { get; }
        }

        private static Form stdrule_deconjugate_inner(Form myForm,
            Rule myRule)
        {
            // tag doesn't match
            if (myForm.Tags.Count > 0 &&
                myForm.Tags[^1] != myRule.ConTag.First())
            {
                // Debug.WriteLine("TAG DIDN'T MATCH; my_form: " + JsonSerializer.Serialize(
                //     my_form, new JsonSerializerOptions
                //     {
                //         Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                //     }));
                // Debug.WriteLine("TAG DIDN'T MATCH; my_rule: " + JsonSerializer.Serialize(
                //     my_rule, new JsonSerializerOptions
                //     {
                //         Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                //     }));
                return null;
            }

            // ending doesn't match
            if (!myForm.Text.EndsWith(myRule.ConEnd.First()))
                return null;

            var newtext =
                myForm.Text.Substring(0, myForm.Text.Length - myRule.ConEnd.First().Length)
                +
                myRule.DecEnd.First();

            // Debug.WriteLine(JsonSerializer.Serialize("newtext: " + newtext, new JsonSerializerOptions
            // {
            //     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            // }));

            var clone = JsonSerializer.Deserialize<Form>(JsonSerializer.Serialize(myForm));
            var newform = new Form(
                newtext,
                myForm.OriginalText,
                clone?.Tags,
                clone?.Seentext,
                clone?.Process
            );

            newform.Process.Add(myRule.Detail);

            if (newform.Tags.Count == 0)
                newform.Tags.Add(myRule.ConTag.First());
            newform.Tags.Add(myRule.DecTag.First());

            if (newform.Seentext.Count == 0)
                newform.Seentext.Add(myForm.Text);
            newform.Seentext.Add(newtext);

            return newform;
        }

        private static HashSet<Form> stdrule_deconjugate(Form myForm,
            Rule myRule)
        {
            // can't deconjugate nothingness
            if (myForm.Text == "")
                return null;
            // deconjugated form too much longer than conjugated form
            if (myForm.Text.Length > myForm.OriginalText.Length + 10)
                return null;
            // impossibly information-dense
            if (myForm.Tags.Count > myForm.OriginalText.Length + 6)
                return null;
            // blank detail mean it can't be the last (first applied, but rightmost) rule
            if (myRule.Detail == "" && myForm.Tags.Count == 0)
                return null;

            var array = myRule.DecEnd;

            if (array.Count == 1)
            {
                var result = stdrule_deconjugate_inner(myForm, myRule);

                // Debug.WriteLine("result: " + JsonSerializer.Serialize(result, new JsonSerializerOptions
                // {
                //     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                // }));

                return result == null ? null : new HashSet<Form> {result};
            }
            else if (array.Count > 1)
            {
                var collection = new HashSet<Form>();

                var maybeDecEnd = myRule.DecEnd[0];
                var maybeConEnd = myRule.ConEnd[0];
                var maybeDecTag = myRule.DecTag[0];
                var maybeConTag = myRule.ConTag[0];

                var length = array.Count;
                for (var i = 0; i < length; i++)
                {
                    maybeDecEnd = myRule.DecEnd.ElementAtOrDefault(i) ?? maybeDecEnd;
                    maybeConEnd = myRule.ConEnd.ElementAtOrDefault(i) ?? maybeConEnd;
                    maybeDecTag = myRule.DecTag.ElementAtOrDefault(i) ?? maybeDecTag;
                    maybeConTag = myRule.ConTag.ElementAtOrDefault(i) ?? maybeConTag;

                    var virtualRule = new Rule
                    (
                        myRule.Type,
                        new List<string> {maybeDecEnd},
                        new List<string> {maybeConEnd},
                        new List<string> {maybeDecTag},
                        new List<string> {maybeConTag},
                        myRule.Detail
                    );
                    // Debug.WriteLine("virtual_rule: " + JsonSerializer.Serialize(virtual_rule, new JsonSerializerOptions
                    // {
                    //     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    // }));

                    // Debug.WriteLine("sending to inner my_form: " + JsonSerializer.Serialize(my_form,
                    //     new JsonSerializerOptions
                    //     {
                    //         Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    //     }));
                    var ret = stdrule_deconjugate_inner(myForm, virtualRule);

                    // Debug.WriteLine("ret: " + JsonSerializer.Serialize(ret, new JsonSerializerOptions
                    // {
                    //     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    // }));

                    if (ret != null) collection.Add(ret);
                }

                return collection;
            }

            return null;
        }

        private static HashSet<Form> rewriterule_deconjugate(Form myForm,
            Rule myRule)
        {
            if (myForm.Text != myRule.ConEnd.First())
                return null;
            return stdrule_deconjugate(myForm, myRule);
        }

        private static HashSet<Form> onlyfinalrule_deconjugate(Form myForm,
            Rule myRule)
        {
            if (myForm.Tags.Count != 0)
                return null;
            return stdrule_deconjugate(myForm, myRule);
        }

        private static HashSet<Form> neverfinalrule_deconjugate(Form myForm,
            Rule myRule)
        {
            if (myForm.Tags.Count == 0)
                return null;
            return stdrule_deconjugate(myForm, myRule);
        }

        private static HashSet<Form> contextrule_deconjugate(Form myForm,
            Rule myRule)
        {
            var result = myRule.Detail switch
            {
                "v1inftrap" => v1inftrap_check(myForm),
                "saspecial" => saspecial_check(myForm, myRule),
                _ => false
            };
            if (!result)
                return null;
            return stdrule_deconjugate(myForm, myRule);
        }

        private static Form substitution_inner(Form myForm,
            Rule myRule)
        {
            if (!myForm.Text.Contains(myRule.ConEnd.First()))
                return null;
            var newtext = new Regex(myRule.ConEnd.First())
                .Replace(myForm.Text, myRule.DecEnd.First());

            var clone = JsonSerializer.Deserialize<Form>(JsonSerializer.Serialize(myForm));
            var newform = new Form(
                newtext,
                myForm.OriginalText,
                clone?.Tags,
                clone?.Seentext,
                clone?.Process
            );

            newform.Process.Add(myRule.Detail);

            if (newform.Seentext.Count == 0)
                newform.Seentext.Add(myForm.Text);
            newform.Seentext.Add(newtext);

            return newform;
        }

        private static HashSet<Form> substitution_deconjugate(Form myForm,
            Rule myRule)
        {
            if (myForm.Process.Count != 0)
                return null;

            // can't deconjugate nothingness
            if (myForm.Text == "")
                return null;

            var array = myRule.DecEnd;

            if (array.Count == 1)
            {
                var result = substitution_inner(myForm, myRule);
                return result == null ? null : new HashSet<Form> {result};
            }
            else if (array.Count > 1)
            {
                var collection = new HashSet<Form>();

                var maybeDecEnd = myRule.DecEnd[0];
                var maybeConEnd = myRule.ConEnd[0];

                var length = array.Count;
                for (var i = 0; i < length; i++)
                {
                    maybeDecEnd = myRule.DecEnd.ElementAtOrDefault(i) ?? maybeDecEnd;
                    maybeConEnd = myRule.ConEnd.ElementAtOrDefault(i) ?? maybeConEnd;

                    var virtualRule = new Rule
                    (
                        myRule.Type,
                        new List<string> {maybeDecEnd},
                        new List<string> {maybeConEnd},
                        null,
                        null,
                        myRule.Detail
                    );

                    var ret = substitution_inner(myForm, virtualRule);
                    if (ret != null) collection.Add(ret);
                }

                return collection;
            }

            return null;
        }

        private static bool v1inftrap_check(Form myForm)
        {
            if (myForm.Tags.Count != 1) return true;
            var myTag = myForm.Tags[0];
            if (myTag == "stem-ren")
                return false;
            return true;
        }

        private static bool saspecial_check(Form myForm,
            Rule myRule)
        {
            if (myForm.Text == "") return false;
            if (!myForm.Text.EndsWith(myRule.ConEnd.First())) return false;
            var baseText = myForm.Text.Substring(0, myForm.Text.Length - myRule.ConEnd.First().Length);
            if (baseText.EndsWith("さ"))
                return false;
            return true;
        }

        internal static HashSet<Form> Deconjugate(string mytext)
        {
            var processed = new HashSet<Form>();
            var novel = new HashSet<Form>();

            var startForm =
                new Form(mytext,
                    mytext,
                    new List<string>(),
                    new HashSet<string>(),
                    new List<string>()
                );
            novel.Add(startForm);

            var myrules = Rules;

            while (novel.Count > 0)
            {
                // foreach (var n in novel)
                // {
                //     Debug.WriteLine("novel: " + JsonSerializer.Serialize(n, new JsonSerializerOptions
                //     {
                //         Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                //     }));
                // }

                var newNovel = new HashSet<Form>();
                foreach (Form form in novel)
                {
                    foreach (Rule rule in myrules)
                    {
                        HashSet<Form> newform = null;

                        // Debug.WriteLine("rule: " + JsonSerializer.Serialize(rule, new JsonSerializerOptions
                        // {
                        //     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                        // }));

                        switch (rule.Type)
                        {
                            case "stdrule":
                                newform = stdrule_deconjugate(form, rule);
                                break;
                            case "rewriterule":
                                newform = rewriterule_deconjugate(form, rule);
                                break;
                            case "onlyfinalrule":
                                newform = onlyfinalrule_deconjugate(form, rule);
                                break;
                            case "neverfinalrule":
                                newform = neverfinalrule_deconjugate(form, rule);
                                break;
                            case "contextrule":
                                newform = contextrule_deconjugate(form, rule);
                                break;
                            case "substitution":
                                newform = substitution_deconjugate(form, rule);
                                break;
                        }

                        if (newform == null || newform.Count == 0) continue;

                        foreach (var myform in newform)
                        {
                            if (myform != null &&
                                !processed.Contains(myform) &&
                                !novel.Contains(myform) &&
                                !newNovel.Contains(myform))
                            {
                                newNovel.Add(myform);
                            }
                        }
                    }
                }

                processed = Union(processed, novel);
                novel = newNovel;
            }

            return processed;
        }

        private static HashSet<Form> Union(HashSet<Form> setA, HashSet<Form> setB)
        {
            foreach (var elem in setB)
                setA.Add(elem);
            return setA;
        }
    }
}