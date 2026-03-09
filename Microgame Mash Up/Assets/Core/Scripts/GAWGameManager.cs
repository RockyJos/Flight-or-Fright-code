using System;
using UnityEngine;

public abstract class GAWGameManager : MonoBehaviour
{
    public abstract void OnGameLoad();
    public abstract void OnGameStart();
    public abstract void OnGameSucceeded();
    public abstract void OnGameFailed();
}
