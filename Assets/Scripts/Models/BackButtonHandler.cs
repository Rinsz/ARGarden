using UnityEngine;
using UnityEngine.UI;

namespace Models
{
    public class BackButtonHandler : MonoBehaviour
    {
        [SerializeField] private GameObject form;
        [SerializeField] private Button approveButton;
        [SerializeField] private Button revertButton;

        private void Start()
        {
            approveButton.onClick.AddListener(Application.Quit);
            revertButton.onClick.AddListener(() => form.SetActive(false));
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
                form.SetActive(true);
        }
    }
}