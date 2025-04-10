using UnityEngine;

public class HumanoidUnit : Unit
{
    private Animator anim => GetComponentInChildren<Animator>();
    private SpriteRenderer sr => GetComponentInChildren<SpriteRenderer>();
    protected Vector2 m_Velocity;
    protected Vector3 m_lastPosition;
    [Header("Audio Sources")]
    [SerializeField] private AudioSource selectAudio;
    [SerializeField] private AudioSource[] moveAudio;
    [SerializeField] private AudioSource deathAudio;
    private Material m_originalMaterial;
    private bool isDead = false;

    void Start()
    {
        m_lastPosition = transform.position;
        m_originalMaterial = sr.material;
    }

    void Update()
    {
        if(!isDead)
        {
            m_Velocity = new Vector2(transform.position.x - m_lastPosition.x, transform.position.y - m_lastPosition.y) / Time.deltaTime;
            m_lastPosition = transform.position;
        
            isMoving = m_Velocity.magnitude > 0f;
            anim.SetBool("Move", isMoving);
        }
      
    }

    public void UnitSelected() 
    {
        sr.material = Resources.Load<Material>("Materials/Outline");
        selectAudio.Play();
    }
    public void UnitActed() 
    {
        int index = Random.Range(0, moveAudio.Length);
        moveAudio[index].Play();
    }

    public void UnSelectedUnit() 
    {
        sr.material = m_originalMaterial;
    }

    public void Death() 
    {
        deathAudio.Play();
        isDead = true;
        anim.SetBool("Death", true);
    }
}
