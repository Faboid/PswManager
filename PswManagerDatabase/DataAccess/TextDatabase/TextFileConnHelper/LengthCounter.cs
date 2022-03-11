using PswManagerDatabase.Config;
using PswManagerHelperMethods;
using System;
using System.IO;

namespace PswManagerDatabase.DataAccess.TextDatabase.TextFileConnHelper {
    internal class LengthCounter {

        readonly IPaths paths;

        public LengthCounter(IPaths paths) {
            this.paths = paths;
            Refresh();
        }

        public int Current { get; private set; }

        public void Refresh() {
            int length = 0;

            using(var reader = new StreamReader(paths.AccountsFilePath)) {
                while(reader.ReadLine() != null) {
                    length++;
                }
            }

            Current = length;
        }

        public void AddOne() {
            Current++;
        }

        public void SubtractOne() {
            Current--;
        }

        /// <summary>
        /// First checks if the given position is equal to <see cref="Current"/>. 
        /// If it's not, it refreshes <see cref="Current"/> and checks again. 
        /// If it's wrong once more, throws <see cref="ArgumentOutOfRangeException"/>.
        /// <br/><br/>
        /// Important: only use this function if you're sure it's supposed to be correct. This is NOT for user input validation.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void ValidateLength(int position) {
            //stores the function to avoid duplicate logic
            bool checkLength() => position < 0 || position > Current;
            if(checkLength()) {

                //in case it's a logistic problem, refreshes the length
                Refresh();

                //if it's still faulty, throws the exception
                checkLength().IfTrueThrow(
                    new ArgumentOutOfRangeException(
                        nameof(position),
                        "The given name has been found in an out of range position. The files might be corrupted."
                    )
                );
            }
        }

    }
}
