using UnityEngine;
using UnityEngine.Events;

namespace Models.Descriptors
{
    internal class ModelCardDescriptor
    {
        public ModelMeta Meta { get; set; }
        public Sprite Image { get; set; }
        public UnityAction SelectAction { get; set; }

        public void Deconstruct(out ModelMeta meta, out Sprite image, out UnityAction selectAction)
        {
            meta = this.Meta;
            image = this.Image;
            selectAction = this.SelectAction;
        }
    }
}