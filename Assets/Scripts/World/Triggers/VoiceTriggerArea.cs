using UnityEngine;

public class VoiceTriggerArea : TriggerArea {
    [Header("Voice")]
    public VoiceCommand command;

    protected override void TriggerAction() {
        VoiceManager.Instance.SendCommand(this.command);
    }
}
