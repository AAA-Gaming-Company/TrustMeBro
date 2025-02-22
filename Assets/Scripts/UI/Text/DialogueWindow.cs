using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// This class is attached to a single Unity prefab that will display a dialogue window. None
/// of the text needs to be set up, as it will be written by this script, only the layout of
/// the window needs to be set up.
/// To create the dialogue window, see the DialogueBuilder class.
/// </summary>
public class DialogueWindow : MonoBehaviour, IMessageElement {
    public TextMeshProUGUI text;
    public TextMeshProUGUI speakerName;
    public Image speakerImage;
    public Button nextButton;
    public Button skipButton;

    private int currentMessageIndex = -1;
    private int currentEntryIndex = 0;

    private UnityEvent onClose;
    private DialogueEntry[] entries;

    private void Awake() {
        this.nextButton.onClick.AddListener(this.NextMessage);

        this.skipButton.GetComponentInChildren<TextMeshProUGUI>().text = "Skip";
        this.skipButton.onClick.AddListener(this.Close);
    }

    public void Init(DialogueEntry[] entries, UnityEvent onClose) {
        this.entries = entries;
        this.onClose = onClose;

        MessageManager.Instance.RegisterElement(this);

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
            this.Close();
        }

        // Choose if the button should say next or close
        if (this.currentEntryIndex == this.entries.Length - 1 && this.currentMessageIndex == this.entries[this.currentEntryIndex].messages.Length - 1) {
            this.skipButton.gameObject.SetActive(false);
            this.nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
        } else {
            this.skipButton.gameObject.SetActive(true);
            this.nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        }
    }

    public void Close() {
        Time.timeScale = 1f;
        MessageManager.Instance.UnregisterElement(this);
        this.onClose.Invoke();
        Destroy(this.gameObject);
    }
}
