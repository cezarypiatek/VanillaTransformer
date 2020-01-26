using System;
using System.Collections.Generic;
using System.IO;
using Fclp;
using VanillaTransformer.Core;

namespace VanillaTransformer.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var parser = SetupParser();
                var transformerParameters = parser.Parse(args);
                if (transformerParameters != null)
                {
                    var vanillaTransformer = new Core.VanillaTransformer(transformerParameters);
                    var results = vanillaTransformer.LaunchTransformations();
                    results.PrintDescription(s =>
                    {
                        var regularColor = System.Console.ForegroundColor;
                        System.Console.ForegroundColor = ConsoleColor.Green;
                        System.Console.Write("\u2713 ");
                        System.Console.ForegroundColor = regularColor;
                        System.Console.WriteLine(s);
                    }, s =>
                    {
                        var regularColor = System.Console.ForegroundColor;
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        System.Console.Error.Write("\u2717 ");
                        System.Console.ForegroundColor = regularColor;
                        System.Console.Error.WriteLine(s);
                        System.Console.Error.Flush();
                    }, Directory.GetCurrentDirectory());

                    if (results.Success)
                    {
                        return 0;
                    }

                    System.Console.Error.WriteLine("There were some issues during the config transformation. Please examine the log above to get more details.");
                    return -1;
                }
            }
            catch (Exception exception)
            {
                var processedException = exception;
                while (processedException != null)
                {
                    System.Console.Error.WriteLine($"ERROR: {processedException.Message}");
                    processedException = processedException.InnerException;
                }

                return -1;
            }
            return 0;
        }

        private static MultiGroupParser SetupParser()
        {
            return new MultiGroupParser(new List<FluentCommandLineParser<InputParameters>>()
            {
                SetupSingleFileParser(),
                SetupConfigFileParser()
            });
        }

        private static FluentCommandLineParser<InputParameters> SetupConfigFileParser()
        {
            var parser = new FluentCommandLineParser<InputParameters>();
            parser.Setup(arg => arg.TransformConfiguration)
                .As('t', "TransformConfiguration")
                .WithDescription("Path to a file with transform configuration")
                .Required();

            parser.Setup(arg => arg.TransformConfigurationFormat)
                .As('f', "ConfigurationFormat")
                .WithDescription("Format of the transform configuration file. Available options: 'deployment', 'default'");

            SetupCommonOptions(parser);
            return parser;
        }

        private static FluentCommandLineParser<InputParameters> SetupSingleFileParser()
        {
            var parser = new FluentCommandLineParser<InputParameters>();
            parser.Setup(arg => arg.PatternFile)
                .As('p', "PatternFile")
                .WithDescription("Path to a file with configuration pattern")
                .Required();

            parser.Setup(arg => arg.OutputPath)
                .As('o', "OutputPath")
                .WithDescription("Path to a output file")
                .Required();

            parser.Setup(arg => arg.ValuesSource)
                .As('s', "ValuesSource")
                .WithDescription("Path to a file with values required by the pattern file")
                .Required();

            parser.Setup(arg => arg.OutputArchivePath)
                .As('a', "OutputArchivePath")
                .WithDescription("Path to output archive");

            parser.Setup(arg => arg.ValuesProviderName)
                .As('n', "ValuesProviderName")
                .WithDescription("Name of the value provider");
           
            SetupCommonOptions(parser);
            return parser;
        }

        private static void SetupCommonOptions(FluentCommandLineParser<InputParameters> parser)
        {
            parser.Setup(arg => arg.ProjectRootPath)
                .As('r', "ProjectRootPath")
                .WithDescription("If not set, the path of the executable will be taken");

            parser.Setup(arg => arg.PlaceholderPattern)
                .As('h', "PlaceholderPattern")
                .WithDescription("String that define placeholders format. Every placeholder must contain KEY token. Example \"${KEY}\"");
        }
    }
}

