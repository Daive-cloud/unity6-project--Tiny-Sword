using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class StructureUnit : Unit
{
    protected BuildingProcess m_BuildingProcess;
    [SerializeField] private GameObject defenseRadiusCircle;
    public bool IsUnderConstruction => m_BuildingProcess != null;
    [SerializeField] private Slider m_ProcessSlider;
    [SerializeField] private float m_ProcessWindow;
    [SerializeField] private GameObject m_ContructrueUnit;
    private EntityFX fx => GetComponent<EntityFX>();
    private CapsuleCollider2D cd => GetComponent<CapsuleCollider2D>();

    private bool IsWorkerAssigned => m_BuildingProcess.HasActiveWorker;
    private float processTimer;
    protected virtual void Update()
    {   
        if(!IsWorkerAssigned)
        {
            fx.StopBuildingEffect();
        }

        if(IsUnderConstruction && IsWorkerAssigned && m_ProcessSlider.value < 1f)
        {
            fx.PlayBuildingEffect();
            processTimer -= Time.deltaTime;
            if(processTimer<0f)
            {
                processTimer = m_ProcessWindow;
                m_ProcessSlider.value += m_BuildingProcess.BuildAction.BuildingProcess * (float)m_BuildingProcess.workersCount;

                if(m_ProcessSlider.value >= 1f)
                {
                    fx.StopBuildingEffect();
                    cd.size = new Vector2(1.2f,2.6f);
                    AudioManager.Get().PlaySFX(4);
                    m_ProcessSlider.gameObject.SetActive(false);
                    GetComponent<SpriteRenderer>().sprite = m_BuildingProcess.BuildAction.CompletionSprite;

                    if(m_ContructrueUnit != null)
                    {
                        m_ContructrueUnit.SetActive(true);
                    }
                    FinishedProcess();
                }
            }
        
        }
    }

    public virtual void RegisterProcess(BuildingProcess _process)
    {
        m_BuildingProcess = _process;
    }

    public virtual void FinishedProcess()
    {
        m_BuildingProcess.OnConstructionCompleted();
        m_BuildingProcess = null;
    }

    public override void UnitSelected()
    {
        base.UnitSelected();
        defenseRadiusCircle.GetComponent<DrawCircleWithDotween>().RegisterDefensiveCircle();
    }

    public override void UnitUnselected()
    {
        base.UnitUnselected();
        defenseRadiusCircle.GetComponent<DrawCircleWithDotween>().ShrinkAndDisappear();
    }

    public void AssignWorkerToBuildingProcess(WorkerUnit _unit)
    {
        m_BuildingProcess?.RegisterWorker(_unit);
    }

    public void UnassignWorkerFromBuildingProcess(WorkerUnit _unit)
    {
        m_BuildingProcess?.RemoveWorker(_unit);
    }

  
}
