using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effects : MonoBehaviour
{
    [SerializeField] Dropdown dropdown;
    [SerializeField] GameObject EffectHolder;
    [SerializeField] GameObject[] Effect;
    [SerializeField] Vector2[] Positions;
    enum EffectType
    {
        None,
        Lightning,
        PurpleParticles,
        Letters,
        GoldKeys,
        RGBKeys
    }
    [SerializeField] EffectType effectType;
    public void PlayEffect(Key key)
    {
        string keyName = key.gameObject.name;
        Vector3 keyPosition = key.gameObject.transform.position;

        switch (effectType)
        {
            case EffectType.None:
                break;
            case EffectType.Lightning:
                if (!key.KeyName.Contains("/"))
                {
                    Instantiate(Effect[(int)effectType], new Vector3(keyPosition.x, Positions[(int)effectType].x, Positions[(int)effectType].y), Quaternion.identity, EffectHolder.transform);
                }
                else
                {
                    Instantiate(Effect[(int)effectType], new Vector3(keyPosition.x, Positions[(int)effectType].x, Positions[(int)effectType].y + 0.3f), Quaternion.identity, EffectHolder.transform);
                }
                break;
            case EffectType.PurpleParticles:
                if (!key.KeyName.Contains("/"))
                {
                    Instantiate(Effect[(int)effectType], new Vector3(keyPosition.x, Positions[(int)effectType].x, Positions[(int)effectType].y), Quaternion.identity, EffectHolder.transform);
                }
                else
                {
                    Instantiate(Effect[(int)effectType], new Vector3(keyPosition.x, Positions[(int)effectType].x, Positions[(int)effectType].y + 0.25f), Quaternion.identity, EffectHolder.transform);
                }
                break;
            case EffectType.Letters:
                foreach (Transform letter in Effect[(int)effectType].transform)
                {
                    if (keyName[0].ToString() == letter.name)
                    {
                        GameObject ILetter = Instantiate(letter, new Vector3(keyPosition.x, Positions[(int)effectType].x, Positions[(int)effectType].y), Quaternion.Euler(-90, 180, 0), EffectHolder.transform).gameObject;
                        ILetter.AddComponent<Rigidbody>().useGravity = false; 
                        ILetter.GetComponent<Rigidbody>().linearVelocity = new Vector3(0,0,-1);
                        Destroy(ILetter, 5);
                    }
                }
                break;
            case EffectType.GoldKeys:
                key.GetComponentInChildren<MeshRenderer>().material = Effect[(int)effectType].transform.GetChild(2).GetComponent<MeshRenderer>().sharedMaterial;
                break;
            case EffectType.RGBKeys:
                key.GetComponentInChildren<MeshRenderer>().material = Effect[(int)effectType].transform.GetChild(2).GetComponent<MeshRenderer>().sharedMaterial;
                break;
            default:
                break;
        }
    }
    public void StopEffect(Key key)
    {
        switch (effectType)
        {
            case EffectType.GoldKeys:
                if (key.KeyName.Contains("/"))
                {
                    key.GetComponentInChildren<MeshRenderer>().material = Effect[(int)effectType].transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial;
                }
                else
                {
                    key.GetComponentInChildren<MeshRenderer>().material = Effect[(int)effectType].transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;
                }
                break;
            case EffectType.RGBKeys:
                if (key.KeyName.Contains("/"))
                {
                    key.GetComponentInChildren<MeshRenderer>().material = Effect[(int)effectType].transform.GetChild(1).GetComponent<MeshRenderer>().sharedMaterial;
                }
                else
                {
                    key.GetComponentInChildren<MeshRenderer>().material = Effect[(int)effectType].transform.GetChild(0).GetComponent<MeshRenderer>().sharedMaterial;
                }
                break;
            default:
                break;
        }
    }
    public void EffectChange()
    {
        effectType = (EffectType)(dropdown.value);
    }
}
