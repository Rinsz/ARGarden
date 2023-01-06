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
        private HashSet<TranslatedObject> handledObjects = new();

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
                    foreach (var objectToDestroy in handledObjects)
                        Destroy(objectToDestroy);
                }
                else
                {
                    foreach (var objectToDestroy in handledObjects)
                        objectToDestroy.SetOutLineEnabled(false);
                }
                handledObjects.Clear();
                objectsTransformController.ReleaseAllChildren();
                SetState(SceneBuildControllerState.Default);
            });
            revertButton.onClick.AddListener(() =>
            {
                if (state == SceneBuildControllerState.Create)
                {
                    objectsTransformController.DestroyAllChildren();
                }
                else if (state == SceneBuildControllerState.Delete)
                {
                    foreach (var obj in handledObjects)
                        obj.gameObject.SetActive(true);
                }
                else
                    objectsTransformController.RevertAllChildren();

                handledObjects.Clear();

                SetState(SceneBuildControllerState.Default);
            });

            modelsBrowserManager.onClosedWithoutSpawn.AddListener(() => SetState(SceneBuildControllerState.Default));

            objectSpawnController.OnSpawned.AddListener(obj =>
            {
                handledObjects.Add(obj);
                obj.OnTap.AddListener(() =>
                {
                    handledObjects.Add(obj);
                    switch (state)
                    {
                        case SceneBuildControllerState.Default:
                        case SceneBuildControllerState.Create:
                            return;
                        case SceneBuildControllerState.Delete:
                            obj.gameObject.SetActive(false);
                            return;
                        case SceneBuildControllerState.Edit:
                            if (objectsTransformController.ContainsChild(obj.transform))
                            {
                                objectsTransformController.RemoveChild(obj.transform);
                                obj.SetOutLineEnabled(false);
                            }
                            else
                            {
                                objectsTransformController.AddChild(obj.transform);
                                obj.SetOutLineEnabled(true);
                            }
                            return;
                    }
                });
            });
        }
    }
}