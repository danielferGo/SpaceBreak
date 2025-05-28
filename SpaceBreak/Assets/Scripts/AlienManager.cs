using System.Collections;
using UnityEngine;

public class AlienManager : MonoBehaviour
{
    [SerializeField] private GameObject alienPrefab;
    [SerializeField] private int alienCount = 10;
    [SerializeField] private Transform spaceShip;
    [SerializeField] private float spawnRadius = 1f;
    [SerializeField] private GameObject visualEffect;

    void Start()
    {
        StartCoroutine(SpawnAliens());
    }

    private IEnumerator SpawnAliens()
    {
        yield return new WaitForSeconds(1f);
        spaceShip = GameObject.FindWithTag("SpaceShip").transform;
        for (int i = 0; i < alienCount; i++)
        {
            Vector3 spawnPos = transform.position + Random.onUnitSphere * spawnRadius;
            spawnPos.y = Mathf.Max(spawnPos.y, 0f); // Keep above ground

            GameObject alienObj = Instantiate(alienPrefab, spawnPos, Quaternion.identity);
            Alien alien = alienObj.GetComponent<Alien>();
            alien.Init(spaceShip);
        }

        yield return new WaitForSeconds(3f);
        visualEffect.SetActive(false);
    }
}