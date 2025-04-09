using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : SingletonManager<GameManager>
{
    #region Parameters
    public Unit ActiveUnit;
    [SerializeField] private GameObject mousePrefab;
    [SerializeField] private float rayDuration = 2f; // 射线持续时间，默认为2秒
    [SerializeField] private Color rayColor = Color.green; // 射线颜色
    [SerializeField] private float rayWidth = 0.1f; // 射线宽度
    private LineRenderer movementRay; 
    private Vector2 targetPosition; 
    private bool isMoving = false; 
    #endregion
    
    void Start()
    {
        InitializeLineRenderer();
    }

    void Update()
    {
        Vector2 mousePosition = Input.touchCount > 0 ? Input.GetTouch(0).position : Input.mousePosition;

        if(Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            DetectClick(mousePosition);
        }

        // 如果单位正在移动，更新射线
        if (isMoving && ActiveUnit != null && movementRay != null && movementRay.enabled)
        {
            UpdateMovementRay();
        }
    }

    private void InitializeLineRenderer()
    {
        movementRay = GetComponent<LineRenderer>();
        if (movementRay == null)
        {
            movementRay = gameObject.AddComponent<LineRenderer>();
        }
        
        movementRay.startWidth = rayWidth;
        movementRay.endWidth = rayWidth;
        movementRay.material = new Material(Shader.Find("Sprites/Default"));
        movementRay.startColor = rayColor;
        movementRay.endColor = rayColor;
        movementRay.positionCount = 2;
        movementRay.enabled = false;
    }
    
    private void DetectClick(Vector2 _inputPosition)
    {
        AudioManager.Get().PlaySFX(0);
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(_inputPosition); // convert screen position to world position
        Instantiate(mousePrefab,worldPoint,Quaternion.identity);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if(HasClickedOnUnit(hit,out Unit _unit))
        {
            HandleClickOnNewUnit(_unit,worldPoint);
        }
        else{
            HandleClickOnOriginalUnit(worldPoint);
        }

    }

    private void HandleClickOnNewUnit(Unit _unit,Vector2 _worldPosition)
    {
        SelectNewUnit(_unit);
        HandleClickOnOriginalUnit(_worldPosition);
    }

    private bool HasClickedOnUnit(RaycastHit2D hit, out Unit _unit)
    {
        _unit = null;
        if (hit.collider != null && hit.collider.GetComponent<Unit>() != null)
        {
            _unit = hit.collider.GetComponent<Unit>();
            return true;
        }
        return false;
    }

    private void HandleClickOnOriginalUnit(Vector2 _worldPosition)
    {
        if (ActiveUnit != null)
        {
            targetPosition = _worldPosition;
            isMoving = true;
            DrawMovementRay(ActiveUnit.transform.position, _worldPosition);
            ActiveUnit.MoveToDestionation(_worldPosition);
        }
    }

    private void SelectNewUnit(Unit _unit)
    {
        ActiveUnit = _unit;
    }
    
    // 绘制从起点到终点的射线
    private void DrawMovementRay(Vector3 startPosition, Vector3 endPosition)
    {
        if (movementRay == null) return;
        
        movementRay.SetPosition(0, new Vector3(startPosition.x, startPosition.y, 0));
        movementRay.SetPosition(1, new Vector3(endPosition.x, endPosition.y, 0));
        movementRay.enabled = true;
        
        StartCoroutine(HideRayAfterDelay(rayDuration));
    }
    
    // 更新移动射线
    private void UpdateMovementRay()
    {
        Vector2 currentPosition = ActiveUnit.transform.position;
        
        movementRay.SetPosition(0, new Vector3(currentPosition.x, currentPosition.y, 0)); // 这里的0是索引，代表线段的起点
        
        // 检查是否到达目标位置
        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
        if (distanceToTarget < 0.1f)
        {
            movementRay.enabled = false;
            isMoving = false;
        }
    }
    
    private IEnumerator HideRayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (movementRay != null)
        {
            movementRay.enabled = false;
            isMoving = false;
        }
    }

}
