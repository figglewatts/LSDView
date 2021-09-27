using CommandLine;
using Serilog.Events;

namespace LSDView.Headless
{
    public class GlobalOptions
    {
        [Option('v', "verbosity", Default = LogEventLevel.Information, HelpText = "The verbosity of the logging.")]
        public LogEventLevel Verbosity { get; set; }
    }
}
