using System.Collections.Generic;

namespace ObjectsHandling.SceneBuilderStates
{
    public class EditState : SceneBuilderState
    {
        private readonly ObjectsTransformController objectsTransformController;
        private readonly ControlButtonsManager controlButtonsManager;

        public EditState(
            ObjectsTransformController objectsTransformController,
            ControlButtonsManager controlButtonsManager)
        {
            this.objectsTransformController = objectsTransformController;
            this.controlButtonsManager = controlButtonsManager;
        }

        public override void Approve(HashSet<TranslatedObject> handledObjects)
        {
            foreach (var handledObject in handledObjects)
                handledObject.SetOutLineEnabled(false);
        }

        public override void Revert(HashSet<TranslatedObject> handledObjects)
        {
            objectsTransformController.RevertAllChildren();
        }

        public override void OnObjectTap(TranslatedObject translatedObject)
        {
            if (objectsTransformController.ContainsChild(translatedObject.transform))
            {
                objectsTransformController.ReleaseChild(translatedObject.transform);
                translatedObject.SetOutLineEnabled(false);
            }
            else
            {
                objectsTransformController.AddChild(translatedObject.transform);
                translatedObject.SetOutLineEnabled(true);
            }
        }

        public override void StartState(HashSet<TranslatedObject> handledObjects)
        {
            controlButtonsManager.SetAllButtons(true);
        }
    }
}