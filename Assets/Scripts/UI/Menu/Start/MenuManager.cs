using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MenuManager : Singleton<MenuManager> {
    [Header("UI Elements")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;
    public Button creditsButton;

    [Header("UI Panels")]
    public LoadingPanel loadingPanel;
    public DifficultyPanel difficultyPanel;

    private void Awake() {
        Time.timeScale = 1f;
        GameManager.lastCheckpoint = null;

        this.playButton.onClick.AddListener(PlayButton);
        this.quitButton.onClick.AddListener(QuitButton);
        this.creditsButton.onClick.AddListener(CreditsButton);

        this.loadingPanel.gameObject.SetActive(false);
        this.difficultyPanel.gameObject.SetActive(false);
    }

    public void PlayButton() {
        this.difficultyPanel.gameObject.SetActive(true);
    }

    public void QuitButton() {
        Application.Quit();
    }

    public void CreditsButton() {
        SceneManager.LoadScene("Credits");
    }

    public void LaunchGame(DifficultyLevel level) {
        GameManager.difficultyLevel = level;
        this.loadingPanel.gameObject.SetActive(true);
        this.loadingPanel.LoadGameScene();
    }
}
