using UnityEngine;
using UnitySQLite;
using UnitySQLite.Utilities;

public class PasswordReset : MonoBehaviour
{
    void Start()
    {
        //no need to keep this script running after logging in
        AccountManagement.onSuccessfulLogin += (acc) => this.enabled = false;
        //might need to re enable after logging out though
        AccountManagement.onLogout += () => this.enabled = true;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) 
        {
            if (Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKey(KeyCode.U))
                {
                    Account.ResetAccountPassword("user");
                    Debug.Log("Reset password for user");
                }
                else if (Input.GetKey(KeyCode.LeftAlt) | Input.GetKey(KeyCode.RightAlt)) 
                {
                    if (Input.GetKey(KeyCode.A))
                    {
                        Account.ResetAccountPassword("admin");
                        Debug.Log("Reset password for admin");
                    }
                }
            }
        }
    }
}
