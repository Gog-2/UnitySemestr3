namespace SpaceInveidors.UI
{
    using SpaceInveidors;
    using TMPro;
    using UnityEngine;

    public class PlayerLivesView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _livesText;
        [SerializeField] private string _prefix = "Lives: ";

        private void OnEnable()
        {
            GameService.Instance.OnLivesChanged += HandleLivesChanged;
            HandleLivesChanged(GameService.Instance.Lives);
        }

        private void OnDisable()
        {
            GameService.Instance.OnLivesChanged -= HandleLivesChanged;
        }

        private void HandleLivesChanged(int lives)
        {
                _livesText.text = _prefix + lives;
        }
    }
}
