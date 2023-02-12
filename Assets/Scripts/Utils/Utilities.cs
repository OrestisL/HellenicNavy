using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace UnitySQLite.Utilities
{
    public enum SelectFromDatabaseMode
    {
        /// <summary>
        /// Selects the whole table
        /// </summary>
        everything = 0,
        /// <summary>
        /// Selects specific columns
        /// </summary>
        specificColumns,
        /// <summary>
        /// Selects specific rows
        /// </summary>
        specificRows
    }

    public enum SortResultsBy
    {
        none = 0,
        ASC,
        DESC,
    }

    [Serializable]
    public class TableColumn
    {
        [SerializeField]
        private string columnName;
        [SerializeField]
        private string columnType;
        [SerializeField]
        private bool isUnique;
        [SerializeField]
        private bool isPrimaryKey;
        public string ColumnName { get { return columnName; } }
        public string ColumnType { get { return columnType; } }
        public bool Unique { get { return isUnique; } }
        public bool Primary { get { return isPrimaryKey; } }

        public TableColumn(string cName, string cType, bool primary = false, bool unique = false)
        {
            columnName = cName;
            columnType = cType;
            isPrimaryKey = primary;
            isUnique = unique;
        }

        public override string ToString()
        {
            string final = string.Format("{0} {1}", columnName, columnType);

            if (isUnique)
                final = string.Format("{0} UNIQUE", final);
            if (isPrimaryKey)
                final = string.Format("{0} PRIMARY KEY", final);

            return final;
        }
    }

    [Serializable]
    public class TableRow
    {
        public List<TableColumn> columns;
        public List<DataEntry> rowEntries;

        public TableRow()
        {
            columns = new List<TableColumn>();
            rowEntries = new List<DataEntry>();
        }

        public TableRow(TableColumn[] cols)
        {
            columns = cols.ToList();
        }

        public TableRow(List<TableColumn> cols)
        {
            columns = cols;
        }

        public void AddColumn(TableColumn col)
        {
            columns.Add(col);
        }

        public void AddValues(DataEntry[] entries)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].GetTypeName() != columns[i].ColumnType)
                {
                    if (columns[i].ColumnType.Equals("REAL") && (entries[i].GetTypeName().Equals("FLOAT") | entries[i].GetTypeName().Equals("DOUBLE")))
                        continue;

                    throw new Exception(string.Format("Attempted to insert value type {0} when it should be {1} at position {2}",
                        entries[i].GetTypeName(), columns[i].ColumnType, i));
                }

            }
            rowEntries = new List<DataEntry>(entries.Length);
            rowEntries = entries.ToList();
        }

        public TableRow(TableColumn[] cols, DataEntry[] entries)
        {
            columns = cols.ToList();
            rowEntries = entries.ToList();
        }

        public TableRow(List<TableColumn> cols, List<DataEntry> entries)
        {
            columns = cols;
            rowEntries = entries;
        }

        public string GetColumnNames()
        {
            string names = string.Empty;
            for (int i = 0; i < columns.Count; i++)
            {
                names += string.Format("{0},", columns[i].ColumnName);
            }
            names = names.Remove(names.Length - 1);

            return names;
        }

        public string GetColumns()
        {
            string cols = string.Empty;
            for (int i = 0; i < columns.Count; i++)
            {
                cols += string.Format("{0},", columns[i].ToString());
            }
            cols = cols.Remove(cols.Length - 1);

            return cols;
        }

        public string GetValuesAndTypes()
        {
            string values = string.Empty;
            for (int i = 0; i < rowEntries.Count; i++)
            {
                values += string.Format("{0},", rowEntries[i].ToString());
            }
            values = values.Remove(values.Length - 1);
            return values;
        }

        public string GetValues()
        {
            string values = string.Empty;
            for (int i = 0; i < rowEntries.Count; i++)
            {
                values += string.Format("{0},", rowEntries[i].GetValue().Replace(",", "."));
            }
            values = values.Remove(values.Length - 1);
            return values;
        }

        public static bool operator ==(TableRow left, TableRow right)
        {
            if (left is null) { return right is null; }
            if (left.columns.Count != right.columns.Count) { return false; }

            for (int i = 0; i < left.columns.Count; i++)
            {
                if (!left.columns[i].ColumnType.Equals(right.columns[i].ColumnType) &&
                    !left.columns[i].ColumnName.Equals(right.columns[i].ColumnName)) { return false; }
            }

            return true;
        }

        public static bool operator !=(TableRow left, TableRow right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return obj is TableRow right ? (columns == right.columns && rowEntries == right.rowEntries) : false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(columns, rowEntries);
        }
    }

    public struct DataEntry
    {
        public readonly float SingleValue;
        public readonly double DoubleValue;
        public readonly string StringValue;
        public readonly int IntegerValue;

        public DataEntry(object entry, Type t)
        {
            SingleValue = float.MinValue;
            DoubleValue = double.MinValue;
            StringValue = null;
            IntegerValue = int.MinValue;

            if (t == typeof(int))
            {
                IntegerValue = Convert.ToInt32(entry);
            }
            else if (t == typeof(double) | t == typeof(float))
            {
                DoubleValue = Convert.ToDouble(entry);
                SingleValue = Convert.ToSingle(entry);
            }
            else if (t == typeof(string))
            {
                StringValue = Convert.ToString(entry);
            }
        }

        public DataEntry(object entry)
        {
            SingleValue = float.MinValue;
            DoubleValue = double.MinValue;
            StringValue = null;
            IntegerValue = int.MinValue;

            Type t = entry.GetType();

            if (t == typeof(int))
            {
                IntegerValue = Convert.ToInt32(entry);
            }
            else if (t == typeof(double) | t == typeof(float))
            {
                DoubleValue = Convert.ToDouble(entry);
                SingleValue = Convert.ToSingle(entry);
            }
            else if (t == typeof(string))
            {
                StringValue = Convert.ToString(entry);
            }
        }

        public override string ToString()
        {
            string value = "{0} {1}";
            if (SingleValue != float.MinValue) { value = string.Format(value, SingleValue, "FLOAT"); }
            else if (DoubleValue != double.MinValue) { value = string.Format(value, DoubleValue, "DOUBLE"); }
            else if (StringValue != null) { value = string.Format("\"{0}\" {1}", StringValue, "TEXT"); }
            else if (IntegerValue != int.MinValue) { value = string.Format(value, IntegerValue, "INT"); }

            return value;
        }

        public string GetValue()
        {
            string value = "{0}";
            if (SingleValue != float.MinValue) { value = string.Format(value, SingleValue); }
            else if (DoubleValue != double.MinValue) { value = string.Format(value, DoubleValue); }
            else if (StringValue != null) { value = string.Format("\"{0}\"", StringValue); }
            else if (IntegerValue != int.MinValue) { value = string.Format(value, IntegerValue); }

            return value;
        }
        public string GetTypeName()
        {
            string type = string.Empty;
            if (SingleValue != float.MinValue) { type = "FLOAT"; }
            else if (DoubleValue != double.MinValue) { type = "DOUBLE"; }
            else if (StringValue != null) { type = "TEXT"; }
            else if (IntegerValue != int.MinValue) { type = "INT"; }

            return type;
        }
    }

    [Serializable]
    public class Account
    {
        [SerializeField]
        private int _UID;
        [SerializeField]
        private string _accountName, _accountPasswordHash, _salt, _email, _creationDate;
        public string AccountName { get { return _accountName; } }
        public string Salt { get { return _salt; } }
        public string Email { get { return _email; } }
        public string CreationDate { get { return _creationDate; } }

        private static Account _currentAccount;
        public static Account CurrentAccount { get { return _currentAccount; } }


        public static Action onEntryExists = delegate
        {
            //probably needs to show some message somewhere
            Debug.Log("Username already exists");
            AccountManagement.onLoginAttempt(false, "Username already exists");
        };
        public static Action onAccountNotExists = delegate
        {
            //again this should show a popup or something
            Debug.Log("Account username does not exist");
            AccountManagement.onLoginAttempt(false, "Account username does not exist");
        };
        public static Action<bool> onValidateCredentials;

        public Account()
        {
            _UID = 0;
            _accountName = null;
            _accountPasswordHash = null;
            _email = null;
        }

        public Account(string accountName, string accountPassword, string email)
        {
            _creationDate = DateTime.Now.ToString("yyyy-MM-dd").ToString();
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[16];
                rng.GetNonZeroBytes(bytes);
                _salt = bytes.ByteArrayToString();
            }

            _accountName = accountName;
            _accountPasswordHash = CreateSHA256(accountPassword, _salt);
            _email = email;

            CreateAccount();
        }

        private Account(int UID, string name, string pass, string salt, string email, string date)
        {
            _UID = UID;
            _accountName = name;
            _accountPasswordHash = pass;
            _salt = salt;
            _email = email;
            _creationDate = date;
        }

        public static void RetrieveAccount(int currentUID)
        {
            _currentAccount = null;
            DatabaseManager.Instance.ReadData("UserData", SelectFromDatabaseMode.everything,
                                                SaveRetrievedAccount, SortResultsBy.none, null, "", 0, 0, $"WHERE UID = {currentUID}");
        }

        public static void RetrieveAccount(string username)
        {
            _currentAccount = null;
            DatabaseManager.Instance.ReadData("UserData", SelectFromDatabaseMode.everything,
                                                SaveRetrievedAccount, SortResultsBy.none, null, "", 0, 0, $"WHERE Username = \'{username}\'");
        }

        private static void SaveRetrievedAccount(List<List<DataEntry>> data)
        {
            if (data == null || data.Count == 0)
            {
                onAccountNotExists.Invoke();
                return;
            }
            int idx = data.Count - 1;
            _currentAccount = new Account(
                                   data[idx][0].IntegerValue,
                                   data[idx][1].StringValue,
                                   data[idx][2].StringValue,
                                   data[idx][3].StringValue,
                                   data[idx][4].StringValue,
                                   data[idx][5].StringValue);
        }

        /// <summary>
        /// Create account on database if not exists. 
        /// First get UID and increase it by 1, then create.
        /// </summary>
        private void CreateAccount()
        {
            DatabaseManager.Instance.ReadData("UserData", SelectFromDatabaseMode.everything, (data) =>
            {
                if (data.Count > 0)
                    _UID = data[data.Count - 1][0].IntegerValue + 1;
                else
                    _UID = 0;

                List<DataEntry> content = new List<DataEntry>
                {
                    new DataEntry(_UID),
                    new DataEntry(_accountName),
                    new DataEntry(_accountPasswordHash),
                    new DataEntry(_salt),
                    new DataEntry(_email),
                    new DataEntry(DateTime.Now.ToString("yyyy-MM-dd"))
                };

                List<TableColumn> cols = AccountManagement.Instance.content.columns;
                TableRow row = new TableRow(cols, content);

                DatabaseManager.Instance.WriteOnce(() =>
                {
                    DatabaseManager.Instance.ThreadedWriteToDatabase("UserData", row, true, Account.onEntryExists);
                });
            });
        }

        public static bool ValidateCredentials(string username, string passwordHash, Account account)
        {
            bool valid = false;
            if (username.Equals(account._accountName) && passwordHash.Equals(account._accountPasswordHash))
            {
                valid = true;
            }

            onValidateCredentials += (v) => Debug.Log(string.Format("{0} when logging in", v ? "success" : "failure"));

            onValidateCredentials.Invoke(valid);
            AccountManagement.onLoginAttempt?.Invoke(valid, string.Format("{0} when logging in", valid ? "success" : "failure"));
            onValidateCredentials = null;
            return valid;
        }

        public override string ToString()
        {
            string s = string.Format("UID:{0}\nusername:{1}\nemail:{2}\ncreation date:{3}", _UID, _accountName, _email, _creationDate);
            Debug.Log(s);
            return s;
        }

        public static string CreateSHA256(string data, string salt)
        {
            string toHash = data + salt;
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(toHash.SerializeToByteArray());
                return bytes.ByteArrayToString();
            }
        }
    }

}
