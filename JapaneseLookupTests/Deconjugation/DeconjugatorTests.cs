﻿using System;
using System.Text.Json;
using JapaneseLookup.Deconjugation;
using NUnit.Framework;

namespace JapaneseLookupTests.Deconjugation
{
    [TestFixture]
    public class DeconjugatorTests
    {
        private static readonly JsonSerializerOptions Jso = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        [Test]
        public void Deconjugate_わからない()
        {
            // Arrange
            var expected =
                "[{\"Text\":\"わからない\",\"OriginalText\":\"わからない\",\"Tags\":[],\"Seentext\":[],\"Process\":[]},{\"Text\":\"わからなう\",\"OriginalText\":\"わからない\",\"Tags\":[\"stem-ren\",\"v5u\"],\"Seentext\":[\"わからない\",\"わからなう\"],\"Process\":[\"(infinitive)\"]},{\"Text\":\"わからなう\",\"OriginalText\":\"わからない\",\"Tags\":[\"stem-ren\",\"v5u-s\"],\"Seentext\":[\"わからない\",\"わからなう\"],\"Process\":[\"(infinitive)\"]},{\"Text\":\"わからないる\",\"OriginalText\":\"わからない\",\"Tags\":[\"stem-ren\",\"v1\"],\"Seentext\":[\"わからない\",\"わからないる\"],\"Process\":[\"(infinitive)\"]},{\"Text\":\"わからないい\",\"OriginalText\":\"わからない\",\"Tags\":[\"stem-adj-base\",\"adj-i\"],\"Seentext\":[\"わからない\",\"わからないい\"],\"Process\":[\"(stem)\"]},{\"Text\":\"わから\",\"OriginalText\":\"わからない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\"],\"Seentext\":[\"わからない\",\"わから\"],\"Process\":[\"negative\"]},{\"Text\":\"わから\",\"OriginalText\":\"わからない\",\"Tags\":[\"adj-i\",\"stem-ku\"],\"Seentext\":[\"わからない\",\"わから\"],\"Process\":[\"negative\"]},{\"Text\":\"わからない\",\"OriginalText\":\"わからない\",\"Tags\":[\"stem-ren\",\"v1\",\"stem-izenkei\"],\"Seentext\":[\"わからない\",\"わからないる\"],\"Process\":[\"(infinitive)\",\"potential\"]},{\"Text\":\"わからな\",\"OriginalText\":\"わからない\",\"Tags\":[\"stem-ren\",\"v1\",\"stem-te\"],\"Seentext\":[\"わからない\",\"わからないる\",\"わからな\"],\"Process\":[\"(infinitive)\",\"teiru\"]},{\"Text\":\"わからない\",\"OriginalText\":\"わからない\",\"Tags\":[\"stem-ren\",\"v1\",\"stem-te\"],\"Seentext\":[\"わからない\",\"わからないる\"],\"Process\":[\"(infinitive)\",\"teru (teiru)\"]},{\"Text\":\"わから\",\"OriginalText\":\"わからない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\",\"stem-a\"],\"Seentext\":[\"わからない\",\"わから\"],\"Process\":[\"negative\",\"(mizenkei)\"]},{\"Text\":\"わからる\",\"OriginalText\":\"わからない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\",\"v1\"],\"Seentext\":[\"わからない\",\"わから\",\"わからる\"],\"Process\":[\"negative\",\"(mizenkei)\"]},{\"Text\":\"わかる\",\"OriginalText\":\"わからない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\",\"stem-a\",\"v5r\"],\"Seentext\":[\"わからない\",\"わから\",\"わかる\"],\"Process\":[\"negative\",\"(mizenkei)\",\"(\'a\' stem)\"]},{\"Text\":\"わから\",\"OriginalText\":\"わからない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-izenkei\"],\"Seentext\":[\"わからない\",\"わから\",\"わからる\"],\"Process\":[\"negative\",\"(mizenkei)\",\"potential\"]},{\"Text\":\"わから\",\"OriginalText\":\"わからない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-te\"],\"Seentext\":[\"わからない\",\"わから\",\"わからる\"],\"Process\":[\"negative\",\"(mizenkei)\",\"teru (teiru)\"]}]";

            // Act
            var result = Deconjugator.Deconjugate("わからない");
            var actual = JsonSerializer.Serialize(result, Jso);

            // Assert
            StringAssert.AreEqualIgnoringCase(expected, actual);
        }

        [Test]
        public void Deconjugate_このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない()
        {
            // Arrange
            var expected =
                "[{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[],\"Seentext\":[],\"Process\":[]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくなう\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"stem-ren\",\"v5u\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくなう\"],\"Process\":[\"(infinitive)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくなう\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"stem-ren\",\"v5u-s\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくなう\"],\"Process\":[\"(infinitive)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくないる\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"stem-ren\",\"v1\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくないる\"],\"Process\":[\"(infinitive)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくないい\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"stem-adj-base\",\"adj-i\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくないい\"],\"Process\":[\"(stem)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\"],\"Process\":[\"negative\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\"],\"Process\":[\"negative\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"stem-ren\",\"v1\",\"stem-izenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくないる\"],\"Process\":[\"(infinitive)\",\"potential\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくな\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"stem-ren\",\"v1\",\"stem-te\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくないる\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくな\"],\"Process\":[\"(infinitive)\",\"teiru\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"stem-ren\",\"v1\",\"stem-te\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくないる\"],\"Process\":[\"(infinitive)\",\"teru (teiru)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\",\"stem-a\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\"],\"Process\":[\"negative\",\"(mizenkei)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくる\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\",\"v1\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくる\"],\"Process\":[\"negative\",\"(mizenkei)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\"],\"Process\":[\"negative\",\"(adverbial stem)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-izenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくる\"],\"Process\":[\"negative\",\"(mizenkei)\",\"potential\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-te\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくる\"],\"Process\":[\"negative\",\"(mizenkei)\",\"teru (teiru)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"stem-a\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくる\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-izenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\",\"potential\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-te\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\",\"teru (teiru)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"stem-a\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくる\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-izenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\",\"potential\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-te\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\",\"teru (teiru)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"stem-a\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくる\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃない\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃない\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-izenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\",\"potential\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-te\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\",\"teru (teiru)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃ\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃない\",\"このスレってよくなくなくなくなくなくなくなくないじゃ\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃ\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃない\",\"このスレってよくなくなくなくなくなくなくなくないじゃ\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃ\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"stem-a\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃない\",\"このスレってよくなくなくなくなくなくなくなくないじゃ\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃる\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃない\",\"このスレってよくなくなくなくなくなくなくなくないじゃ\",\"このスレってよくなくなくなくなくなくなくなくないじゃる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃ\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-izenkei\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃない\",\"このスレってよくなくなくなくなくなくなくなくないじゃ\",\"このスレってよくなくなくなくなくなくなくなくないじゃる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\",\"potential\"]},{\"Text\":\"このスレってよくなくなくなくなくなくなくなくないじゃ\",\"OriginalText\":\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"Tags\":[\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-ku\",\"adj-i\",\"stem-mizenkei\",\"v1\",\"stem-te\"],\"Seentext\":[\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃなくない\",\"このスレってよくなくなくなくなくなくなくなくないじゃなく\",\"このスレってよくなくなくなくなくなくなくなくないじゃない\",\"このスレってよくなくなくなくなくなくなくなくないじゃ\",\"このスレってよくなくなくなくなくなくなくなくないじゃる\"],\"Process\":[\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(adverbial stem)\",\"negative\",\"(mizenkei)\",\"teru (teiru)\"]}]";

            // Act
            var result = Deconjugator.Deconjugate("このスレってよくなくなくなくなくなくなくなくないじゃなくなくなくなくない");
            var actual = JsonSerializer.Serialize(result, Jso);

            // Assert
            StringAssert.AreEqualIgnoringCase(expected, actual);
        }

        [Test]
        public void Deconjugate_MemoryUsageIsAcceptable100()
        {
            // Arrange
            int iterations = 100;
            double expected = 75000000 + 8000000;
            // Act
            // double start = GC.GetTotalMemory(true);
            double start = GC.GetAllocatedBytesForCurrentThread();

            for (int i = 0; i < iterations; i++)
            {
                Deconjugator.Deconjugate("飽きて");
                Deconjugator.Deconjugate("座り込む");
                Deconjugator.Deconjugate("していられない");
                Deconjugator.Deconjugate("なく");
                Deconjugator.Deconjugate("握って");
                Deconjugator.Deconjugate("開き");
                Deconjugator.Deconjugate("伸ばして");
                Deconjugator.Deconjugate("戻す");
            }

            //var end = GC.GetTotalMemory(true);
            var end = GC.GetAllocatedBytesForCurrentThread();
            var actual = end - start;
            // Assert
            Assert.Less(actual, expected);
            // Assert.AreEqual(expected, actual, 8000000);
        }

        [Test]
        public void Deconjugate_MemoryUsageIsAcceptable1000()
        {
            // Arrange
            int iterations = 1000;
            double expected = 750000000 + 80000000;
            // Act
            // double start = GC.GetTotalMemory(true);
            double start = GC.GetAllocatedBytesForCurrentThread();

            for (int i = 0; i < iterations; i++)
            {
                Deconjugator.Deconjugate("飽きて");
                Deconjugator.Deconjugate("座り込む");
                Deconjugator.Deconjugate("していられない");
                Deconjugator.Deconjugate("なく");
                Deconjugator.Deconjugate("握って");
                Deconjugator.Deconjugate("開き");
                Deconjugator.Deconjugate("伸ばして");
                Deconjugator.Deconjugate("戻す");
            }

            //var end = GC.GetTotalMemory(true);
            var end = GC.GetAllocatedBytesForCurrentThread();
            var actual = end - start;
            // Assert
            Assert.Less(actual, expected);
            // Assert.AreEqual(expected, actual, 80000000);
        }

        [Test]
        public void Deconjugate_MemoryUsageIsAcceptable10000()
        {
            // Arrange
            int iterations = 10000;
            double expected = 7500000000 + 800000000;
            // Act
            // double start = GC.GetTotalMemory(true);
            double start = GC.GetAllocatedBytesForCurrentThread();

            for (int i = 0; i < iterations; i++)
            {
                Deconjugator.Deconjugate("飽きて");
                Deconjugator.Deconjugate("座り込む");
                Deconjugator.Deconjugate("していられない");
                Deconjugator.Deconjugate("なく");
                Deconjugator.Deconjugate("握って");
                Deconjugator.Deconjugate("開き");
                Deconjugator.Deconjugate("伸ばして");
                Deconjugator.Deconjugate("戻す");
            }

            //var end = GC.GetTotalMemory(true);
            var end = GC.GetAllocatedBytesForCurrentThread();
            var actual = end - start;
            // Assert
            Assert.Less(actual, expected);
            // Assert.AreEqual(expected, actual, 800000000);
        }

        [Test, Explicit]
        public void Deconjugate_MemoryUsageIsAcceptable100000()
        {
            // Arrange
            int iterations = 100000;
            double expected = 75000000000 + 8000000000;
            // Act
            // double start = GC.GetTotalMemory(true);
            double start = GC.GetAllocatedBytesForCurrentThread();

            for (int i = 0; i < iterations; i++)
            {
                Deconjugator.Deconjugate("飽きて");
                Deconjugator.Deconjugate("座り込む");
                Deconjugator.Deconjugate("していられない");
                Deconjugator.Deconjugate("なく");
                Deconjugator.Deconjugate("握って");
                Deconjugator.Deconjugate("開き");
                Deconjugator.Deconjugate("伸ばして");
                Deconjugator.Deconjugate("戻す");
            }

            //var end = GC.GetTotalMemory(true);
            var end = GC.GetAllocatedBytesForCurrentThread();
            var actual = end - start;
            // Assert
            Assert.Less(actual, expected);
            // Assert.AreEqual(expected, actual, 8000000000);
        }
    }
}