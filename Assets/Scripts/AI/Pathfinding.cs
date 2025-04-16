using UnityEngine;

public class Pathfinding 
{
    private Node[,] m_Grid;
    private Node[,] Grid => m_Grid;
    private TilemapManager m_tilemapManager;
    private int m_width;
    private int m_height;

    public Pathfinding(TilemapManager _pathFindingManager)
    {
        m_tilemapManager = _pathFindingManager;
        _pathFindingManager.WalkableTilemap.CompressBounds();
        var bounds = m_tilemapManager.WalkableTilemap.cellBounds;
        m_width = bounds.size.x;
        m_height = bounds.size.y;
        m_Grid = new Node[m_width, m_height];
        InitializeGrid(bounds.min); // 加上这个偏移量就能得到世界坐标
    }

    public void InitializeGrid(Vector3Int _offset)
    {
        Vector3 halfCellsize = m_tilemapManager.WalkableTilemap.cellSize * .5f;
        for(int i=0;i<m_width;i++)
        {
            for(int j=0;j<m_height;j++)
            {
                var nodeLeftButtonPosition = new Vector3Int(i + _offset.x, j + _offset.y, 0);
                bool isWalkable = m_tilemapManager.CanWalkAtTile(nodeLeftButtonPosition);
                var node = new Node(nodeLeftButtonPosition,halfCellsize,isWalkable);
                m_Grid[i,j] = node;
            }
        }
    }

    public void FindPath(Vector3 _startPosition,Vector3 _destionationPosition)
    {

    }
}
