using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Orchestrator;

namespace Microsoft.Bot.Builder.AI.Orchestrator
{
    public class OrchestratorRecognizer
    {
        private const float UnknownIntentFilterScore = 0.4F;
        private const string NoneIntent = "None";
        private static Microsoft.Orchestrator.Orchestrator orchestrator = null;
        private static string modelPath = null;
        private string snapshotPath = null;
        private ILabelResolver resolver = null;
        private bool useCompactEmbeddings = true;

        public OrchestratorRecognizer(string modelPath, string snapshotPath, bool useCompactEmbeddings = true)
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
            this.useCompactEmbeddings = useCompactEmbeddings;
            InitializeModel();
        }

        /// <summary>
        /// Returns recognition results.
        /// </summary>
        /// <param name="turnContext">Turn context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Analysis of utterance.</returns>
        public RecognizerResult Recognize(ITurnContext turnContext, CancellationToken cancellationToken)
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

            InitializeModel();

            // Score with orchestrator
            var result = resolver.Score(text);
            AddTopScoringIntent(result, ref recognizerResult);

            return recognizerResult;
        }

        private RecognizerResult AddTopScoringIntent(IReadOnlyList<Result> result, ref RecognizerResult recognizerResult)
        {
            var topScoringIntent = result.First().label.name;
            var topScore = result.First().score;

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
                        Score = result.First().score
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
                    orchestrator = new Microsoft.Orchestrator.Orchestrator(fullModelPath, useCompactEmbeddings);
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
                resolver = orchestrator.CreateLabelResolver(snapShotByteArray, useCompactEmbeddings);
            }
        }
    }
}
