using UnityEngine;

public abstract class PickableUpObject : MonoBehaviour
{
    public bool PickedUp;
    public bool disposed;

    public abstract void DestroyObject();
}