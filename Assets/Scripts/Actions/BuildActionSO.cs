using UnityEngine;

[CreateAssetMenu(fileName = "BuildAction", menuName = "Action/Build Action")]
public class BuildActionSO : ActionSO
{
    [SerializeField] private Sprite m_FoundationSprite;
    [SerializeField] private Sprite m_CompletionSprite;
    [SerializeField] private GameObject m_TowerPrefab;
    [SerializeField] private int m_GoldCost;
    [SerializeField] private int m_WoodCost;
    [SerializeField] private Vector3Int m_BuildingSize;
    [SerializeField] private Vector3Int m_BuildingOffset;
    [SerializeField] private float m_BuildingProcess;
    public Sprite FoundationSprite => m_FoundationSprite; // 这里相当于提供了一个只读的属性访问器
    public Sprite CompletionSprite => m_CompletionSprite;
    public int GoldCost => m_GoldCost;
    public int WoodCost => m_WoodCost;
    public Vector3Int BulidingSize => m_BuildingSize;
    public Vector3Int BuildingOffset => m_BuildingOffset;
    public GameObject TowerPrefab => m_TowerPrefab;
    public float BuildingProcess => m_BuildingProcess;


    public override void ExecuteAction()
    {
        GameManager.Get().StartBuildProcess(this);
    }
}
