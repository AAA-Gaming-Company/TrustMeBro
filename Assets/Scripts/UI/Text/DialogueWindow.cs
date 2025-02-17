using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueWindow : MonoBehaviour {
    public TextMeshProUGUI text;
    public TextMeshProUGUI speakerName;
    public Image speakerImage;
    public Button nextButton;

    private int currentMessageIndex = -1;
    private int currentEntryIndex = 0;

    private DialogueEntry[] entries;

    private void Awake() {
        this.nextButton.onClick.AddListener(this.NextMessage);
    }

    public void Init(DialogueEntry[] entries) {
        this.entries = entries;

        Time.timeScale = 0f;
        this.NextMessage();
    }

    public void NextMessage() {
        this.currentMessageIndex++;
        if (this.currentMessageIndex == this.entries[this.currentEntryIndex].messages.Length) {
            this.currentMessageIndex = 0;
            this.currentEntryIndex++;
        }

        if (this.currentEntryIndex < this.entries.Length) {
            this.speakerName.text = this.entries[this.currentEntryIndex].speakerName;
            this.speakerImage.sprite = this.entries[this.currentEntryIndex].speakerImage;
            this.text.text = this.entries[this.currentEntryIndex].messages[this.currentMessageIndex];
        } else {
            Time.timeScale = 1f;
            Destroy(this.gameObject);
        }

        // Choose if the button should say next or close
        if (this.currentEntryIndex == this.entries.Length - 1 && this.currentMessageIndex == this.entries[this.currentEntryIndex].messages.Length - 1) {
            this.nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
        } else {
            this.nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        }
    }
}
