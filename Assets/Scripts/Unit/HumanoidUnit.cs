using System.Collections.Generic;
using UnityEngine;

public class HumanoidUnit : Unit
{
    [Header("Humanoid Unit Info")]
     public bool isMoving;
    protected AIPawn m_AIPawn;

    public float facingDir {get;private set;} = 1;
    public bool isFacingRight = true;
    protected Animator anim => GetComponentInChildren<Animator>();
    protected Vector2 m_Velocity;
    protected Vector3 m_lastPosition;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource selectAudio;
    [SerializeField] private AudioSource[] moveAudio;
    [SerializeField] private AudioSource deathAudio;
    
    private bool isDead = false;

    protected override void Awake()
    {
        base.Awake();
        if(TryGetComponent<AIPawn>(out var pawn))
        {
            m_AIPawn = pawn;
        }
    }

    protected override void Start()
    {
        base.Start();
        m_lastPosition = transform.position;
    }

    void Update()
    {
        if(!isDead)
        {
            // 这里的逻辑是处理动画的播放逻辑，以及移动状态的更新
            m_Velocity = new Vector2(transform.position.x - m_lastPosition.x, transform.position.y - m_lastPosition.y) / Time.deltaTime;
            m_lastPosition = transform.position;
            var state = m_Velocity.magnitude > 0f ? UnitState.Moving : UnitState.Idle;
            ChangeState(state);
            anim.SetBool("Move", state == UnitState.Moving);
            UpdateBehaviour();
        }
      
    }

    protected virtual void UpdateBehaviour()
    {

    }
    public override void UnitSelected() 
    {
        base.UnitSelected();
        selectAudio.Play();
    }
    public void UnitActed() 
    {
        int index = Random.Range(0, moveAudio.Length);
        moveAudio[index].Play();
    }

    public override void UnitUnselected() 
    {
        base.UnitUnselected();
    }

    public void Death() 
    {
        deathAudio.Play();
        isDead = true;
        anim.SetBool("Death", true);
    }

    #region Move Functions
     public virtual void MoveToDestionation(Vector2 _destination)
    {   
        m_AIPawn.RegisterDestination(_destination);
        FlipController(_destination);

        OnRegisterDestination();
    }

    protected virtual void OnRegisterDestination()
    {

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
    #endregion
}
