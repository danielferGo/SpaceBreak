using DefaultNamespace;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FindSpawnPositions alienSpawner;
    [SerializeField] private FindSpawnPositions portalSpawner;
    [SerializeField] private DestructableGlobalMeshManager destructableGlobalMesh;
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private float portalInterval = 15f;
    [SerializeField] private float destructibleInterval = 4f;
    private float _spawnTimer;
    private float _portalSpawnTimer;
    private float destructibleSpawnTimer;


    private void Update()
    {
        _spawnTimer += Time.deltaTime;
        _portalSpawnTimer += Time.deltaTime;
        destructibleSpawnTimer += Time.deltaTime;
        if (destructibleSpawnTimer >= destructibleInterval)
        {
            destructableGlobalMesh.DestroySegment();
            destructibleSpawnTimer = 0f;
        }

        if (_portalSpawnTimer >= portalInterval)
        {
            Debug.Log("Portal Spawned");
            portalSpawner.StartSpawn();
            _portalSpawnTimer = 0f;
        }

        if (_spawnTimer >= spawnInterval)
        {
            alienSpawner.StartSpawn();
            _spawnTimer = 0f;
        }
    }
}