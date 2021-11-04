using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PswManagerLibrary.Cryptography {
    public class CryptoString {
        //This class is a refactored version of "A Gazhal"'s answer on https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp

        //todo - change the salt from constant to randomized
        private readonly byte[] salt = new byte[] { 0x44, 0x53, 0x33, 0x22, 0x68, 0x73, 0xFF, 0x00, 0x33, 0xFA, 0xAA, 0x6E, 0x12, 0xFC, 0xBC };
        private readonly string key;

        public CryptoString(string password) {
            key = password;
        }

        public string Encrypt(string plainText) {

            byte[] bites = Encoding.Unicode.GetBytes(plainText);

            using(Aes encryptor = GetAes()) {

                using(MemoryStream ms = new MemoryStream()) {
                    WriteToStream(ms, encryptor.CreateEncryptor(), bites);
                    plainText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return plainText;
        }

        public string Decrypt(string cipherText) {

            cipherText = cipherText.Replace(" ", "+");
            byte[] bites = Convert.FromBase64String(cipherText);

            using(Aes encryptor = GetAes()) {

                using(MemoryStream ms = new MemoryStream()) {
                    WriteToStream(ms, encryptor.CreateDecryptor(), bites);
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }

        private Aes GetAes() {
            Aes aes = Aes.Create();
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(key, salt);
            aes.Key = rfc.GetBytes(32);
            aes.IV = rfc.GetBytes(16);

            return aes;
        }

        private static void WriteToStream(MemoryStream ms, ICryptoTransform transform, byte[] bites) {
            using(CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write)) {
                cs.Write(bites);
            }
        }

    }

}
