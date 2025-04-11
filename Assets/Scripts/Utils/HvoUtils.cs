using UnityEngine;

public static class HvoUtils 
{
    // 使用一个静态类来封装一些常用的工具函数，例如获取鼠标位置、判断是否为左键点击等

    public static Vector2 MousePosition => Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;
    public static bool IsLeftClickOrTapDown => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    public static bool IsLeftClickOrTapUp => Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);

    public static Vector2 InputHoldWorldPosition => Input.touchCount > 0 ?
        Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position) : Input.GetMouseButton(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : Vector2.zero;

}
