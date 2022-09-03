using System.Collections;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] int MIDIIndex;
    public string KeyName;

    public AudioManager audioManager;
    MIDIReader reader;
    Synth synth;

    [Header("Key Animation")]
    public float pressAngle = 10f;
    public float pressSpeed = 10f;

    private Quaternion restRotation;
    private Coroutine rotationCoroutine;

    void Start()
    {
        synth = gameObject.AddComponent<Synth>();
        audioManager = FindObjectOfType<AudioManager>();
        reader = FindObjectOfType<MIDIReader>();

        synth.hz = 440f * Mathf.Pow(2f, (MIDIIndex - 69) / 12f);
        AudioManager.KeyDictionary.Add(MIDIIndex, this);

        restRotation = transform.localRotation; // Save original rotation
    }

    public void Play(float velocity, int channel)
    {
        synth.pan = reader.ChannelPan[channel];
        synth.sustainPedal = reader.ChannelSustain[channel];

        var channelVolume = reader.ChannelVolume[channel];
        synth.velocity = ((((velocity / 127f) * (channelVolume / 100f)) * audioManager.MasterVolume)) / 100f;

        synth.Play();

        // Rotate key down
        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
        rotationCoroutine = StartCoroutine(RotateKey(restRotation * Quaternion.Euler(-pressAngle, 0f, 0f)));
    }

    public void Stop()
    {
        synth.Stop();
        audioManager.gameObject.GetComponent<Effects>().StopEffect(GetComponent<Key>());

        // Rotate key back up
        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
        rotationCoroutine = StartCoroutine(RotateKey(restRotation));
    }

    private IEnumerator RotateKey(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * pressSpeed);
            yield return null;
        }
        transform.localRotation = targetRotation;
    }
}
