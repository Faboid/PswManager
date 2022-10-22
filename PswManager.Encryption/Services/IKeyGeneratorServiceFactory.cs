namespace PswManager.Encryption.Services;
public interface IKeyGeneratorServiceFactory {

    IKeyGeneratorService CreateGeneratorService(char[] masterKey);

}

internal interface IKeyGeneratorServiceInternalFactory : IKeyGeneratorServiceFactory {

    internal IKeyGeneratorService CreateGeneratorService(byte[] salt, char[] masterKey);
    internal IKeyGeneratorService CreateGeneratorService(byte[] salt, char[] masterKey, int iterations);

}