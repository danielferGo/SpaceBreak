using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Portal : MonoBehaviour
{
    [SerializeField] private float teleportOffset = 0.5f;
    [SerializeField] private float destroyTime = 5f;
    private bool _isActivated = true;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isActivated) return;
        if (!other.TryGetComponent<SpaceShip>(out var spaceShip)) return;
        var portals = GameObject.FindGameObjectsWithTag("Portal");
        if (portals.Length == 0) return;
        while (true)
        {
            var randomIndex = Random.Range(0, portals.Length);
            var portal = portals[randomIndex];
            if (portal.Equals(gameObject)) continue;
            portal.GetComponent<Portal>()._isActivated = false;
            _isActivated = false;
            spaceShip.Teleport(portal.transform, teleportOffset);
            break;
        }
    }
}