using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFilterReadMethod : MonoBehaviour
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
        if (key)
        {
            increment = key.Hz * 2.0 * Mathf.PI / samplingFrequency;
            for (int i = 0; i < data.Length; i += channels)
            {
                phase += increment;

                float y1 = (float)(Mathf.Sin(1 * (float)phase));
                float y2 = (float)(Mathf.Sin(2 * (float)phase));
                float y3 = (float)(Mathf.Sin(3 * (float)phase));
                float y4 = (float)(Mathf.Sin(4 * (float)phase));
                float y5 = (float)(Mathf.Sin(5 * (float)phase));
                float y6 = (float)(Mathf.Sin(6 * (float)phase));

                data[i] = ((gain * harmonicStrengths[0]) * ((10 - key.Pan) / 5)) * y1;
                data[i] += ((gain * harmonicStrengths[1]) * ((10 - key.Pan) / 5)) * y2;
                data[i] += ((gain * harmonicStrengths[2]) * ((10 - key.Pan) / 5)) * y3;
                data[i] += ((gain * harmonicStrengths[3]) * ((10 - key.Pan) / 5)) * y4;
                data[i] += ((gain * harmonicStrengths[4]) * ((10 - key.Pan) / 5)) * y5;
                data[i] += ((gain * harmonicStrengths[5]) * ((10 - key.Pan) / 5)) * y6;

                if (channels == 2)
                {
                    data[i + 1] = ((gain * harmonicStrengths[0]) * ((key.Pan) / 5)) * y1;
                    data[i + 1] += ((gain * harmonicStrengths[1]) * ((key.Pan) / 5)) * y2;
                    data[i + 1] += ((gain * harmonicStrengths[2]) * ((key.Pan) / 5)) * y3;
                    data[i + 1] += ((gain * harmonicStrengths[3]) * ((key.Pan) / 5)) * y4;
                    data[i + 1] += ((gain * harmonicStrengths[4]) * ((key.Pan) / 5)) * y5;
                    data[i + 1] += ((gain * harmonicStrengths[5]) * ((key.Pan) / 5)) * y6;
                }
                if (phase >= (Mathf.PI * 2))
                {
                    phase -= (Mathf.PI * 2);
                }
            }
        }
    }
}
