﻿namespace PswManager.Encryption.Services; 
public interface ICryptoService {

    string Encrypt(string plainText);
    string Decrypt(string cipherText);

}
