#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

public class DaysLeft : Editor
{
    [MenuItem("Army/DaysLeft")]
    static void ShowDaysLeft() 
    {
        Debug.Log(string.Format("{0} και σήμερα", (DateTime.ParseExact("09-07-23", "dd-MM-yy", null) - DateTime.Now).Days - 1));
    }
}
#endif