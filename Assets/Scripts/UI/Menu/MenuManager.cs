using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [Header("UI Elements")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;
    public Button creditsButton;

    [Header("UI Panels")]
    public MenuPanel settingsPanel;
    public MenuPanel creditsPanel;
    public LoadingPanel loadingPanel;

    private void Awake() {
        Time.timeScale = 1f;

        this.playButton.onClick.AddListener(PlayButton);
        this.settingsButton.onClick.AddListener(SettingsButton);
        this.quitButton.onClick.AddListener(QuitButton);
        this.creditsButton.onClick.AddListener(CreditsButton);

        this.settingsPanel.gameObject.SetActive(false);
        this.creditsPanel.gameObject.SetActive(false);
        this.loadingPanel.gameObject.SetActive(false);

        Debug.Log("TODO: ADD MUSIC");
        Debug.Log("TODO: ADD DIFFICULTY SETTINGS");
    }

    public void PlayButton() {
        this.loadingPanel.gameObject.SetActive(true);
        this.loadingPanel.LoadGameScene();
    }

    public void SettingsButton() {
        this.settingsPanel.gameObject.SetActive(true);
    }

    public void QuitButton() {
        Application.Quit();
    }

    public void CreditsButton() {
        this.creditsPanel.gameObject.SetActive(true);
    }
}
