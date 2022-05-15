using PswManager.Encryption.Random;
using System;
using System.Linq;
using Xunit;

namespace PswManager.Tests.Encryption {
    public class SaltRandomTests {

        [Fact]
        public void SaltRandomIsConsistent() {

            //arrange
            var random = new SaltRandom(50, 500, 100);
            int[] expected = new int[5] { 250, 350, 450, 100, 200 };
            int[] actual;

            //act
            actual = Enumerable.Range(0, 5)
                .Select(x => random.Next())
                .ToArray();

            //assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void WithinRange() {

            //arrange
            int min = 50;
            int max = 150;
            var random = new SaltRandom(min, max, 543667);

            //act
            var result = !Enumerable.Range(0, 100)
                .Select(x => random.Next())
                .Any(x => x < min || x > max);

            //assert
            Assert.True(result);

        }

        [Fact]
        public void ThrowsIfMinLessThanMax() {

            int min = 50;
            int max = -50;

            Assert.Throws<ArgumentOutOfRangeException>(() => new SaltRandom(min, max, 50));

        }

    }
}
