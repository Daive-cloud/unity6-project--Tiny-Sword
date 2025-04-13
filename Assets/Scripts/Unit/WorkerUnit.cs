using System.Threading.Tasks;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class WorkerUnit : HumanoidUnit
{
    public AudioSource buildingSound;
    protected override void UpdateBehaviour()
    {
        if(currentTask == UnitTask.Build && HasRegisterUnit) // 这里已经设置了建造目标，和建造任务
        {
            DetectBuildingTargetDistance();
            anim.SetBool("Build",currentState == UnitState.Building);
        }
    }

    private void DetectBuildingTargetDistance()
    {
        var distanceToConstructure = Vector2.Distance(transform.position, Target.transform.position);

        if(distanceToConstructure <= ObjectDetectionRadius)
        {
            FlipController(Target.transform.position);
            m_AIPawn.StopMoving();
            ChangeState(UnitState.Building);
            StartBuildingProcess(Target.GetComponent<StructureUnit>());
        }
    }
    public void SendToBuildingProcess(StructureUnit _structure)
    {
        MoveToDestionation(_structure.transform.position);
        RegisterTarget(_structure);
        RegisterTask(UnitTask.Build);
    }

    protected override void OnRegisterDestination()
    {
        ResetUnit();
    }

    private void StartBuildingProcess(StructureUnit _structure)
    {
        _structure.AssignWorkerToBuildingProcess(this);
    }

    protected override void ResetUnit()
    {
        if(Target is StructureUnit structure)
        {
            structure.UnassignWorkerFromBuildingProcess(this);
        }
        ChangeState(UnitState.Idle);
        anim.SetBool("Build",false);
        base.ResetUnit();
    }

    public void FinishedConstructionProcess() 
    {
        ChangeState(UnitState.Idle);
        anim.SetBool("Build",currentState == UnitState.Building);

        RegisterTask(UnitTask.None);
        if(HasRegisterUnit) RegisterTarget(null);
    }
   
}
