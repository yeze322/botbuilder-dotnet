using System;
using Microsoft.Bot.Builder.LanguageGeneration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PerfTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // input the Path of meeting assistant
            var maPath = @"D:\projects\Github\meeting_assistant";

            var file = maPath + @"\AssistantCards\cards\actionsCard.test.lg";
            var template = "testCard";
            var time = true;

            var templates = Templates.ParseFile(file);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"run {i} time");
                templates.Evaluate(template);
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();

            for (var i = 0; i < 5; i++)
            {
                templates.Evaluate(template);
            }

            watch.Stop();
            var activity = templates.Evaluate(template);

            var jActivity = JToken.FromObject(activity);
            if (time)
            {
                Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds / 5} ms");
                Console.WriteLine($"Execution card_applyStyle Time: {Evaluator.CardApplyStyleCost} ms");
            }
            else if (!(jActivity is JObject))
            {
                Console.WriteLine(jActivity.ToString());
            }
            else
            {
                Console.WriteLine(JsonConvert.SerializeObject(jActivity, Newtonsoft.Json.Formatting.Indented));
            }
        }
    }
}
