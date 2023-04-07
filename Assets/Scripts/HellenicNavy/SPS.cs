using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace SPS
{
    /// <summary>
    /// Each service will contain some assignments. These will be decribed here. WIP
    /// </summary>
    [Serializable]
    public class ServiceAssignment
    {
        //public string date;
        public string description;
        public bool isCompleted;

        public ServiceAssignment(/*string _date,*/ string _description, bool _isCompleted)
        {
            //date = _date;
            description = _description;
            isCompleted = _isCompleted;
        }

        public void Complete()
        {
            isCompleted = true;
        }
    }

    [Serializable]
    public enum ServiceAssignmentType
    {
        None = 0,
        Spot_Check,
        other,
        Overhaul,
    }
    [Serializable]
    public enum ServiceStatus
    {
        nothing,
        pending,
        completed,
        postponed
    }

    /// <summary>
    /// Service class holds all required information for each service.
    /// </summary>
    [Serializable]
    public class Service
    {
        public string name, id, descr;
        private int _currentHours;
        public DateTime lastServiceDate;
        public int lastServiceHours;
        public int nextServiceHours;
        //public ServiceStatus status;
        public string postponedServiceDescr;
        public string completedServiceDescr;
        public string serviceHistoryPostponed;
        public string serviceHistoryCompleted;
        public int CurrentHours
        {
            get { return _currentHours; }
            set { _currentHours = value; }
        }
        public int CurrentDays
        {
            get
            {
                return Mathf.Abs((lastServiceDate - DateTime.Now).Days);
            }
        }
        public int lastServiceDays;
        public int nextServiceDays;
        public int systemName;
        public List<string> descriptions;
        public List<int> serviceHours;
        public List<int> serviceDays;
        public List<ServiceAssignmentType> serviceTypesHours;
        public List<ServiceAssignmentType> serviceTypesDays;
        public List<ServiceStatus> serviceStatuses;
        public List<List<ServiceAssignment>> serviceAssignments; //WIP
        public List<List<ServiceStatus>> serviceAssignmentsStatuses;

        public Service() { }

        public Service(string json)
        {
            json = json.Replace(".", ",");
            Service s = JsonConvert.DeserializeObject<Service>(json);
            name = s.name;
            id = s.id;
            descr = s.descr;
            systemName = s.systemName;
            CurrentHours = s.CurrentHours;
            lastServiceDate = s.lastServiceDate;
            postponedServiceDescr = s.postponedServiceDescr;
            completedServiceDescr = s.completedServiceDescr;
            serviceHistoryPostponed = s.serviceHistoryPostponed;
            serviceHistoryCompleted = s.serviceHistoryCompleted;
            lastServiceHours = s.lastServiceHours;
            nextServiceHours = s.nextServiceHours;
            lastServiceDays = s.lastServiceDays;
            nextServiceDays = s.nextServiceDays;
            descriptions = s.descriptions;
            serviceHours = s.serviceHours;
            serviceDays = s.serviceDays;
            serviceTypesHours = s.serviceTypesHours;
            serviceTypesDays = s.serviceTypesDays;
            serviceStatuses = s.serviceStatuses;
            serviceAssignments = s.serviceAssignments;
            serviceAssignmentsStatuses = s.serviceAssignmentsStatuses;
        }

        public Service(string name, string descr, string id, int hours, int lastHours, int nextHours, string lastDate, int system, List<ServiceEntry> entries)
        {
            this.name = name;
            this.id = id;
            this.descr = descr;
            CurrentHours = hours;
            //date SHOULD HAVE BEEN saved like this
            lastServiceDate = DateTime.ParseExact(lastDate, "dd-MM-yy", null);
            lastServiceHours = lastHours;
            nextServiceHours= nextHours;
            this.systemName = system;

            serviceHours = new List<int>();
            serviceDays = new List<int>();
            descriptions = new List<string>();
            serviceTypesHours = new List<ServiceAssignmentType>();
            serviceTypesDays = new List<ServiceAssignmentType>();
            serviceStatuses = new List<ServiceStatus>();
            serviceAssignments = new List<List<ServiceAssignment>>();
            serviceAssignmentsStatuses = new List<List<ServiceStatus>>();


            for (int i = 0; i < entries.Count; i++)
            {
                serviceHours.Add(entries[i].Hours);
                serviceDays.Add(entries[i].Days);
                descriptions.Add(entries[i].Descr);
                //serviceTypesHours.Add(entries[i].TypesHours);
                //serviceTypesDays.Add(entries[i].TypesDays);
                serviceStatuses.Add(entries[i].Status);
                serviceAssignments.Add(entries[i].assignments);
                serviceAssignmentsStatuses.Add(entries[i].assignmentsStatuses);
            }
        }

        public void ChangeHours(int hours)
        {
            CurrentHours = hours;
        }

        public void ChangeEntries(ServiceEntry[] entries)
        {

            serviceHours = new List<int>();
            serviceDays = new List<int>();
            descriptions = new List<string>();
            serviceTypesHours = new List<ServiceAssignmentType>();
            serviceTypesDays = new List<ServiceAssignmentType>();
            serviceStatuses = new List<ServiceStatus>();
            serviceAssignments = new List<List<ServiceAssignment>>();
            serviceAssignmentsStatuses = new List<List<ServiceStatus>>();

            for (int i = 0; i < entries.Length; i++)
            {
                serviceHours.Add(entries[i].Hours);
                serviceDays.Add(entries[i].Days);
                descriptions.Add(entries[i].Descr);
                //serviceTypesHours.Add(entries[i].TypesHours);
                //serviceTypesDays.Add(entries[i].TypesDays);
                serviceStatuses.Add(entries[i].Status);
                serviceAssignments.Add(entries[i].assignments);
                serviceAssignmentsStatuses.Add(entries[i].assignmentsStatuses);
            }
        }

        public void ChangeDescription(int index, string description)
        {
            descriptions[index] = description;
        }

        public void SetServiceStatusPostponed(ServiceEntry[] entries)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].IsSelected)
                {
                    serviceHistoryPostponed += string.Format("Την {0} αναβλήθησαν οι κάτωθι επισκευές:\n", DateTime.Now.ToString("dd-MM-yy"));
                    postponedServiceDescr = string.Format("Την {0} αναβλήθησαν οι εξής επισκευές:", DateTime.Now.ToString("dd-MM-yy"));

                    entries[i].Status = ServiceStatus.postponed;
                    postponedServiceDescr = string.Format("{0}\n{1}", postponedServiceDescr, entries[i].Descr);
                    serviceHistoryPostponed += string.Format("{0}\n", entries[i].Descr);
                    entries[i].IsSelected = false;

                }
            }
        }

        public void SetServiceStatusCompleted(ServiceEntry[] entries)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].IsSelected)
                {
                    serviceHistoryCompleted += string.Format("Την {0} ολοκληρώθηκαν οι κάτωθι επισκευές:\n", DateTime.Now.ToString("dd-MM-yy"));
                    completedServiceDescr = string.Format("Την {0} ολοκληρώθηκαν οι εξής επισκευές:", DateTime.Now.ToString("dd-MM-yy"));

                    entries[i].Status = ServiceStatus.completed;
                    completedServiceDescr = string.Format("{0}\n{1}", completedServiceDescr, entries[i].Descr);
                    serviceHistoryCompleted += string.Format("{0}\n", entries[i].Descr);
                    entries[i].IsSelected = false;
                }
            }

            if (entries.Length > 0)
            {
                lastServiceHours = CurrentHours / serviceHours.Min() * serviceHours.Min();
                nextServiceHours = (CurrentHours / serviceHours.Min() + 1) * serviceHours.Min();
                lastServiceDays = CurrentDays / serviceDays.Min() * serviceDays.Min();
                nextServiceDays = (CurrentDays / serviceDays.Min() + 1) * serviceDays.Min();

                UpdateServiceStatuses(entries.Select(entry => entry.Status).ToArray());
            }
        }

        public void UpdateServiceStatuses(ServiceStatus[] statuses)
        {
            serviceStatuses = statuses.ToList();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
