namespace PswManager.Encryption.Services;

public class KeyGeneratorServiceFactory : IKeyGeneratorServiceInternalFactory {
    public IKeyGeneratorService CreateGeneratorService(char[] masterKey) => new KeyGeneratorService(masterKey);
    IKeyGeneratorService IKeyGeneratorServiceInternalFactory.CreateGeneratorService(byte[] salt, char[] masterKey) => new KeyGeneratorService(salt, masterKey);
    IKeyGeneratorService IKeyGeneratorServiceInternalFactory.CreateGeneratorService(byte[] salt, char[] masterKey, int iterations) => new KeyGeneratorService(salt, masterKey, iterations);
}