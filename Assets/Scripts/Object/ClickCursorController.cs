using UnityEngine;

public class ClickCursorController : MonoBehaviour
{
    [SerializeField] private float m_Duration = 1f;
    [SerializeField] private AnimationCurve curve; // 这是一种相对来说更快设置动画的方式，曲线动画
    private Vector3 m_initilalScale;
    private SpriteRenderer sr => GetComponent<SpriteRenderer>();
    private float m_Timer;

    void Start()
    {
        m_Timer = m_Duration;
        m_initilalScale = transform.localScale;
    }

    void Update()
    {
        m_Timer -= Time.deltaTime;

        float scaleModifier = curve.Evaluate(m_Timer);
        transform.localScale = m_initilalScale * scaleModifier;

        if(m_Timer < m_Duration * .5f)
        {
            float fadeSpeed = 2 * m_Timer/m_Duration;
            sr.color = new Color(1,1,1,1-fadeSpeed);
        }

        if(m_Timer < 0f)
        {
            Destroy(gameObject);
        }
    }


}
