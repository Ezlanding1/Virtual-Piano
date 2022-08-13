using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] int MIDIIndex;
    public float Hz;
    public string KeyName;

    public bool play;
    float i;
    

    public AudioManager audioManager;
    MIDIReader reader;
    AudioFilterReadMethod oafr;

    float velocity = -1;
    float volume;

    int thisChannel = -1;
    float channelVolume = 100;
    [Range(0.0f, 10.0f)]
    public float Pan = 5;
    bool sustain;


    // Start is called before the first frame update

    void Start()
    {
        oafr = gameObject.AddComponent<AudioFilterReadMethod>();
        if (transform.childCount != 0) { transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.3f);
            transform.GetChild(0).position = new Vector3(transform.GetChild(0).position.x, transform.GetChild(0).position.y, transform.GetChild(0).position.z - 0.3f);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.3f);
        }
        audioManager = FindObjectOfType<AudioManager>();
        reader = FindObjectOfType<MIDIReader>();
        Hz = 440 * (Mathf.Pow(2, (((float)MIDIIndex - 69) / 12)));
        AudioManager.KeyDictionary.Add(MIDIIndex, GetComponent<Key>());
    }
    private void Update()
    { if (oafr.gain != 0) { oafr.enabled = true; } else { oafr.enabled = false; } 
        if (thisChannel != -1) 
        {
            Pan = reader.ChannelPan[thisChannel];
            channelVolume = reader.ChannelVolume[thisChannel];
            sustain = reader.ChannelSustain[thisChannel];
        }
        
    }
    private void FixedUpdate()
    {
        //if (r){ t += v; oafr.gain = Mathf.Lerp(volume, 0, t); } else if (t != 0) { t = 0; }
        //if (oafr.gain == 0 && r == true) { r = false; t = 0; }
        if (play)
        {
            if (i < audioManager.ADSR_Envelope.keys[1].time) //Attack Curve
            {
                oafr.gain = ((audioManager.ADSR_Envelope.Evaluate(i) / 8) * volume);
                i += 0.06f;
            }
            else if (i < audioManager.ADSR_Envelope.keys[2].time) //Decay Curve
            {
                oafr.gain = ((audioManager.ADSR_Envelope.Evaluate(i) / 8) * volume);
                i += 0.06f;
            }
            else if (i < audioManager.ADSR_Envelope.keys[3].time) //Sustain curve
            {
                oafr.gain = ((audioManager.ADSR_Envelope.Evaluate(i) / 8) * volume);
            }
            if (transform.childCount > 0)
            {
                var step = (0.2f * (velocity != -1 ? velocity : 1)) * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(-4.5f, 0, 0), step);
            }

        }
        if (!play)
        {
            if (!sustain)
            {
                if (i < audioManager.ADSR_Envelope.keys[4].time && i < 1 && oafr.gain != 0) //Release Curve
                {
                    oafr.gain = ((audioManager.ADSR_Envelope.Evaluate(i) / 8) * volume);
                    i += 0.06f;
                }
                else if (oafr.gain != 0)
                {
                    i = 0;
                    oafr.gain = 0;
                }
            }else
            {
                if (i < audioManager.ADSR_Envelope.keys[4].time && i < 1 && oafr.gain != 0) //Release Curve
                {
                    oafr.gain = ((audioManager.ADSR_Envelope.Evaluate(i) / 8) * volume);
                    i += 0.005f * audioManager.Damper_Amplitude;
                }
                else if (oafr.gain != 0)
                {
                    i = 0;
                    oafr.gain = 0;
                }
            }
            if (transform.childCount > 0 && transform.rotation.eulerAngles != new Vector3 (0,0,0))
            {
                var step = (0.2f * (velocity != -1 ? velocity : 1)) * Time.deltaTime;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 0), step);
            }


        }
    }
    public void Play(float _velocity, int channelIndex)
    {
        thisChannel = channelIndex;
        velocity = _velocity;
        volume = (((_velocity / 127) * (channelVolume / 100) ) * audioManager.MasterVolume);
        i = 0;
        play = true;
        
    }
    public void Stop(float velocity)
    {
        /*
         * T < 64 longer release times 
         * T > 64 shorter release times 
         * 1 = slowest release time 
         * 127 = fastest release time
         */
        //if (velocity == 0) { v = 1; } else { v = (velocity / 64); }
        //r = true;
        audioManager.gameObject.GetComponent<Effects>().StopEffect(GetComponent<Key>());
        play = false;
    }

    
}
