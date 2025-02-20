using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour {
    [Header("Main UI")]
    public Button closeButton;

    private void Awake() {
        this.closeButton.onClick.AddListener(CloseButton);
    }

    public void CloseButton() {
        this.gameObject.SetActive(false);
    }
}
