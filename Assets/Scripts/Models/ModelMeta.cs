using System;

namespace Models
{
    [Serializable]
    public class ModelMeta
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ModelGroup ModelGroup { get; set; }
        public int Version { get; set; }
    }
}