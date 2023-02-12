using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySQLite;
using UnitySQLite.Utilities;

public class CreateTables : MonoBehaviour
{
    public TableRow users;
    public TableRow machineryList; // this will have to be remade in the future

    private void Start()
    {
        DatabaseManager.Instance.Initialize("Databases", "Machinery");
       // DatabaseManager.Instance.CreateTableOnDatabase("Users", users.GetColumns());
        DatabaseManager.Instance.CreateTableOnDatabase("Machinery list", machineryList.GetColumns());
    }
}
