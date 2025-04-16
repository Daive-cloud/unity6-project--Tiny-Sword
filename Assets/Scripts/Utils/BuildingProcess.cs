using System.Collections.Generic;
using UnityEngine;

public class BuildingProcess 
{
    private BuildActionSO m_BuildAction;
    private List<WorkerUnit> m_Workers = new();
    public bool HasActiveWorker => m_Workers.Count > 0;
    public int workersCount => m_Workers.Count;
    public BuildActionSO BuildAction => m_BuildAction;

    public BuildingProcess(BuildActionSO _buildAction,Vector3 _placePosition,WorkerUnit _unit)
    {
        this.m_BuildAction = _buildAction;
        var structureGo = Object.Instantiate(m_BuildAction.TowerPrefab);
        structureGo.GetComponent<SpriteRenderer>().sprite = m_BuildAction.FoundationSprite;
        structureGo.transform.position = _placePosition;
        structureGo.GetComponent<StructureUnit>().RegisterProcess(this);
        _unit.MoveToDestionation(_placePosition);

        _unit.RegisterTask(UnitTask.Build);
        _unit.RegisterTarget(structureGo.GetComponent<Unit>());
    }

    public void RegisterWorker(WorkerUnit _unit)
    {
        if(!m_Workers.Contains(_unit))
            m_Workers.Add(_unit);
    }

    public void RemoveWorker(WorkerUnit _unit)
    {
        if(!HasActiveWorker)
        {
            return;
        }
        m_Workers.Remove(_unit);
    }

    public void OnConstructionCompleted()
    {
        foreach (var worker in m_Workers)
        {
            worker.FinishedConstructionProcess();
        }
        m_Workers.Clear();
    }

}
