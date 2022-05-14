using PswManager.Async.Locks;
using System;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper {
    internal static class ConnectionHandler {

        public static async Task<AsyncConnection> GetConnectionAsync(this SQLiteConnection cnn) {
            await cnn.OpenAsync().ConfigureAwait(false);
            return new(cnn);
        }

        public static Connection GetConnection(this SQLiteConnection cnn) {
            cnn.Open();
            return new(cnn);
        }

        public struct Connection : IDisposable {

            readonly SQLiteConnection cnn;
            readonly bool isNotDefaulted;
            bool isDisposed = false;

            internal Connection(SQLiteConnection connection) {
                cnn = connection;
                isNotDefaulted = true;
            }

            public void Dispose() {
                if(!isNotDefaulted) {
                    return;
                }

                lock(cnn) {
                    if(isDisposed) {
                        throw new ObjectDisposedException(nameof(Connection));
                    }

                    if(cnn.State == System.Data.ConnectionState.Open) {
                        cnn.Close();
                    }

                    isDisposed = true;
                }
            }
        }

        public struct AsyncConnection : IAsyncDisposable {

            readonly SQLiteConnection cnn;
            readonly Locker locker = new(1);
            readonly bool isNotDefaulted;
            bool isDisposed = false;

            internal AsyncConnection(SQLiteConnection connection) {
                cnn = connection;
                isNotDefaulted = true;
            }

            public async ValueTask DisposeAsync() {
                if(!isNotDefaulted) {
                    return;
                }

                using var heldLock = await locker.GetLockAsync().ConfigureAwait(false);
                if(isDisposed) {
                    throw new ObjectDisposedException(nameof(Connection));
                }

                if(cnn.State == System.Data.ConnectionState.Open) {
                    await cnn.CloseAsync().ConfigureAwait(false);
                }

                isDisposed = true;
            }
        }

    }
}
