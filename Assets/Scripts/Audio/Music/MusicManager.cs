using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour {
    private static MusicManager instance;

    private AudioSource audioSource;

    private void Awake() {
        if (MusicManager.instance == null) {
            MusicManager.instance = this;
            this.gameObject.transform.parent = null;
            DontDestroyOnLoad(this.gameObject);

            this.audioSource = this.GetComponent<AudioSource>();
            this.audioSource.loop = true;
        } else {
            Destroy(this.gameObject);
        }
    }

    public static void StopMusic() {
        if (MusicManager.instance != null) {
            MusicManager.instance.audioSource.Stop();
            Destroy(MusicManager.instance.gameObject);
        }
    }

    private void OnApplicationQuit() {
        MusicManager.instance = null;
    }
}
