using UnityEngine;
using DG.Tweening;

public class StructureUnit : Unit
{
    protected BuildingProcess m_BuildingProcess;
    [SerializeField] private GameObject defenseRadiusCircle;

    protected bool IsUnderConstrution => m_BuildingProcess != null;
    protected virtual void Update()
    {
        if(IsUnderConstrution)
        {
            m_BuildingProcess.Update();
        }
    }

    public virtual void RegisterProcess(BuildingProcess _process)
    {
        m_BuildingProcess = _process;
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

  
}
