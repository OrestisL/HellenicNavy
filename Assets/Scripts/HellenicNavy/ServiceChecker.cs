using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySQLite;
using UnitySQLite.Utilities;
using SPS;
using System.Linq;
using UnityEngine.UI;

public class ServiceChecker : GenericSingleton<ServiceChecker>
{
    [SerializeField]
    private DateTime today;
    [SerializeField]
    private List<int> dateDistances;
    [SerializeField]
    private List<string> names;

    private struct ServiceTableEntry
    {
        int timeInterval;
        List<int> services;
        public ServiceTableEntry(int repeat, List<int> services)
        {
            this.timeInterval = repeat;
            this.services = services;
        }

        public int TimeInterval { get { return timeInterval; } }
        public List<int> Services { get { return services; } }
    }

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
            names = new List<string>();
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

    private /*void*/ IEnumerator CheckHours()
    {
        //wait for a bit
        yield return new WaitForSeconds(1);
        for (int i = 0; i < names.Count; i++)
        {
            DatabaseManager.Instance.ReadData(names[i], SelectFromDatabaseMode.specificColumns,
                (data) =>
                {
#if UNITY_EDITOR
                    System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

                    watch.Start();
#endif
                    List<Button> machineryButtonsToDisplay = new List<Button>();
                    #region hours
                    Service serv = new Service(data[0][0].StringValue);
                    List<int> serviceTimes = serv.serviceHours;

                    int iterHours = SettingsHolder.Instance.settings.maxLookupTableHours / serviceTimes.Min();
                    List<ServiceTableEntry> serviceHours = new List<ServiceTableEntry>();
                    for (int j = 0; j <= iterHours; j++)
                    {
                        int currentHours = j * serviceTimes.Min();
                        List<int> servicesToBeDone = new List<int>();

                        for (int k = 0; k < serviceTimes.Count; k++)
                        {
                            if (j == 0)
                                break;
                            if (currentHours % serviceTimes[k] == 0)
                            {
                                servicesToBeDone.Add(k);
                            }

                        }

                        serviceHours.Add(new ServiceTableEntry(currentHours, servicesToBeDone));
                        if (j > 1)
                        {
                            if (serv.CurrentHours >= serviceHours[j - 1].TimeInterval & serv.CurrentHours <= serviceHours[j].TimeInterval)
                            {
                                string descr = "";
                                servicesToBeDone = serviceHours[j - 1].Services;

                                int l = servicesToBeDone.Max();
                                while (l >= 0)
                                {
                                    if (servicesToBeDone.Contains(l))
                                    {
                                        descr += string.Format("{0}\n", serv.descriptions[l]);
                                    }
                                    l--;
                                }
                                Debug.Log(string.Format("services (hours) to be done for machinery {0}:\n{1}", serv.name, descr.Trim()));
#if UNITY_EDITOR
                                watch.Stop();
                                Debug.Log(string.Format("Took {0}ms to create lookup table and determine services for {1}", watch.ElapsedMilliseconds, serv.name));
#endif
                                //add button here
                                break;
                            }
                        }
                    }
                    #endregion
                    #region days
                    List<int> serviceDays = serv.serviceDays;
                    int iterDays = SettingsHolder.Instance.settings.maxLookupTableDays / serviceDays.Min();
                    List<ServiceTableEntry> servDays = new List<ServiceTableEntry>();
                    for (int jj = 0; jj < iterDays; jj++)
                    {
                        int currentDays = jj * serviceDays.Min();
                        List<int> servicesToBeDone = new List<int>();

                        for (int kk = 0; kk < serviceDays.Count; kk++)
                        {
                            if (jj == 0)
                                break;
                            if (currentDays % serviceDays[kk] == 0)
                            {
                                servicesToBeDone.Add(kk);
                            }
                        }
                        servDays.Add(new ServiceTableEntry(currentDays, servicesToBeDone));
                        if (jj > 1)
                        {
                            if (serv.CurrentDays >= servDays[jj - 1].TimeInterval & serv.CurrentDays <= servDays[jj].TimeInterval)
                            {
                                string descr = "";
                                servicesToBeDone = servDays[jj - 1].Services;

                                int ll = servicesToBeDone.Max();
                                while (ll >= 0)
                                {
                                    if (servicesToBeDone.Contains(ll))
                                    {
                                        descr += string.Format("{0}\n", serv.descriptions[ll]);
                                    }
                                    ll--;
                                }
                                //add button here, if not already exissts
                                Debug.Log(string.Format("services (days) to be done for machinery {0}:\n{1}", serv.name, descr.Trim()));
                                break;
                            }
                        }
                    }
                    #endregion
                    #region postponed
                    //check if any service entries are marked as postponed
                    if (serv.serviceStatuses.Select(x => x == ServiceStatus.postponed).Count() > 0) 
                    {
                        //add button here, if not already exists
                    }
                    #endregion
                    MessageBox.Instance.HideMessageBox();
                },
                SortResultsBy.none, null, "ServiceDescr");

            yield return new WaitForSeconds(1);
        }
        //TODO populate some interface with buttons for each machinery that has pending services 
    }
}
