using System;
using System.IO;
using System.Reflection;

namespace AL.library.Utility.Tools
{
    public static class IOTools
    {
        /// <summary>
        /// Obtém o caminho da pasta atual que está o .exe
        /// </summary>
        public static string GetExecutableFolderPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Obtém o caminho completo do arquivo .exe
        /// </summary>
        public static string GetExecutableExeFilePath()
        {
            var path = Assembly.GetEntryAssembly().Location;
            return path;
        }
    }
}
