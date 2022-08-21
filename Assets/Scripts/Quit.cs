using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    [SerializeField] GameObject CurtainR, CurtainL;
    public void OnQuit()
    {
        StartCoroutine(QuitAnim());
    }
    IEnumerator QuitAnim()
    {
        GameObject IR = Instantiate(CurtainR);
        IR.GetComponent<Cloth>().externalAcceleration = new Vector3(-1, 0, 0);
        GameObject IL = Instantiate(CurtainL);
        IL.GetComponent<Cloth>().externalAcceleration = new Vector3(1, 0, 0);
        yield return new WaitForSeconds(3.5f);
        Application.Quit();
    }
}
