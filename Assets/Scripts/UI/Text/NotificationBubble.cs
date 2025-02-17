using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationBubble : MonoBehaviour {
    public TextMeshProUGUI text;
    public TextMeshProUGUI speakerName;
    public Image speakerImage;
    public Button nextButton;

    private int currentMessageIndex = -1;
    private string[] messages;

    private void Awake() {
        this.nextButton.onClick.AddListener(this.NextMessage);
    }

    public void Init(string[] messages, string speakerName, Sprite speakerImage) {
        this.messages = messages;
        this.speakerName.text = speakerName;
        this.speakerImage.sprite = speakerImage;

        Time.timeScale = 0f;
        this.NextMessage();
    }

    public void NextMessage() {
        this.currentMessageIndex++;
        if (this.currentMessageIndex < this.messages.Length) {
            this.text.text = this.messages[this.currentMessageIndex];
        } else {
            Time.timeScale = 1f;
            Destroy(this.gameObject);
        }

        // Choose if the button should say next or close
        if (this.currentMessageIndex == this.messages.Length - 1) {
            this.nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
        } else {
            this.nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        }
    }
}
