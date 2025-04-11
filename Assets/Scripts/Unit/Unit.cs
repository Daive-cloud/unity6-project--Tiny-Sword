using UnityEngine;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour
{
    public bool isMoving;
    protected AIPawn m_AIPawn;

    public float facingDir {get;private set;} = 1;
    public bool isFacingRight = true;
     [Header("Mono Actions")]
    public List<ActionSO> actions = new();

    protected virtual void Awake()
    {
        
        if(TryGetComponent<AIPawn>(out var pawn))
        {
            m_AIPawn = pawn;
        }
    }

    public virtual void MoveToDestionation(Vector2 _destination)
    {   
        m_AIPawn.SetDestination(_destination);
        FlipController(_destination);
    }

    protected void FlipController(Vector2 mousePosition)
    {
        if(mousePosition.x > transform.position.x)
        {
            if(!isFacingRight) Flip(); 
        }
        else if(mousePosition.x < transform.position.x)
        {
            if(isFacingRight) Flip(); 
        }
    }

    protected void Flip()
    {
        facingDir *= -1;
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }
}
