using UnityEngine;
using UnityEngine.UI;

public class DifficultyPanel : MonoBehaviour {
    [Tooltip("Difficulty buttons, index 0 is the easiest.")]
    public Button[] difficultyButtons;

    private void Awake() {
        for (int i = 0; i < this.difficultyButtons.Length; i++) {
            DifficultyLevel level = (DifficultyLevel) i;
            this.difficultyButtons[i].onClick.AddListener(() => MenuManager.Instance.LaunchGame(level));
        }
    }
}
