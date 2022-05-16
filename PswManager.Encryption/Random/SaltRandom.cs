namespace PswManager.Encryption.Random {
    internal class SaltRandom {

        readonly int seed;
        IntRange current;

        public SaltRandom(int seed) : this(34, 64, seed) { }

        public SaltRandom(int min, int max, int seed) {
            this.seed = seed;
            current = new IntRange(min, max, seed);
        }

        public int Next() {
            current += seed;
            return current;
        }

    }
}
