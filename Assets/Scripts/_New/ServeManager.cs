using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Serve

{
    public float upForce;
    public float hitForce;
}

public class ServeManager : MonoBehaviour
{
    public Serve topSpin;
    public Serve flat;
}
