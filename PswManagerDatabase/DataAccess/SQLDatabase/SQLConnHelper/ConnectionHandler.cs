using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PswManagerDatabase.DataAccess.SQLDatabase.SQLConnHelper {
    internal static class ConnectionHandler {

        public static T Open<T>(this SqlConnection cnn, Func<T> predicate) {
            try {
                cnn.Open();

                return predicate.Invoke();

            } finally {
                if(cnn.State == System.Data.ConnectionState.Open) {
                    cnn.Close();
                }
            }
        }

    }
}
