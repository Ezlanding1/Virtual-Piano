using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MidiParser;
public class MIDIReader : MonoBehaviour {

    public static MidiFile midiFile;
    [SerializeField] Effects effects;

    [SerializeField] string FilePath;
    [SerializeField] int Format;
    [SerializeField] int TicksPerQuarterNote;
    [SerializeField] int TracksCount;

    public double lastTime;
    public double waitDelta;
    
    [SerializeField] GameObject PianoKeys;

    [Header("Song Details")]

    [SerializeField] float BeatsPerMinute = 60;
    [SerializeField] float TimeSignatureA = 4;
    [SerializeField] float TimeSignatureB = 4;
    public double wait;

    [Header("Channel Details")]

    public float[] ChannelVolume;
    public float[] ChannelPan;
    public bool[] ChannelSustain;

    public void Main(string path, MonoBehaviour instance)
    {
        string Path = path;
        Debug.Log(string.Format("Parsing: {0}\n", Path));
        midiFile = new MidiFile(Path);
        Debug.Log(string.Format("Format: {0}", midiFile.Format));
        Debug.Log(string.Format("TicksPerQuarterNote: {0}", midiFile.TicksPerQuarterNote));
        Debug.Log(string.Format("TracksCount: {0}", midiFile.TracksCount));
        #region Setting Variables
        FilePath = Path;
        Format = midiFile.Format;
        TicksPerQuarterNote = midiFile.TicksPerQuarterNote;
        TracksCount = midiFile.TracksCount;

        ChannelVolume = new float[midiFile.TracksCount];
        for (int i = 0; i < ChannelVolume.Length; i++) { ChannelVolume[i] = 100; }
        ChannelPan = new float[midiFile.TracksCount];
        for (int i = 0; i < ChannelPan.Length; i++){ ChannelPan[i] = 5; }
        ChannelSustain = new bool[midiFile.TracksCount];
        #endregion

        foreach (var track in midiFile.Tracks)
        {
            Debug.Log(string.Format("\nTrack: {0}\n", track.Index));

            StartCoroutine(ParseNotes(track));
        }
        //StartCoroutine(ParseNotes(midiFile.Tracks[3]));
        //StartCoroutine(ParseNotes(midiFile.Tracks[6]));
    }

    IEnumerator ParseNotes(MidiTrack track)
    {
        foreach (var midiEvent in track.MidiEvents)
        {
            if (!AudioManager.started) { AudioManager.started = true; }
            waitDelta = midiEvent.Time;
            wait = (((double)(waitDelta - lastTime) / 1000d) * BeatsPerMinuteToSpeed(BeatsPerMinute));
            yield return new WaitForSecondsRealtime((float)wait);
            lastTime = midiEvent.Time;

            const string Format = "{0} Channel {1} Time {2} Args {3} {4}";
            if (midiEvent.MidiEventType == MidiEventType.MetaEvent)
            {
                Debug.Log(
                    string.Format(Format,
                    midiEvent.MetaEventType,
                    "-",
                    midiEvent.Time,
                    midiEvent.Arg2,
                    midiEvent.Arg3));

                switch (midiEvent.MetaEventType)
                {
                    case MetaEventType.Tempo:
                        BeatsPerMinute = midiEvent.Arg2;
                        break;
                    case MetaEventType.TimeSignature:
                        TimeSignatureA = midiEvent.Arg2;
                        TimeSignatureB = midiEvent.Arg3;
                        break;
                    case MetaEventType.KeySignature:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Debug.Log(
                //                 string.Format(Format,
                //                 midiEvent.MidiEventType,
                //                 midiEvent.Channel,
                //                 midiEvent.Time,
                //                 midiEvent.Arg2,
                //                 midiEvent.Arg3));

                switch (midiEvent.MidiEventType)
                {
                    case MidiEventType.NoteOn:
                        {
                            var key = AudioManager.KeyDictionary[midiEvent.Arg2];
                            key.Play(midiEvent.Arg3, track.Index);
                            effects.PlayEffect(key);
                        }
                        break;
                    case MidiEventType.NoteOff:
                        {
                            var key = AudioManager.KeyDictionary[midiEvent.Arg2];
                            key.Stop();
                        }
                        break;
                    case MidiEventType.ControlChange:
                        ControlChangeEvents(midiEvent, track.Index);
                        break;
                }


            }
        }
    }

    double BeatsPerMinuteToSpeed(float BPM)
    {
        //return(60,000 / BeatsPerMinute) * Beats in a mesaure
        return (double)((60000d / (double)(BPM * TicksPerQuarterNoteToSpeed(TicksPerQuarterNote))) / 1000d);
    }
    double TicksPerQuarterNoteToSpeed(float PPQ)
    {
        return (double)(PPQ / 960d) ;
    }

    void ControlChangeEvents(MidiEvent midiEvent, int trackIndex)
    {
        switch (midiEvent.Arg2)
        {
            case 0:
                break;
            case 1:
                //Modulation Wheel or Lever
                break;
            case 7:
                //Channel Volume
                ChannelVolume[trackIndex] = midiEvent.Arg3;
                break;
            case 10:
                //Stereo Pan
                ChannelPan[trackIndex] = (midiEvent.Arg3 / 12.8f);
                break;
            case 64:
                if (midiEvent.Arg3 <= 63) ChannelSustain[trackIndex] = false;
                if (midiEvent.Arg3 >= 64) ChannelSustain[trackIndex] = true;
                break;
            default:
                break;
        }
    }
}