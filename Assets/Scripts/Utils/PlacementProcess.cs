using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess 
{
    private GameObject m_PlacementOutline;
    private BuildActionSO m_BuildAction;
    private Sprite m_TileholderSprite;
    private TilemapManager m_TilemapManager;
    private Vector3Int[] m_HighlightedPosition;
    public BuildActionSO BuildAction => m_BuildAction;
    public PlacementProcess(BuildActionSO _buildAction,TilemapManager _tilemapManager)
    {
        var manager = TilemapManager.Get();
        m_BuildAction = _buildAction;
        m_TilemapManager = _tilemapManager;
        m_TileholderSprite = Resources.Load<Sprite>("Image/Tileholder");
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
                m_TilemapManager.OverlayTilemap.SetTile(position, null);
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
            m_TilemapManager.OverlayTilemap.SetTile(position, tile);
            m_TilemapManager.OverlayTilemap.SetTileFlags(position, TileFlags.None);
    
            if(m_TilemapManager.CanPlaceBuilding(position))
            {
                m_TilemapManager.OverlayTilemap.SetColor(position, Color.green);
            }
            else{
                m_TilemapManager.OverlayTilemap.SetColor(position, Color.red);
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
            if(!m_TilemapManager.CanPlaceBuilding(position))
                return false;
        }
        return true;
    }
    
    
}
