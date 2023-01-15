using System.Collections.Generic;
using Models;

namespace ObjectsHandling.SceneBuilderStates
{
    public class CreateState : SceneBuilderState
    {
        private readonly LocalModelsBrowserManager modelsBrowserManager;
        private readonly ControlButtonsManager controlButtonsManager;
        private readonly ObjectsTransformController objectsTransformController;

        public CreateState(
            LocalModelsBrowserManager modelsBrowserManager,
            ControlButtonsManager controlButtonsManager,
            ObjectsTransformController objectsTransformController)
        {
            this.modelsBrowserManager = modelsBrowserManager;
            this.controlButtonsManager = controlButtonsManager;
            this.objectsTransformController = objectsTransformController;
        }

        public override void Approve(HashSet<TranslatedObject> handledObjects)
        {
            foreach (var handledObject in handledObjects)
                handledObject.SetOutLineEnabled(false);
        }

        public override void Revert(HashSet<TranslatedObject> handledObjects)
        {
            objectsTransformController.DestroyAllChildren();
        }

        public override void OnObjectTap(TranslatedObject translatedObject)
        {
        }

        public override void StartState(HashSet<TranslatedObject> handledObjects)
        {
            modelsBrowserManager.SetMenuActive(true);
        }
    }
}