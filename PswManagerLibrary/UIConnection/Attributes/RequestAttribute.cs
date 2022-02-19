﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.UIConnection.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequestAttribute : Attribute {

        public string DisplayName { get; private set; }
        public string RequestMessage { get; private set; }
        public bool Optional { get; private set; }

        public RequestAttribute(string displayName, string requestMessage) {
            DisplayName = displayName;
            RequestMessage = requestMessage;
            Optional = false;
        }

        public RequestAttribute(string displayName, string requestMessage, bool optional) {
            DisplayName = displayName;
            RequestMessage = requestMessage;
            Optional = optional;
        }

        public RequestAttribute(string displayName, bool optional, params string[] multiLinedRequestMessage) {
            DisplayName = displayName;
            Optional = optional;
            RequestMessage = string.Join(Environment.NewLine, multiLinedRequestMessage);
        }

    }
}
