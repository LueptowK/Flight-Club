using UnityEngine;
using System.Collections;

public abstract class Controller : MonoBehaviour {

    public abstract float MoveVer { get; }
    public abstract float MoveHor { get; }
    public abstract float LookVer { get; }
    public abstract float LookHor { get; }
}
