using UnityEngine;

public class PauseManager : Singleton<PauseManager> {
    public PauseMenuDisplay pauseMenu;

    private bool isPaused = false;

    private void Start() {
        this.pauseMenu.gameObject.SetActive(false);
    }

    private void Update() {
        if (InputManager.Instance.GetPauseDown()) {
            this.TogglePause();
        }
    }

    public void PauseGame(bool pause) {
        this.isPaused = pause;
        this.pauseMenu.gameObject.SetActive(pause);

        Time.timeScale = pause ? 0f : 1f;
        InputManager.Instance.DisableMoveActions(pause);
    }

    public void TogglePause() {
        this.PauseGame(!this.isPaused);
    }
}
