using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class has to be attached to a GameObject in the scene, as it is used to find the
/// prefabs and the canvas to display the messages. It is a singleton, so it can be accessed
/// from anywhere in the code without a variable reference.
/// </summary>
[DisallowMultipleComponent]
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
        this.messages = null;
        this.speakernName = null;
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
        if (this.messages == null || this.speakernName == null || this.speakerImage == null) {
            throw new System.Exception("NotificationBuilder: Missing required fields.");
        }

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
        if (this.entries.Count == 0) {
            throw new System.Exception("DialogueBuilder: No entries added.");
        }

        DialogueWindow window = Object.Instantiate(MessageManager.Instance.dialoguePrefab, MessageManager.Instance.canvas.transform);
        window.Init(this.entries.ToArray());
    }

    public static DialogueBuilder Builder() {
        return new DialogueBuilder();
    }
}

/// <summary>
/// Internal class to store data required to display a dialogue window.
/// </summary>
public class DialogueEntry {
    public string[] messages;
    public string speakerName;
    public Sprite speakerImage;
}