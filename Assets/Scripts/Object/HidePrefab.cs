using UnityEngine;

public class HidePrefab : MonoBehaviour
{
   [SerializeField] private GameObject prefab;

    private void DestoryObject() => Destroy(prefab);
}
   
