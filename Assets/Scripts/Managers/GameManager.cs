using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using DG.Tweening;


public class GameManager : SingletonManager<GameManager>
{
    #region Parameters
    public Unit ActiveUnit;
    private Unit[] SelectedUnits = new Unit[100];
    [SerializeField] private GameObject mousePrefab;
    [Header("Line Renderer Parameters")]
    [SerializeField] private float rayDuration = 2f; // 射线持续时间，默认为2秒
    [SerializeField] private Color rayColor = Color.green; // 射线颜色
    [SerializeField] private float rayWidth = 0.1f; // 射线宽度
    [SerializeField] private float clickDetectionRadius = 0.3f; // 点击检测半径
    [SerializeField] private LineRenderer movementRay; 
    private Vector2 targetPosition; 
    private Vector2 m_MousePosition;
    private Coroutine showRayRedenerer; 
    [Header("UI Parameters")]
    [SerializeField] private GameObject m_ActionBar;
    [SerializeField] private GameObject m_BuildComfiremationBar;
    private bool isMoving = false; 
    private PlacementProcess m_PlacementProcess;

    [Header("Multi-Selection")]
    private Vector2 multiSelectStartPos;
    private bool isMultiSelecting = false;
    [SerializeField] private LineRenderer multiSelectRay;
    [SerializeField] private float multiSelectRayWidth = 0.05f;
    [SerializeField] private Color multiSelectRayColor = Color.yellow;
    
    [Header("Resources Parameters")]
    [SerializeField] private int m_Gold = 100;
    [SerializeField] private int m_Wood = 100;
    [SerializeField] private int m_Meat = 1000;
    public int Gold => m_Gold;
    public int Wood => m_Wood;
    public int Meat => m_Meat;

    #endregion
    
    void Start()
    {
        InitializeLineRenderer();
        InitializeMultiSelectRenderer();
        ClearActionBarUI();
    }

    void Update() 
    {
        if(m_PlacementProcess != null)
        {
            m_PlacementProcess.Update();
        }
        else
        {
            RegisterSquare(); // 绘制出一个矩形框选单位

            CommandUnitToMove(); // 只会单个单位进行移动

            if (Input.GetMouseButtonUp(1) && ActiveUnit != null)
            {
                ClearActionBarUI();
                UnSelectUnit();
            }

        }
    }

    private void CommandUnitToMove()
    {
        if (HvoUtils.IsLeftClickOrTapDown)
        {
            m_MousePosition = HvoUtils.MousePosition;
        }

        if (HvoUtils.IsLeftClickOrTapUp)
        {
            DetectClick(m_MousePosition);
        }

        if (isMoving && ActiveUnit != null && movementRay != null && movementRay.enabled)
        {
            UpdateMovementRay();
        }
    }

    #region Generate Square Area
    private void RegisterSquare()
    {
        // 左键按下开始框选
        if (Input.GetMouseButtonDown(0))
        {
            StartMultiSelect(Input.mousePosition);
        }

        // 左键按住时更新框选
        if (Input.GetMouseButton(0) && isMultiSelecting)
        {
            UpdateMultiSelect(Input.mousePosition);
        }

        // 左键抬起结束框选
        if (Input.GetMouseButtonUp(0))
        {
            EndMultiSelect();
        }
    }

    private void InitializeMultiSelectRenderer()
    {
        //multiSelectRay = gameObject.GetComponent<LineRenderer>();
        multiSelectRay.startWidth = multiSelectRayWidth;
        multiSelectRay.endWidth = multiSelectRayWidth;
        multiSelectRay.material = new Material(Shader.Find("Sprites/Default"));
        multiSelectRay.startColor = multiSelectRayColor;
        multiSelectRay.endColor = multiSelectRayColor;
        multiSelectRay.positionCount = 5; // 绘制一个闭合的矩形
        multiSelectRay.loop = true;
        multiSelectRay.enabled = false;
    }

    private void StartMultiSelect(Vector2 startPos)
    {
        // 如果点击在UI上，不进行框选
        if(HvoUtils.IsPointerOverUIElement())
            return;

        multiSelectStartPos = Camera.main.ScreenToWorldPoint(startPos);
        isMultiSelecting = true;
    }

    private void UpdateMultiSelect(Vector2 currentPos)
    {
        Vector2 currentWorldPos = Camera.main.ScreenToWorldPoint(currentPos);
        
        // 计算矩形的四个角点
        Vector3 topLeft = new Vector3(Mathf.Min(multiSelectStartPos.x, currentWorldPos.x), 
                                      Mathf.Max(multiSelectStartPos.y, currentWorldPos.y), 0);
        Vector3 topRight = new Vector3(Mathf.Max(multiSelectStartPos.x, currentWorldPos.x), 
                                       Mathf.Max(multiSelectStartPos.y, currentWorldPos.y), 0);
        Vector3 bottomRight = new Vector3(Mathf.Max(multiSelectStartPos.x, currentWorldPos.x), 
                                          Mathf.Min(multiSelectStartPos.y, currentWorldPos.y), 0);
        Vector3 bottomLeft = new Vector3(Mathf.Min(multiSelectStartPos.x, currentWorldPos.x), 
                                         Mathf.Min(multiSelectStartPos.y, currentWorldPos.y), 0);

        // 设置LineRenderer的点以绘制矩形
        multiSelectRay.SetPosition(0, topLeft);
        multiSelectRay.SetPosition(1, topRight);
        multiSelectRay.SetPosition(2, bottomRight);
        multiSelectRay.SetPosition(3, bottomLeft);
        multiSelectRay.SetPosition(4, topLeft);
        multiSelectRay.enabled = true;
    }

    private void EndMultiSelect()
    {
        if (!isMultiSelecting)
            return;

        isMultiSelecting = false;
        
        // 确保线渲染器被禁用
        if (multiSelectRay != null)
        {
            multiSelectRay.enabled = false;
        }
    }

    private void UnSelectUnit()
    {
        if(ActiveUnit != null)
        {
            ActiveUnit.GetComponent<Unit>().UnitUnselected();
            ActiveUnit = null;
        }
    }

    #endregion

    #region Detect Click on Unit
    private void InitializeLineRenderer()
    {
        // movementRay = GetComponent<LineRenderer>();
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
        if(HvoUtils.IsPointerOverUIElement())
        {
            return;
        }

        AudioManager.Get().PlaySFX(0);
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(_inputPosition); // convert screen position to world position
        
        RaycastHit2D hit = Physics2D.CircleCast(worldPoint, clickDetectionRadius, Vector2.zero);
        if(HasClickedOnUnit(hit,out Unit _unit))
        {
            HandleClickOnNewUnit(_unit);
        }
        else{
            HandleClickOnOriginalUnit(worldPoint);
        }
    }

    private void HandleClickOnNewUnit(Unit _unit)
    {
        if(_unit == ActiveUnit && ActiveUnit != null)
        {
            ClearActionBarUI();
            UnSelectUnit();
            return;
        }
        else if(WorkerClickedOnUnfinishedUnit(_unit))
        {
            (ActiveUnit as WorkerUnit).SendToBuildingProcess(_unit as StructureUnit); // 这里为了方便使用多态而使用了基类型，在某些情况下要进行类型的安全转换
            (ActiveUnit as WorkerUnit).buildingSound.Play();
            return;
        }

        SelectNewUnit(_unit);
        ActiveUnit.GetComponent<Unit>().UnitSelected();
    }

    private bool WorkerClickedOnUnfinishedUnit(Unit _clickedUnit)  // 这里已经帮助过滤了，是还未完成建造的建筑！
    {
        return ActiveUnit is WorkerUnit &&
                _clickedUnit is StructureUnit structure &&
                structure.IsUnderConstruction;
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
        if (ActiveUnit != null && ActiveUnit.GetComponent<HumanoidUnit>() != null)
        {
            Instantiate(mousePrefab,_worldPosition,Quaternion.identity);
            ActiveUnit.GetComponent<HumanoidUnit>().UnitActed();
            targetPosition = _worldPosition;
            isMoving = true;
            DrawMovementRay(ActiveUnit.transform.position, _worldPosition);
            ActiveUnit.GetComponent<HumanoidUnit>().MoveToDestionation(_worldPosition);
        }
    }

    private void SelectNewUnit(Unit _unit)
    {
        if(ActiveUnit != null)
        {
            ActiveUnit.GetComponent<Unit>().UnitUnselected();
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
   
    public void StartBuildProcess(BuildActionSO _buildAction)
    {
        if(m_PlacementProcess != null)
            return;
        var manager = TilemapManager.Get();
        // 先创建PlacementProcess
        m_PlacementProcess = new(_buildAction,manager);
        
        // 然后再使用它的属性
        m_BuildComfiremationBar.GetComponent<ConstructureUI>().ShowRectanle(_buildAction.GoldCost, _buildAction.WoodCost);
        m_BuildComfiremationBar.GetComponent<ConstructureUI>().RegisterHooks(ComfirmConstruction,CancelConstruction);
        
        m_PlacementProcess.ShowPlacementOutline();
    }

    private void ComfirmConstruction()
    {
        // 安全检查：确保BuildAction不为null
        var buildAction = m_PlacementProcess.BuildAction;
        if (buildAction == null)
        {
            return;
        }

        // 检查资源是否足够
        if(!TryDeduckResources(buildAction.GoldCost, buildAction.WoodCost, 0))
        {
            UnableToPlaceConstructure();
            return;
        }

        // 尝试放置建筑
        if(m_PlacementProcess.TryFinalizePlacement(out Vector3 _placementPosition))
        {
            new BuildingProcess(buildAction, _placementPosition,ActiveUnit as WorkerUnit);
            UnSelectUnit();
            ClearupPlacement();
            ClearActionBarUI();
            AudioManager.Get().PlaySFX(3);
        }
        else
        {
            UnableToPlaceConstructure();
        }
    }

    private void UnableToPlaceConstructure()
    {
        // 为确认按钮添加缩放动画效果
        m_BuildComfiremationBar.GetComponent<ConstructureUI>().comfirmButton.transform
        .DOScale(new Vector3(.8f, .8f, .8f), .05f)
        .SetEase(Ease.OutBack)
        .OnComplete(() =>
        {
            m_BuildComfiremationBar.GetComponent<ConstructureUI>().comfirmButton.transform
                .DOScale(Vector3.one, .05f)
                .SetEase(Ease.InBack);
        });
        AudioManager.Get().PlaySFX(2);
    }

    private void CancelConstruction()
    {
        ClearupPlacement();
        AudioManager.Get().PlaySFX(1);
    }

    private void ClearupPlacement()
    {
        m_PlacementProcess.ClearupPlacement();
        m_PlacementProcess = null;
        m_BuildComfiremationBar.GetComponent<ConstructureUI>().HideRectangle();
    }

    private bool TryDeduckResources(int _goldCost,int _woodCost,int _meatCost)
    {
        return m_Gold >= _goldCost && m_Wood >= _woodCost && m_Meat >= _meatCost;
    }

}
