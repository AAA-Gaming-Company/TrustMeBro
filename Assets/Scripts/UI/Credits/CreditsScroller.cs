using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour {
    public RectTransform content;
    public float speed = 10f;
    public Button menuButton;

    private void Awake() {
        Time.timeScale = 1f;
        menuButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Menu");
        });
    }

    private void Update() {
        content.anchoredPosition += Vector2.up * speed * Time.deltaTime;
    }
}
