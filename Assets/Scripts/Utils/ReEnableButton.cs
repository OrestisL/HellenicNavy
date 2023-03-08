using UnityEngine;
using UnityEngine.UI;

public class ReEnableButton : MonoBehaviour
{
    private void OnDisable()
    {
        GetComponent<Button>().interactable = true;
    }
}
