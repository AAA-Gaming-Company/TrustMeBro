using UnityEngine;

public class NotificaionTriggerCover : TriggerCover {
    [Header("Notificaion")]
    public string[] messages;
    public string speakerName;
    public Sprite speakerImage;
    [Tooltip("Time in seconds to display the notification. If 0, the notification will stay until the player closes it.")]
    [Min(0)]
    public float timeToDisplay;

    protected override void TriggerFunction(GameObject entity) {
        NotificationBuilder.Builder()
            .WithMessages(this.messages)
            .WithSpeakerName(this.speakerName)
            .WithSpeakerImage(this.speakerImage)
            .WithTimeToDisplay(this.timeToDisplay)
            .BuildAndDisplay();
    }
}
