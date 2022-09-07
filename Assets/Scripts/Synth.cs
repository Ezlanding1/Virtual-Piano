using UnityEngine;

public class Synth : MonoBehaviour
{
    [Header("Note")]
    public float hz = 440f;
    [Range(0,1)]
    public float velocity = 0.8f;
    [Range(0,1)]
    public float pan = 0.5f;

    [Header("ADSR Envelope")]
    public float attack = 0.01f;
    public float decay = 0.2f;
    public float sustain = 0.7f;
    public float release = 0.5f;

    [Header("Piano Modeling")]
    public int partialCount = 35;
    public float inharmonicity = 0.0005f;
    [Range(0,1)]
    public float brightness = 0.5f;

    [Header("Engine")]
    public float sampleRate = 48000f;
    public bool sustainPedal = false;

    float env;
    float envTime;
    bool gate;

    float[] partialPhases;
    float[] partialAmps;

    void Awake()
    {
        partialPhases = new float[partialCount];
        partialAmps = new float[partialCount];

        // Calculate amplitude of the pth harmonic based on the formula: 1/(p+1) (1-p/N)^b
        // Where p is the harmonic number, N is the total amount of harmonics, and b is the brightness of the piano
        // This formula gives us basic harmonic decay, as well as the brightness, which controls the sharpness of the decay ("roundedness" of sound)
        for (int p = 0; p < partialCount; p++)
        {
            partialAmps[p] = (1f / (p + 1)) * Mathf.Pow(1f - (p / (float)partialCount), brightness);
        }
    }

    public void Play()
    {
        gate = true;
        envTime = 0f;
    }

    public void Stop()
    {
        gate = false;
        envTime = 0f;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        float nyquist = sampleRate * 0.5f;

        for (int i = 0; i < data.Length; i += channels)
        {
            // ADSR Envelope
            if (gate)
            {
                // As envTime increases, it will go through the stages of the ADSR envelope
                if (envTime < attack)
                    env = envTime / attack;
                else if (envTime < attack + decay)
                    env = Mathf.Lerp(1f, sustain, (envTime - attack) / decay);
                else
                    env = sustain;
            }
            else
            {
                // Bring sound back down to 0 if not playing
                float relTime = envTime / release;
                env = Mathf.Lerp(env, 0f, relTime);
            }

            envTime += 1f / sampleRate;
            env = Mathf.Clamp01(env);

            float sample = 0f;

            // Harmonics
            for (int p = 0; p < partialCount; p++)
            {
                float harmonic = p + 1;
                // The basic formula for calculating a harmonic is (p+1)*f0 (where f0 is the fundamental freq, or the variable "hz" here)
                // For more realistic sound, an inharmonicity is applied giving us (p+1)*f0 * sqrt(1 + B *(p+1)^2)
                float freq = hz * harmonic * Mathf.Sqrt(1f + inharmonicity * harmonic * harmonic);

                // Use nyquist fequency to prevent problems from aliasing
                if (freq > nyquist)
                    continue;

                float partialEnv = partialAmps[p] * env * Mathf.Exp(-harmonic * 2f * envTime * 0.5f);

                // Add harmonics, apply frequency on sine wave
                partialPhases[p] += (freq * 2f * Mathf.PI) / sampleRate;
                sample += Mathf.Sin(partialPhases[p]) * partialEnv;
            }

            // Scale by volume
            sample *= velocity;

            // Panning
            // Copy data to channels, apply panning amount
            if (channels == 2)
            {
                data[i]     += sample * (1f - pan);
                data[i + 1] += sample * pan;
            }
            else
            {
                data[i] += sample;
            }

            if (!gate && env <= 0.0001f)
                break;
        }
    }
}
