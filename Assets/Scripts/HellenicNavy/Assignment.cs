using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Assignment : MonoBehaviour
{
    public TMP_InputField descrInput;
    public string description;
    public Toggle isSelected;
    public bool isCompleted;

    public void Set(string _descr) 
    {
        descrInput.text = _descr;
    }
    public void Complete()
    {
        isCompleted = true;
    }
}
