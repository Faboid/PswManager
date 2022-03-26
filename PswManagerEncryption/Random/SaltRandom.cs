namespace PswManagerEncryption.Random {
    internal class SaltRandom {

        readonly int seed;
        Range current;

        public SaltRandom() : this(Environment.TickCount) { }

        public SaltRandom(int seed) : this(34, 64, seed) { }

        public SaltRandom(int min, int max, int seed) {
            this.seed = seed;
            current = new Range(min, max, seed);
        }

        public int Next() {
            current += seed;
            return current;
        }

    }
}
