using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Custom/Audio Data", order = 1)]
public class ScriptableAudioFile : ScriptableObject
{
    [SerializeField] private AudioClip audioClip;
    [HideInInspector] public float volume = 1.0f;
    [HideInInspector] public float treble = 0.0f;
    [HideInInspector] public float bass = 0.0f;
    [HideInInspector] public float pitch = 1.0f;
    [HideInInspector] public bool loopSound;

    // Add an AudioSource field to the ScriptableObject
    [System.NonSerialized]
    public AudioSource audioSource;
    // Play the audio using the internal AudioSource
    public void PlayAudio()
    {
        if (audioClip != null)
        {
            if (audioSource == null)
            {
                // Create an AudioSource component if it doesn't exist
                GameObject audioObject = new GameObject("AudioPlayer");
                audioSource = audioObject.AddComponent<AudioSource>();
            }
            else
            {
                // Reset previous settings
                audioSource.Stop();
                audioSource.pitch = 1.0f;
            }

            // Set AudioSource properties
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.loop = loopSound;

            // Apply bass boost using low-pass filter
            ApplyBassBoost();

            // Play the audio
            audioSource.Play();
        }
    }

    // Apply bass boost using low-pass filter
    private void ApplyBassBoost()
    {
        AudioLowPassFilter lowPassFilter = audioSource.GetComponent<AudioLowPassFilter>();

        // Check if a low-pass filter component is attached; if not, add one
        if (lowPassFilter == null)
        {
            lowPassFilter = audioSource.gameObject.AddComponent<AudioLowPassFilter>();
        }
        else
        {
            // Reset previous settings
            lowPassFilter.cutoffFrequency = 22000f;
        }

        // Adjust the cutoff frequency to simulate bass boost
        lowPassFilter.cutoffFrequency = 22000f - (bass * 20000f);
    }

    // Stop the audio
    public void StopAudio()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            
            if (audioSource.clip != null && audioSource.clip.length > 0.5f)
            {
            // Check if a clip is assigned to audioSource and its duration is greater than 0.5 seconds
             float clipDuration = audioSource.clip.length;
            if (clipDuration > 0.5f)
           {
            DestroyImmediate(audioSource.gameObject);
           }
            }
        }
    }

    public void ResetAudioControls()
    {
        volume = 1.0f;
        treble = 0.0f;
        bass = 0.0f;
        pitch = 1.0f;
        loopSound = false;
    }

    // Restart the audio
    public void RestartAudio()
    {
        StopAudio();
        PlayAudio();
    }
}



