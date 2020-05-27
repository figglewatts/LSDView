using System;

namespace LSDView.Util
{
    public static class PathUtil
    {
        public static string MakeRelative(string fullPath, string relativeRoot)
        {
            var fullUri = new Uri(fullPath);
            var relativeRootUri = new Uri(relativeRoot);

            return relativeRootUri.MakeRelativeUri(fullUri).ToString();
        }
    }
}
