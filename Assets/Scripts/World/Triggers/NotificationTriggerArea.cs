using UnityEngine;

public class NotificationTriggerArea : TriggerArea {
    [Header("Notification")]
    public string[] messages;
    public string speakerName;
    public Sprite speakerImage;
    [Tooltip("Time in seconds to display the notification. If 0, the notification will stay until the player closes it.")]
    [Min(0)]
    public float timeToDisplay;

    protected override void TriggerAction(PlayerController player) {
        NotificationBuilder.Builder()
            .WithMessages(this.messages)
            .WithSpeakerName(this.speakerName)
            .WithSpeakerImage(this.speakerImage)
            .WithTimeToDisplay(this.timeToDisplay)
            .BuildAndDisplay();
    }
}
