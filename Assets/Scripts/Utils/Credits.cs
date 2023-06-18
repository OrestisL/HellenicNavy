
using UnityEngine.EventSystems;
using UnityEngine;

public class Credits : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Texture2D texture;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            Application.OpenURL("https://www.linkedin.com/in/orestis-liaskos/");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private void Start()
    {
        transform.SetAsLastSibling();
    }

}
