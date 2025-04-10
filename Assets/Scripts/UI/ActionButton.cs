using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    private Button button => GetComponent<Button>();

    [SerializeField] private Image buttomIcon;
}
