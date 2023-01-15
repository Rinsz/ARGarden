using System.Collections.Generic;

namespace ObjectsHandling.SceneBuilderStates
{
    public abstract class SceneBuilderState
    {
        public abstract void Approve(HashSet<TranslatedObject> handledObjects);
        public abstract void Revert(HashSet<TranslatedObject> handledObjects);
        public abstract void OnObjectTap(TranslatedObject translatedObject);
        public abstract void StartState(HashSet<TranslatedObject> handledObjects);
    }
}