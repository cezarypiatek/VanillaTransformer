using System;
using System.IO;
using Fclp;
using Fclp.Internals.Extensions;

namespace VanillaTransformer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parser = new FluentCommandLineParser<InputParameters>();
                SetupParser(parser);

                var result = parser.Parse(args);
                if (result.HasErrors == false)
                {
                    IVanillaTransformer vanillaTransformer = new VanillaTransformer(parser.Object);
                    vanillaTransformer.PrepareAndLaunchTransform();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        private static void SetupParser(FluentCommandLineParser<InputParameters> fluentCommandLineParser)
        {
            fluentCommandLineParser.Setup(arg => arg.TransformConfiguration)
                .As('t', "TransformConfiguration");
            fluentCommandLineParser.Setup(arg => arg.PatternFile)
                .As('p', "PatternFile");
            fluentCommandLineParser.Setup(arg => arg.OutputPath)
                .As('o', "OutputPath");
            fluentCommandLineParser.Setup(arg => arg.ValuesSource)
                .As('s', "ValuesSource");
            fluentCommandLineParser.Setup(arg => arg.OutputArchivePath)
                .As('a', "OutputArchivePath");
            fluentCommandLineParser.Setup(arg => arg.ValuesProviderName)
                .As('n', "ValuesProviderName");
            fluentCommandLineParser.Setup(arg => arg.PlaceholderPattern)
                .As('h', "PlaceholderPattern");
            fluentCommandLineParser.Setup(arg => arg.ProjectRootPath)
                .As('r', "ProjectRootPath");
        }

      

    }
}
