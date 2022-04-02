using PswManagerEncryption.Services;
using PswManagerHelperMethods;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PswManagerTests.Encryption {
    public class KeyGeneratorServiceTests {

        [Fact]
        public void ProducesConsistentResults() {

            //arrange
            char[] masterKey1 = "Hellothere,sup!".ToCharArray();
            char[] masterKey2 = "Hellothere,sup!".ToCharArray();
            byte[] salt = new byte[] { 47, 83, 43, 238, 32, 01, 23, 23 };
            int iterations = 5000;
            var generator1 = new KeyGeneratorService(salt, masterKey1, iterations);
            var generator2 = new KeyGeneratorService(salt, masterKey2, iterations);

            //act
            var keys1 = generator1.GenerateKeys(5).Select(x => x.Get()).ToList();
            var keys2 = generator2.GenerateKeys(5).Select(x => x.Get()).ToList();

            //assert
            Enumerable
                .Range(0, 1)
                .ForEach(x => {
                    Assert.Equal(keys1[x], keys2[x]);
                });

        }

        [Fact]
        public void BackwardCompatibleResults() {

            //arrange
            char[] masterKey = "RandomValues##".ToCharArray();
            byte[] salt = new byte[] { 23, 34, 45, 03, 23, 56, 231, 123 };
            int iterations = 5000;
            var generator = new KeyGeneratorService(salt, masterKey, iterations);

            char[] first = "ښಂ䃋찼閥㲂�ℴද㶒ฺ�峤�㈇맜⩦Ҽ㕻縙剮ⵙ�槫ꣴⰡ쾣ඡ䨓뮀萃⽭풇삡립넮缜ꙻ轈嬭酅ꁔ⧸ጪ앧쏵燦ꕴ频볏�뎎刁ﹳ杠ନॕㆂ墨ꮌ".ToCharArray();
            char[] second = "아ᔬ惡䐨⫝̸╆ᾗ唼Ꮅ螂꽾ꯄＦ던酣禇蔻帷༡ꢢ覮쵹䧌䊜ᣵ긟헣긷꥽芷밦⭫놪ᤚ촗牧쿁嗂롶箩橆눖遀Ğ䫌겟ₕ籑탣ង�".ToCharArray();
            char[] fourth = "㣵櫕枡飹㰐経㸩㖑꼨巟躏਽㈈゘錡藥ኰ컹⋫栋ᑃ饽ힺ诐菥節ᬝ틩칙▭ꌂ㌿�".ToCharArray();
            char[] tenth = "蛯鴀쓍蕪ꊑ��Ά�낸拷똥쯘儩◡풗繰瓃ഴ䙢憨웂鄕鱁쮧ﲡ㪡鑊밅�줎뢙ਃ颒唣⊓�嚙蔃䄫蔃⌑㓏释麧羠詣鏱쓶㟵쵙놏䜁觷㊣䣴뒄ᆴ＃ዉ苢Ⓥ븱⿹ઋ㑡䓲⋨쟞녀ᑵᘺ⧒쩲⸨촉鶈⋴️犚�".ToCharArray();

            //act
            var firstKey = generator.GenerateKeys(1).First().Get(); //1st
            var secondKey = generator.GenerateKeys(1).First().Get(); //2nd
            var fourthKey = generator.GenerateKeys(2).Skip(1).First().Get(); //3rd - 4th
            var tenthKey = generator.GenerateKeys(6).Skip(5).First().Get(); //5th - 10th

            //assert
            Assert.Equal(first, firstKey);
            Assert.Equal(second, secondKey);
            Assert.Equal(fourth, fourthKey);
            Assert.Equal(tenth, tenthKey);

        }

        [Fact]
        public void GenerateDistinctKeys() {

            //arrange
            char[] masterKey = "randomVeryLongKeyHere".ToCharArray();
            byte[] salt = new byte[] { 0, 21, 32, 45, 23, 43, 23, 11, 78 };
            int iterations = 500;
            var generator = new KeyGeneratorService(salt, masterKey, iterations);

            //act
            var listKeys = Enumerable.Range(0, 100).Select(x => generator.GenerateKeys(1).First().Get()).ToList();
            var hashKeys = new HashSet<char[]>(listKeys);

            //assert
            Assert.True(listKeys.Count == hashKeys.Count);

        }


    }
}
