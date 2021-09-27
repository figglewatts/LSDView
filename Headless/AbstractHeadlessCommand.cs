using System;
using CommandLine;
using LSDView.Util;
using Serilog;

namespace LSDView.Headless
{
    public abstract class AbstractHeadlessCommand : IHeadlessCommand
    {
        public abstract Type OptionsType { get; }
        public abstract void Register(ref ParserResult<object> parserResult);

        protected void handleGlobalOptions(GlobalOptions options)
        {
            LoggingUtil.LoggingLevelSwitch.MinimumLevel = options.Verbosity;
            Log.Information($"Setting verbosity to: {options.Verbosity}");
        }
    }
}
