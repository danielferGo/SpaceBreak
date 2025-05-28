using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DefaultNamespace
{
    public class DestructableGlobalMeshManager : MonoBehaviour
    {
        public DestructibleGlobalMeshSpawner destructibleGlobalMesh;
        public DestructibleMeshComponent destructibleMeshComponent;
        private List<GameObject> destructibleMeshes = new List<GameObject>();
        private int destructibleMeshesCount;

        void Start()
        {
            destructibleGlobalMesh.OnDestructibleMeshCreated.AddListener(SetupDescructibleMesh);
        }

        public void SetupDescructibleMesh(DestructibleMeshComponent component)
        {
            component.GetDestructibleMeshSegments(destructibleMeshes);
            destructibleMeshComponent = component;
        }

        public void DestroySegment()
        {
            if (destructibleMeshesCount >= destructibleMeshes.Count)
            {
                Debug.LogWarning("No more segments to destroy.");
                return;
            }
            destructibleMeshesCount++;
            var segment = destructibleMeshes[destructibleMeshesCount -1];
            if (destructibleMeshComponent.ReservedSegment == segment) return;
            destructibleMeshComponent.DestroySegment(segment);
        }
    }
}