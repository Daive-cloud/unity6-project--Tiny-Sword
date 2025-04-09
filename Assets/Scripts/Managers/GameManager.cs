using UnityEngine;

public class GameManager : SingletonManager<GameManager>
{

    void Update()
    {
        Vector2 mousePosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        if(Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            DetectClick(mousePosition);
        }
    }

    private void DetectClick(Vector2 _inputPosition)
    {

    }
}
