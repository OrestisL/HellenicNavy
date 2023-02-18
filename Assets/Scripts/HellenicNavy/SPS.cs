using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;


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
        public string name, id;
        private int _currentHours;
        public int CurrentHours
        {
            get { return _currentHours; }
            set { _currentHours = value; }
        }

        public List<string> descriptions;
        public List<List<int>> serviceHours;
        public List<List<int>> serviceDays;
        public List<List<ServiceAssignmentType>> serviceTypes;
        public List<ServiceAssignment> serviceAssignments; //WIP

        public Service() { }

        public Service(string json)
        {
            Service s = JsonConvert.DeserializeObject<Service>(json);
            name = s.name;
            id = s.id;
            CurrentHours = s.CurrentHours;
            descriptions = s.descriptions;
            serviceHours = s.serviceHours;
            serviceTypes = s.serviceTypes;
            serviceAssignments = s.serviceAssignments;
        }

        public Service(string name, string id, int hours, List<ServiceEntry> entries)
        {
            this.name = name;
            this.id = id;
            CurrentHours = hours;

            serviceHours = new List<List<int>>();
            serviceDays = new List<List<int>>();
            descriptions = new List<string>();
            serviceTypes = new List<List<ServiceAssignmentType>>();

            for (int i = 0; i < entries.Count; i++)
            {
                serviceHours.Add(entries[i].Hours);
                serviceDays.Add(entries[i].Days);
                descriptions.Add(entries[i].Descr);
                serviceTypes.Add(entries[i].Types);
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

        //public void ChangeServiceType(int index, ServiceAssignmentType type)
        //{
        //    serviceTypes[index] = type;
        //}

        public void ChangeAssignmentDescription(int index, string description)
        {
            serviceAssignments[index].description = description;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
