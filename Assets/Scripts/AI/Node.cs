using UnityEngine;

public class Node 
{
    public int x;
    public int y;
    public float centerX;
    public float centerY;
    public bool isWalkable;

    public Node(Vector3Int _position,Vector3 _offset,bool isWalkable)
    {
        this.x = _position.x;
        this.y = _position.y;

        Vector3 halfCellSize = _offset * .5f;
        centerX = _position.x + halfCellSize.x;
        centerY = _position.y + halfCellSize.y;

        this.isWalkable = isWalkable;
    }
  
}
