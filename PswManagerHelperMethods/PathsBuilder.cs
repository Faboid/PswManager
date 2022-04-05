using System;
using System.IO;

namespace PswManagerHelperMethods {
    public static class PathsBuilder {

        static PathsBuilder() {

            //as some classes rely on this directory's existence,
            //it's best to create it from here instead of doing a "worst case protection" by creating it in all of them
            if(!Directory.Exists(GetDataDirectory)) {
                Directory.CreateDirectory(GetDataDirectory);
            }
        }

        public static readonly string GetWorkingDirectory = Path.GetDirectoryName(Environment.ProcessPath);
        public static readonly string GetDataDirectory = Path.Combine(GetWorkingDirectory, "Data");

    }
}
