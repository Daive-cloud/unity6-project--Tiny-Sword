using UnityEngine;

public class WaterController : MonoBehaviour
{
    // 检测区域大小
    [SerializeField] private Vector2 detectionSize = new Vector2(1f, 1f);
    // 检测层级
    [SerializeField] private LayerMask unitLayerMask;
    
    private void Update()
    {
        // 使用Physics2D.OverlapBox检测矩形区域内的单位
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, detectionSize, 0f, unitLayerMask);
        
        foreach (Collider2D collision in colliders)
        {
            if (collision.CompareTag("Unit"))
            {
                HumanoidUnit unit = collision.GetComponent<HumanoidUnit>();
                if (unit != null)
                {
                    unit.Death();
                }
            }
        }
    }
    
    // 在Scene视图中显示检测区域
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectionSize.x, detectionSize.y, 0.1f));
    }
}
