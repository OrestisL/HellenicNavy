#define TESTING

using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnitySQLite.Utilities;
using TMPro;
using UnityEngine.UI;

namespace UnitySQLite
{
    /// <summary>
    /// Handles creating accounts, logging in and changing passwords.
    /// </summary>
    public class AccountManagement : GenericSingleton<AccountManagement>
    {
        [Header("User data")]
        [SerializeField]
        public string DatabaseName;
        [SerializeField]
        public Account CurrentAccount;
        public static Action<Account> onSuccessfulLogin;
        private static bool validLogin;
        public static Action<bool, Account> onAfterLogin;
        public static Action onLogout;
        public static Action<bool, string> onLoginAttempt = delegate (bool success, string message)
        {
            Debug.Log(string.Format("login attempt: {0}", message));
            if (!success)
                MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
                {
                    useRightButton = false,
                    useLeftButton = false,
                    showLabel = false,
                    mainText = "Λανθασμένος κωδικός πρόσβασης"
                });
        };

        public TMP_InputField usernameField, passwordField, output;
#if TESTING 
        public Button createAccount;
#endif
        public TableRow content;
        public override void Awake()
        {
            base.Awake();
            //consider added a check so as not to do this over and over
            Account.CreateDefaultAccounts();
            // onLoginAttempt += (b, text) => output.text = text;
            onSuccessfulLogin += (acc) => { CurrentAccount = acc; };

#if TESTING
            createAccount.onClick.AddListener(() =>
            {
                Account acc;
                if (usernameField.text != "" & passwordField.text != "")
                    acc = new Account(usernameField.text, passwordField.text, (int)CurrentAccount.AccessLevel);
            });
#endif
        }

        public void SetCurrentAccount(int id)
        {
            StartCoroutine(GetAccount(id));
        }

        public void SetCurrentAccount(string username)
        {
            StartCoroutine(GetAccount(username));
        }

        public void ClearCurrentAccount()
        {
            CurrentAccount = new Account();
        }

        public void Login()
        {
            string username = usernameField.text;
            string password = passwordField.text;
            if (username == string.Empty | password == string.Empty)
                return;

            onSuccessfulLogin += (account) =>
            {
                string hash = Account.CreateSHA256(password, account.Salt);
                validLogin = Account.ValidateCredentials(username, hash, account);
            };
            SetCurrentAccount(username);
        }

        private IEnumerator GetAccount(int ID)
        {
            CurrentAccount = null;
            Account.RetrieveAccount(ID);
            while (Account.CurrentAccount == null)
            {
                yield return null;
            }
#if UNITY_EDITOR
            CurrentAccount = Account.CurrentAccount;
#endif
            onSuccessfulLogin?.Invoke(Account.CurrentAccount);
            onSuccessfulLogin = null;
        }

        private IEnumerator GetAccount(string username)
        {
            CurrentAccount = null;
            Account.RetrieveAccount(username);
            while (Account.CurrentAccount == null)
            {
                yield return null;
            }

            CurrentAccount = Account.CurrentAccount;

            onSuccessfulLogin?.Invoke(Account.CurrentAccount);
            onAfterLogin?.Invoke(validLogin, Account.CurrentAccount);
            onSuccessfulLogin = null;
        }

        public void ChangeInterfaceLayout(InterfaceManager.LoginInterfaceSetup setup)
        {
            switch (setup)
            {
                case InterfaceManager.LoginInterfaceSetup.login:
                    passwordField.transform.parent.gameObject.SetActive(true);
                    usernameField.transform.parent.gameObject.SetActive(true);
                    break;
                case InterfaceManager.LoginInterfaceSetup.changePW:
                    passwordField.transform.parent.gameObject.SetActive(true);
                    usernameField.transform.parent.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }

}

