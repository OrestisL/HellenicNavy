#region using
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using UnitySQLite.Utilities;
#endregion
namespace UnitySQLite
{
    public class DatabaseManager : GenericSingleton<DatabaseManager>, IDisposable
    {
        #region Variables, Delegates and Events
        //database stuff
        private static SqliteConnection sm_dbConnection;
        private static SqliteCommand sm_dbCommand;
        private static SqliteTransaction sm_dbTransaction;
        //used for commands
        [Header("Database location and name")]
        [Tooltip("Starts from Application.persistentDataPath")]
        public string m_dbLocation = "Databases";
        public string m_dbName;

        private static string sm_dbLocation;
        private static string sm_dbName;
        private static string sm_CurrentConnection;

        private static Dictionary<int, string> sm_Connections = new Dictionary<int, string>();

        public bool m_limitRows;
        public int m_maxRowsAllowed = 50;

        //threads
        private Thread writeThread;
        private Thread readThread;
        private Thread createTableThread;

        //data sync variables
        private List<List<DataEntry>> data;
        private bool isReadySync = false; //data has been read and is ready to sync
                                          //private bool isReadingData = false;

        public float m_writeFrequency;
        /// <summary>
        /// Used for adding values to a table that should be continuous (eg positions).
        /// </summary>
        private Action onWriteToDatabaseContinuous;
        private void ExecuteWriteToDatabaseContinuous()
        {
            onWriteToDatabaseContinuous?.Invoke();
        }

        /// <summary>
        /// Used for adding values to a table that shouldnt be continuous (eg pressing a button).
        /// </summary>
        private Action onWriteToDatabaseOnce;

        private void ExecuteWriteToDatabaseOnce()
        {
            onWriteToDatabaseOnce?.Invoke();
            onWriteToDatabaseOnce = null;
        }

        //respectively for creating tables.
        //necessary because calling the async method more than once 
        //might not run properly
        private Action CreateTable;
        private void ExecuteCreateTable()
        {
            CreateTable?.Invoke();
            CreateTable = null;
        }

        /// <summary>
        /// Called when data is ready. Use this Action to access data.
        /// Action is cleared after every use.
        /// </summary>
        private Action<List<List<DataEntry>>> OnDataReady;

        #endregion

        #region Initializaton
        public override void Awake()
        {
            //initialize singleton
            base.Awake();

#if UNITY_ANDROID
            Screen.orientation = ScreenOrientation.LandscapeLeft;
#endif
        }

        public void Initialize(string dbLocation, string dbName)
        {
            //ensure write frequency is not 0
            m_writeFrequency = Mathf.Clamp(m_writeFrequency, 0.1f, m_writeFrequency);

            sm_dbLocation = dbLocation;
            sm_dbName = dbName;

            //add proper file ending 
            if (!sm_dbName.EndsWith(".db"))
                sm_dbName = string.Format("{0}.db", sm_dbName);//dbName += ".db";

            //create log
            Logger.Instance.CreateLogFile(Path.Combine(Application.persistentDataPath, "Logs"), "Log.txt");

            //create directory if not exists
            string directory = Path.Combine(Application.persistentDataPath, sm_dbLocation);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            int currentHash = AddConnection(sm_dbName);
            ChangeConnection(currentHash);
        }
        #endregion

        #region Base functions
        /// <summary>
        /// Creates a table with name at the connected database at path with contents.
        /// Adds one or multiple columns, but remember to use FormatValues<T> before to have a properly formatted string of columns.
        /// </summary>
        /// <paramref name="TableName"/>The name of the table to be created.</param>
        /// <param name="columns">Separated by commas, need name and type (example: number INT, something INT). 
        /// Possible types: INT, REAL (floats), TEXT, booleans should be saved as integers with 0 = false and 1 = true.
        /// Search "SQLITE datatypes" for more information.</param>
        public void CreateTableOnDatabase(string TableName, string columns)
        {
            //remove spaces because it no work
            if (TableName.Contains(" "))
                TableName = TableName.Replace(" ", "_");

            //set the query string

            string query = string.Format("CREATE TABLE IF NOT EXISTS {0}({1});",
                TableName, columns);

            //create and execute command
            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            //sm_dbTransaction = sm_dbConnection.BeginTransaction();
            //dbcommand.CommandText = query;
            sm_dbCommand.ExecuteNonQuery();
            sm_dbCommand.Dispose();
            //commit changes
            //sm_dbTransaction.Commit();
            //close connection
            //dbConnection.Close();
            Logger.Instance.AddMessage(query);
        }

        /// <summary>
        /// Deletes table tableName from database.
        /// </summary>
        public void DeleteTableFromDatabase(string TableName)
        {
            //remove spaces because it no work
            if (TableName.Contains(" "))
                TableName = TableName.Replace(" ", "_");

            //dbConnection = CreateConnectionToDB(path);
            //create query
            string query = string.Format("DROP TABLE IF EXISTS {0};", TableName);

            //create command
            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            //sm_dbTransaction = sm_dbConnection.BeginTransaction();
            //execute command
            sm_dbCommand.ExecuteReader().Dispose();
            sm_dbCommand.Dispose();
            //commit changes
            //sm_dbTransaction.Commit();
            //close connection
            //dbConnection.Close();
            Logger.Instance.AddMessage(query);
        }

        /// <summary>
        /// Checks if the entry exists in the table. Entry must be primary key.
        /// </summary>
        /// <param name="values">Comma separated values to check</param>
        /// <returns>True if the entry exists and false otherwise.</returns>
        private bool CheckIfEntryExists(string TableName, string values)
        {
            //remove spaces because it no work
            if (TableName.Contains(" "))
                TableName = TableName.Replace(" ", "_");

            string query = string.Empty;
            GetColumnNamesOnTable(TableName, out TableRow row);
            ReadOnlySpan<char> val = values;

            int idx = 0, current = 0;
            bool isLastLoop = false;
            string value = string.Empty;
            while (!isLastLoop)
            {
                int startIdx = idx;
                idx = values.IndexOf(',', startIdx);

                isLastLoop = (idx == -1);
                if (isLastLoop)
                {
                    idx = values.Length;
                }

                if (row.columns[current].Unique)
                {
                    value = val.Slice(startIdx, idx - startIdx).ToString();
                    query = string.Format("SELECT {0} FROM {1} WHERE {0}={2}", row.columns[current].ColumnName, TableName, value);
                    //table should have 1 primary key
                    value = string.Format("{0} = {1}", row.columns[current].ColumnName, value);
                    sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
                    SqliteDataReader reader = sm_dbCommand.ExecuteReader();
                    sm_dbCommand.Dispose();
                    bool exists = reader.Read();

                    reader.Dispose();
                    if (exists)
                    {
                        Debug.LogWarning(string.Format("Entry {0} in table {1} already exists", value, TableName));
                        return true;
                    }
                }

                current++;
                idx++;
            }

            return false;

        }

        /// <summary>
        /// Used to insert values to a table.
        /// Use FormatValues before calling this function.
        /// </summary>
        private void InsertSingleColumnValueToTable(string TableName, string columnName, string values)
        {
            //remove spaces because it no work
            if (TableName.Contains(" "))
                TableName = TableName.Replace(" ", "_");

            //dbConnection = CreateConnectionToDB(pathToDatabase);
            //dbConnection.Open();
            string query = string.Empty;

            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            //sm_dbTransaction = sm_dbConnection.BeginTransaction();
            string[] commaSeparatedValues = values.Split(',');
            for (int i = 0; i < commaSeparatedValues.Length; i++)
            {
                query = string.Format("INSERT INTO {0}({1}) VALUES ({2});", TableName, columnName, commaSeparatedValues[i]);
                sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
                sm_dbCommand.ExecuteReader().Dispose();
                sm_dbCommand.Dispose();
            }
            //sm_dbTransaction.Commit();
            //commit changes
            //transaction.Commit();
            //dbConnection.Close();
            Logger.Instance.AddMessage(query);
        }


        private void WriteToDatabase(string tableName, TableRow tableRow)
        {
            // remove spaces because it no work
            if (tableName.Contains(" "))
                tableName = tableName.Replace(" ", "_");

            if (!CheckIfTableExists(tableName))
            {
                Logger.Instance.AddMessage(string.Format("Attempt to insert values to table {0} but it doesnt exist.", tableName));
                return;
            };

            //get row of table
            GetColumnNamesOnTable(tableName, out TableRow row);

            //check if given row and read row match
            if (row != tableRow)
            {
                string message = string.Format("Attempt to write values {0} in table {1} but the row doesnt support the input (table columns are {2})",
                    tableRow.GetValuesAndTypes(), tableName, row.GetColumns());
                Logger.Instance.AddMessage(message);
                return;
            }

            //create query
            string query = string.Empty;
            string values = tableRow.GetValues();
            ReadOnlySpan<char> newlinesSeparatedSpan = values;
            int nextNLIndex = 0;
            bool isLastLoop = false;
            while (!isLastLoop)
            {
                int startIdx = nextNLIndex;
                nextNLIndex = values.IndexOf('\n', startIdx);

                isLastLoop = (nextNLIndex == -1);
                if (isLastLoop)
                {
                    nextNLIndex = values.Length;
                }

                ReadOnlySpan<char> slice = newlinesSeparatedSpan.Slice(startIdx, nextNLIndex - startIdx);
                query = string.Format("INSERT OR IGNORE INTO {0}({1}) VALUES ({2});", tableName, tableRow.GetColumnNames(), slice.ToString());
                sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
                sm_dbCommand.ExecuteNonQuery();
                sm_dbCommand.Dispose();
                //sometimes this throws and error (x values for y columns, but it's incosistent)
            }

            if (m_limitRows)
                CheckNumberOfRows(sm_dbConnection, tableName);

            //commit changes
            //transaction.Commit();
            //dbConnection.Close();
            Logger.Instance.AddMessage(query);
        }

        /// <summary>
        /// Inserts values to multiple columns of a table. Make sure values string has rows separated by new lines.
        /// </summary>
        //private void InsertMultipleColumnValueToTable(string TableName, string ColumnNames, string values)
        //{
        //    //remove spaces because it no work
        //    if (TableName.Contains(" "))
        //        TableName = TableName.Replace(" ", "_");

        //    if (!CheckIfTableExists(TableName))
        //    {
        //        Logger.Instance.AddMessage(string.Format("Attempt to insert values to table {0} but it doesnt exist.", TableName));
        //        return;
        //    }

        //    //dbConnection = CreateConnectionToDB(pathToDatabase);
        //    //dbConnection.Open();
        //    string query = string.Empty;

        //    sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
        //    //sm_dbTransaction = sm_dbConnection.BeginTransaction();

        //    //string[] newlineSeparatedLines = values.Split('\n');
        //    ReadOnlySpan<char> newlineSeparatedSpan = values.AsSpan();
        //    int nextNLIndex = 0;
        //    bool isLastLoop = false;
        //    while (!isLastLoop)
        //    {
        //        int startIdx = nextNLIndex;
        //        nextNLIndex = values.IndexOf('\n', startIdx);

        //        isLastLoop = (nextNLIndex == -1);
        //        if (isLastLoop)
        //        {
        //            nextNLIndex = values.Length;
        //        }

        //        ReadOnlySpan<char> slice = newlineSeparatedSpan.Slice(startIdx, nextNLIndex - startIdx);
        //        query = string.Format("INSERT OR IGNORE INTO {0}({1}) VALUES ({2});", TableName, ColumnNames, slice.ToString());
        //        sm_dbCommand.CommandText = query;
        //        sm_dbCommand.ExecuteReader();
        //    }

        //    //sm_dbTransaction.Commit();
        //    //for (int i = 0; i < newlineSeparatedLines.Length; i++)
        //    //{
        //    //    query = string.Format("INSERT OR IGNORE INTO {0}({1}) VALUES ({2});", TableName, ColumnNames, newlineSeparatedLines[i]);
        //    //    dbCommand.CommandText = query;
        //    //    dbCommand.ExecuteReader();
        //    //}
        //    if (m_limitRows)
        //        CheckNumberOfRows(sm_dbConnection, TableName);

        //    //commit changes
        //    //transaction.Commit();
        //    //dbConnection.Close();
        //    Logger.Instance.AddMessage(query);
        //}

        private bool CheckIfTableExists(string tableName)
        {
            //check if table exists first
            string query = string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}';", tableName);
            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            SqliteDataReader reader = sm_dbCommand.ExecuteReader();
            sm_dbCommand.Dispose();
            bool exists = reader.Read();
            if (!exists)
            {
                DataVisualizer.Instance.ChangeDefaultMessage(string.Format("Table {0} does not exist in database", tableName));
            }
            reader.Dispose();
            return exists;
        }

        /// <summary>
        /// Reads from database at path. If you don't want to read everything, use a different mode.
        /// </summary>
        private void ReadFromDatabase(string TableName, SelectFromDatabaseMode mode, out List<List<DataEntry>> readValues,
            SortResultsBy sort = SortResultsBy.none, TableColumn SortColumn = null, string ColumnNames = "", int minRow = 0, int maxRow = 0, string Condition = "")
        {
            //remove spaces because it no work
            if (TableName.Contains(" "))
                TableName = TableName.Replace(" ", "_");

            //dbConnection = CreateConnectionToDB(pathToDatabase);
            //dbConnection.Open();
            string query = string.Empty;
            //string read = string.Empty;
            readValues = new List<List<DataEntry>>();

            if (TableName.Equals("") || !CheckIfTableExists(TableName))
            {
                //we need to call the ReadValuesFromDatabase function to complete some things
                ReadValuesFromDatabase(null, out readValues);
                return;
            }

            switch (mode)
            {
                case SelectFromDatabaseMode.everything:
                    query = string.Format("SELECT * FROM {0}", TableName);
                    break;
                case SelectFromDatabaseMode.specificColumns:
                    query = string.Format("SELECT {0} FROM {1}", ColumnNames, TableName);
                    break;
                case SelectFromDatabaseMode.specificRows:
                    if (maxRow == 0)
                        maxRow = Mathf.Min(LastIDInTable(TableName), m_maxRowsAllowed);

                    if (minRow > maxRow)
                    {
                        Debug.LogWarning(string.Format("Attempt to read specific rows with minRow = {0} & maxRow = {1}", minRow, maxRow));
                        return;
                    }

                    query = string.Format("SELECT {0} FROM {1} WHERE {2}", ColumnNames, TableName, string.Format("rowid BETWEEN {0} AND {1}", minRow, maxRow));
                    break;
            }

            if (!Condition.Equals(""))
            {
                if (mode == SelectFromDatabaseMode.specificRows)
                {
                    query += " AND ";
                }
                query = string.Format("{0} {1}", query, Condition);
            }

            if (sort != SortResultsBy.none)
            {
                string sortString = "";
                switch (SortColumn.ColumnType)
                {
                    case "INT":
                        sortString = string.Format("CAST({0} AS {1})", SortColumn.ColumnName, "INT");
                        break;
                    case "TEXT":
                        sortString = SortColumn.ColumnName;
                        break;
                    case "REAL":
                    case "FLOAT":
                    case "DOUBLE":
                        sortString = string.Format("CAST({0} AS {1})", SortColumn.ColumnName, "DOUBLE");
                        break;
                }
                query = string.Format("{0} ORDER BY {1} {2}", query, sortString, sort.ToString());
            }
            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            //sm_dbTransaction = sm_dbConnection.BeginTransaction();
            SqliteDataReader reader = sm_dbCommand.ExecuteReader();
            ReadValuesFromDatabase(reader, out readValues);
            //sm_dbTransaction.Commit();
            Logger.Instance.AddMessage(query);
        }


        /// <summary>
        /// Use this function to continuously write to the database. Do not call this function in the Update method.
        /// </summary>
        public void WriteContinuous(Action write)
        {
            //check if the event already contains the action
            if (onWriteToDatabaseContinuous != null)
            {
                Delegate[] methods = onWriteToDatabaseContinuous.GetInvocationList();
                foreach (Delegate method in methods)
                {
                    if (method.Method.Name.Equals(write.Method.Name))
                        return;
                }
            }
            onWriteToDatabaseContinuous += write;
        }

        /// <summary>
        /// Use this function to write to the database once.
        /// </summary>
        public void WriteOnce(Action write)
        {
            //check if the event already contains the action
            if (onWriteToDatabaseOnce != null)
            {
                Delegate[] methods = onWriteToDatabaseOnce.GetInvocationList();
                foreach (Delegate method in methods)
                {
                    if (method.Method.Name.Equals(write.Method.Name))
                        return;
                }
            }
            onWriteToDatabaseOnce += write;
        }


        /// <summary>
        /// Call this function to read data from the table with select mode.
        /// </summary>
        /// <param name="callback"> Function to be called after data is loaded.</param>
        public void ReadData(string tableName, SelectFromDatabaseMode selectMode, Action<List<List<DataEntry>>> callback,
            SortResultsBy sort = SortResultsBy.none, TableColumn SortColumn = null, string columnNames = "", int minRow = 0, int maxRow = 0, string Condition = "")
        {
            //isReadingData = true;
            OnDataReady += callback;
            ThreadedReadFromDatabase(tableName, selectMode, sort, SortColumn, columnNames, minRow, maxRow, Condition);
            StartCoroutine(RetrieveData());
        }

        /// <summary>
        /// Waits for the last update of the database, then waits for the data to be ready,
        /// then invokes the OnDataReady action. 
        /// OnDataReady must be called on the main thread, so it cannot be called in the ReadValuesFromDatabase function.
        /// </summary>
        private IEnumerator RetrieveData()
        {
            //wait for the last update of the database before loading (otherwise the data might be a bit outdated)
            yield return new WaitForSeconds(m_writeFrequency);
            //wait for data to be ready
            while (!isReadySync)
            {
                yield return null;
            }

            //data is ready, call action
            OnDataReady?.Invoke(data);

            //reset action
            OnDataReady = null;
        }
        //WIP
        public bool GetTableName(int index, out string name)
        {
            string query = "SELECT name FROM sqlite_master WHERE type = 'table';";
            name = string.Empty;
            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            //sm_dbTransaction = sm_dbConnection.BeginTransaction();
            SqliteDataReader reader = sm_dbCommand.ExecuteReader();

            int current = 0;
            while (reader.Read())
            {
                if (current == index)
                {
                    name = reader.GetString(0);
                    return true;
                }
                current++;
            }
            reader.Dispose();
            //sm_dbTransaction.Commit();
            //index was out of range
            return false;
        }

        public List<string> GetAllTableNames()
        {
            List<string> names = new List<string>();
            string query = "SELECT name FROM sqlite_master WHERE type = 'table';";
            name = string.Empty;
            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            //sm_dbTransaction = sm_dbConnection.BeginTransaction();
            SqliteDataReader reader = sm_dbCommand.ExecuteReader();

            while (reader.Read())
            {
                names.Add(reader.GetString(0));
            }
            reader.Dispose();
            return names;
        }

        /// <summary>
        /// Returns the last rowid in the table.
        /// The rowid might be greater than the row number, if the are limited entries in the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private int LastIDInTable(string tableName)
        {
            if (!CheckIfTableExists(tableName))
            {
                Logger.Instance.AddMessage(string.Format("Attempt to read from table {0} but it doesnt exist.", tableName));
                return -1;
            }
            //create database command
            string query = string.Format("SELECT MAX(rowid) FROM {0};", tableName);
            //create command

            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            //sm_dbTransaction = sm_dbConnection.BeginTransaction();

            return Convert.ToInt32(sm_dbCommand.ExecuteScalar());
        }

        public void GetColumnNamesOnTable(string tableName, out TableRow row)
        {
            row = new TableRow();

            if (tableName == null || tableName.Equals("") || !CheckIfTableExists(tableName))
                return;

            //this sql command will show names and if the columns are primary keys
            string query = string.Format("PRAGMA table_info({0});", tableName);
            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            SqliteDataReader reader = sm_dbCommand.ExecuteReader();
            sm_dbCommand.Dispose();

            //this command will show if the columns are unique (if the 3rd value is not 0)
            query = string.Format("PRAGMA INDEX_LIST({0});", tableName);
            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            SqliteDataReader reader1 = sm_dbCommand.ExecuteReader();
            sm_dbCommand.Dispose();

            while (reader.Read())
            {
                if (reader1.Read())
                    row.AddColumn(new TableColumn(reader.GetString(1), reader.GetString(2), reader.GetBoolean(5), reader1.GetInt16(2) != 0));
                else
                    row.AddColumn(new TableColumn(reader.GetString(1), reader.GetString(2), reader.GetBoolean(5)));
            }

            reader1.Dispose();
            reader.Dispose();
            //sm_dbTransaction.Commit();
        }
        /// <summary>
        /// Updates values in the databse. Make sure ValueNames and NewValues are separated by commas.
        /// </summary>
        //public void UpdateValuesOnTable(string TableName, string ValueNames, string NewValues, string SearchCondition = "")
        //{
        //    string[] commaSeparatedNames = string.Create(ValueNames.Length, ValueNames, (chars, state) =>
        //    {
        //        state.AsSpan().CopyTo(chars);
        //    }).Split(",");

        //    string[] commaSeparatedValues = string.Create(NewValues.Length, NewValues, (chars, state) =>
        //    {
        //        state.AsSpan().CopyTo(chars);
        //    }).Split(",");

        //    if (commaSeparatedNames.Length != commaSeparatedValues.Length)
        //    {
        //        Debug.LogWarning("The amount of values to update are not the same as the amount of columns. Not updating.");
        //        return;
        //    }
        //    //create query
        //    string query = string.Format("UPDATE {0} SET ", TableName);
        //    for (int i = 0; i < commaSeparatedNames.Length; i++)
        //    {
        //        query += string.Format("{0}={1},\n", commaSeparatedNames[i], commaSeparatedValues[i]);
        //    }
        //    //remove last comma and newline
        //    query = query.Remove(query.Length - 2, 2);

        //    if (SearchCondition != "")
        //        query += string.Format("\nWHERE {0}", SearchCondition);

        //    dbCommand = new SqliteCommand(query, dbConnection, transaction);
        //    dbCommand.ExecuteReader();

        //    Logger.Instance.AddMessage(query);
        //}

        /// <summary>
        /// Updates values in the databse. Make sure ValueNames and NewValues are separated by commas.
        /// </summary>
        public void UpdateValuesOnTable(string TableName, string ValueNames, string NewValues, string SearchCondition = "")
        {
            if (!CheckIfTableExists(TableName))
            {
                return;
            }
            //view strings as spans to slice in the loop
            ReadOnlySpan<char> spanValues = NewValues.AsSpan();
            ReadOnlySpan<char> spanNames = ValueNames.AsSpan();

            int nextCommaIndexValues = 0;
            int nextCommaIndexNames = 0;
            bool isLastLoop = false;

            //initialize query
            string query = string.Format("UPDATE {0} SET ", TableName);

            while (!isLastLoop)
            {
                //find the index of the next occurance of comma in both strings
                int indexStartValues = nextCommaIndexValues;
                nextCommaIndexValues = NewValues.IndexOf(',', indexStartValues);

                int indexStartNames = nextCommaIndexNames;
                nextCommaIndexNames = ValueNames.IndexOf(',', indexStartNames);

                //check if last loop (-1 means end of string and both must be at the end)
                isLastLoop = (nextCommaIndexValues == -1) && (nextCommaIndexNames == -1);
                if (isLastLoop)
                {
                    //set to length to avoid out of range exceptions
                    nextCommaIndexNames = ValueNames.Length;
                    nextCommaIndexValues = NewValues.Length;
                }
                //take slices from span 
                ReadOnlySpan<char> nameSlice = spanNames.Slice(indexStartNames, nextCommaIndexNames - indexStartNames);
                ReadOnlySpan<char> valueSlice = spanValues.Slice(indexStartValues, nextCommaIndexValues - indexStartValues);

                //add to query
                query += string.Format("{0}={1},\n", nameSlice.ToString(), valueSlice.ToString());

                //move indeces by 1 to avoid being stuck in an infinite loop
                //values should have length more than 0
                nextCommaIndexNames++;
                nextCommaIndexValues++;
            }

            //remove last comma and newline
            query = query.Remove(query.Length - 2, 2);

            if (SearchCondition != "")
                query += string.Format("\nWHERE {0}", SearchCondition);

            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            //sm_dbTransaction = sm_dbConnection.BeginTransaction();

            sm_dbCommand.ExecuteReader().Dispose(); ;
            //sm_dbTransaction.Commit();

            Logger.Instance.AddMessage(query);
        }
        /// <summary>
        /// Used to delete rows in a table. If no condition is given, it will not do anything. Use with caution.
        /// </summary>
        public void DeleteRowsOnTable(string TableName, string Condition)
        {
            if (TableName == "" || Condition == "" || !CheckIfTableExists(TableName))
                return;

            string query = string.Format("DELETE FROM {0} WHERE {1}", TableName, Condition);
            sm_dbCommand = new SqliteCommand(query, sm_dbConnection, sm_dbTransaction);
            Logger.Instance.AddMessage(query);
        }

        public string GetDatabaseLocation()
        {
            return Path.Combine(Application.persistentDataPath, m_dbLocation, m_dbName);
        }
        #endregion // base functions

        #region Threaded functions
        /// <summary>
        /// Uses a separate thread to write to the database.
        /// </summary>
        public void ThreadedWriteToDatabase(string TableName, TableRow row, bool check = false, Action onEntyExists = null)
        {
            //this is necessary because otherwise the thread keeps spamming writes
            //sleep at the beginning to avoid null reference exception table row
            //(since this thread isnt the main thread, it might run before the main thread
            //actually calculates the values)
            Thread.Sleep((int)(m_writeFrequency * 1000));

            if (check)
                if (CheckIfEntryExists(TableName, row.GetValues())) { onEntyExists?.Invoke(); return; }

            WriteToDatabase(TableName, row);
            //write to log file
            string log = string.Format("Successfully wrote to database {0}, {1}, at {2}\n",
                    m_dbName, row.GetValues(), DateTime.Now.ToString("HH:mm:ss"));

            Logger.Instance.AddMessage(log);
        }

        ///// <summary>
        ///// Uses a separate thread to write to the database.
        ///// </summary>
        //public void ThreadedWriteToDatabase(string TableName, string ColumnNames, string values)
        //{
        //    //this is necessary because otherwise the thread keeps spamming writes
        //    //sleep at the beginning to avoid null reference exception on values string
        //    //(since this thread isnt the main thread, it might run before the main thread
        //    //actually calculates the values)
        //    Thread.Sleep((int)(m_writeFrequency * 1000));
        //    if (CheckIfEntryExists(TableName, values)) { return; }

        //    InsertMultipleColumnValueToTable(TableName, ColumnNames, values);
        //    //write to log file
        //    string log = string.Format("Successfully wrote to database {0}, {1}, at {2}\n",
        //            m_dbName, values, DateTime.Now.ToString("HH:mm:ss"));

        //    Logger.Instance.AddMessage(log);
        //} //TODO: consider refactoring this to a command pattern 

        /// <summary>
        /// Use  this function to create another table on the database.
        /// Don't forget to subscribe to onCreateTable event BEFORE calling this function. Event will be cleared after calling.
        /// This function creates a new instance of the create table thread.
        /// </summary>
        public void ThreadedCreateTable(Action createTable)
        {
            CreateTable += createTable;
            createTableThread = new Thread(() =>
            {
                ExecuteCreateTable();
            });
            createTableThread.Start();
        }

        /// <summary>
        /// Creates a new instance of a thread to read data from table with select mode.
        /// </summary>
        private void ThreadedReadFromDatabase(string tableName, SelectFromDatabaseMode selectMode, SortResultsBy sort = SortResultsBy.none,
            TableColumn SortColumn = null, string ColumnNames = "", int minRow = 0, int maxRow = 0, string Condition = "")
        {
            readThread = new Thread(() =>
            {
                data = new List<List<DataEntry>>();
                ReadFromDatabase(tableName, selectMode, out data, sort, SortColumn, ColumnNames, minRow, maxRow, Condition);
            });
            readThread.Start();
        }

        #endregion // threaded functions

        #region Tasks
        //public async Task<List<List<DataEntry>>> ReadFromDatabaseTask(string TableName, SelectFromDatabaseMode mode, string ColumnNames = "", int minRow = 0, int maxRow = 0)
        //{
        //    return await Task.Run(() =>
        //    {
        //        ReadFromDatabase(TableName, mode, out List<List<DataEntry>> objs, ColumnNames, minRow, maxRow);
        //        Debug.Log(string.Format("Read values from table {0} from colums {1}", TableName, ColumnNames));
        //        return objs;
        //    });
        //    //return result;
        //}

        //private Task WriteToDatabaseTask(string TableName, string ColumnNames, string values)
        //{
        //    return Task.Run(() =>
        //    {
        //        InsertMultipleColumnValueToTable(TableName, ColumnNames, values);
        //        Debug.Log(string.Format("Wrote values to table {0} to colums {1}", TableName, ColumnNames));
        //    });
        //}

        //private Task CreateTableTask(string TableName, string columns)
        //{
        //    return Task.Run(() =>
        //    {
        //        CreateTableOnDatabase(TableName, columns);
        //        Debug.Log(string.Format("Created table {0} with columns {1} at database {2}", TableName, columns));
        //    });
        //}

        //private Task DeleteTableTask(string path, string tableName)
        //{
        //    return Task.Run(() =>
        //    {
        //        DeleteTableFromDatabase(tableName);
        //        Debug.Log(string.Format("Deleted table {0} from database {1}", tableName, path));
        //    });
        //}


        //#endregion // tasks

        //#region Async functions
        //public async Task<List<List<DataEntry>>> ReadFromDatabaseAsync(string TableName, SelectFromDatabaseMode mode, string ColumnNames = "", int minRow = 0, int maxRow = 0)
        //{
        //    return await ReadFromDatabaseTask(TableName, mode, ColumnNames, minRow, maxRow);
        //}

        //public async void InsertToDatabaseAsync(string TableName, string ColumnNames, string values)
        //{
        //    await WriteToDatabaseTask(TableName, ColumnNames, values);
        //}

        //public async void CreateTableAsync(string TableName, string columns)
        //{
        //    await CreateTableTask(TableName, columns);
        //}

        //public async void DeleteTableAsync(string path, string TableName)
        //{
        //    await DeleteTableTask(path, TableName);
        //}
        #endregion

        #region Format values for table insert
        /// <summary>
        /// Formats values of any type in a string separated by commas.
        /// </summary>
        //public static string FormatValues<T>(T[] values)
        //{
        //    string result = string.Empty;
        //    for (int i = 0; i < values.Length; i++)
        //    {
        //        result += values[i].ToString() + ",";
        //    }

        //    result = result.Remove(result.Length - 1);
        //    return result;
        //}

        public static string FormatValues<T>(T[] values)
        {
            string result = string.Empty;

            for (int i = 0; i < values.Length; i++)
            {
                //for strings, we need to add ""
                if (values[i].GetType() == typeof(string))
                    result += string.Format("\"{0}\",", values[i].ToString().Replace(',', '.')); //result will be a string which will contain "" 
                else
                    result += string.Format("{0},", values[i].ToString().Replace(',', '.'));

                if ((i + 1) % values.Length == 0)
                {
                    result = result.Remove(result.Length - 1);
                    result += "\n";
                }
            }

            result = result.Remove(result.Length - 1);

            return result;
        }
        #endregion

        #region Internal functions


        private void StartThreads()
        {
            //initialize threads
            writeThread = new Thread(() =>
            {
                while (true)
                {
                    //sometimes, the following delegates do not get executed because this thread runs too frequently
                    //for that, make it wait a bit between runs
                    Thread.Sleep((int)(m_writeFrequency * 1000)); //write frequency is in seconds and the function expects millisecons
                    ExecuteWriteToDatabaseContinuous();
                    ExecuteWriteToDatabaseOnce();
                }
            });

            writeThread.Start();
        }

        /// <summary>
        /// Stops running background threads and sets them to null.
        /// </summary>
        private void StopThreads()
        {
            //abort to stop
            //set to null to clear

            writeThread?.Abort();
            writeThread = null;

            readThread?.Abort();
            readThread = null;

            createTableThread?.Abort();
            createTableThread = null;
        }

        private void CheckNumberOfRows(SqliteConnection connection, string tableName)
        {
            if (!CheckIfTableExists(tableName))
            { return; }
            //this query returns the amount of entries in the table 
            string query = string.Format("SELECT COUNT(rowid) FROM {0};", tableName);
            //create command
            SqliteCommand dbCommand = new SqliteCommand(query, connection);
            //get the number of rows
            int numRows = Convert.ToInt16(dbCommand.ExecuteScalar());

            //check number of rows against max rows allowed
            if (numRows > m_maxRowsAllowed)
            {
                Debug.Log("Rows are more than allowed, deleting the first entry");
                query = string.Format("DELETE FROM {0} WHERE rowid IN (SELECT rowid FROM {0} LIMIT 1);", tableName);
                dbCommand = new SqliteCommand(query, connection);
                dbCommand.ExecuteReader().Dispose();
            }

        }

        /// <summary>
        /// Creates database if it doesnt exist and opens the connection the the database.
        /// </summary>
        /// <returns>Returns the connection to be used in the other functions.</returns>
        private SqliteConnection CreateConnectionToDB(string pathToDatabase = null)
        {
            //if empty string, default to connection
            if (pathToDatabase == null)
                pathToDatabase = sm_CurrentConnection;

            if (!pathToDatabase.StartsWith("URI=file:"))
            {
                pathToDatabase = string.Format("URI=file:{0}", pathToDatabase);
            }

            SqliteConnection dbConnection = new SqliteConnection(pathToDatabase);

            return dbConnection;
        }

        private void CloseConnection()
        {
            sm_dbTransaction.Commit();
            sm_dbTransaction.Dispose();
            sm_dbTransaction = null;

            sm_dbConnection.Close();
            sm_dbConnection.Dispose();
            sm_dbConnection = null;
        }

        void ReadValuesFromDatabase(SqliteDataReader reader, out List<List<DataEntry>> entries)
        {
            isReadySync = false;
            entries = new List<List<DataEntry>>();

            if (reader == null)
            {
                isReadySync = true;
                return;
            }

            int i = 0;

            while (reader.Read())
            {
                entries.Add(new List<DataEntry>());
                for (int j = 0; j < reader.FieldCount; j++)
                {
                    object v = reader.GetValue(j);
                    entries[i].Add(new DataEntry(v));
                }
                i++;
            }
            if (entries.Count > 0)
                Debug.Log($"Read {i} rows, each containing {entries[0].Count} columns.");

            reader.Dispose();
            isReadySync = true;
            //isReadingData = false;
        }

        string ReadOnlyValuesFromDatabase(SqliteDataReader reader)
        {
            string result = string.Empty;
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    result += string.Format("{0} ", reader.GetValue(i));
                    //add new line on the end
                    if ((i + 1) % reader.FieldCount == 0)
                    {
                        result += "\n";
                    }
                }
            }

            result = result.Remove(result.Length - 2);
            reader.Dispose();
            return result;
        }
        #endregion // internal functions 

        #region Multi connection handling
        /// <summary>
        /// Adds connection to the connections dictionary.
        /// </summary>
        /// <returns>The hashcode of the connection.</returns>
        public int AddConnection(string databaseName)
        {
            if (!databaseName.EndsWith(".db"))
            {
                databaseName = string.Format("{0}.db", databaseName);
            }
            string fullpath = Path.Combine(Application.persistentDataPath, sm_dbLocation, databaseName);
            int hash = fullpath.GetHashCode();

            if (!sm_Connections.ContainsKey(hash))
                sm_Connections.Add(hash, string.Format("URI=file:{0}", fullpath));

            return hash;
        }

        /// <summary>
        /// Stops current connection and connects to another database, if it exists.
        /// </summary>
        /// <param name="hash"></param>
        public void ChangeConnection(int hash)
        {
            if (!sm_Connections.ContainsKey(hash))
            {
                Debug.LogWarning($"There is no such key in the connections dictionary {hash}");
                return;
            }

            StopThreads();

            //close previous connection if open
            if (sm_dbConnection != null && sm_dbConnection.State == ConnectionState.Open)
                CloseConnection();

            //create new connection
            sm_CurrentConnection = sm_Connections[hash];

#if UNITY_EDITOR
            //display change in the editor
            ReadOnlySpan<char> conn = sm_CurrentConnection;
            int idx = conn.LastIndexOf('\\') + 1;
            m_dbName = conn.Slice(idx, conn.Length - idx).ToString();
#endif
            Debug.Log(sm_CurrentConnection);
            sm_dbConnection = CreateConnectionToDB(sm_CurrentConnection);

            //open communication
            sm_dbConnection.Open();
            sm_dbTransaction = sm_dbConnection.BeginTransaction();
            Debug.Log($"Connection with {sm_CurrentConnection} opened, transaction started (hash {hash}).");

            //restart threads
            StartThreads();
        }

        #endregion

        private void OnApplicationQuit()
        {
            Dispose();
        }

        public void Dispose()
        {
            //stop threads first to avoid crashes due to dll (Thread.Join() makes the application hang)
            StopThreads();

            //clear events
            onWriteToDatabaseContinuous = null;
            onWriteToDatabaseOnce = null;
            CreateTable = null;
            OnDataReady = null;

            CloseConnection();
            Destroy(gameObject);
        }
    }

 
}