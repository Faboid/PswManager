using System;
using System.IO;

namespace PswManagerHelperMethods {
    public static class PathsBuilder {

        public static readonly string GetWorkingDirectory = Path.GetDirectoryName(Environment.ProcessPath);

    }
}
