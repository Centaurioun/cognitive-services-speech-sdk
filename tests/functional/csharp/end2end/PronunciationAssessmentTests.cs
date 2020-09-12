//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.CognitiveServices.Speech.Tests.EndToEnd.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.CognitiveServices.Speech.Tests.EndToEnd
{
    using System.IO;
    using static AssertHelpers;
    using static SPXTEST;
    using static Config;
    using static SpeechRecognitionTestsHelper;

    [TestClass]
    public class PronunciationAssessmentTests : RecognitionTestBase
    {
        private SpeechRecognitionTestsHelper helper;

        [ClassInitialize]
        public static void TestClassInitialize(TestContext context)
        {
            LoggingTestBaseInit(context);
            BaseClassInit(context);
        }

        [ClassCleanup]
        new public static void TestClassCleanup()
        {
            LoggingTestBaseCleanup();
        }

        [TestInitialize]
        public void Initialize()
        {
            helper = new SpeechRecognitionTestsHelper();
        }

        [TestMethod]
        public void TestPronunciationAssessmentConfig()
        {
            var config = new PronunciationAssessmentConfig("reference", GradingSystem.HundredMark);
            SPXTEST_ISNOTNULL(config);
            SPXTEST_ARE_EQUAL("reference", config.ReferenceText);
            var jo = JObject.Parse(config.ToJson());
            SPXTEST_ARE_EQUAL(config.ReferenceText, jo["referenceText"]);
            SPXTEST_ARE_EQUAL("HundredMark", jo["gradingSystem"]);
            SPXTEST_ARE_EQUAL("Phoneme", jo["granularity"]);
            SPXTEST_ARE_EQUAL("Comprehensive", jo["dimension"]);

            config = new PronunciationAssessmentConfig("reference", GradingSystem.HundredMark, Granularity.Word, true, "id");
            config.ReferenceText = "new reference";
            jo = JObject.Parse(config.ToJson());
            SPXTEST_ARE_EQUAL("new reference", jo["referenceText"]);
            SPXTEST_ARE_EQUAL("HundredMark", jo["gradingSystem"]);
            SPXTEST_ARE_EQUAL("Word", jo["granularity"]);
            SPXTEST_ARE_EQUAL("Comprehensive", jo["dimension"]);
            SPXTEST_ISTRUE(jo["enableMiscue"].ToObject<bool>());
            SPXTEST_ARE_EQUAL("id", jo["scenarioId"]);

            var config2 = PronunciationAssessmentConfig.FromJson(config.ToJson());
            SPXTEST_ISNOTNULL(config2);
            SPXTEST_ARE_EQUAL(config.ToJson(), config2.ToJson());

            SPXTEST_THROWS<ApplicationException>(() =>
            {
                var pronunciationAssessmentConfig = new PronunciationAssessmentConfig("reference", (GradingSystem) 8);
            });

            SPXTEST_THROWS<ApplicationException>(() =>
            {
                var pronunciationAssessmentConfig = new PronunciationAssessmentConfig("reference",
                        GradingSystem.HundredMark, (Granularity) 8);
            });

            SPXTEST_THROWS<ApplicationException>(() =>
            {
                var pronunciationAssessmentConfig = PronunciationAssessmentConfig.FromJson("invalid json");
            });
        }

        [TestMethod]
        public void TestPronunciationAssessmentConfigApplyToRecognizer()
        {
            var audioInput = AudioConfig.FromWavFileInput(AudioUtterancesMap[AudioUtteranceKeys.PRONUNCIATION_ASSESSMENT_GOOD_PRONUNCIATION_CHINESE].FilePath.GetRootRelativePath());
            using (var recognizer = TrackSessionId(new SpeechRecognizer(this.defaultConfig, audioInput)))
            {
                var config = new PronunciationAssessmentConfig("reference");
                config.ApplyTo(recognizer);
                SPXTEST_ARE_EQUAL(config.ToJson(), recognizer.Properties.GetProperty(PropertyId.PronunciationAssessment_Params));
            }
        }

        [TestMethod]
        // [DataRow(true)] enable this when pronunciation assessment deployed to north europe
        [DataRow(false)]
        public async Task TestPronunciationAssessment(bool useReferenceText)
        {
            var audioInput = AudioConfig.FromWavFileInput(AudioUtterancesMap[AudioUtteranceKeys.PRONUNCIATION_ASSESSMENT_GOOD_PRONUNCIATION_CHINESE].FilePath.GetRootRelativePath());
            var pronunciationAssessmentConfig = new PronunciationAssessmentConfig(
                useReferenceText ? AudioUtterancesMap[AudioUtteranceKeys.PRONUNCIATION_ASSESSMENT_GOOD_PRONUNCIATION_CHINESE].Utterances["zh-CN"][0].Text : string.Empty,
                GradingSystem.HundredMark, Granularity.Phoneme);

            var speechConfig = this.defaultConfig;
            speechConfig.SpeechRecognitionLanguage = "zh-CN";

            using (var recognizer = TrackSessionId(new SpeechRecognizer(speechConfig, audioInput)))
            {
                pronunciationAssessmentConfig.ApplyTo(recognizer);
                SPXTEST_ARE_EQUAL(pronunciationAssessmentConfig.ToJson(),
                    recognizer.Properties.GetProperty(PropertyId.PronunciationAssessment_Params));
                var result = await helper.CompleteRecognizeOnceAsync(recognizer).ConfigureAwait(false);
                Assert.IsTrue(LevenshteinRatio(
                    AudioUtterancesMap[AudioUtteranceKeys.PRONUNCIATION_ASSESSMENT_GOOD_PRONUNCIATION_CHINESE]
                        .Utterances["zh-CN"][0].Text, result.Text) >= TestData.Levenshtein.PronunciationSimilarityScoreThreshold);
                var pronResult = PronunciationAssessmentResult.FromResult(result);

                SPXTEST_ISTRUE(pronResult.AccuracyScore > 0);
                SPXTEST_ISTRUE(pronResult.PronunciationScore > 0);
                SPXTEST_ISTRUE(pronResult.CompletenessScore > 0);
                SPXTEST_ISTRUE(pronResult.FluencyScore > 0);

                SPXTEST_ISNOTNULL(pronResult.Words);
                SPXTEST_ISTRUE(pronResult.Words.First().AccuracyScore > 0);
                SPXTEST_ISNOTNULL(pronResult.Words.First().Phonemes);
                SPXTEST_ISTRUE(pronResult.Words.First().Phonemes.First().AccuracyScore > 0);
            }
        }
    }
}