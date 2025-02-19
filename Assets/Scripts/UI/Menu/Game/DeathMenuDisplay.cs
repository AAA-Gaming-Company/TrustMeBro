using UnityEngine;
using UnityEngine.UI;

public class DeathMenuDisplay : MonoBehaviour {
    public Button restartButton;
    public Button returnMenuButton;

    private void Awake() {
        this.restartButton.onClick.AddListener(StateManager.Instance.RestartGame);;
        this.returnMenuButton.onClick.AddListener(StateManager.Instance.ReturnMenu);
    }
}
