using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySQLite;
using UnitySQLite.Utilities;
using SPS;
using System.Linq;

public class ServiceChecker : GenericSingleton<ServiceChecker>
{
    [SerializeField]
    private DateTime today;
    [SerializeField]
    private List<int> dateDistances;
    [SerializeField]
    private List<string> names;
    public override void Awake()
    {
        base.Awake();
        today = DateTime.Now;
    }

    public void Check()
    {
        MessageBox.Instance.ShowMessageBox(new MessageBoxSettings()
        {
            showLabel = true,
            label = "Έλεγχος για επισκευές",
            mainText = "Παρακαλώ περιμένετε όσο γίνεται έλεγχος για επισκευές.",
            showLoadingIndicator = true,
            useLeftButton = false,
            useRightButton = false,
        }, -1);

        DatabaseManager.Instance.ReadData("MachineryList", SelectFromDatabaseMode.specificColumns, (data) =>
        {
            //after reading all data, should check the "days distance" between today and last service time
            dateDistances = new List<int>();
            for (int i = 0; i < data.Count; i++)
            {
                DateTime last = DateTime.ParseExact(data[i][1].StringValue, "dd-MM-yy", null);
                int distance = (int)(today - last).TotalDays;
                dateDistances.Add(distance);
                names.Add(data[i][0].StringValue);
            }
            //after populating the list, should read all tables and check the days of the service entries
            //Invoke(nameof(CheckDates), 2f);         
            //StartCoroutine(CheckDates());
            StartCoroutine(CheckHours());
        }, SortResultsBy.none, null, "Name, LastServiceTime");
    }

    private /*void*/ IEnumerator CheckDates()
    {
        //wait for a bit
        yield return new WaitForSeconds(1);
        for (int i = 0; i < names.Count; i++)
        {
            DatabaseManager.Instance.ReadData(names[i], SelectFromDatabaseMode.specificColumns,
                (data) =>
                {
                        //create new service for each description
                        Service serv = new Service(data[0][0].StringValue);
                        //for the given day distance, find the closest service days
                        int closestDays = serv.serviceDays.Where(d => d <= dateDistances[i]).Count() > 0 ? serv.serviceDays.Where(d => d <= dateDistances[i]).Max() : 0;

                        int actualCheckDays = serv.CurrentDays > closestDays ? serv.CurrentDays - closestDays : serv.CurrentDays;
                        //this loop should be performed (int)currentDays/maxClosest + 1 times (i think)
                        int iterations = closestDays > 0 ? (int)(serv.CurrentDays / closestDays) : 0;
                        for (int iter = 0; iter < iterations; iter++)
                        {
                            closestDays = serv.serviceDays.Where(d => d <= actualCheckDays).Max();
                            actualCheckDays -= closestDays;
                        }

                        //find which services need to be done
                        string allDescr = string.Empty;
                        for (int j = 0; j < serv.serviceDays.Count; j++)
                        {
                            if (actualCheckDays / serv.serviceDays[j] >= 1) //(actualCheckDays % serv.serviceDays[j] == 0)
                                allDescr += string.Format("{0}\n", serv.descriptions[j]);
                        }

                        Debug.Log(allDescr.TrimEnd());
                },
                SortResultsBy.none, null, "ServiceDescr");
            //TODO populate some interface with buttons for each machinery that has pending services 
            yield return new WaitForSeconds(1);
        }

        MessageBox.Instance.HideMessageBox();
    }
    private /*void*/ IEnumerator CheckHours()
    {
        //wait for a bit
        yield return new WaitForSeconds(1);
        for (int i = 0; i < names.Count; i++)
        {
            DatabaseManager.Instance.ReadData(names[i], SelectFromDatabaseMode.specificColumns,
                (data) =>
                {
                    Service serv = new Service(data[0][0].StringValue);
                    int closestHours = serv.serviceHours.Where(d => d <= serv.CurrentHours).Count() > 0 ? serv.serviceHours.Where(d => d <= serv.CurrentHours).Max() : 0;

                    int actualCheckHours = serv.CurrentHours > closestHours ? serv.CurrentHours - closestHours : serv.CurrentHours;
                    int iterations = closestHours > 0 ? (int)(serv.CurrentHours / closestHours) - 1 : 0;
                    //for (int iter = 0; iter < iterations; iter++)
                    //{
                    //    closestHours = serv.serviceHours.Where(d => d <= actualCheckHours).Max();
                    //    actualCheckHours -= closestHours;
                    //}

                    string allDescr = string.Empty;
                    for (int j = 0; j < serv.serviceHours.Count; j++)
                    {
                        if (actualCheckHours / serv.serviceHours[j] >= 1)//(actualCheckHours % serv.serviceHours[j] == 0)
                            allDescr += string.Format("{0}\n", serv.descriptions[j]);
                    }

                    Debug.Log(allDescr.TrimEnd());
                },
                SortResultsBy.none, null, "ServiceDescr");

            yield return new WaitForSeconds(1);
        }
        //TODO populate some interface with buttons for each machinery that has pending services 
        MessageBox.Instance.HideMessageBox();
    }
}
