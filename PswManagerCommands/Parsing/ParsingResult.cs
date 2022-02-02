﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Parsing {

    public class ParsingResult {

        public enum Success {
            Failure = 0,
            Success = 1,
            Questionable = 2
        }

        public Success Result { get; init; }
        public ICommandArguments Object { get; init; }
        public string ErrorMessage { get; init; }

        public ParsingResult(Success success, ICommandArguments objectResult) {
            Result = success;
            Object = objectResult;
        }

        public ParsingResult(Success success, string errorMessage) {
            Result = success;
            ErrorMessage = errorMessage;
        }

    }
}
