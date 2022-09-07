using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Diagnostics;
using System.IO;

public class AudioManager : MonoBehaviour
{
    [SerializeField] InputField input;
    [SerializeField] private string filePath;

    public static Dictionary<int, Key> KeyDictionary = new Dictionary<int, Key>();
    MonoBehaviour instance;
    [SerializeField] MIDIReader reader;

    [SerializeField] GameObject PedalL, PedalM, PedalR;
    [SerializeField] private float sustainPedalSpeed = 100f;

    [SerializeField] GameObject RCurtain, LCurtain;

    [SerializeField] GameObject[] UIObjects;

    [Header("Global Settings")]
    [SerializeField] public float MasterVolume = 10f;
    
    // Start is called before the first frame update
    void Start()
    {
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
        
        // Rotate Pedal
        var step = sustainPedalSpeed * Time.deltaTime;

        var pedalRotation = (!reader.SongDone && reader.ChannelSustain.Any(b => b)) ? 
            Quaternion.Euler(30f, 180, 0) : // Pressed angle
            Quaternion.Euler(0, 180, 0); // Resting angle

        PedalR.transform.rotation = Quaternion.RotateTowards(
            PedalR.transform.rotation, 
            pedalRotation,
            step
        );

        // Play song
        if (Input.GetKeyDown(KeyCode.S) && reader.SongDone && input.gameObject.activeSelf == false) 
            Play();

        // Set song filepath and hide UI
        if (Input.GetKeyDown(KeyCode.Return) && input.gameObject.activeSelf == true)
            ApplyUIChanges();

    }

    public void Play()
    {
        reader.StartRead(filePath, instance);
    }

    public void ApplyUIChanges()
    {
        filePath = input.text;
        input.gameObject.SetActive(false);
    }
}
