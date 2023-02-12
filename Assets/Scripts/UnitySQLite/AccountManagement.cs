using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnitySQLite.Utilities;
using TMPro;
using UnityEngine.UI;

namespace UnitySQLite
{
    public class AccountManagement : GenericSingleton<AccountManagement>
    {
        [Header("User data")]
        [SerializeField]
        public string DatabaseName;
        public Account _currentAccount;
        public static Action<Account> onAccountGet;
        public static Action<bool, string> onLoginAttempt;

        public TMP_InputField username, password, output;
        public Button login, createAccount;

        public TableRow content;
        public override void Awake()
        {
            base.Awake();

           // onLoginAttempt += (b, text) => output.text = text;
            onAccountGet += (acc) => _currentAccount = acc;
            login.onClick.AddListener(() => Login(username.text, password.text));
            createAccount.onClick.AddListener(() =>
            {
                Account acc;
                if (username.text != "" & password.text != "")
                    acc = new Account(username.text, password.text, (int)_currentAccount.Dept, (int)_currentAccount.AccessLevel);
            });
        }

        //private void Start()
        //{
        //    DatabaseManager.Instance.Initialize("Databases", DatabaseName);
            
        //    DatabaseManager.Instance.CreateTableOnDatabase("UserData", content.GetColumns());    
        //} 

        public void SetCurrentAccount(int id)
        {
            StartCoroutine(GetAccount(id));
        }

        public void SetCurrentAccount(string username)
        {
            StartCoroutine(GetAccount(username));
        }

        public void Login(string username, string password)
        {
            if (username == string.Empty | password == string.Empty)
                return;

            onAccountGet += (account) =>
            {
                string hash = Account.CreateSHA256(password, account.Salt);
                Account.ValidateCredentials(username, hash, account);
            };
            SetCurrentAccount(username);
        }

        private IEnumerator GetAccount(int ID)
        {
            _currentAccount = null;
            Account.RetrieveAccount(ID);
            while (Account.CurrentAccount == null)
            {
                yield return null;
            }
#if UNITY_EDITOR
            _currentAccount = Account.CurrentAccount;
#endif
            onAccountGet?.Invoke(Account.CurrentAccount);
            onAccountGet = null;
        }

        private IEnumerator GetAccount(string username)
        {
            _currentAccount = null;
            Account.RetrieveAccount(username);
            while (Account.CurrentAccount == null)
            {
                yield return null;
            }
#if UNITY_EDITOR
            _currentAccount = Account.CurrentAccount;
#endif
            onAccountGet?.Invoke(Account.CurrentAccount);
            onAccountGet = null;
        }

    }


}

