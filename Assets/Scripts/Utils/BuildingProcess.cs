using UnityEngine;

public class BuildingProcess 
{
    private BuildActionSO buildAction;

    public BuildingProcess(BuildActionSO _buildAction,Vector3 _placePosition)
    {
        this.buildAction = _buildAction;
        var structureGo = Object.Instantiate(buildAction.TowerPrefab);
        structureGo.GetComponent<SpriteRenderer>().sprite = buildAction.FoundationSprite;
        structureGo.transform.position = _placePosition;
        structureGo.GetComponent<StructureUnit>().RegisterProcess(this);
    }

    public void Update()
    {

    }

}
