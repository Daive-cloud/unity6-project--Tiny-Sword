using UnityEngine;
using System.Collections.Generic;

public enum UnitState
{
    Idle,Moving,Attacking,Chopping,Mining
}

public enum UnitTask
{
    None,Build,Chop,Mine,Attack
}

public abstract class Unit : MonoBehaviour
{
    protected SpriteRenderer sr => GetComponentInChildren<SpriteRenderer>();
    private Material m_originalMaterial;
   
     [Header("Mono Actions")]
    public List<ActionSO> actions = new();
    [Header("Object Detection")]
    public float ObjectDetectionRadius = 3f; 
    [SerializeField] private Color color;

    public UnitState currentState {get;protected set;} = UnitState.Idle;
    public UnitTask currentTask {get;protected set;} = UnitTask.None;

    protected virtual void Awake()
    {
       
    }

    protected virtual void Start()
    {
        m_originalMaterial = sr.material;
    }

    public virtual void UnitSelected()
    {
        sr.material = Resources.Load<Material>("Materials/Outline");
    }

    public virtual void UnitUnselected()
    {
        sr.material = m_originalMaterial;
    }

    public Collider2D[] UnitProximityDection()
    {
        return Physics2D.OverlapCircleAll(transform.position, ObjectDetectionRadius);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, ObjectDetectionRadius);
    }

    public void ChangeState(UnitState _newState)
    {
        OnChangeState(currentState, _newState);
    }

    protected virtual void OnChangeState(UnitState _originalState, UnitState _newState)
    {
        currentState = _newState;
    }
 
    public void RegisterTask(UnitTask _newTask)
    {
        OnRegisterTask(currentTask, _newTask);
    }

    protected virtual void OnRegisterTask(UnitTask _originalTask, UnitTask _newTask)
    {
        currentTask = _newTask;
    }



}
