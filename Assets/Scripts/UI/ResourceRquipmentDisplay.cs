using System;
using TMPro;
using UnityEngine;

public class ResourceRquipmentDisplay : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI m_GoldCost;
   [SerializeField] private TextMeshProUGUI m_WoodCost;

   public void ShowResourceCost(int _reqgold, int _reqwood)
   {
      m_GoldCost.text = _reqgold.ToString();
      m_WoodCost.text = _reqwood.ToString();

      UpdateRequipmentUIColor(_reqgold,_reqwood);
   }

    void OnDisable()
    {
        m_GoldCost.text = string.Empty;
        m_WoodCost.text = string.Empty;
    }

    private void UpdateRequipmentUIColor(int _goldCost,int _woodCost)
    {
        var manager = GameManager.Get();

        m_GoldCost.color = manager.Gold >= _goldCost ? Color.green : Color.red;
        m_WoodCost.color = manager.Wood >= _woodCost ? Color.green : Color.red;

    }
}
