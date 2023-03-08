using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine.UI;
using System.Collections;

public static class ExtensionMethods
{
    public static int ToInt(this bool b)
    {
        return b ? 1 : -1;
    }

    //Extension class to provide serialize / deserialize methods to object.
    //src: http://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
    //NOTE: You need add [Serializable] attribute in your class to enable serialization
    public static byte[] SerializeToByteArray(this object obj)
    {
        if (obj == null)
        {
            return null;
        }
        var bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static T DeserializeFromByteArray<T>(this byte[] byteArray) where T : class
    {
        if (byteArray == null)
        {
            return null;
        }
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(byteArray, 0, byteArray.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            T obj = (T)binForm.Deserialize(memStream);
            return obj;
        }
    }

    public static string ByteArrayToString(this byte[] bytes)
    {
        // Convert byte array to a string   
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }

    public static void ClearChildren(this Transform trans)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            if (trans.GetChild(i).name.Equals("Label") | trans.GetChild(i).name.Equals("Buttons"))
                continue;

            UnityEngine.Object.Destroy(trans.GetChild(i).gameObject);
        }

        Resources.UnloadUnusedAssets();
    }

    public static void DelayedEnableButton(this Button button, float delay)
    {
        button.onClick.AddListener(() => button.StartCoroutine(ToggleButton(button, delay)));
    }

    private static IEnumerator ToggleButton(Button button, float delay)
    {
        button.interactable = false;
        yield return new WaitForSeconds(delay);
        button.interactable = true;
        button.onClick.RemoveListener(() => button.StartCoroutine(ToggleButton(button, delay)));
    }
}
