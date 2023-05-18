using System.Collections;
using UnityEngine;
using UnitySQLite;
using UnitySQLite.Utilities;

/// <summary>
/// This class holds all the table templates that are used throught the app.
/// </summary>
public class CreateTables : MonoBehaviour
{
    public TableRow users;
    public TableRow machineryList;
    public TableRow updateMachinery;
    public TableRow departmentsList;
    public TableRow systemsList;

    void Start()
    {
        DatabaseManager.Instance.Initialize("Databases", "Machinery");
        DatabaseManager.Instance.onDatabaseCreated += () =>
        {
            StartCoroutine(CreateWithDelay(0.2f));
        };

        //yield return new WaitForEndOfFrame();
        //Account.CreateDefaultAccounts();
    }

    private IEnumerator CreateWithDelay(float delay)
    {
        DatabaseManager.Instance.CreateTableOnDatabase("Users", users.GetColumns());
        DatabaseManager.Instance.CreateTableOnDatabase("DepartmentsList", departmentsList.GetColumns());
        DatabaseManager.Instance.CreateTableOnDatabase("SystemsList", systemsList.GetColumns());
        DatabaseManager.Instance.CreateTableOnDatabase("MachineryList", machineryList.GetColumns());
        yield return new WaitForSeconds(delay);
        Account.CreateDefaultAccounts();
        yield return new WaitForSeconds(delay);
        Account.CreateDefaultDepts();
    }
}