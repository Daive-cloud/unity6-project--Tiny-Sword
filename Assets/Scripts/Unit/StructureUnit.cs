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
    private float processTimer;
    protected virtual void Update()
    {
        if(IsUnderConstruction && m_BuildingProcess.HasActiveWorker && m_ProcessSlider.value < 1f)
        {
            processTimer -= Time.deltaTime;
            if(processTimer<0f)
            {
                processTimer = m_ProcessWindow;
                m_ProcessSlider.value += m_BuildingProcess.BuildAction.BuildingProcess * (float)m_BuildingProcess.workersCount;

                if(m_ProcessSlider.value >= 1f)
                {
                    AudioManager.Get().PlaySFX(4);
                    m_ProcessSlider.gameObject.SetActive(false);
                    GetComponent<SpriteRenderer>().sprite = m_BuildingProcess.BuildAction.CompletionSprite;
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
