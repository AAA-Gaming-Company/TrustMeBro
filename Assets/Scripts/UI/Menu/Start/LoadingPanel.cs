using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviour {
    public ProgressBar loadingBar;

    private void Awake() {
        this.gameObject.transform.parent = null;
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadGameScene() {
        StartCoroutine(this.LoadGameSceneInt());
    }

    private IEnumerator LoadGameSceneInt() {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Game");

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            this.loadingBar.UpdateValue(progress);
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
