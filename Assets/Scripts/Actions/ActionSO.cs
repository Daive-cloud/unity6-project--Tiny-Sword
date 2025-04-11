using UnityEngine;

public abstract class ActionSO : ScriptableObject
{
    public Sprite Icon;
    public string ActionName;

    public string ID = System.Guid.NewGuid().ToString();

    public abstract void ExecuteAction();
}
