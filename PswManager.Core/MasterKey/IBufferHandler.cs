using System.Threading.Tasks;

namespace PswManager.Core.MasterKey;
internal interface IBufferHandler {
    bool Exists { get; }

    Task Backup();
    void Free();
    Task Restore();
}