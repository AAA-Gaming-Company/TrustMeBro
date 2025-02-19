using UnityEngine;
using UnityEngine.UI;

public class PauseMenuDisplay : MonoBehaviour {
    public Button resumeButton;
    public Button returnMenuButton;

    private void Awake() {
        this.resumeButton.onClick.AddListener(() => StateManager.Instance.PauseGame(false));
        this.returnMenuButton.onClick.AddListener(StateManager.Instance.ReturnMenu);
    }
}
