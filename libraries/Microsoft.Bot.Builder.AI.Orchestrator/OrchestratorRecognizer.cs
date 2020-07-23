// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Orchestrator;

namespace Microsoft.Bot.Builder.AI.Orchestrator
{
    public class OrchestratorRecognizer
    {
        public const string ResultProperty = "Result";

        private const float UnknownIntentFilterScore = 0.4F;
        private const string NoneIntent = "None";
        private static Microsoft.Orchestrator.Orchestrator orchestrator = null;
        private static string modelPath = null;
        private string snapshotPath = null;
        private ILabelResolver resolver = null;

        public OrchestratorRecognizer(string modelPath, string snapshotPath)
        {
            if (modelPath == null)
            {
                throw new ArgumentNullException($"Missing `ModelPath` information.");
            }

            if (snapshotPath == null)
            {
                throw new ArgumentNullException($"Missing `SnapshotPath` information.");
            }

            OrchestratorRecognizer.modelPath = modelPath;
            this.snapshotPath = snapshotPath;
            InitializeModel();
        }

        /// <summary>
        /// Returns recognition results.
        /// </summary>
        /// <param name="turnContext">Turn context.</param>
        /// <returns>Analysis of utterance.</returns>
        public RecognizerResult Recognize(ITurnContext turnContext)
        {
            var text = turnContext.Activity.Text ?? string.Empty;
            var recognizerResult = new RecognizerResult()
            {
                Text = text,
                Intents = new Dictionary<string, IntentScore>(),
            };

            if (string.IsNullOrWhiteSpace(text))
            {
                // nothing to recognize, return empty recognizerResult
                return recognizerResult;
            }

            // Score with orchestrator
            var result = resolver.Score(text);
            AddTopScoringIntent(result, ref recognizerResult);

            // Add full recognition result as a 'result' property
            recognizerResult.Properties.Add(ResultProperty, result);

            return recognizerResult;
        }

        private RecognizerResult AddTopScoringIntent(IReadOnlyList<Result> result, ref RecognizerResult recognizerResult)
        {
            var topScoringIntent = result.First().Label.Name;
            var topScore = result.First().Score;

            // if top scoring intent is less than threshold, return None
            if (topScore < UnknownIntentFilterScore)
            {
                recognizerResult.Intents.Add(NoneIntent, new IntentScore() { Score = 1.0 });
            }
            else
            {
                if (!recognizerResult.Intents.ContainsKey(topScoringIntent))
                {
                    recognizerResult.Intents.Add(topScoringIntent, new IntentScore()
                    {
                        Score = result.First().Score
                    });
                }
            }

            return recognizerResult;
        }

        private void InitializeModel()
        {
            if (modelPath == null)
            {
                throw new ArgumentNullException($"Missing `ModelPath` information.");
            }

            if (snapshotPath == null)
            {
                throw new ArgumentNullException($"Missing `ShapshotPath` information.");
            }

            if (orchestrator == null)
            {
                var fullModelPath = Path.GetFullPath(PathUtils.NormalizePath(modelPath));

                // Create Orchestrator 
                try
                {
                    orchestrator = new Microsoft.Orchestrator.Orchestrator(fullModelPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to find or load Model", ex);
                }
            }

            if (resolver == null)
            {
                var fullSnapShotPath = Path.GetFullPath(PathUtils.NormalizePath(snapshotPath));

                // Load the snapshot
                string content = File.ReadAllText(fullSnapShotPath);
                byte[] snapShotByteArray = Encoding.UTF8.GetBytes(content);

                // Load shapshot and create resolver
                resolver = orchestrator.CreateLabelResolver(snapShotByteArray);
            }
        }
    }
}
