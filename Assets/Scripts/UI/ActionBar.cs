using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
   private Image image => GetComponent<Image>();
   [SerializeField] private GameObject m_ActionButtonPrefab;

   private List<ActionButton> actionButtons = new List<ActionButton>();

    private void Start()
    {
        HideRectangle();
    }

    public void RegisterActions()
    {
        var actionButton = Instantiate(m_ActionButtonPrefab,transform);
        actionButtons.Add(actionButton.GetComponent<ActionButton>());
    }

    public void ClearActions()
    {
        if(actionButtons.Count > 0)
        {
            for(int i=actionButtons.Count;i >=0 ;i--)
            {
                Destroy(actionButtons[i].gameObject);
                actionButtons.RemoveAt(i);
            }
        }
    }

    public void HideRectangle()
    {
        image.color = Color.clear;
    }

    public void ShowRectangle()
    {
        image.color = Color.white;
    }

}
