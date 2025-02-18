using System.Collections;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class VoiceManager : Singleton<VoiceManager> {
    public TextMeshProUGUI subtitleElement;

    private AudioSource audioSource;
    private Coroutine subtitleCoroutine;

    private void Start() {
        this.audioSource = this.gameObject.AddComponent<AudioSource>();
        this.audioSource.playOnAwake = false;

        this.subtitleElement.gameObject.SetActive(false);
        this.subtitleElement.text = "";
    }

    //Can be called like so: VoiceManager.Instance.SendCommand([VOICE COMMAND SO]);
    public void SendCommand(VoiceCommand command) {
        //If a sound is already playing, don't play another one
        if (this.subtitleCoroutine != null) {
            Debug.LogError("You shouldn't be playing a sound while another sound is playing!");
            return;
        }

        //Choose a random line from the command and play it
        VoiceLine line = command.ChooseRandomLine();

        this.audioSource.clip = line.clip;
        this.audioSource.Play();

        this.subtitleCoroutine = StartCoroutine(this.ShowText(line.transcription, line.displayTime));
    }

    private IEnumerator ShowText(string line, float time) {
        this.subtitleElement.gameObject.SetActive(true);
        this.subtitleElement.text = line;

        yield return new WaitForSeconds(time);

        this.subtitleElement.text = "";
        this.subtitleElement.gameObject.SetActive(false);

        this.subtitleCoroutine = null;
    }
}
