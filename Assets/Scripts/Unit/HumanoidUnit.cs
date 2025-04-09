using UnityEngine;

public class HumanoidUnit : Unit
{
    private Animator anim => GetComponentInChildren<Animator>();
    protected Vector2 m_Velocity;
    protected Vector3 m_lastPosition;

    void Update()
    {
        m_Velocity = new Vector2(transform.position.x - m_lastPosition.x, transform.position.y - m_lastPosition.y) / Time.deltaTime;
        m_lastPosition = transform.position;
        
        isMoving = m_Velocity.magnitude > 0f;
        anim.SetBool("Move", isMoving);
    }
}
