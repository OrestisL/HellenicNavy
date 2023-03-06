using TMPro;
using UnityEngine;
using System.Collections.Generic;
using SPS;
using System;
using System.Linq;
using UnityEngine.UI;

public class ServiceEntry : MonoBehaviour
{
    public TMP_InputField descriptionField;
    public TMP_InputField hoursField;
    public TMP_InputField daysField;
    public Toggle selectionToggle;
    public Image bgImg;
    public Color normalColor, selectedColor;
    public RectTransform serviceTypesParentHours;
    public RectTransform serviceTypesParentDays;
    public List<ServiceAssignmentType> serviceAssignmentTypesHours;
    public List<ServiceAssignmentType> serviceAssignmentTypesDays;

    public List<int> Hours
    {
        get
        {
            hoursField.text = hoursField.text.Trim();
            List<int> result = new List<int>();
            if (hoursField.text.Length == 0)
            {
                return result;
            }
            ReadOnlySpan<char> text = hoursField.text.AsSpan();
            int nextNLIndex = 0;
            bool isLastLoop = false;
            while (!isLastLoop)
            {
                int indexStart = nextNLIndex;
                nextNLIndex = hoursField.text.IndexOf('\n', indexStart);

                isLastLoop = nextNLIndex == -1;
                if (isLastLoop)
                {
                    nextNLIndex = hoursField.text.Length;
                }
                ReadOnlySpan<char> nameSlice = text.Slice(indexStart, nextNLIndex - indexStart);
                result.Add(int.Parse(nameSlice.ToString()));
                nextNLIndex++;
            }
            return result;
        }
    }
    public List<int> Days
    {
        get
        {
            daysField.text = daysField.text.Trim();
            List<int> result = new List<int>();
            if (daysField.text.Length == 0)
            {
                return result;
            }
            ReadOnlySpan<char> text = daysField.text.AsSpan();
            int nextNLIndex = 0;
            bool isLastLoop = false;
            while (!isLastLoop)
            {
                int indexStart = nextNLIndex;
                nextNLIndex = daysField.text.IndexOf('\n', indexStart);

                isLastLoop = nextNLIndex == -1;
                if (isLastLoop)
                {
                    nextNLIndex = daysField.text.Length;
                }
                ReadOnlySpan<char> nameSlice = text.Slice(indexStart, nextNLIndex - indexStart);
                result.Add(int.Parse(nameSlice.ToString()));
                nextNLIndex++;
            }
            return result;
        }
    }

    public string Descr { get { return descriptionField.text; } }

    public List<ServiceAssignmentType> TypesHours
    {
        get
        {
            serviceAssignmentTypesHours = new List<ServiceAssignmentType>();
            //get all assignment types
            for (int i = 0; i < serviceTypesParentHours.childCount; i++)
            {
                serviceAssignmentTypesHours.Add((ServiceAssignmentType)serviceTypesParentHours.GetChild(i).GetComponent<TMP_Dropdown>().value);
            }

            return serviceAssignmentTypesHours;
        }
    }

    public List<ServiceAssignmentType> TypesDays
    {
        get
        {
            serviceAssignmentTypesDays = new List<ServiceAssignmentType>();
            for (int i = 0; i < serviceTypesParentDays.childCount; i++)
            {
                serviceAssignmentTypesDays.Add((ServiceAssignmentType)serviceTypesParentDays.GetChild(i).GetComponent<TMP_Dropdown>().value);
            }

            return serviceAssignmentTypesDays;
        }
    }

    public bool IsSelected { get { return selectionToggle.isOn; } }

    private void Start()
    {
        //validator for input
        TextValidator validator = ScriptableObject.CreateInstance<TextValidator>();
        validator.name = "Input Validation for Hours and Days";
        hoursField.inputValidator = validator;
        daysField.inputValidator = validator;

        hoursField.onEndEdit.AddListener((s) =>
        {
            //clear whitespace
            hoursField.text = hoursField.text.Trim();
            if (s.Equals(string.Empty))
            {
                ClearChildren(serviceTypesParentHours);
            }
            else
            {
                int lineCount = hoursField.text.Count(x => x == '\n') + 1;
                int existing = serviceTypesParentHours.childCount;
                if (existing < lineCount)
                {
                    //need to add more 
                    for (int i = 0; i < lineCount - existing; i++)
                    {
                        Instantiate(InterfaceManager.Instance.serviceTypePrefab, serviceTypesParentHours);
                    }
                }
                else if (existing > lineCount)
                {
                    //delete from bottom
                    for (int i = existing - 1; i >= lineCount; i--)
                    {
                        Destroy(serviceTypesParentHours.GetChild(i).gameObject);
                    }
                }

            }
        });

        daysField.onEndEdit.AddListener((s) =>
        {
            //clear whitespace
            daysField.text = daysField.text.Trim();
            if (s.Equals(string.Empty))
            {
                ClearChildren(serviceTypesParentDays);
            }
            else
            {
                int lineCount = daysField.text.Count(x => x == '\n') + 1;
                int existing = serviceTypesParentDays.childCount;
                if (existing < lineCount)
                {
                    //need to add more 
                    for (int i = 0; i < lineCount - existing; i++)
                    {
                        Instantiate(InterfaceManager.Instance.serviceTypePrefab, serviceTypesParentDays);
                    }
                }
                else if (existing > lineCount)
                {
                    //delete from bottom
                    for (int i = existing - 1; i >= lineCount; i--)
                    {
                        Destroy(serviceTypesParentDays.GetChild(i).gameObject);
                    }
                }
            }
        });

        selectionToggle.onValueChanged.AddListener((b) => bgImg.color = b ? selectedColor : normalColor);
    }

    public void DisplayFromData(string descr, List<int> hours, List<int> days, List<ServiceAssignmentType> serviceTypesHours, List<ServiceAssignmentType> serviceTypesDays) 
    {
        descriptionField.text = descr;
        for (int i = 0; i < hours.Count; i++)
        {
            hoursField.text += string.Format("{0}\n", hours[i]);
            TMP_Dropdown d = Instantiate(InterfaceManager.Instance.serviceTypePrefab, serviceTypesParentHours).GetComponent<TMP_Dropdown>();
            d.value = (int)serviceTypesHours[i];
        }
        hoursField.text.TrimEnd();

        for (int j = 0; j < days.Count; j++)
        {
            daysField.text += string.Format("{0}\n", days[j]);
            TMP_Dropdown d = Instantiate(InterfaceManager.Instance.serviceTypePrefab, serviceTypesParentDays).GetComponent<TMP_Dropdown>();
            d.value = (int)serviceTypesDays[j];
        }
        daysField.text.TrimEnd();
    }
    void ClearChildren(Transform parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children) { if (!child.name.Equals(parent.name)) Destroy(child.gameObject); }
    }
}
