using System.IO;
using System;
using System.IO.Abstractions;

namespace PswManager.Core;
#pragma warning disable CA1822 // Mark members as static

public class PathsHandler {

    private readonly IDirectoryInfoFactory _directoryInfoFactory;

    public PathsHandler(IDirectoryInfoFactory directoryInfoFactory) {
        _directoryInfoFactory = directoryInfoFactory;
        EnsureDirectoriesExistence();
    }

    public string GetTokenPath() => DefaultPaths.TokenFile;

    public void EnsureDirectoriesExistence() {
        _directoryInfoFactory.FromDirectoryName(DefaultPaths.DataDirectory).Create();
        _directoryInfoFactory.FromDirectoryName(DefaultPaths.LogsDirectory).Create();
    }

}

public static class DefaultPaths {

    /// <summary>
    /// The directory that contains the exe this application is running from.
    /// </summary>
    public static string WorkingDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;

    /// <summary>
    /// The directory that contains all data related to this application.
    /// </summary>
    public static string DataDirectory { get; } = Path.Combine(WorkingDirectory, "Data");

    /// <summary>
    /// The token file.
    /// </summary>
    public static string TokenFile { get; } = Path.Combine(DataDirectory, "Token.txt");

    /// <summary>
    /// The directory that contains all logs.
    /// </summary>
    public static string LogsDirectory { get; } = Path.Combine(DataDirectory, "Logs");

    /// <summary>
    /// The settings file.
    /// </summary>
    public static string SettingsFile { get; } = Path.Combine(DataDirectory, "Config.txt");

}