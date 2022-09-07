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

    // Add effect to a specific played key
    public void PlayEffect(Key key)
    {
        string keyName = key.gameObject.name;

        switch (effectType)
        {
            case EffectType.None:
                break;
            case EffectType.Lightning:
                InstantiateEffect(key, 0.30f);
                break;
            case EffectType.PurpleParticles:
                InstantiateEffect(key, 0.25f);
                break;
            case EffectType.Letters:
                foreach (Transform letter in Effect[(int)effectType].transform)
                {
                    if (keyName[0].ToString() == letter.name)
                    {
                        var ILetter = Instantiate(
                            letter, 
                            EffectDefaultPosition(key), 
                            Quaternion.Euler(-90, 180, 0), 
                            EffectHolder.transform
                        ).gameObject;
                        ILetter.AddComponent<Rigidbody>().useGravity = false; 
                        ILetter.GetComponent<Rigidbody>().linearVelocity = new Vector3(0,0,-1);
                        Destroy(ILetter, 5);
                        break;
                    }
                }
                break;
            case EffectType.GoldKeys:
            case EffectType.RGBKeys:
                var newMaterial = Effect[(int)effectType].transform.GetChild(2).GetComponent<MeshRenderer>().sharedMaterial;
                key.GetComponentInChildren<MeshRenderer>().material = newMaterial;
                key.materialChanged = true;
                break;
            default:
                break;
        }
    }

    private GameObject InstantiateEffect(Key key, float blackKeyZPosOffset)
    {
        var effectLocation = EffectDefaultPosition(key);
        effectLocation.z += key.IsBlackKey() ? blackKeyZPosOffset : 0;

        return Instantiate(
            Effect[(int)effectType], 
            effectLocation, 
            Quaternion.identity,
            EffectHolder.transform
        );
    }

    private Vector3 EffectDefaultPosition(Key key) =>
        new(key.gameObject.transform.position.x, Positions[(int)effectType].x, Positions[(int)effectType].y);
    

    // Remove effect from key
    public void StopEffect(Key key)
    {
        if (key.materialChanged)
        {
            // Reset material. The RGBKey effect has the default materials as its children
            var childNum = key.IsBlackKey() ? 1 : 0;
            var newMaterial = Effect[(int)EffectType.RGBKeys].transform.GetChild(childNum).GetComponent<MeshRenderer>().sharedMaterial;
            key.GetComponentInChildren<MeshRenderer>().material = newMaterial;
            key.materialChanged = false;
        }
    }

    public void EffectChange()
    {
        effectType = (EffectType)dropdown.value;
    }
}
