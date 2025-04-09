using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public bool isMoving;
    protected AIPawn m_AIPawn;

    public float facingDir {get;private set;} = 1;
    public bool isFacingRight = true;

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
