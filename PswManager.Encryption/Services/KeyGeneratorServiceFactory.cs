namespace PswManager.Encryption.Services;

public class KeyGeneratorServiceFactory : IKeyGeneratorServiceInternalFactory {
    public IKeyGeneratorService CreateGeneratorService(char[] masterKey) => new KeyGeneratorService(masterKey);
    IKeyGeneratorService IKeyGeneratorServiceInternalFactory.CreateGeneratorService(byte[] salt, char[] masterKey) => new KeyGeneratorService(salt, masterKey);
    IKeyGeneratorService IKeyGeneratorServiceInternalFactory.CreateGeneratorService(byte[] salt, char[] masterKey, int iterations) => new KeyGeneratorService(salt, masterKey, iterations);
}

public class MockKeyGeneratorServiceFactory : IKeyGeneratorServiceFactory {

    private readonly byte[] _salt;
    private readonly int _iterations;

    public MockKeyGeneratorServiceFactory(byte[] salt, int iterations) {
        _salt = salt;
        _iterations = iterations;
    }

    public IKeyGeneratorService CreateGeneratorService(char[] masterKey) => new KeyGeneratorService(_salt, masterKey, _iterations);
}