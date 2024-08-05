using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

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

        // Overloaded method to create a command with both @message and @results parameters
        public static SqlCommand CreateCommand(SqlConnection connection, string storedProcedure, bool includeResultsParameter)
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

            if (includeResultsParameter)
            {
                SqlParameter resultParam = new SqlParameter("@results", SqlDbType.VarChar, 100)
                {
                    Direction = ParameterDirection.Output
                };

                command.Parameters.Add(resultParam);
            }

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

        private static bool ColumnExists(SqlDataReader reader, string columnName)
        {
            try
            {
                return reader.GetOrdinal(columnName) >= 0;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public static string GetOutputParameter(SqlCommand command, string parameterName)
        {
            return command.Parameters[parameterName].Value?.ToString();
        }

        public static async Task<List<T>> RetrieveAllAsync<T>(SqlConnection connection, string storedProcedure,string genflag) where T : new()
        {
            try
            {
                List<T> entities = new List<T>();

                using (var command = CreateCommand(connection, storedProcedure))
                {
                    AddParameters(command, genflag, new { });

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            T entity = new T();
                            foreach (var property in typeof(T).GetProperties())
                            {

                                if (ColumnExists(reader, property.Name) && !reader.IsDBNull(reader.GetOrdinal(property.Name)))
                                {
                                    property.SetValue(entity, reader.GetValue(reader.GetOrdinal(property.Name)));
                                }
                            }
                            entities.Add(entity);
                        }
                    }
                    // Retrieve the output parameter after closing the reader
                    string message = GetOutputParameter(command, "@message");
                    if (!string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine(message);
                    }
                }

                return entities;
            }
            catch (Exception ex)
            { 

                throw ex;
            }
        }

        public static async Task<T> ReadAsync<T>(SqlConnection connection, T key, string storedProcedure,string genflag) where T : new()
        {
            using (var command = CreateCommand(connection, storedProcedure))
            {
               AddParameters(command, genflag, key);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        T entity = new T();
                        foreach (var property in typeof(T).GetProperties())
                        {
                            if (ColumnExists(reader, property.Name) && !reader.IsDBNull(reader.GetOrdinal(property.Name)))
                            {
                                property.SetValue(entity, reader.GetValue(reader.GetOrdinal(property.Name)));
                            }
                        }
                        return entity;
                    }
                }
            }
            return default;
        }


        public static int GetNextSequenceValue(string sequenceName,SqlConnection connection)
        {
                using (SqlCommand command = new SqlCommand($"SELECT NEXT VALUE FOR {sequenceName}", connection))
                {
                long result = (long)command.ExecuteScalar();
                return (int)result;
            }    
        }

    }
}

