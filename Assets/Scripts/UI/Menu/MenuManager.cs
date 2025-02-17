using UnityEngine;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager> {
    [Header("UI Elements")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;
    public Button creditsButton;

    [Header("UI Panels")]
    public MenuPanel settingsPanel;
    public MenuPanel creditsPanel;
    public LoadingPanel loadingPanel;
    public DifficultyPanel difficultyPanel;

    private void Awake() {
        Time.timeScale = 1f;

        this.playButton.onClick.AddListener(PlayButton);
        this.settingsButton.onClick.AddListener(SettingsButton);
        this.quitButton.onClick.AddListener(QuitButton);
        this.creditsButton.onClick.AddListener(CreditsButton);

        this.settingsPanel.gameObject.SetActive(false);
        this.creditsPanel.gameObject.SetActive(false);
        this.loadingPanel.gameObject.SetActive(false);
        this.difficultyPanel.gameObject.SetActive(false);

        Debug.Log("TODO: ADD MUSIC");
    }

    public void PlayButton() {
        this.difficultyPanel.gameObject.SetActive(true);
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

    public void LaunchGame(DifficultyLevel level) {
        GameManager.difficultyLevel = level;
        this.loadingPanel.gameObject.SetActive(true);
        this.loadingPanel.LoadGameScene();
    }
}
