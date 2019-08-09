using System.Collections;
using UnityEngine;

public class TrashController : MonoBehaviour
{
    public GameObject LightTrashPrefab;
    public GameObject HeavyTrashPrefab;

    Trash[] TrashList;

    public Trash HeavyTrash
    {
        get { return TrashList[0]; }
        private set { TrashList[0] = value; }
    }

    public Trash LightTrash1
    {
        get { return TrashList[1]; }
        private set { TrashList[1] = value; }
    }

    public Trash LightTrash2
    {
        get { return TrashList[2]; }
        private set { TrashList[2] = value; }
    }

    void Start()
    {
        TrashList = new Trash[]
        {
            // SpawnTrash(TrashWeight.Light), 
            SpawnTrash(TrashWeight.Heavy),
            SpawnTrash(TrashWeight.Light),
            SpawnTrash(TrashWeight.Light)
        };

        for (int i = 0; i < TrashList.Length; i++)
        {
            TrashList[i].gameObject.layer = LayerMask.NameToLayer("Trash" + i);
        }
    }

    Trash SpawnTrash(TrashWeight weight)
    {
        int n = 4;
        float margin = 5;
        Vector3 position = Vector3.zero;

        // ignore elements of the room (floor and walls)
        int LayerMask = ~(1 << UnityEngine.LayerMask.NameToLayer("Room"));

        int tries = 100;

        // while (true)
        while (tries-- > 0)
        {
            int x = Random.Range(-n, n + 1);
            int z = Random.Range(-n, n + 1);

            if (Mathf.Abs(x) + Mathf.Abs(z) == 2 * n)
            {
                // if too close to the corners, then choose another position
                continue;
            }

            position = new Vector3(x * margin, 1, z * margin);

            // check if we hit something (robot or trash) in this position
            Collider[] HitColliders = Physics.OverlapSphere(position, margin, LayerMask);

            if (HitColliders.Length == 0)
            {
                break;
            }
        }

        if (tries == 0)
        {
            Debug.Log(name + ": not found a valid location where to spawn the trash");
            return null;
        }

        Trash trash;

        switch (weight)
        {
            case TrashWeight.Light:
                trash = (Instantiate(LightTrashPrefab) as GameObject).GetComponent<Trash>();
                break;
            case TrashWeight.Heavy:
                trash = (Instantiate(HeavyTrashPrefab) as GameObject).GetComponent<Trash>();
                break;
            default:
                return null;
        }

        trash.transform.position = position;

        return trash;
    }

    Coroutine[] coroutines = new Coroutine[3];

    void Update()
    {
        if (HeavyTrash == null && coroutines[0] == null)
        {
            coroutines[0] = StartCoroutine(SpawnHeavyTrash());
        }

        if (LightTrash1 == null && coroutines[1] == null)
        {
            coroutines[1] = StartCoroutine(SpawnLightTrash1());
        }

        if (LightTrash2 == null && coroutines[2] == null)
        {
            coroutines[2] = StartCoroutine(SpawnLightTrash2());
        }
    }

    float SpawnDelay = 1;

    IEnumerator SpawnHeavyTrash()
    {
        yield return new WaitForSeconds(SpawnDelay);
        // HeavyTrash = SpawnTrash(TrashWeight.Light);
        HeavyTrash = SpawnTrash(TrashWeight.Heavy);
        HeavyTrash.gameObject.layer = LayerMask.NameToLayer("Trash0");
        coroutines[0] = null;
    }

    IEnumerator SpawnLightTrash1()
    {
        yield return new WaitForSeconds(SpawnDelay);
        LightTrash1 = SpawnTrash(TrashWeight.Light);
        LightTrash1.gameObject.layer = LayerMask.NameToLayer("Trash1");
        coroutines[1] = null;
    }

    IEnumerator SpawnLightTrash2()
    {
        yield return new WaitForSeconds(SpawnDelay);
        LightTrash2 = SpawnTrash(TrashWeight.Light);
        LightTrash2.gameObject.layer = LayerMask.NameToLayer("Trash2");
        coroutines[2] = null;
    }
}