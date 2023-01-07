using System.Collections.Generic;

namespace ObjectsHandling.SceneBuilderStates
{
    public class DefaultState : SceneBuilderState
    {

        public override void Approve(HashSet<TranslatedObject> handledObjects)
        {
        }

        public override void Revert(HashSet<TranslatedObject> handledObjects)
        {
        }

        public override void OnObjectTap(TranslatedObject translatedObject)
        {
        }

        public override void StartState(HashSet<TranslatedObject> handledObjects)
        {
            handledObjects.Clear();
        }
    }
}