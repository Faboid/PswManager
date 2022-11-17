using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace PswManager.Core.IO;

internal class InternalDirectoryInfoWrapper : IDirectoryInfoWrapper {

    private readonly IDirectoryInfo _info;
    
    public InternalDirectoryInfoWrapper(IFileSystem fileSystem, DirectoryInfo instance) {
        _info = new System.IO.Abstractions.DirectoryInfoWrapper(fileSystem, instance);
    }
    public InternalDirectoryInfoWrapper(IDirectoryInfo info) {
        _info = info;
    }

    public void CopyTo(string path) => _info.CopyTo(path);
    public Task CopyToAsync(string path) => _info.CopyToAsync(path);

    #region Inherited
    public IDirectoryInfo Parent => _info.Parent;
    public IDirectoryInfo Root => _info.Root;
    public IFileSystem FileSystem => _info.FileSystem;
    public FileAttributes Attributes { get => _info.Attributes; set => _info.Attributes = value; }
    public DateTime CreationTime { get => _info.CreationTime; set => _info.CreationTime = value; }
    public DateTime CreationTimeUtc { get => _info.CreationTimeUtc; set => _info.CreationTimeUtc = value; }
    public bool Exists => _info.Exists;
    public string Extension => _info.Extension;
    public string FullName => _info.FullName;
    public DateTime LastAccessTime { get => _info.LastAccessTime; set => _info.LastAccessTime = value; }
    public DateTime LastAccessTimeUtc { get => _info.LastAccessTimeUtc; set => _info.LastAccessTimeUtc = value; }
    public DateTime LastWriteTime { get => _info.LastWriteTime; set => _info.LastWriteTime = value; }
    public DateTime LastWriteTimeUtc { get => _info.LastWriteTimeUtc; set => _info.LastWriteTimeUtc = value; }
    public string LinkTarget => _info.LinkTarget;
    public string Name => _info.Name;
    public void Create() => _info.Create();
    public void Create(DirectorySecurity directorySecurity) => _info.Create(directorySecurity);
    public IDirectoryInfo CreateSubdirectory(string path) => _info.CreateSubdirectory(path);
    public void Delete(bool recursive) => _info.Delete(recursive);
    public IEnumerable<IDirectoryInfo> EnumerateDirectories() => _info.EnumerateDirectories();
    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern) => _info.EnumerateDirectories(searchPattern);
    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption) => _info.EnumerateDirectories(searchPattern, searchOption);
    public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, EnumerationOptions enumerationOptions) => _info.EnumerateDirectories(searchPattern, enumerationOptions);
    public IEnumerable<IFileInfo> EnumerateFiles() => _info.EnumerateFiles();
    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern) => _info.EnumerateFiles(searchPattern);
    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption) => _info.EnumerateFiles(searchPattern, searchOption);
    public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, EnumerationOptions enumerationOptions) => _info.EnumerateFiles(searchPattern, enumerationOptions);
    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos() => _info.EnumerateFileSystemInfos();
    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern) => _info.EnumerateFileSystemInfos(searchPattern);
    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption) => _info.EnumerateFileSystemInfos(searchPattern, searchOption);
    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, EnumerationOptions enumerationOptions) => _info.EnumerateFileSystemInfos(searchPattern, enumerationOptions);
    public DirectorySecurity GetAccessControl() => _info.GetAccessControl();
    public DirectorySecurity GetAccessControl(AccessControlSections includeSections) => _info.GetAccessControl(includeSections);
    public IDirectoryInfo[] GetDirectories() => _info.GetDirectories();
    public IDirectoryInfo[] GetDirectories(string searchPattern) => _info.GetDirectories(searchPattern);
    public IDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption) => _info.GetDirectories(searchPattern, searchOption);
    public IDirectoryInfo[] GetDirectories(string searchPattern, EnumerationOptions enumerationOptions) => _info.GetDirectories(searchPattern, enumerationOptions);
    public IFileInfo[] GetFiles() => _info.GetFiles();
    public IFileInfo[] GetFiles(string searchPattern) => _info.GetFiles(searchPattern);
    public IFileInfo[] GetFiles(string searchPattern, SearchOption searchOption) => _info.GetFiles(searchPattern, searchOption);
    public IFileInfo[] GetFiles(string searchPattern, EnumerationOptions enumerationOptions) => _info.GetFiles(searchPattern, enumerationOptions);
    public IFileSystemInfo[] GetFileSystemInfos() => _info.GetFileSystemInfos();
    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern) => _info.GetFileSystemInfos(searchPattern);
    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption) => _info.GetFileSystemInfos(searchPattern, searchOption);
    public IFileSystemInfo[] GetFileSystemInfos(string searchPattern, EnumerationOptions enumerationOptions) => _info.GetFileSystemInfos(searchPattern, enumerationOptions);
    public void MoveTo(string destDirName) => _info.MoveTo(destDirName);
    public void SetAccessControl(DirectorySecurity directorySecurity) => _info.SetAccessControl(directorySecurity);
    public void Delete() => _info.Delete();
    public void Refresh() => _info.Refresh();
    #endregion inherited
}