using UnityEngine;

public class NotificationTriggerArea : TriggerArea {
    [Header("Notification")]
    public string[] messages;
    public string speakerName;
    public Sprite speakerImage;

    protected override void TriggerAction() {
        NotificationBuilder.Builder()
            .WithMessages(this.messages)
            .WithSpeakerName(this.speakerName)
            .WithSpeakerImage(this.speakerImage)
            .BuildAndDisplay();
    }
}
