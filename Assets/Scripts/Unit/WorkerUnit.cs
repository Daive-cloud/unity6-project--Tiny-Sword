using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
    protected override void UpdateBehaviour()
    {
        CheckForCloseToTarget();
    }

    private void CheckForCloseToTarget()
    {
        var hits = UnitProximityDection();

        foreach(var hit in hits)
        {
            if(hit.CompareTag("Building") || hit.CompareTag("Trees"))
            {
                Debug.Log(hit.ToString());
            }
        }
    }
}
