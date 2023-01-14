using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Models.Descriptors
{
    [RequireComponent(typeof(Button))]
    public class ModelGroupCard : MonoBehaviour
    {
        [SerializeField] private ModelGroup modelGroup;

        public UnityEvent<ModelGroup> OnGroupChoose { get; } = new();

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => OnGroupChoose.Invoke(modelGroup));
        }
    }
}