using UnityEngine;
using UnityEngine.SceneManagement;

public class StateManager : Singleton<StateManager> {
    public PauseMenuDisplay pauseMenu;
    public DeathMenuDisplay deathMenu;

    private bool isPaused = false;

    private void Awake() {
        Time.timeScale = 1f;
    }

    private void Start() {
        this.pauseMenu.gameObject.SetActive(false);
        this.deathMenu.gameObject.SetActive(false);
    }

    private void Update() {
        if (InputManager.Instance.GetPauseDown()) {
            this.TogglePause();
        }
    }

    private void StartStopGame(bool stop) {
        Time.timeScale = stop ? 0f : 1f;
        InputManager.Instance.DisableMoveActions(stop);
    }

    public void PauseGame(bool pause) {
        this.isPaused = pause;
        this.pauseMenu.gameObject.SetActive(pause);
        this.StartStopGame(pause);
    }

    public void TogglePause() {
        this.PauseGame(!this.isPaused);
    }

    public void TriggerDeathMenu() {
        Time.timeScale = 0f;
        this.deathMenu.gameObject.SetActive(true);
        this.StartStopGame(true);
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnMenu() {
        SceneManager.LoadScene("Menu");
    }
}
