namespace PswManagerEncryption.Random {
    internal class Range {

        readonly int min;
        readonly int max;
        public int Value { get; private set; }

        /// <summary>
        /// Instantiates <see cref="Range"/> and sets <see cref="Value"/> at <paramref name="min"/>.
        /// </summary>
        public Range(int min, int max) : this(min, max, 0) { }

        /// <summary>
        /// Instantiates <see cref="Range"/> and sets <see cref="Value"/> at <paramref name="min"/> + <paramref name="valueOverMin"/> 
        /// while keeping it between the <paramref name="min"/> to <paramref name="max"/> range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="valueOverMin"></param>
        public Range(int min, int max, int valueOverMin) {
            this.min = min;
            this.max = max;
            Value = min;
            Add(valueOverMin);
        }

        public Range Add(int value) {
            var range = max - min;
            var remainder = value % range;
            var summedValue = Value + remainder;
            if(summedValue > max) {
                summedValue -= range;
            }
            Value = summedValue;

            return this;
        }

        public static Range operator +(Range a) => a;
        public static Range operator +(Range a, Range b) => a.Add(b.Value);
        public static Range operator +(Range a, int b) => a.Add(b);

        public static implicit operator int(Range range) => range.Value;

    }

}
