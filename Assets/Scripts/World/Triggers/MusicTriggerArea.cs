using UnityEngine;

public class MusicTriggerArea : TriggerArea {
    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip music;

    protected override void TriggerAction(PlayerController player) {
        if (this.music == null) {
            throw new System.Exception("Music is not set!");
        }

        this.musicSource.Stop();
        this.musicSource.clip = this.music;
        this.musicSource.loop = true;
        this.musicSource.Play();
    }
}
