using TMPro;
using UnityEngine;
using System.Collections.Generic;
using SPS;
using System;

public class ServiceEntry : MonoBehaviour
{
    public TMP_InputField descriptionField;
    public TMP_InputField hoursField;
    public TMP_InputField daysField;
    public RectTransform serviceTypesParent;
    public List<ServiceAssignmentType> serviceAssignmentTypes;

    public List<int> Hours
    {
        get
        {
            hoursField.text.Trim();
            List<int> result = new List<int>();
            ReadOnlySpan<char> text = hoursField.text;
            int nextNewlineIndex = 0;
            bool isLastLoop = false;
            while (!isLastLoop)
            {
                int indexStart = nextNewlineIndex;
                nextNewlineIndex = hoursField.text.IndexOf(",", indexStart);

                isLastLoop = nextNewlineIndex == -1;
                if (isLastLoop)
                {
                    nextNewlineIndex = hoursField.text.Length;
                }

                ReadOnlySpan<char> currentValue = text.Slice(indexStart, nextNewlineIndex - indexStart);

                result.Add(int.Parse(currentValue.ToString()));
            }
            return result;
        }
    }
    public List<int> Days
    {
        get
        {
            daysField.text.Trim();
            List<int> result = new List<int>();
            ReadOnlySpan<char> text = daysField.text;
            int nextNewlineIndex = 0;
            bool isLastLoop = false;
            while (!isLastLoop)
            {
                int indexStart = nextNewlineIndex;
                nextNewlineIndex = daysField.text.IndexOf(",", indexStart);

                isLastLoop = nextNewlineIndex == -1;
                if (isLastLoop)
                {
                    nextNewlineIndex = daysField.text.Length;
                }

                ReadOnlySpan<char> currentValue = text.Slice(indexStart, nextNewlineIndex - indexStart);
                result.Add(int.Parse(currentValue.ToString()));
            }
            return result;
        }
    }

    public string Descr { get { return descriptionField.text; } }

    public List<ServiceAssignmentType> Types
    {
        get
        {
            serviceAssignmentTypes = new List<ServiceAssignmentType>();
            //get all assignment types
            for (int i = 0; i < serviceTypesParent.childCount; i++)
            {
                serviceAssignmentTypes.Add((ServiceAssignmentType)serviceTypesParent.GetChild(i).GetComponent<TMP_Dropdown>().value);
            }

            return serviceAssignmentTypes;
        }
    }

    private void Start()
    {
        TMP_InputValidator validator = new TextValidator();
        validator.name = "Input Validation for Hours and Days";
        hoursField.inputValidator = validator;
        daysField.inputValidator = validator;

    }
}
