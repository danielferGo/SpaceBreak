using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class DestructibleGlobalMeshManager : MonoBehaviour
{
    public DestructibleGlobalMeshSpawner destructibleGlobalMesh;
    private DestructibleMeshComponent _destructibleMeshComponent;
    private readonly List<GameObject> _destructibleMeshes = new();
    private int _destructibleMeshesCount;

    private void Start()
    {
        destructibleGlobalMesh.OnDestructibleMeshCreated.AddListener(SetupDestructibleMesh);
    }

    public void SetupDestructibleMesh(DestructibleMeshComponent component)
    {
        component.GetDestructibleMeshSegments(_destructibleMeshes);
        _destructibleMeshComponent = component;
    }

    public void DestroySegment()
    {
        if (_destructibleMeshesCount >= _destructibleMeshes.Count)
        {
            Debug.LogWarning("No more segments to destroy.");
            return;
        }

        _destructibleMeshesCount++;
        var segment = _destructibleMeshes[_destructibleMeshesCount - 1];
        if (_destructibleMeshComponent.ReservedSegment == segment) return;
        _destructibleMeshComponent.DestroySegment(segment);
    }
}