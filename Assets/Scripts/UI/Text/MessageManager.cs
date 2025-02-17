using System.Collections.Generic;
using UnityEngine;

public class MessageManager : Singleton<MessageManager> {
    [Header("UI Elements")]
    public Canvas canvas;

    [Header("Prefabs")]
    public NotificationBubble notificationPrefab;
    public DialogueWindow dialoguePrefab;
}

public class NotificationBuilder {
    private string[] messages;
    private string speakernName;
    private Sprite speakerImage;

    private NotificationBuilder() {
        this.messages = new string[0];
        this.speakernName = "";
        this.speakerImage = null;
    }

    public NotificationBuilder WithMessages(params string[] messages) {
        this.messages = messages;
        return this;
    }

    public NotificationBuilder WithSpeakerName(string speakerName) {
        this.speakernName = speakerName;
        return this;
    }

    public NotificationBuilder WithSpeakerImage(Sprite speakerImage) {
        this.speakerImage = speakerImage;
        return this;
    }

    public void BuildAndDisplay() {
        NotificationBubble bubble = Object.Instantiate(MessageManager.Instance.notificationPrefab, MessageManager.Instance.canvas.transform);
        bubble.Init(this.messages, this.speakernName, this.speakerImage);
    }

    public static NotificationBuilder Builder() {
        return new NotificationBuilder();
    }
}

public class DialogueBuilder {
    private List<DialogueEntry> entries;

    private DialogueBuilder() {
        this.entries = new List<DialogueEntry>();
    }

    public DialogueBuilder AddEntry(string speakerName, Sprite speakerImage, params string[] messages) {
        DialogueEntry entry = new DialogueEntry();
        entry.speakerName = speakerName;
        entry.speakerImage = speakerImage;
        entry.messages = messages;

        this.entries.Add(entry);
        return this;
    } 

    public void BuildAndDisplay() {
        DialogueWindow window = Object.Instantiate(MessageManager.Instance.dialoguePrefab, MessageManager.Instance.canvas.transform);
        window.Init(this.entries.ToArray());
    }

    public static DialogueBuilder Builder() {
        return new DialogueBuilder();
    }
}

public class DialogueEntry {
    public string[] messages;
    public string speakerName;
    public Sprite speakerImage;
}