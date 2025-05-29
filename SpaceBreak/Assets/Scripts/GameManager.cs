using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FindSpawnPositions alienSpawner;
    [SerializeField] private FindSpawnPositions portalSpawner;
    [SerializeField] private DestructibleGlobalMeshManager destructibleGlobalMesh;
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private float portalInterval = 15f;
    [SerializeField] private float destructibleInterval = 4f;
    private float _spawnTimer;
    private float _portalSpawnTimer;
    private float _destructibleSpawnTimer;

    private void Update()
    {
        _spawnTimer += Time.deltaTime;
        _portalSpawnTimer += Time.deltaTime;
        _destructibleSpawnTimer += Time.deltaTime;
        if (_destructibleSpawnTimer >= destructibleInterval)
        {
            destructibleGlobalMesh.DestroySegment();
            _destructibleSpawnTimer = 0f;
        }

        if (_portalSpawnTimer >= portalInterval)
        {
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