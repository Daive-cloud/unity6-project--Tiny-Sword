using UnityEngine;

public class AIPawn : MonoBehaviour
{
   [SerializeField] private float moveSpeed = 5f;

   private Vector3? m_Destination;
   public Vector3? Destination => m_Destination;

    void Update()
    {
        if(m_Destination.HasValue)
        {
            var direction = m_Destination.Value - transform.position;
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;

            float distanceToMove = Vector3.Distance(transform.position, m_Destination.Value);
            if(distanceToMove < .1f)
            {
                m_Destination = null;
            }

        }
    }

    public void SetDestination(Vector3? destination)
   {
       m_Destination = destination;
   }
}
