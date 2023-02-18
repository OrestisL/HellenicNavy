using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine;

public class ServiceEntry : MonoBehaviour
{
    public TMP_InputField descriptionField;
    public TMP_InputField hoursField;
    public RectTransform serviceTypesParent;

    public Tuple<string,string> GetInfo() 
    {
        return new Tuple<string, string>(descriptionField.text, hoursField.text);
    }
}
