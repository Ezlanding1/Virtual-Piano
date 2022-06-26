using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Synth : MonoBehaviour
{
    [SerializeField] float hz = 440f;
    [SerializeField] float amplitude = 1f;

    double phase;
    double sampleRate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        double increment = hz * 2.0 * Math.PI / sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = (float)(amplitude * Math.Sin(phase));
            phase += increment;

            for (int c = 0; c < channels; c++)
            {
                data[i + c] = sample;
            }
        }
    }
}
