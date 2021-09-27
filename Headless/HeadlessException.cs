using System;

namespace LSDView.Headless
{
    /// <summary>
    /// Exception for errors when running headless commands.
    /// </summary>
    public class HeadlessException : Exception
    {
        public HeadlessException(string message) : base(message) { }
    }
}
