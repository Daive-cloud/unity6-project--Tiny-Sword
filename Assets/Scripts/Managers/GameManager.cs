using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GameManager : SingletonManager<GameManager>
{
    #region Parameters
    private Unit ActiveUnit;
    [SerializeField] private GameObject mousePrefab;
    [Header("Line Renderer Parameters")]
    [SerializeField] private float rayDuration = 2f; // 射线持续时间，默认为2秒
    [SerializeField] private Color rayColor = Color.green; // 射线颜色
    [SerializeField] private float rayWidth = 0.1f; // 射线宽度
    [SerializeField] private float clickDetectionRadius = 0.3f; // 点击检测半径
    private LineRenderer movementRay; 
    private Vector2 targetPosition; 
    private Vector2 m_MousePosition;
    
    private Coroutine showRayRedenerer; 
    [Header("UI Parameters")]
    [SerializeField] private GameObject m_ActionBar;
    private bool isMoving = false; 
    private PlacementProcess m_PlacementProcess;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap m_WalkableTilemap;
    [SerializeField] private Tilemap m_overlayTilemap;

    #endregion
    
    void Start()
    {
        InitializeLineRenderer();
        ClearActionBarUI();
    }

    void Update() 
    {
        if(m_PlacementProcess != null)
        {
            m_PlacementProcess.Update();
        }
        else{
             if(HvoUtils.IsLeftClickOrTapDown)
            {
                m_MousePosition = HvoUtils.MousePosition;
            }
        
            if(HvoUtils.IsLeftClickOrTapUp)
            {
                DetectClick(m_MousePosition);
            }

            if(Input.GetMouseButtonUp(1) && ActiveUnit != null)
            {
                ClearActionBarUI();
                ActiveUnit.GetComponent<HumanoidUnit>().UnSelectedUnit();
                ActiveUnit = null;
            }

            if (isMoving && ActiveUnit != null && movementRay != null && movementRay.enabled)
            {
                UpdateMovementRay();
            }
        }
    }

    #region Detect Click on Unit
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
        if(IsPointerOverUIElement())
        {
            return;
        }

        AudioManager.Get().PlaySFX(0);
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(_inputPosition); // convert screen position to world position
        
        RaycastHit2D hit = Physics2D.CircleCast(worldPoint, clickDetectionRadius, Vector2.zero);
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
        if(_unit == ActiveUnit && ActiveUnit != null)
        {
            ActiveUnit.GetComponent<HumanoidUnit>().UnSelectedUnit();
            ActiveUnit = null;
            return;
        }

        SelectNewUnit(_unit);
        ActiveUnit.GetComponent<HumanoidUnit>().UnitSelected();
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
            Instantiate(mousePrefab,_worldPosition,Quaternion.identity);
            ActiveUnit.GetComponent<HumanoidUnit>().UnitActed();
            targetPosition = _worldPosition;
            isMoving = true;
            DrawMovementRay(ActiveUnit.transform.position, _worldPosition);
            ActiveUnit.MoveToDestionation(_worldPosition);
        }
    }

    private void SelectNewUnit(Unit _unit)
    {
        if(ActiveUnit != null)
        {
            ActiveUnit.GetComponent<HumanoidUnit>().UnSelectedUnit();
        }
        ShowActionBarUI(_unit);
        ActiveUnit = _unit;
    }
    
    // 绘制从起点到终点的射线
    private void DrawMovementRay(Vector3 startPosition, Vector3 endPosition)
    {
        if (movementRay == null) return;
        
        movementRay.SetPosition(0, new Vector3(startPosition.x, startPosition.y, 0));
        movementRay.SetPosition(1, new Vector3(endPosition.x, endPosition.y, 0));
        movementRay.enabled = true;

        if(showRayRedenerer != null)
        {
            StopCoroutine(showRayRedenerer);
        }
        showRayRedenerer = StartCoroutine(HideRayAfterDelay(rayDuration));
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
    #endregion

    public void ClearActionBarUI()
    {
        m_ActionBar.GetComponent<ActionBar>().ClearActions();
        m_ActionBar.GetComponent<ActionBar>().HideRectangle();
    }

    public void ShowActionBarUI(Unit _unit)
    {
        ClearActionBarUI();

       if(_unit.actions.Count == 0)
       {
            return;
       }
        m_ActionBar.GetComponent<ActionBar>().ShowRectangle();
        
        foreach(var action in _unit.actions)
        {
            m_ActionBar.GetComponent<ActionBar>().RegisterActions(action.Icon,() => action.ExecuteAction());
        }
    }
    /// <summary>
    /// 这个方法是用来判断是否触摸到了UI元素，如果是的话就不处理点击事件
    /// </summary>
    /// <returns></returns>
    private bool IsPointerOverUIElement()
    {
        if(Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        else{
            return EventSystem.current.IsPointerOverGameObject();
        }
    }

    public void StartBuildProcess(BuildActionSO _buildAction)
    {
        m_PlacementProcess = new(_buildAction,m_WalkableTilemap,m_overlayTilemap);
        m_PlacementProcess.ShowPlacementOutline();
    }
}
