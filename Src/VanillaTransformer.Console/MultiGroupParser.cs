using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fclp;
using Fclp.Internals;
using VanillaTransformer.Core;

namespace VanillaTransformer.Console
{
    internal class MultiGroupParser
    {
        private readonly IList<FluentCommandLineParser<InputParameters>> parsers;

        public MultiGroupParser(IList<FluentCommandLineParser<InputParameters>> parsers)
        {
            this.parsers = parsers;
            foreach (var parser in parsers)
            {
                parser.SetupHelp("?", "help")
                    .Callback(text => PrintCompleteHelp());
            }
        }

        private void PrintCompleteHelp()
        {
            if (helpWasFired)
            {
                return;
            }
            System.Console.WriteLine($"=============== VanillaTransformer {Assembly.GetEntryAssembly().GetName().Version} HELP ===============");
            for (int i = 0; i < this.parsers.Count; i++)
            {
                if (this.parsers[i] is var parser && parser.HelpOption is HelpCommandLineOption help)
                {
                    System.Console.WriteLine($"=============== Parameter Group {i + 1} ===============");
                    var formattedHelp = FormattedHelp(help, parser);
                    System.Console.WriteLine(formattedHelp);
                }
            }

            this.helpWasFired = true;
        }

        private static string FormattedHelp(HelpCommandLineOption helpCommandLineOption, FluentCommandLineParser<InputParameters> fluentCommandLineParser)
        {
            var helpText = helpCommandLineOption.OptionFormatter.Format(fluentCommandLineParser.Options).Trim();
            var helpLines = helpText.Split('\n').Select(x =>
            {
                var parts = x.Split(':');
                return $"  -{parts[0].Trim()}  --{parts[1].TrimStart()}";
            });
            return string.Join("\n", helpLines);
        }

        private bool helpWasFired = false;

        public InputParameters Parse(string[] args)
        {
            this.helpWasFired = false;
            var results =  parsers.Select(x => new {parser = x, result = x.Parse(args)})
                .OrderByDescending(x => x.result.UnMatchedOptions.Count()).ToList();

            if (results.Any(x=>x.result.HelpCalled))
            {
                return null;
            }

            var succeeded = results.FirstOrDefault(x => x.result.HasErrors == false);
            if (succeeded != null)
            {
                return succeeded.parser.Object;
            }

            var firstAlmostMatched = results.FirstOrDefault(x=>x.result.UnMatchedOptions.Count() != x.parser.Options.Count());
            throw new Exception(firstAlmostMatched?.result.ErrorText ?? "Cannot match any parameters. Run with -? parameter to get complete list of parameters.");
        }
    }
}