using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

public class ControlButtonsManager : MonoBehaviour
{
    private const string NullButtonError = "{0} must be specified";

    private GameObject[] allButtons;

    [SerializeField, RequiredMember] private GameObject approveButton;
    [SerializeField] private GameObject revertButton;
    [SerializeField] private GameObject translateButton;
    [SerializeField] private GameObject scaleButton;
    [SerializeField] private GameObject rotateButton;

    public void Awake()
    {
        allButtons = new[] { approveButton, revertButton, translateButton, scaleButton, rotateButton }
            .Where(component => component)
            .ToArray();
    }

    public void SetAllButtons(bool active)
    {
        foreach (var button in allButtons)
        {
            button.SetActive(active);
        }
    }

    public void SetDecisionButtons()
    {
        SetAllButtons(false);
        approveButton.SetActive(true);
        revertButton.SetActive(true);
    }
}