using UnityEngine;


public class Spawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnData 
    {
        public GameObject Obj;
        public bool OnlySpawnOnceTheEntireGame = true;
    }
    private static bool HasBeenSpawned = false;
    public SpawnData ObjectToSpawn;
    private void Awake()
    {
        if (ObjectToSpawn == null)
            return;
        if (ObjectToSpawn.OnlySpawnOnceTheEntireGame)
            if (!HasBeenSpawned) 
            {
                Instantiate(ObjectToSpawn.Obj, Vector3.zero, Quaternion.identity);
                HasBeenSpawned =true;
            }
    }

    private void OnDisable()
    {
        HasBeenSpawned=false;
    }

}
