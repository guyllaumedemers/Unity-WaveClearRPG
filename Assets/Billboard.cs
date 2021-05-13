using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    public Camera _main;
    public void Awake()
    {
        _main = Camera.main;
    }
    public void LateUpdate()
    {
        transform.forward = _main.transform.forward;
    }
}
