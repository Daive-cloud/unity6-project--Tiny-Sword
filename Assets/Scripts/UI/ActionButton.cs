using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    private Button button => GetComponent<Button>();
    [SerializeField] private Image buttonIcon;

    public void InitializeButton(Sprite _icon,UnityAction _action)
    {
        buttonIcon.sprite = _icon;
        button.onClick.AddListener(_action);
    }

    void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
