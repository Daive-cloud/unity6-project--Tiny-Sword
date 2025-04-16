using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : SingletonManager<TilemapManager>
{
    [SerializeField] private Tilemap m_WalkableTilemap;
    [SerializeField] private Tilemap m_overlayTilemap;
    [SerializeField] private Tilemap[] m_unreachableTilemaps;
    public Tilemap WalkableTilemap => m_WalkableTilemap;
    public Tilemap OverlayTilemap => m_overlayTilemap;
    public Tilemap[] UnreachableTilemaps => m_unreachableTilemaps;

    private Pathfinding m_pathfinding;

    void Start()
    {
        m_pathfinding = new(this);
    }

    #region Detect Place to Place Constructure
    public bool CanPlaceBuilding(Vector3Int _placePosition)
    {
        return m_WalkableTilemap.HasTile(_placePosition) && !IsPlaceOverUnreachableArea(_placePosition) && !IsPlaceOverObstacle(_placePosition);
    }

    public bool CanWalkAtTile(Vector3Int _placePosition)
    {
        return m_WalkableTilemap.HasTile(_placePosition) && !IsPlaceOverUnreachableArea(_placePosition);
    }

    public bool IsPlaceOverUnreachableArea(Vector3Int _placePosition)
    {
        foreach(var tilemap in m_unreachableTilemaps)
        {
            if(tilemap.HasTile(_placePosition))
                return true;
        }
        return false;
    }

    public bool IsPlaceOverObstacle(Vector3Int _placePosition)
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
