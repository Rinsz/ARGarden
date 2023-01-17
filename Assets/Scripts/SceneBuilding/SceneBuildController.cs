using System.Collections.Generic;
using Models;
using ObjectsHandling.SceneBuilderStates;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ObjectsHandling
{
    public class SceneBuildController : MonoBehaviour
    {
        private StateMachine<SceneBuildControllerState, SceneBuilderState> stateMachine;
        private HashSet<TranslatedObject> handledObjects = new();

        [SerializeField] private ObjectsTransformController objectsTransformController;
        [SerializeField] private ControlButtonsManager controlButtonsManager;
        [SerializeField] private ObjectSpawner objectSpawner;
        [SerializeField] private LocalModelsBrowserManager modelsBrowserManager;

        [SerializeField] private Button approveButton;
        [SerializeField] private Button revertButton;

        public UnityEvent<SceneBuildControllerState> OnStateChanged = new();

        public void SetState(SceneBuildControllerState state)
        {
            stateMachine.SetState(state);
            stateMachine.CurrentState.StartState(handledObjects);
            OnStateChanged.Invoke(state);
        }

        private void Start()
        {
            stateMachine = new StateMachine<SceneBuildControllerState, SceneBuilderState>
            (
                new Dictionary<SceneBuildControllerState, SceneBuilderState>
                {
                    [SceneBuildControllerState.Default] = new DefaultState(),
                    [SceneBuildControllerState.Create] = new CreateState(
                        modelsBrowserManager,
                        controlButtonsManager,
                        objectsTransformController),
                    [SceneBuildControllerState.Edit] = new EditState(objectsTransformController, controlButtonsManager),
                    [SceneBuildControllerState.Delete] = new DeleteState(controlButtonsManager)
                },
                SceneBuildControllerState.Default
            );

            approveButton.onClick.AddListener(() =>
            {
                stateMachine.CurrentState.Approve(handledObjects);
                objectsTransformController.ReleaseAllChildren();
                controlButtonsManager.SetAllButtons(false);
                SetState(SceneBuildControllerState.Default);
            });
            revertButton.onClick.AddListener(() =>
            {
                stateMachine.CurrentState.Revert(handledObjects);
                SetState(SceneBuildControllerState.Default);
            });

            modelsBrowserManager.onClosedWithoutSpawn.AddListener(() => SetState(SceneBuildControllerState.Default));

            objectSpawner.OnSpawned.AddListener(obj =>
            {
                handledObjects.Add(obj);
                controlButtonsManager.SetAllButtons(true);
                obj.OnTap.AddListener(() =>
                {
                    handledObjects.Add(obj);
                    stateMachine.CurrentState.OnObjectTap(obj);
                });
            });
        }
    }
}