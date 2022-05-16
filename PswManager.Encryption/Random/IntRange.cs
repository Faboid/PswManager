namespace PswManager.Encryption.Random {
    internal class IntRange {

        readonly int min;
        readonly int max;
        public int Value { get; private set; }

        /// <summary>
        /// Instantiates <see cref="IntRange"/> and sets <see cref="Value"/> at <paramref name="min"/>.
        /// </summary>
        public IntRange(int min, int max) : this(min, max, 0) { }

        /// <summary>
        /// Instantiates <see cref="IntRange"/> and sets <see cref="Value"/> at <paramref name="min"/> + <paramref name="valueOverMin"/> 
        /// while keeping it between the <paramref name="min"/> to <paramref name="max"/> range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="valueOverMin"></param>
        public IntRange(int min, int max, int valueOverMin) {
            if(min > max) {
                throw new ArgumentOutOfRangeException($"{nameof(min)}, {nameof(max)}", "The given minimum range value is bigger than the given maximum value.");
            }

            this.min = min;
            this.max = max;
            Value = min;
            Add(valueOverMin);
        }

        public IntRange Add(int value) {
            var range = max - min;
            var remainder = value % range;
            var summedValue = Value + remainder;
            if(summedValue > max) {
                summedValue -= range;
            }
            Value = summedValue;

            return this;
        }

        public static IntRange operator +(IntRange a) => a;
        public static IntRange operator +(IntRange a, IntRange b) => a.Add(b.Value);
        public static IntRange operator +(IntRange a, int b) => a.Add(b);

        public static implicit operator int(IntRange range) => range.Value;

    }

}
