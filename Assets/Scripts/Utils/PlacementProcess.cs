using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess 
{
    private GameObject m_PlacementOutline;
    private BuildActionSO m_BuildAction;
    private Tilemap m_WalkableTilemap;
    private Tilemap m_overlayTilemap;
    private Tilemap[] m_unreachableTilemaps;
    private Sprite m_TileholderSprite;
    private Vector3Int[] m_HighlightedPosition;
    public BuildActionSO BuildAction => m_BuildAction;
    public PlacementProcess(BuildActionSO _buildAction,Tilemap _walkableTilemap,Tilemap _overlayTilemap,Tilemap[] _unreachableTilemaps)
    {
        m_BuildAction = _buildAction;
        m_WalkableTilemap = _walkableTilemap;
        m_overlayTilemap = _overlayTilemap;
        m_TileholderSprite = Resources.Load<Sprite>("Image/Tileholder");
        m_unreachableTilemaps = _unreachableTilemaps;
    }   

    public void ShowPlacementOutline()
    {
        m_PlacementOutline = new GameObject("PlacementOutline");
    }

    public void Update()
    {
        Vector2 worldPosition = HvoUtils.InputHoldWorldPosition;

        if(HvoUtils.IsPointerOverUIElement())
            return;
        
        if(m_PlacementOutline != null)
        {
            HigtlightPlacementArea(m_PlacementOutline.transform.position);
        }

        if(worldPosition != Vector2.zero)
        {
            m_PlacementOutline.transform.position = SnapToGrid(worldPosition);
        }
    }

    private Vector3 SnapToGrid(Vector3 _worldPosition)
    {
        return new Vector3(Mathf.FloorToInt(_worldPosition.x),Mathf.FloorToInt(_worldPosition.y),0f);
    }

    public void ClearAllHighlights()
    {
        if (m_HighlightedPosition != null)
        {
            foreach (var position in m_HighlightedPosition)
            {
                m_overlayTilemap.SetTile(position, null);
            }
            m_HighlightedPosition = null;
        }
    }

    private void HigtlightPlacementArea(Vector3 _outlinePosition)
    {
        ClearAllHighlights();

        Vector3Int buildingSize = m_BuildAction.BulidingSize;
        Vector3 pivotPosition = _outlinePosition + m_BuildAction.BuildingOffset;
        m_HighlightedPosition = new Vector3Int[buildingSize.x * buildingSize.y];

        for(int x=0;x<buildingSize.x;x++)
        {
            for(int y=0;y<buildingSize.y;y++)
            {
                m_HighlightedPosition[x + y * buildingSize.x] = new Vector3Int((int)pivotPosition.x + x, (int)pivotPosition.y + y, 0);
            }
        }

        // 设置新的高亮
        foreach(var position in m_HighlightedPosition)
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = m_TileholderSprite;
            m_overlayTilemap.SetTile(position, tile);
            m_overlayTilemap.SetTileFlags(position, TileFlags.None);

            if(canPlaceBuilding(position))
            {
                 m_overlayTilemap.SetColor(position, Color.green);
            }
            else{
                 m_overlayTilemap.SetColor(position, Color.red);
            }
           
        }
    }
    public void ClearupPlacement()
    {
        Object.Destroy(m_PlacementOutline);
        ClearAllHighlights();
    }

    public bool TryFinalizePlacement(out Vector3 _placementPosition)
    {
        if(IsPlacementValid())
        {
            _placementPosition = m_PlacementOutline.transform.position;
            ClearupPlacement();
            return true;
        }
        _placementPosition = Vector3.zero;
        return false;
    }

    private bool IsPlacementValid()
    {
        foreach(var position in m_HighlightedPosition)
        {
            if(!canPlaceBuilding(position))
                return false;
        }
        return true;
    }
    
    #region Detect Place to Place Constructure
    private bool canPlaceBuilding(Vector3Int _placePosition)
    {
        return m_WalkableTilemap.HasTile(_placePosition) && !IsPlaceOverUnreachableArea(_placePosition) && !IsPlaceOverObstacle(_placePosition);
    }

    private bool IsPlaceOverUnreachableArea(Vector3Int _placePosition)
    {
        foreach(var tilemap in m_unreachableTilemaps)
        {
            if(tilemap.HasTile(_placePosition))
                return true;
        }
        return false;
    }

    private bool IsPlaceOverObstacle(Vector3Int _placePosition)
    {
        Vector3 tileSize = m_WalkableTilemap.cellSize;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(_placePosition + tileSize * .5f,tileSize * .9f,0f);
        foreach(var hit in colliders)
        {
            if(hit.gameObject.tag == "Unit" || hit.gameObject.tag == "Building")
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
