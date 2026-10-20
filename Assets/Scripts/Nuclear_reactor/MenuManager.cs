using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nuclear_reactor
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject menu;

        void OnEnable()  => FirstPersonController.OnEscPressed += OnEsc;
        void OnDisable() => FirstPersonController.OnEscPressed -= OnEsc;

        private void OnEsc(bool blocked)
        {
            menu.SetActive(blocked);
        }

        public void ChangeScene(int index)
        {
            SceneManager.LoadScene(index);
        }
    }
}