using UnityEngine;

public class Cleanup : MonoBehaviour
{
    public bool clearChildren, clearSelf;

    private void OnDisable()
    {
        if (clearChildren)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        if (clearSelf) { Destroy(gameObject); }

        Resources.UnloadUnusedAssets();
    }
}
