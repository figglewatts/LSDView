using System;
using LSDView.Util;

namespace LSDView
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += unhandledExceptionHandler;

            new MainWindow().Run();
        }

        private static void unhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Logger.Log()(LogLevel.ERR, $"{e.Message}\n{e.StackTrace}");
        }
    }
}
