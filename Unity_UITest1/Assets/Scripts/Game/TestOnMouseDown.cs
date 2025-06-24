using UnityEngine;

public class TestOnMouseDown : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log($"[OnMouseDown] Object clicked: {gameObject.name}");
    }

    void OnMouseEnter()
    {
        Debug.Log($"[OnMouseEnter] Mouse entered: {gameObject.name}");
    }

    void OnMouseExit()
    {
        Debug.Log($"[OnMouseExit] Mouse exited: {gameObject.name}");
    }
}
