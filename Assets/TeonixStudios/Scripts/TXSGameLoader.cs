using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TXSGameLoader : MonoBehaviour
{
    void Awake()
    {
        LocalSDK.Init();
    }
}
