﻿using PswManagerHelperMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Unused.Parsing.Helpers {
    internal static class Splitter {

        public static IEnumerable<string> GetArgs(this string input, string separator)
            => input
            .Split(separator)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        public static IEnumerable<string> GetKeys(this IEnumerable<string> arguments, char equalSign)
            => arguments
            .Select(x => x.Split(equalSign).First());

        public static IEnumerable<string> GetValues(this IEnumerable<string> arguments, char equalSign)
            => arguments
            .Select(x => x.Split(equalSign).Skip(1).JoinStrings(equalSign));

    }
}