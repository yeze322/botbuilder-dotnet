// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Schema;
using Microsoft.Orchestrator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Builder.AI.Orchestrator
{
    public class OrchestratorRecognizer : Recognizer
    {
        [JsonProperty("$kind")]
        public const string Kind = "Microsoft.OrchestratorRecognizer";

        /// <summary>
        /// Intent name that will be produced by this recognizer if the child recognizers do not have consensus for intents.
        /// </summary>
        private const string ChooseIntent = "ChooseIntent";

        /// <summary>
        /// Property name for candidate intents that meet the ambiguity threshold.
        /// </summary>
        private const string CandidatesCollection = "candidates";

        /// <summary>
        /// Standard none intent that means none of the recognizers recognize the intent.
        /// </summary>
        /// <remarks>
        /// If each recognizer returns no intents or None intents, then this recognizer will return None intent.
        /// </remarks>
        private const string NoneIntent = "None";

        /// <summary>
        /// Gets or sets the full path to the NLR model to use.
        /// </summary>
        /// <value>
        /// Model path.
        /// </value>
        [JsonProperty("modelPath")]
        public StringExpression ModelPath { get; set; }

        /// <summary>
        /// Gets or sets the full path to the snapshot to use.
        /// </summary>
        /// <value>
        /// Snapshot path.
        /// </value>
        [JsonProperty("snapshotPath")]
        public StringExpression SnapshotPath { get; set; }

        /// <summary>
        /// Gets or sets if compact embeddings should be used.
        /// </summary>
        /// <value>
        /// Boolean flag to signal if compact embeddings should be used.
        /// </value>
        [JsonProperty("useCompactEmbeddings")]
        public BoolExpression UseCompactEmbeddings { get; set; } = true;

        /// <summary>
        /// Gets or sets the entity recognizers.
        /// </summary>
        /// <value>
        /// The entity recognizers.
        /// </value>
        [JsonProperty("entityRecognizers")]
        public List<EntityRecognizer> EntityRecognizers { get; set; } = new List<EntityRecognizer>();

        /// <summary>
        /// Gets or sets the disambiguation score threshold.
        /// </summary>
        /// <value>
        /// Recognizer returns ChooseIntent (disambiguation) if other intents are classified within this score of the top scoring intent.
        /// </value>
        [JsonProperty("disambiguationScoreThreshold")]
        public NumberExpression DisambiguationScoreThreshold { get; set; } = 0.05F;

        /// <summary>
        /// Gets or sets detect ambiguous intents.
        /// </summary>
        /// <value>
        /// When true, recognizer will look for ambiguous intents (intents with close recognition scores from top scoring intent).
        /// </value>
        [JsonProperty("detectAmbiguousIntents")]
        public BoolExpression DetectAmbiguousIntents { get; set; } = false;

#pragma warning disable SA1201 // Elements should appear in the correct order
        private static Microsoft.Orchestrator.Orchestrator orchestrator = null;
#pragma warning restore SA1201 // Elements should appear in the correct order
        private ILabelResolver resolver = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestratorRecognizer"/> class.
        /// </summary>
        /// <param name="callerLine">caller line.</param>
        /// <param name="callerPath">caller path.</param>
        [JsonConstructor]
        public OrchestratorRecognizer([CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(callerPath, callerLine)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestratorRecognizer"/> class.
        /// </summary>
        /// <param name="modelPath">Path to model file.</param>
        /// <param name="snapshotPath">Path to snapshot.</param>
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

            ModelPath = modelPath;
            SnapshotPath = snapshotPath;
        }

        /// <summary>
        /// Return results of the call to QnA Maker.
        /// </summary>
        /// <param name="dialogContext">Context object containing information for a single turn of conversation with a user.</param>
        /// <param name="activity">The incoming activity received from the user. The Text property value is used as the query text for QnA Maker.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="telemetryProperties">Additional properties to be logged to telemetry with the LuisResult event.</param>
        /// <param name="telemetryMetrics">Additional metrics to be logged to telemetry with the LuisResult event.</param>
        /// <returns>A <see cref="RecognizerResult"/> containing the QnA Maker result.</returns>
        public override async Task<RecognizerResult> RecognizeAsync(DialogContext dialogContext, Activity activity, CancellationToken cancellationToken, Dictionary<string, string> telemetryProperties = null, Dictionary<string, double> telemetryMetrics = null)
        {
            var text = activity.Text ?? string.Empty;
            var detectAmbiguity = DetectAmbiguousIntents.GetValue(dialogContext.State); 

            var recognizerResult = new RecognizerResult()
            {
                Text = text,
                Intents = new Dictionary<string, IntentScore>(),
            };

            InitializeModel(dialogContext);

            if (resolver != null)
            {
                if (EntityRecognizers != null)
                {
                    // Run entity recognition
                    recognizerResult = await RecognizeEntitiesAsync(dialogContext, activity, recognizerResult).ConfigureAwait(false);
                }

                // Score with orchestrator
                IReadOnlyList<Result> result = resolver.Score(text);

                // Disambiguate if configured
                if (detectAmbiguity == true)
                {
                    double topScore = result.First().score;
                    float thresholdScore = DisambiguationScoreThreshold.GetValue(dialogContext.State);
                    double classifyingScore = Math.Round(topScore, 2) - Math.Round(thresholdScore, 2);
                    IEnumerable<Result> ambiguousIntents = result.Where(item => item.score >= classifyingScore);

                    if (ambiguousIntents.Count() >= 1)
                    {
                        // Add ambiguous intents that meet the threshold as candidates.
                        List<JObject> candidates = new List<JObject>();
                        foreach (Result ambiguousIntent in ambiguousIntents)
                        {
                            dynamic candidate = new JObject();
                            candidate.intent = ambiguousIntent.label.name;
                            candidate.score = ambiguousIntent.score;
                            candidate.closestText = ambiguousIntent.closest_text;
                            candidates.Add(candidate);
                        }

                        recognizerResult.Intents.Add(ChooseIntent, new IntentScore() { Score = 1.0 });
                        recognizerResult.Properties = new Dictionary<string, object>() { { CandidatesCollection, candidates } };
                    } 
                    else
                    {
                        AddTopScoringIntent(result, ref recognizerResult);
                    }
                }
                else
                {
                    AddTopScoringIntent(result, ref recognizerResult);
                }
            }

            // if no match return None intent
            if (!recognizerResult.Intents.Keys.Any())
            {
                recognizerResult.Intents.Add(NoneIntent, new IntentScore() { Score = 1.0 });
            }

            TurnContext tempTurnContext = new TurnContext(dialogContext.Context.Adapter, activity);
            await tempTurnContext.TraceActivityAsync(nameof(OrchestratorRecognizer), JObject.FromObject(recognizerResult), nameof(OrchestratorRecognizer), "Orchestrator Recognition ", cancellationToken).ConfigureAwait(false);

            TrackRecognizerResult(dialogContext, nameof(OrchestratorRecognizer), FillRecognizerResultTelemetryProperties(recognizerResult, telemetryProperties), telemetryMetrics);

            return recognizerResult;
        }

        private RecognizerResult AddTopScoringIntent(IReadOnlyList<Result> result, ref RecognizerResult recognizerResult)
        {
            var topScoringIntent = result.First().label.name;
            if (!recognizerResult.Intents.ContainsKey(topScoringIntent))
            {
                recognizerResult.Intents.Add(topScoringIntent, new IntentScore()
                {
                    Score = 1.0
                });
            }

            return recognizerResult;
        }

        private async Task<RecognizerResult> RecognizeEntitiesAsync(DialogContext dialogContext, Activity activity, RecognizerResult recognizerResult)
        {
            var text = activity.Text ?? string.Empty;
            var entityPool = new List<Entity>();
            if (EntityRecognizers != null)
            {
                // add entities from regexrecgonizer to the entities pool
                var textEntity = new TextEntity(text);
                textEntity.Properties["start"] = 0;
                textEntity.Properties["end"] = text.Length;
                textEntity.Properties["score"] = 1.0;

                entityPool.Add(textEntity);

                // process entities using EntityRecognizerSet
                var entitySet = new EntityRecognizerSet(this.EntityRecognizers);
                var newEntities = await entitySet.RecognizeEntitiesAsync(dialogContext, activity, entityPool).ConfigureAwait(false);
                if (newEntities.Any())
                {
                    entityPool.AddRange(newEntities);
                }

                entityPool.Remove(textEntity);
            }

            // map entityPool of Entity objects => RecognizerResult entity format
            recognizerResult.Entities = new JObject();

            foreach (var entityResult in entityPool)
            {
                // add value
                JToken values;
                if (!recognizerResult.Entities.TryGetValue(entityResult.Type, StringComparison.OrdinalIgnoreCase, out values))
                {
                    values = new JArray();
                    recognizerResult.Entities[entityResult.Type] = values;
                }

                // The Entity type names are not consistent, map everything to camelcase so we can process them cleaner.
                dynamic entity = JObject.FromObject(entityResult);
                ((JArray)values).Add(entity.text);

                // get/create $instance
                JToken instanceRoot;
                if (!recognizerResult.Entities.TryGetValue("$instance", StringComparison.OrdinalIgnoreCase, out instanceRoot))
                {
                    instanceRoot = new JObject();
                    recognizerResult.Entities["$instance"] = instanceRoot;
                }

                // add instanceData
                JToken instanceData;
                if (!((JObject)instanceRoot).TryGetValue(entityResult.Type, StringComparison.OrdinalIgnoreCase, out instanceData))
                {
                    instanceData = new JArray();
                    instanceRoot[entityResult.Type] = instanceData;
                }

                dynamic instance = new JObject();
                instance.startIndex = entity.start;
                instance.endIndex = entity.end;
                instance.score = (double)1.0;
                instance.text = entity.text;
                instance.type = entity.type;
                instance.resolution = entity.resolution;
                ((JArray)instanceData).Add(instance);
            }

            return recognizerResult;
        }

        private void InitializeModel(DialogContext dc)
        {
            if (ModelPath == null)
            {
                throw new ArgumentNullException($"Missing `ModelPath` information.");
            }

            if (SnapshotPath == null)
            {
                throw new ArgumentNullException($"Missing `ShapshotPath` information.");
            }

            var compactEmbeddings = UseCompactEmbeddings.GetValue(dc.State);
            string modelPath = null;
            string snapShotPath = null;

            if (orchestrator == null)
            {
                modelPath = this.ModelPath.GetValue(dc.State);

                modelPath = Path.GetFullPath(PathUtils.NormalizePath(modelPath));

                // Create Orchestrator 
                try
                {
                    orchestrator = new Microsoft.Orchestrator.Orchestrator(modelPath, compactEmbeddings);
                } 
                catch (Exception ex)
                {
                    throw new Exception("Failed to find or load Model", ex);
                }
            }

            if (resolver == null)
            {
                snapShotPath = this.SnapshotPath.GetValue(dc.State);

                snapShotPath = Path.GetFullPath(PathUtils.NormalizePath(snapShotPath));

                // Load the snapshot
                string content = File.ReadAllText(snapShotPath);
                byte[] snapShotByteArray = Encoding.UTF8.GetBytes(content);

                // Load shapshot and create resolver
                resolver = orchestrator.CreateLabelResolver(snapShotByteArray, compactEmbeddings);
            }
        }
    }
}
