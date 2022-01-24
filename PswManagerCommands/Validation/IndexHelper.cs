using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerCommands.Validation {
    public sealed class IndexHelper {

        public IndexHelper(ushort index, params int[] requiredSuccesses) {
            Index = index;
            RequiredSuccesses = requiredSuccesses;
        }

        internal IndexHelper(int index, params int[] requiredSuccesses) {
            Index = index;
            RequiredSuccesses = requiredSuccesses;
        }

        public int Index { get; private set; }

        public int[] RequiredSuccesses { get; private set; }

    }
}
