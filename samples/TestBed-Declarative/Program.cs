// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // add luis LU model environment settings
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);

                    // add orchestrator settings file
                    var di = new DirectoryInfo(".");
                    foreach (var file in di.GetFiles($"orchestrator.settings.json", SearchOption.AllDirectories))
                    {
                        var relative = file.FullName.Substring(di.FullName.Length);
                        if (!relative.Contains("bin\\") && !relative.Contains("obj\\"))
                        {
                            config.AddJsonFile(file.FullName, optional: false, reloadOnChange: true);
                        }
                    }
                })
            .UseStartup<Startup>()
            .Build();
    }
}
