using System;
using System.Collections;
using System.Collections.Generic;
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
        public string date;
        public string description;
        public bool isCompleted;

        public ServiceAssignment(string _date, string _description, bool _isCompleted)
        {
            date = _date;
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
        public int systemName;
        public List<string> descriptions;
        public List<int> serviceHours;
        public List<int> serviceDays;
        public List<ServiceAssignmentType> serviceTypesHours;
        public List<ServiceAssignmentType> serviceTypesDays;
        public List<ServiceAssignment> serviceAssignments; //WIP

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
            lastServiceHours = s.lastServiceHours;
            descriptions = s.descriptions;
            serviceHours = s.serviceHours;
            serviceDays = s.serviceDays;
            serviceTypesHours = s.serviceTypesHours;
            serviceTypesDays = s.serviceTypesDays;
            serviceAssignments = s.serviceAssignments;
        }

        public Service(string name,string descr, string id, int hours, string lastDate, int system, List<ServiceEntry> entries)
        {
            this.name = name;
            this.id = id;
            this.descr = descr;
            CurrentHours = hours;
            //date SHOULD HAVE BEEN saved like this
            lastServiceDate = DateTime.ParseExact(lastDate, "dd-MM-yy", null);
            this.systemName = system;

            serviceHours = new List<int>();
            serviceDays = new List<int>();
            descriptions = new List<string>();
            serviceTypesHours = new List<ServiceAssignmentType>();
            serviceTypesDays = new List<ServiceAssignmentType>();

            for (int i = 0; i < entries.Count; i++)
            {
                serviceHours.Add(entries[i].Hours);
                serviceDays.Add(entries[i].Days);
                descriptions.Add(entries[i].Descr);
                serviceTypesHours.Add(entries[i].TypesHours);
                serviceTypesDays.Add(entries[i].TypesDays);
            }
        }

        public void AddHours(int hours)
        {
            CurrentHours += hours;
        }

        public void ChangeDescription(int index, string description)
        {
            descriptions[index] = description;
        }

        public void ChangeAssignmentDescription(int index, string description)
        {
            serviceAssignments[index].description = description;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
