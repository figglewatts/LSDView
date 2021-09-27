using System;
using Autofac;
using LSDView.Util;
using Serilog;
using Serilog.Enrichers.WithCaller;

namespace LSDView
{
    public static class Program
    {
        private static IContainer Container { get; set; }

        private const string LOG_FORMAT = "{Timestamp} <{Caller}> {Level:u} | {Message:lj}{NewLine}{Exception}";

        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += unhandledExceptionHandler;

            using (var log = new LoggerConfiguration()
                            .WriteTo.File("app.log", fileSizeLimitBytes: null,
                                 outputTemplate: LOG_FORMAT)
                            .Enrich.WithCaller()
                            .MinimumLevel.ControlledBy(LoggingUtil.LoggingLevelSwitch)
                            .CreateLogger())
            {
                Log.Logger = log;
                Log.Information("LSDView has started!");

                var builder = new ContainerBuilder();
                bool headless = args.Length > 0;
                builder.RegisterModule(new ApplicationModule { Headless = headless });
                Container = builder.Build();

                int exitCode = 0;
                using (var scope = Container.BeginLifetimeScope())
                {
                    if (headless)
                    {
                        Log.Information($"Entering batch mode due to presence of {args.Length} args");
                        exitCode = new HeadlessApplication(scope).Run(args);
                    }
                    else
                    {
                        Log.Information("Entering GUI mode");
                        new GuiApplication(scope).Run();
                    }
                }

                return exitCode;
            }
        }


        private static void unhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Log.Fatal($"{e.Message}\n{e.StackTrace}");
        }
    }
}
