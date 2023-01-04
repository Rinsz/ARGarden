using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ObjectsHandling
{
    public class SceneBuildController : MonoBehaviour
    {
        private SceneBuildControllerState state = SceneBuildControllerState.Default;
        private HashSet<GameObject> objectsToDestroy = new();

        [SerializeField] private ObjectsTransformController objectsTransformController;
        [SerializeField] private ControlButtonsManager controlButtonsManager;
        [SerializeField] private ObjectSpawnController objectSpawnController;
        [SerializeField] private LocalModelsBrowserManager modelsBrowserManager;

        [SerializeField] private Button approveButton;
        [SerializeField] private Button revertButton;

        public UnityEvent<SceneBuildControllerState> OnStateChanged = new();

        public void SetState(SceneBuildControllerState state)
        {
            this.state = state;

            if (state == SceneBuildControllerState.Delete)
                controlButtonsManager.SetDecisionButtons();
            else if (state == SceneBuildControllerState.Create)
            {
                modelsBrowserManager.SetMenuActive(true);
                controlButtonsManager.SetAllButtons(true);
            }
            else
                controlButtonsManager.SetAllButtons(state != SceneBuildControllerState.Default);

            OnStateChanged.Invoke(state);
        }

        private void Start()
        {
            approveButton.onClick.AddListener(() =>
            {
                if (state == SceneBuildControllerState.Delete)
                {
                    foreach (var objectToDestroy in objectsToDestroy)
                        Destroy(objectToDestroy);
                }
                objectsTransformController.ReleaseAllChildren();
                SetState(SceneBuildControllerState.Default);
            });
            revertButton.onClick.AddListener(() =>
            {
                if (state == SceneBuildControllerState.Create)
                {
                    objectsTransformController.DestroyAllChildren();
                }
                else
                    objectsTransformController.RevertAllChildren();

                SetState(SceneBuildControllerState.Default);
            });

            modelsBrowserManager.onClosedWithoutSpawn.AddListener(() => SetState(SceneBuildControllerState.Default));

            objectSpawnController.OnSpawned.AddListener(obj =>
            {
                obj.GetComponent<TranslatedObject>().OnTap.AddListener(() =>
                {
                    switch (state)
                    {
                        case SceneBuildControllerState.Default:
                        case SceneBuildControllerState.Create:
                            return;
                        case SceneBuildControllerState.Delete:
                            obj.SetActive(false);
                            objectsToDestroy.Add(obj);
                            return;
                        case SceneBuildControllerState.Edit:
                            if (objectsTransformController.ContainsChild(obj.transform))
                                objectsTransformController.RemoveChild(obj.transform);
                            else
                                objectsTransformController.AddChild(obj.transform);
                            return;
                    }
                });
            });
        }
    }
}