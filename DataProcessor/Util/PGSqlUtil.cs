using System.Collections;
using Npgsql;

namespace FinBeatTech.DataProcessor.Util;

public static class PGSqlUtil
    {
        static int batchSize = 100;
        static object[] emptyArgs = new object[0];
        public const string DEFAULT_SCHEMA = "public";


        public static void AutoCreateTable(string tableName, string tableSchema, string connectionString)
        {
            if (tableName == null || connectionString == null)
                return;
            if (tableSchema.IndexOf("{0}") != -1) // PK_{0}
                tableSchema = string.Format(tableSchema, tableName);
            string cmd = string.Format("create table if not exists \"{0}\" ({1})", tableName, tableSchema);
            ExecuteNonQuery(new NpgsqlCommand(cmd), connectionString);
        }

        public static bool TableExists(string tableName, string connectionString)
        {
            var rslt = ExecuteScalar(new NpgsqlCommand($"select exists (select from information_schema.columns where table_name='{tableName}')"), connectionString);
            return rslt != null && (bool)rslt;
        }

        // executes a parametrized command with supplied values
        // allocates parameter values from args as follows
        // if there are more args than params, several calls will be made, using all values (example: 5 params and 10 values => 2 calls using blocks of 5 values)
        // if there are less args than params, trailing params are set to DBNull, single call is made
        // IEnumerable passed as a single arg is treated as an array of values
        // rethrows exceptions
        public static void ExecuteReader(NpgsqlCommand command, Action<NpgsqlDataReader> action, string connectionString, params object[] args)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    lock (command)
                    {
                        command.Connection = conn;
                        int i = 0, imax = command.Parameters.Count;
                        IEnumerable? values;
                        if (args == null || imax == 0) // caller provided empty array => flush params and call once
                            values = emptyArgs;
                        else if (args.Length == 0) // args not supplied: no params or caller has already filled parameters => call 'as is'
                        {
                            values = emptyArgs;
                            i = imax; // skip flushing params
                        }
                        else if (args.Length == 1 && args[0] is IEnumerable && !(args[0] is string)) // args supplied, but type mismatch with object[] causes them to be wrapped => unwrap the first element
                            values = args[0] as IEnumerable;
                        else // args supplied as normal
                            values = args;
                        foreach (var value in values!)
                        {
                            if (i == imax)
                            {
                                using (var rdr = command.ExecuteReader())
                                    while (rdr.Read())
                                        action(rdr);
                                i = 0;
                            }
                            SetParameter(command.Parameters[i++], value); // command.Parameters[i++].Value = value ?? DBNull.Value;
                        }
                        if (i != 0 || values == emptyArgs)
                        {
                            for (int j = i; j < imax; j++)
                                command.Parameters[j].Value = DBNull.Value;
                            using (var rdr = command.ExecuteReader())
                                while (rdr.Read())
                                    action(rdr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExecuteNonQuery(NpgsqlCommand command, string connectionString, params object[] args)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    lock (command)
                    {
                        command.Connection = conn;
                        int i = 0, imax = command.Parameters.Count;
                        IEnumerable? values;
                        if (args == null || imax == 0) // caller provided empty array => flush params and call once
                            values = emptyArgs;
                        else if (args.Length == 0) // args not supplied: no params or caller has already filled parameters => call 'as is'
                        {
                            command.ExecuteNonQuery();
                            return;
                        }
                        else if (args.Length == 1 && args[0] is IEnumerable && !(args[0] is string)) // args supplied, but type mismatch with object[] causes them to be wrapped => unwrap the first element
                            values = args[0] as IEnumerable;
                        else // args supplied as normal
                            values = args;
                        foreach (var value in values!)
                        {
                            if (i == imax)
                            {
                                command.ExecuteNonQuery();
                                i = 0;
                            }
                            SetParameter(command.Parameters[i++], value); // command.Parameters[i++].Value = value ?? DBNull.Value;
                        }
                        if (i != 0 || values == emptyArgs)
                        {
                            for (int j = i; j < imax; j++)
                                command.Parameters[j].Value = DBNull.Value;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static object? ExecuteScalar(NpgsqlCommand command, string connectionString, params object?[] args)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    lock (command)
                    {
                        command.Connection = conn;
                        int i = 0, imax = command.Parameters.Count;
                        IEnumerable? values;
                        if (args == null || imax == 0) // caller provided empty array => flush params and call once
                            values = emptyArgs;
                        else if (args.Length == 0) // args not supplied: no params or caller has already filled parameters => call 'as is'
                            return command.ExecuteScalar();
                        else if (args.Length == 1 && args[0] is IEnumerable && !(args[0] is string)) // args supplied, but type mismatch with object[] causes them to be wrapped => unwrap the first element
                            values = args[0] as IEnumerable;
                        else // args supplied as normal
                            values = args;
                        foreach (var value in values!)
                        {
                            if (i == imax)
                                throw new InvalidOperationException("Too many parameters");
                            SetParameter(command.Parameters[i++], value); // command.Parameters[i++].Value = value ?? DBNull.Value;
                        }
                        if (i != 0 || values == emptyArgs)
                        {
                            for (int j = i; j < imax; j++)
                                command.Parameters[j].Value = DBNull.Value;
                            return command.ExecuteScalar();
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void SetParameter(NpgsqlParameter parameter, object value)
        {
            if (value == null || value is Enum /*|| (value is int)*/ && (int)value == 0)
                parameter.Value = DBNull.Value;
            else
                parameter.Value = value;
        }
    }