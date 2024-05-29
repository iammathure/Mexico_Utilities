using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mexico_Utility
{
    public  class DatabaseHelper
    {
        public static SqlCommand CreateCommand(SqlConnection connection, string storedProcedure)
        {
            SqlCommand command = new SqlCommand(storedProcedure, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            SqlParameter messageParam = new SqlParameter("@message", SqlDbType.VarChar, 100)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(messageParam);

            return command;
        }

        public static void AddParameters<T>(SqlCommand command, string genFlag, T entity)
        {
            command.Parameters.AddWithValue("@genflag", genFlag.ToString());
            if (entity != null)
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
               
                foreach (var property in properties)
                {
                    if (!Attribute.IsDefined(property, typeof(ExcludeParameterAttribute)))
                    {
                        string parameterName = $"@{property.Name}";
                        object value = property.GetValue(entity) ?? DBNull.Value;
                        command.Parameters.AddWithValue(parameterName, value);
                    }
                }
            }
        }


        public static string GetOutputParameter(SqlCommand command, string parameterName)
        {
            return command.Parameters[parameterName].Value?.ToString();
        }
    }
}
