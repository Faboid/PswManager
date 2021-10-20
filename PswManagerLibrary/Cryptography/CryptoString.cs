using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerLibrary.Cryptography {
    public class CryptoString {
        //This class is a refactored version of "A Gazhal"'s answer on https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp

        private static readonly byte[] salt = new byte[] { 0x44, 0x53, 0x33, 0x22, 0x68, 0x73, 0xFF, 0x00, 0x33, 0xFA, 0xAA, 0x6E, 0x12, 0xFC, 0xBC };

        public static string Encrypt(string plainText, string password) {

            byte[] bites = Encoding.Unicode.GetBytes(plainText);

            using(Aes encryptor = GetAes(password)) {

                using(MemoryStream ms = new MemoryStream()) {
                    WriteToStream(ms, encryptor.CreateEncryptor(), bites);
                    plainText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return plainText;
        }

        public static string Decrypt(string cipherText, string password) {

            cipherText = cipherText.Replace(" ", "+");
            byte[] bites = Convert.FromBase64String(cipherText);

            using(Aes encryptor = GetAes(password)) {

                using(MemoryStream ms = new MemoryStream()) {
                    WriteToStream(ms, encryptor.CreateDecryptor(), bites);
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }


        private static Aes GetAes(string key) {
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

    public enum CryptoType {
        Encrypt,
        Decrypt
    }
}
