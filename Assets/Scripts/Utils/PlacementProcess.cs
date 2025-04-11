using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementProcess 
{
    private GameObject m_PlacementOutline;
    private BuildActionSO buildAction;
    private Tilemap m_WalkableTilemap;
    private Tilemap m_overlayTilemap;
    private Sprite m_TileholderSprite;
    private Vector3Int[] m_HighlightedPosition;
    public PlacementProcess(BuildActionSO _buildAction,Tilemap _walkableTilemap,Tilemap _overlayTilemap)
    {
        buildAction = _buildAction;
        m_WalkableTilemap = _walkableTilemap;
        m_overlayTilemap = _overlayTilemap;
        m_TileholderSprite = Resources.Load<Sprite>("Image/Tileholder");
    }   

    public void ShowPlacementOutline()
    {
        m_PlacementOutline = new GameObject("PlacementOutline");
        var renderer = m_PlacementOutline.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 100;
        renderer.color = new Color(1,1,1,.54f);
        renderer.sprite = buildAction.PlacemantSprite;
    }

    public void Update()
    {
        Vector2 worldPosition = HvoUtils.InputHoldWorldPosition;

        if(worldPosition != Vector2.zero)
        {
            m_PlacementOutline.transform.position = SnapToGrid(worldPosition);
            HigtlightPlacementArea(m_PlacementOutline.transform.position);
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

        Vector3Int buildingSize = buildAction.BulidingSize;
        Vector3 pivotPosition = _outlinePosition + buildAction.BuildingOffset;
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
            m_overlayTilemap.SetColor(position, Color.green);
        }
    }
}
