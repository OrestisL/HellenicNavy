using UnityEngine;

public class CleanupButtons : MonoBehaviour
{
    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
