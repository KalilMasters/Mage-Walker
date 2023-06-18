using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasEnabler : MonoBehaviour
{
    static Dictionary<string, GameObject> canvasDic = new();
    [SerializeField] private GameObject[] objects;
    private void Awake()
    {
        print("Registering canvases");
        canvasDic.Clear();
        foreach (GameObject g in objects)
            canvasDic.Add(g.name, g);
    }
    public static void EnableCanvas(string canvasName, bool on)
    {
        GameObject go = null;

        foreach (string key in canvasDic.Keys)
            if (key.ToLower().Contains(canvasName.ToLower()))
            {
                go = canvasDic[key];
                break;
            }
        if (go == null)
        {
            print(canvasName + " is not registered");
            return;
        }
        //Debug.Log("Turning " + (on ? "On" : "Off") + " " + canvasName, go);
        go.gameObject.SetActive(on);
    }
}
