using Meta.XR.MRUtilityKit;
using UnityEngine;

/// <summary>
/// Provides the functionality of spawning ghosts
/// </summary>
public class GhostSpawner : MonoBehaviour
{
    public float spawnTimer = 1.0f; // Interval between spawning Ghosts
    public GameObject prefabToSpawn;

    public float minEdgeDistance =  0.3f;   // The distance from the edge or corner of the room
    public MRUKAnchor.SceneLabels spawnLabels;
    public float normalOffset; 
    public int spawnTry = 1000;

    private float _timer;   // Timeer to count past time

    // Update is called once per frame
    void Update()
    {
        if(!MRUK.Instance && !MRUK.Instance.IsInitialized)        
            return;

        _timer += Time.deltaTime;   // Add the past time from the last frame to the current frame
        if(_timer > spawnTimer)
        {
            SpawnGhost();
            _timer -= spawnTimer;    // Resetting the timer to 0 also works.
//            _timer = 0;    
        }
    }

    /// <summary>
    /// Spawn a Ghost
    /// </summary>
    public void SpawnGhost()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        int currentTry = 0;

        while(currentTry < spawnTry)
        {
            bool hasFoundPosition = room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.VERTICAL, minEdgeDistance, new LabelFilter(spawnLabels), out Vector3 pos, out Vector3 norm);  // SurfaceType.VERTICAL is basically wall // When surfaceType is VERTICAL, selecting label such as floor do nothing.
            if(hasFoundPosition)
            {
    //        Vector3 randomPosition = Random.insideUnitSphere * 3;   // return the random position within the sphera with radios 1.0 x 3.
    //        Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);    // It's needed to orient Ghost toward the player, isn't ?
                Vector3 randomPositionNormalOffset = pos + norm * normalOffset; // Normal direction is toward inside of the room
                randomPositionNormalOffset.y = 0;   // Set the hgith to the floor level.
                Instantiate(prefabToSpawn, randomPositionNormalOffset, Quaternion.identity);

                return;
            }
            else
            {
                currentTry++;
            }
        }
        Debug.Log($"[{this.name}] Failed to find Position to spawn ghosts");
    }
}
