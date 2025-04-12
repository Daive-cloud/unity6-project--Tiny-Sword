using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConstructureUI : MonoBehaviour
{
    public Button comfirmButton;
    public Button cancleButton;
    public GameObject resourceRquipmentBar;

    void Start()
    {
        HideRectangle();
    }
    public void ShowRectanle(int _gold,int _wood)
    {
        gameObject.SetActive(true);
        resourceRquipmentBar.GetComponent<ResourceRquipmentDisplay>().ShowResourceCost(_gold,_wood);
    }

    public void HideRectangle()
    {
        gameObject.SetActive(false);
    }

    public void RegisterHooks(UnityAction _comfirmAction,UnityAction _cancleAction)
    {
        comfirmButton.onClick.AddListener(_comfirmAction);
        cancleButton.onClick.AddListener(_cancleAction);
    }

    void OnDisable()
    {
        comfirmButton.onClick.RemoveAllListeners();
        cancleButton.onClick.RemoveAllListeners();
    }

}
