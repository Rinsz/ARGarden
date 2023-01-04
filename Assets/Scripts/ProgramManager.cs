using ObjectsHandling;
using UnityEngine;
using UnityEngine.UI;

public class ProgramManager : MonoBehaviour
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button editButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private SceneBuildController sceneBuildController;
    [SerializeField] private GameObject menuButtons;

    private void Start()
    {
        createButton.onClick.AddListener(() => sceneBuildController.SetState(SceneBuildControllerState.Create));
        editButton.onClick.AddListener(() => sceneBuildController.SetState(SceneBuildControllerState.Edit));
        deleteButton.onClick.AddListener(() => sceneBuildController.SetState(SceneBuildControllerState.Delete));

        sceneBuildController.OnStateChanged
            .AddListener(state =>
            {
                menuButtons.SetActive(state == SceneBuildControllerState.Default);
            });
    }
}