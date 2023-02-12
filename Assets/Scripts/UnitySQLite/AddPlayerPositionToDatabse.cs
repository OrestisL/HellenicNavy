using UnityEngine;
using System.Collections.Generic;
using System;
using UnitySQLite;
using UnitySQLite.Utilities;

public class AddPlayerPositionToDatabse : MonoBehaviour
{
    public string TableName, TableName1;
    private float[] position;
    public TableRow contents;
    private string columnNames;
    private float timer = .2f;
    private float _timer;
    public Vector3 test;
    [TextArea(20, 30)]
    public List<List<DataEntry>> s;
    object v;
    public LineRenderer line;
    string formattedValues;

    private void Start()
    {
        DatabaseManager.Instance.Initialize("Databases", "positions");

        _timer = timer;
        //string contentsFormatted = DatabaseManager.FormatValues(contents);

        DatabaseManager.Instance.ThreadedCreateTable(() =>
        {
            DatabaseManager.Instance.CreateTableOnDatabase(TableName, contents.GetColumns());
            DatabaseManager.Instance.CreateTableOnDatabase(TableName1, contents.GetColumns());
        });



        GameObject.Find("Read").GetComponent<UnityEngine.UI.Button>().onClick.
            AddListener(() =>
            {
                //read specific column (whole column)
                //DatabaseManager.Instance.ReadData(TableName, DatabaseManager.SelectFromDatabaseMode.specificColumns, "X");
                //read specific column (with min and max) (make sure min and max are within the range of the table)
                //DatabaseManager.Instance.ReadData(TableName, DatabaseManager.SelectFromDatabaseMode.specificColumns, "X", 0, 10);
                //read specific rows (betwen min and max)
                //DatabaseManager.Instance.ReadData(TableName, DatabaseManager.SelectFromDatabaseMode.specificRows, "X", 0);
                //read everything
                DatabaseManager.Instance.ReadData(TableName, SelectFromDatabaseMode.everything, DrawPlayerPosition);
            });

        formattedValues = FormatValue();

        contents.AddValues(new DataEntry[]
           {
                new DataEntry(transform.position.x),
                new DataEntry(transform.position.y),
                new DataEntry(transform.position.z)
           });

        DatabaseManager.Instance.WriteContinuous(() =>
        {
            DatabaseManager.Instance.ThreadedWriteToDatabase(TableName, contents);
            DatabaseManager.Instance.ThreadedWriteToDatabase(TableName1, contents);
        });
    }


    private void Update()
    {
        _timer -= Time.deltaTime;
        contents.AddValues(new DataEntry[]
            {
                new DataEntry(transform.position.x),
                new DataEntry(transform.position.y),
                new DataEntry(transform.position.z)
            });
        if (_timer <= 0)
        {
            // dbManager.InsertToDatabaseAsync("", TableName, columnNames, FormatValue());
            _timer = timer;
        }
    }

    public void DrawPlayerPosition(List<List<DataEntry>> data)
    {
        line.positionCount = 0;
        Vector3[] positions = new Vector3[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            positions[i] = new Vector3(data[i][0].SingleValue, data[i][1].SingleValue, data[i][2].SingleValue);
        }

        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }
    public void DrawPlayerPosition(Span<DataEntry> data, int state)
    {
        line.positionCount = 0;
        Vector3[] positions = new Vector3[data.Length];
        for (int i = 0; i < data.Length; i += 3)
        {
            positions[i] = new Vector3(data[i].SingleValue, data[i + 1].SingleValue, data[i + 2].SingleValue);
        }

        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }

    string FormatValue()
    {
        position = new float[3] { transform.position.x, transform.position.y, transform.position.z };
        // Debug.Log(dbManager.FormatValuesForMultipleColumnInsert(position, 3));
        return DatabaseManager.FormatValues(position);
    }
}
//string s = 1 499,2546 1,08 501,257 
//s.Replace(",",".");
//string[] ss = s.Split(" ");
//for(i=1, i = ss.Length-1, i++)
//   try (float f = float.Parse(ss[i]);