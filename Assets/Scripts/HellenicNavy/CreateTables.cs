using System.Collections;
using UnityEngine;
using UnitySQLite;
using UnitySQLite.Utilities;

public class CreateTables : MonoBehaviour
{
    public TableRow users;
    public TableRow machineryList;
    public TableRow systemsDepartmentsList;


    IEnumerator Start() 
    {
        DatabaseManager.Instance.Initialize("Databases", "Machinery");
        DatabaseManager.Instance.CreateTableOnDatabase("Users", users.GetColumns());
        DatabaseManager.Instance.CreateTableOnDatabase("SystemsDepartmentsList", systemsDepartmentsList.GetColumns());
        DatabaseManager.Instance.CreateTableOnDatabase("MachineryList", machineryList.GetColumns());
        yield return new WaitForEndOfFrame();
        Account.CreateDefaultAccounts();
    }
}
