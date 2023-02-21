using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureToJson : MonoBehaviour
{
    public Transform StructureParent;
    public void ExportToFile()
    {
        string fileName = StructureParent.gameObject.name;
    }
}
