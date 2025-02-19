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

    private List<IMessageElement> openElements = new List<IMessageElement>();

    public void RegisterElement(IMessageElement element) {
        this.openElements.Add(element);
    }

    public void UnregisterElement(IMessageElement element) {
        this.openElements.Remove(element);
    }

    public void DestroyAllMessages() {
        IMessageElement[] elements = this.openElements.ToArray();
        foreach (IMessageElement element in elements) {
            element.Close();
        }

        if (this.openElements.Count > 0) {
            throw new System.Exception("MessageManager: Not all messages were destroyed.");
        }
    }

    public int GetOpenElementsCount() {
        return this.openElements.Count;
    }
}

public class NotificationBuilder {
    private string[] messages;
    private string speakernName;
    private Sprite speakerImage;
    private float timeToDisplay;

    private NotificationBuilder() {
        this.messages = null;
        this.speakernName = null;
        this.speakerImage = null;
        this.timeToDisplay = 0;
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

    public NotificationBuilder WithTimeToDisplay(float timeToDisplay) {
        this.timeToDisplay = timeToDisplay;
        return this;
    }

    public void BuildAndDisplay() {
        if (this.messages == null || this.speakernName == null || this.speakerImage == null) {
            throw new System.Exception("NotificationBuilder: Missing required fields.");
        }

        NotificationBubble bubble = Object.Instantiate(MessageManager.Instance.notificationPrefab, MessageManager.Instance.canvas.transform);
        bubble.Init(this.messages, this.speakernName, this.speakerImage, this.timeToDisplay);
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

    public DialogueBuilder AddEntry(params DialogueEntry[] entries) {
        this.entries.AddRange(entries);
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
[System.Serializable]
public class DialogueEntry {
    public string[] messages;
    public string speakerName;
    public Sprite speakerImage;
}