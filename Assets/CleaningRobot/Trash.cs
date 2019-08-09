public enum TrashWeight
{
    Light, Heavy
}

public class Trash : PickableUpObject
{
    public TrashWeight weight;

    void Start()
    {
        PickedUp = false;
        disposed = false;
    }

    public override void DestroyObject()
    {
        Destroy(gameObject);
    }
}