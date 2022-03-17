using System;
using System.Data.SQLite;

namespace PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper {
    internal static class ConnectionHandler {

        public static T Open<T>(this SQLiteConnection cnn, Func<T> predicate) {
            try {
                cnn.Open();

                return predicate.Invoke();

            } finally {
                if(cnn.State == System.Data.ConnectionState.Open) {
                    cnn.Close();
                }
            }
        }

        public static void Open(this SQLiteConnection cnn, Action predicate) {
            try {
                cnn.Open();
                predicate.Invoke();
            }
            finally {
                if(cnn.State == System.Data.ConnectionState.Open) {
                    cnn.Close();
                }
            }
        }

    }
}
