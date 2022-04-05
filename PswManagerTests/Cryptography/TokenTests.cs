﻿using System;
using PswManagerLibrary.Cryptography;
using PswManagerEncryption.Cryptography;
using System.IO;
using PswManagerHelperMethods;
using Xunit;
using PswManagerEncryption.Services;

namespace PswManagerTests.Cryptography {
    public class TokenTests : IDisposable {

        public TokenTests() {
            CryptoService crypto = GetService(GetPassword);
            _token = new Token(crypto);
            _token.VerifyToken();
        }

        static CryptoService GetService(Key password) => new (password, "test.1");
        static Key GetPassword => new ("password".ToCharArray());

        readonly Token _token;

        [Fact]
        public void SetUpCorrectly() {

            //arrange
            string path = Path.Combine(PathsBuilder.GetDataDirectory, "SetUpCorrectlyToken.txt");
            var token = new Token(GetService(GetPassword), path);

            try {

                bool beforeExists = File.Exists(path);
                bool beforeIsSetUp = token.IsTokenSetUp();
                bool success = token.VerifyToken();
                bool afterIsSetUp = token.IsTokenSetUp();
                bool afterExists = File.Exists(path);

                Assert.False(beforeExists);
                Assert.False(beforeIsSetUp);
                Assert.True(success);
                Assert.True(afterIsSetUp);
                Assert.True(afterExists);

            } finally {
                File.Delete(path);
            }
        }

        [Fact]
        public void RecognizeWrongPassword() {

            //arrange
            var wrongKey = new Key("wrongPass".ToCharArray());
            var token = new Token(GetService(wrongKey));

            //act
            var exists = token.IsTokenSetUp();
            var result = token.VerifyToken();

            //assert
            Assert.True(exists);
            Assert.False(result);

        }

        [Fact]
        public void RecognizeCorrectPassword() {

            //arrange
            var token = new Token(GetService(GetPassword));

            //act
            var result = token.VerifyToken();

            //assert
            Assert.True(result);

        }


        public void Dispose() {
            File.Delete(Token.GetDefaultPath());
            GC.SuppressFinalize(this);
        }
    }
}
