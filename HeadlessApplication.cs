using System;
using System.Linq;
using Autofac;
using CommandLine;
using LSDView.Controllers.Headless;
using LSDView.Headless;
using LSDView.Util;
using Serilog;

namespace LSDView
{
    public class HeadlessApplication
    {
        private const string HEADLESS_LOG_FORMAT =
            "{Level:u} | {Message:lj}{NewLine}{Exception}";

        protected readonly AbstractHeadlessCommand[] Commands;

        protected readonly HeadlessExportController _exportController;
        protected readonly HeadlessLBDController _lbdController;
        protected readonly HeadlessMOMController _momController;
        protected readonly HeadlessTIMController _timController;
        protected readonly HeadlessTIXController _tixController;
        protected readonly HeadlessTMDController _tmdController;

        public HeadlessApplication(ILifetimeScope scope)
        {
            _exportController = scope.Resolve<HeadlessExportController>();
            _lbdController = scope.Resolve<HeadlessLBDController>();
            _momController = scope.Resolve<HeadlessMOMController>();
            _timController = scope.Resolve<HeadlessTIMController>();
            _tixController = scope.Resolve<HeadlessTIXController>();
            _tmdController = scope.Resolve<HeadlessTMDController>();

            Commands = new AbstractHeadlessCommand[]
            {
                new ExportLevelCommand(_lbdController, _tixController, _exportController)
            };
        }

        public int Run(string[] args)
        {
            // create a console window for this application (or assign it to the existing console window)
            if (!ConsoleUtil.AttachConsole(-1)) ConsoleUtil.AllocConsole();

            // set up special console logging for headless mode
            using (var log = new LoggerConfiguration()
                            .MinimumLevel.ControlledBy(LoggingUtil.LoggingLevelSwitch)
                            .WriteTo.Logger(Log.Logger)
                            .WriteTo.Console(outputTemplate: HEADLESS_LOG_FORMAT)
                            .CreateLogger())
            {
                Log.Logger = log;

                // parse the arguments, and log any errors
                int exitCode = 0;
                try
                {
                    var commandTypes = Commands.Select(c => c.OptionsType).ToArray();
                    var parseResult = Parser.Default.ParseArguments(args, commandTypes);
                    foreach (var command in Commands)
                    {
                        command.Register(ref parseResult);
                    }
                }
                catch (HeadlessException e)
                {
                    Log.Fatal(e.Message);
                    exitCode = 1;
                }
                catch (Exception e)
                {
                    var errString = $"{e.Message}\n{e.StackTrace}";
                    Log.Fatal(errString);
                    exitCode = 1;
                }
                finally
                {
                    ConsoleUtil.FreeConsole();
                }

                return exitCode;
            }
        }
    }
}
