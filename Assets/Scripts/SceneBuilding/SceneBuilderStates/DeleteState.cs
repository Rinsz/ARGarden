using System.Collections.Generic;
using UnityEngine;

namespace ObjectsHandling.SceneBuilderStates
{
    public class DeleteState : SceneBuilderState
    {
        private readonly ControlButtonsManager controlButtonsManager;

        public DeleteState(ControlButtonsManager controlButtonsManager)
        {
            this.controlButtonsManager = controlButtonsManager;
        }

        public override void Approve(HashSet<TranslatedObject> handledObjects)
        {
            foreach (var objectToDestroy in handledObjects)
                Object.Destroy(objectToDestroy);
        }

        public override void Revert(HashSet<TranslatedObject> handledObjects)
        {
            foreach (var handledObject in handledObjects)
                handledObject.gameObject.SetActive(true);
            controlButtonsManager.SetAllButtons(false);
        }

        public override void OnObjectTap(TranslatedObject translatedObject)
        {
            translatedObject.gameObject.SetActive(false);
        }

        public override void StartState(HashSet<TranslatedObject> handledObjects)
        {
            controlButtonsManager.SetDecisionButtons();
        }
    }
}