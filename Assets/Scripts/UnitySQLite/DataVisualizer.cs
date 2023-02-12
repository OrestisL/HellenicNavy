using System.Collections.Generic;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnitySQLite.Utilities;

namespace UnitySQLite
{
    public class DataVisualizer : GenericSingleton<DataVisualizer>
    {
        public static float cellSize = 50;

        public GameObject columnPrefab;
        public GameObject entryPrefab;
        public GameObject rowPrefab;
        public GameObject scrolLRectPrefab;

        private GameObject defaultMessage;
        private TextMeshProUGUI defaultMessageText;

        private Action onVisualizerReady;

        private RectTransform visualizerRect;
        private RectTransform rowParent;
        private RectTransform rowOfNames;

        private ScrollRect scrollRect;
        private Scrollbar scrollbar;

        public override void Awake()
        {
            //intialize singleton
            base.Awake();

            visualizerRect = GetComponent<RectTransform>();

            //make sure prefabs exist
            if (!rowPrefab)
            {
                rowPrefab = Resources.Load(Path.Combine("Visualizer", "Rows")) as GameObject;
            }

            if (!columnPrefab)
            {
                columnPrefab = Resources.Load(Path.Combine("Visualizer", "Column")) as GameObject;
            }

            if (!entryPrefab)
            {
                entryPrefab = Resources.Load(Path.Combine("Visualizer", "Entry")) as GameObject;
            }

            if (!scrolLRectPrefab)
            {
                scrolLRectPrefab = Resources.Load(Path.Combine("Visualizer", "ScrollRect")) as GameObject;
            }

            defaultMessage = transform.GetChild(0).gameObject;
            defaultMessageText = defaultMessage.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            defaultMessage.SetActive(false);

            scrollRect = Instantiate(scrolLRectPrefab, transform).GetComponent<ScrollRect>();
            scrollbar = scrollRect.GetComponentInChildren<Scrollbar>();

            rowOfNames = Instantiate(rowPrefab, transform).GetComponent<RectTransform>();
            rowParent = Instantiate(rowPrefab, transform).GetComponent<RectTransform>();

            ChangeVisibility(false);
        }

        public static void Visualize(string tableName, SortResultsBy sort = SortResultsBy.none, TableColumn SortColumn = null)
        {
            //hide default message
            Instance.defaultMessage.SetActive(false);

            //delete previous rows
            if (Instance.rowParent != null)
            {
                //destroy gameobject, not the rect
                Destroy(Instance.rowParent.gameObject);
            }

            //create new row parent
            Instance.rowParent = Instantiate(Instance.rowPrefab, Instance.transform).GetComponent<RectTransform>();

            if (Instance.rowOfNames != null)
            {
                //destroy gameobject, not the rect
                Destroy(Instance.rowOfNames.gameObject);
            }

            //create new row parent
            Instance.rowOfNames = Instantiate(Instance.rowPrefab, Instance.transform).GetComponent<RectTransform>();

            //names should be first
            Instance.rowOfNames.SetAsFirstSibling();

            Instance.ChangeVisibility(false);
            //first instantiate all columns
            //get names
            TableRow rowOfNames = new TableRow();
            DatabaseManager.Instance.GetColumnNamesOnTable(tableName, out rowOfNames);

            if (rowOfNames.columns.Count == 0)
            {
                //no data was read
                Instance.ExecuteVisualizerReady();
            }

            for (int i = 0; i < rowOfNames.columns.Count; i++)
            {
                string colName = rowOfNames.columns[i].ColumnName;
                GameObject names = Instantiate(Instance.columnPrefab, Instance.rowOfNames);
                GameObject empty = Instantiate(Instance.columnPrefab, Instance.rowParent);
                empty.name = colName;

                //add name child
                GameObject columnName = Instantiate(Instance.entryPrefab, names.transform);
                columnName.GetComponent<TMP_InputField>().text = colName;
            }

            //List<List<DataEntry>> entries = new List<List<DataEntry>>();
            DatabaseManager.Instance.ReadData(tableName, SelectFromDatabaseMode.everything,
                (data) =>
                {
                    //entries = data;
                    if (data.Count == 0)
                    {
                        Instance.ExecuteVisualizerReady();
                        Instance.ChangeDefaultMessage(string.Format("No data found in table {0}", tableName));
                        return;
                    }

                    Instance.StartCoroutine(AddEntries(data));
                },
                sort, SortColumn);
        }

        private static IEnumerator AddEntries(List<List<DataEntry>> entries)
        {
            bool ready = false;
            for (int i = 0; i < entries.Count; i++)
            {
                for (int j = 0; j < entries[i].Count; j++)
                {
                    Transform currentRect = Instance.rowParent.GetChild(j);
                    DataEntry currentDataEntry = entries[i][j];
                    GameObject entry = Instantiate(Instance.entryPrefab, currentRect);
                    entry.name = string.Format("Entry {0}-{1}", i, j);
                    TMP_InputField field = entry.GetComponent<TMP_InputField>();

                    //check which field actually has the correct value
                    if (currentDataEntry.StringValue != null)
                    {
                        field.text = currentDataEntry.StringValue;
                    }
                    else if (currentDataEntry.SingleValue != float.MinValue)
                    {
                        field.text = currentDataEntry.SingleValue.ToString().Replace(",", ".");
                    }
                    else if (currentDataEntry.IntegerValue != int.MinValue)
                    {
                        field.text = currentDataEntry.IntegerValue.ToString();
                    }

                    yield return null;
                }
            }
            ready = true;
            //after all the loops are done, move the rowParent inside the scroll rect and set viewport height
            //"Content" child is the first child of the first child ("Viewport")
            RectTransform content = Instance.scrollRect.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
            Instance.rowParent.transform.SetParent(content, false);

            //reset rowParent position 
            Instance.rowParent.anchoredPosition = new Vector2(0, -DataVisualizer.cellSize * 0.5f); //-15 for half height, probably needs a better approach

            //set viewport size
            //every column under row parent should have the same size
            content.sizeDelta = new Vector2(0, DataVisualizer.cellSize * entries.Count);

            while (!ready)
            {
                yield return null;
            }

            Instance.ChangeVisibility(true);

            //change position of visualizer
            Instance.visualizerRect.anchoredPosition = new Vector2();

            //reset scrollbar position (last otherwise it wont update properly)
            Instance.scrollbar.value = 1;

            Instance.ExecuteVisualizerReady();
        }

        public void ChangeDefaultMessage(string message)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(SetMessageText(message));
        }

        private IEnumerator SetMessageText(string message)
        {
            yield return null;
            defaultMessageText.text = message;
            defaultMessage.SetActive(true);
        }

        public void ChangeVisibility(bool status)
        {
            rowParent.gameObject.SetActive(status);
            rowOfNames.gameObject.SetActive(status);
            scrollRect.gameObject.SetActive(status);

            if (!status)
                defaultMessage.SetActive(status);
        }

        public void VisualizerReady(Action function)
        {
            //check if the event already contains the action
            if (onVisualizerReady != null)
            {
                Delegate[] methods = onVisualizerReady.GetInvocationList();
                foreach (Delegate method in methods)
                {
                    if (method.Method.Name.Equals(function.Method.Name))
                        return;
                }
            }
            onVisualizerReady += function;
        }

        private void ExecuteVisualizerReady()
        {
            Instance.onVisualizerReady?.Invoke();
            Instance.onVisualizerReady = null;
        }
    }
}
