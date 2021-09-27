using System.Runtime.InteropServices;

namespace LSDView.Util
{
    public static class ConsoleUtil
    {
        [DllImport("kernel32.dll")]
        public static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();
    }
}
