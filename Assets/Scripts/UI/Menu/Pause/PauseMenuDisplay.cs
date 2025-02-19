using UnityEngine;
using UnityEngine.UI;

public class PauseMenuDisplay : MonoBehaviour {
    public Button resumeButton;

    private void Awake() {
        this.resumeButton.onClick.AddListener(() => PauseManager.Instance.PauseGame(false));
    }
}
