using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour {
    public void LaunchCredits() {
        Time.timeScale = 1f;

        MusicManager.StopMusic();
        SceneManager.LoadScene("Credits");
    }
}
