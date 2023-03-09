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
        DatabaseManager.Instance.ReadData("MachineryList", SelectFromDatabaseMode.everything, (data) => 
        {
            //after reading all data, should check the "days distance" between today and last service time
            dateDistances = new List<int>();
            for (int i = 0; i < data.Count; i++)
            {
                DateTime last = DateTime.ParseExact(data[i][7].StringValue, "dd-MM-yy", null);
                int distance = (int)(today - last).TotalDays;
                dateDistances.Add(distance);
                names.Add(data[i][0].StringValue);
            }
            //after populating the list, should read all tables and check the days of the service entries
            StartCoroutine(CheckDates());           
        });
    }

    private IEnumerator CheckDates() 
    {
        for (int i = 0; i < names.Count; i++)
        {
            DatabaseManager.Instance.ReadData(names[i], SelectFromDatabaseMode.specificColumns, 
                (data) => 
                {
                    for (int j = 0; j < data.Count; j++)
                    {
                        Service serv = new Service(data[j][0].StringValue);
                        for (int ii = 0; ii < serv.serviceDays.Count; ii++)
                        {
                            if (dateDistances[ii] > serv.serviceDays[ii])
                            {                              
                                //algorithm will probably  work like this:
                                //find max value from array that is <= the current date distance
                                //find all numbers in the array that divide the max value
                                int maxVal = serv.serviceDays.Where(d => d < dateDistances[ii]).Max();
                                for (int ij = 0; ij < serv.serviceDays.Count; ij++)
                                {
                                    if (maxVal % serv.serviceDays[ij] == 0)
                                        Debug.Log(string.Format("service {0} in {1} needs to be done", serv.descriptions[ij], serv.name));
                                }
                                //issues: does not work the first time
                                //        works the second time but it throws and exception
                                //        needs to be changed, probably subtract dateDistances[ii] - max then check mod with that?
                                //        rethink algorithm
                            }
                        }
                    }
                }, 
                SortResultsBy.none, null, "ServiceDescr");

            yield return new WaitForSeconds(1);
        }
    }

}
