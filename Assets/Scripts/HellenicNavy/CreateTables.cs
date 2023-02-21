using System.Collections;
using UnityEngine;
using UnitySQLite;
using UnitySQLite.Utilities;

public class CreateTables : MonoBehaviour
{
    public TableRow users;
    public TableRow machineryList;
    public TableRow systemsList;


    IEnumerator Start() 
    {
        DatabaseManager.Instance.Initialize("Databases", "Machinery");
        DatabaseManager.Instance.CreateTableOnDatabase("Users", users.GetColumns());
        DatabaseManager.Instance.CreateTableOnDatabase("Systems List", systemsList.GetColumns());
        DatabaseManager.Instance.CreateTableOnDatabase("Machinery List", machineryList.GetColumns());
        yield return new WaitForEndOfFrame();
        Account.CreateDefaultAccounts();
    }
}
