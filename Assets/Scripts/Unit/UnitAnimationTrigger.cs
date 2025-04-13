using UnityEngine;

public class UnitAnimationTrigger : MonoBehaviour
{
    private void PawnBuildingSound() => AudioManager.Get().PlaySFX(5);
}
