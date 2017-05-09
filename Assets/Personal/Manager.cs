using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Manager : MonoBehaviour
{
    abstract public void Pause();
    abstract public void Quit();
    abstract public void checkpoint(int checkNum, Vector2 position);
}
