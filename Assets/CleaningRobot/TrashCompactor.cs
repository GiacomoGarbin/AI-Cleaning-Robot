using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCompactor : MonoBehaviour
{
    private List<Trash> TrashList = new List<Trash>();

    public int DisposedTrash = 0;
    public int DisposedLightTrash = 0;
    public int DisposedHeavyTrash = 0;

    void Update()
    {
        if (TrashList.Count > 0)
        {
            StartCoroutine(DestroyTrash(TrashList[0]));
            TrashList.RemoveAt(0);
        }
    }

    public void AddTrash(Trash trash)
    {
        TrashList.Add(trash);
        DisposedTrash++;

        switch (trash.weight)
        {
            case TrashWeight.Light:
                DisposedLightTrash++;
                break;
            case TrashWeight.Heavy:
                DisposedHeavyTrash++;
                break;
        }
    }

    IEnumerator DestroyTrash(Trash trash)
    {
        yield return new WaitForSeconds(1);
        trash.DestroyObject();
    }
}