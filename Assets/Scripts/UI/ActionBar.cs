using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    public void RegisterActions(Sprite _icon,UnityAction _action)
    {
        var actionButton = Instantiate(m_ActionButtonPrefab,transform);
        actionButton.GetComponent<ActionButton>().InitializeButton(_icon,_action);
        actionButtons.Add(actionButton.GetComponent<ActionButton>());
    }

    public void ClearActions()
    {
        if(actionButtons.Count > 0)
        {
            for(int i = actionButtons.Count - 1; i >= 0; i--)
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
