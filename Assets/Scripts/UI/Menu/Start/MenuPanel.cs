using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuPanel : MonoBehaviour {
    [Header("Main UI")]
    public Button closeButton;

    [Header("Actions")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown graphicsQualityDropdown;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake() {
        this.closeButton.onClick.AddListener(CloseButton);
    }

    public void CloseButton() {
        this.gameObject.SetActive(false);
    }
}
