using UnityEngine;

public class EntityFX : MonoBehaviour
{
   [SerializeField] private ParticleSystem m_BuildingEffect;

   public void PlayBuildingEffect() => m_BuildingEffect.Play();

   public void StopBuildingEffect() => m_BuildingEffect.Stop();
}
