using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synth : MonoBehaviour
{
    public float gain;

    double increment;
    double phase;
    double samplingFrequency;

    Key key;

    float[] harmonicStrengths;
    private void Start()
    {
        samplingFrequency = AudioSettings.outputSampleRate;
        key = GetComponent<Key>();
        if (key.Hz <= 523.2511)
        {
            harmonicStrengths = key.audioManager.HarmonicStrengths;
        }
        else
        {
            harmonicStrengths = key.audioManager.HarmonicStrengths2;
        }
        
    }
    
    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!key || harmonicStrengths == null) return;

        increment = key.Hz * 2.0 * Mathf.PI / samplingFrequency;

        float leftPan  = (10f - key.Pan) / 5f;
        float rightPan = key.Pan / 5f;

        int harmonicCount = harmonicStrengths.Length;

        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;

            float sampleLeft = 0f;
            float sampleRight = 0f;

            for (int h = 0; h < harmonicCount; h++)
            {
                float harmonic = Mathf.Sin((h + 1) * (float)phase);
                float amp = gain * harmonicStrengths[h];

                sampleLeft += amp * leftPan * harmonic;

                if (channels == 2)
                    sampleRight += amp * rightPan * harmonic;
            }

            data[i] = sampleLeft;

            if (channels == 2)
                data[i + 1] = sampleRight;

            if (phase >= Mathf.PI * 2f)
                phase -= Mathf.PI * 2f;
        }
    }

}
