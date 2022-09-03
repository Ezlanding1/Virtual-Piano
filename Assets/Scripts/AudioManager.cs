using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] InputField input;
    [SerializeField] private string FilePath;
    public static Dictionary<int, Key> KeyDictionary = new Dictionary<int, Key>();
    public static bool started;
    MonoBehaviour instance;
    [SerializeField] MIDIReader reader;

    [SerializeField] GameObject PedalL, PedalM, PedalR;
    int pedalSustains;

    [SerializeField] GameObject RCurtain, LCurtain;

    [SerializeField] GameObject[] UIObjects;
    bool hidden;

    [Header("Global Settings")]
    [SerializeField] public float MasterVolume = 10f;

    [Header("Envelope")]
    [SerializeField] public float attack = 0.005f;
    [SerializeField] public float decay = 0.15f;
    [SerializeField] public float sustain = 0.3f;
    [SerializeField] public float release = 1.8f;

    [Header("Piano Modeling")]
    [SerializeField] public float sampleRate = 48000f;
    [SerializeField] public int partialCount = 105;
    [SerializeField] public float inharmonicity = 0.0008f; // piano string stiffness
    [SerializeField] public float brightness = 0.7f;
    
    // Start is called before the first frame update
    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
        input.gameObject.SetActive(false);
        Destroy(RCurtain, 5);
        Destroy(LCurtain, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            input.gameObject.SetActive(true);
        }
        foreach (bool b in reader.ChannelSustain)
        {
            
            if (b)
            {
                pedalSustains++;
            }
        }
        if (pedalSustains > 0)
        {
            var step = (0.2f * 500) * Time.deltaTime;
            PedalR.transform.rotation = Quaternion.RotateTowards(PedalR.transform.rotation, Quaternion.Euler(30f, 180, 0), step);
            pedalSustains = 0;
        }
        else
        {
            var step = (0.2f * 500) * Time.deltaTime;
            PedalR.transform.rotation = Quaternion.RotateTowards(PedalR.transform.rotation, Quaternion.Euler(0, 180, 0), step);
        }
        if (Input.GetKeyDown(KeyCode.S) && input.gameObject.activeSelf == false) { Debug.Log("Started"); Play(); }
        if (Input.GetKeyDown(KeyCode.Return) && input.gameObject.activeSelf == true) { applyChanges(); }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!hidden)
            {
                foreach (GameObject UI in UIObjects)
                {
                    UI.SetActive(false);
                }
                hidden = true;
            }
            else
            {
                foreach (GameObject UI in UIObjects)
                {
                    UI.SetActive(true);
                }
                hidden = false;
            }
        }
    }

    public void Play()
    {
        reader.Main(FilePath, instance);
    }

    public void applyChanges()
    {
        FilePath = input.text;
        input.gameObject.SetActive(false);
    }
}
//if (midiEvent.MidiEventType.ToString() != "ProgramChange") { Debug.Log(MIDIIndexToKey(midiEvent)); }
//Debug.Log(MIDIReader.midiFile.Tracks[0].MidiEvents[0].Time);