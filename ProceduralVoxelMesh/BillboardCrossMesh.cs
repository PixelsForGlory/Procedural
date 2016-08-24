// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using System;
using UnityEngine;

namespace PixelsForGlory.ProceduralVoxelMesh
{
    /// <summary>
    /// Billboard cross mesh component
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class BillboardCrossMesh : MonoBehaviour
    {
        [SerializeField]
        public static string MaterialResourcePath = string.Empty;

        [SerializeField]
        private BillboardCrossMeshData _billboardData;

        public string UniqueId => _billboardData.UniqueId;
        public string Name => _billboardData.Name;
        public int MiddleCount => _billboardData.MiddleCount;
        public int BottomTextureIndex => _billboardData.BottomTextureIndex;
        public int MiddleTextureIndex => _billboardData.MiddleTextureIndex;
        public int TopTextureIndex => _billboardData.TopTextureIndex;

        private BillboardCrossGeneratorTask _generatorTask;

        public bool WaitingForUpdate
        {
            get;
            private set;
        }

        // References to asset components
        protected MeshFilter MeshFilter;
        protected Mesh Mesh;
        protected MeshCollider MeshCollider;
        protected MeshRenderer MeshRenderer;

        public void Start()
        {
            MeshFilter = GetComponent<MeshFilter>();
            if(Mesh == null)
            {
                // This is written this way to prevent leaking of meshes in the editor
                if(MeshFilter.sharedMesh == null)
                {
                    Mesh = new Mesh();
                    MeshFilter.sharedMesh = Mesh;
                }
                else
                {
                    Mesh = MeshFilter.sharedMesh;
                }
            }
            MeshRenderer = GetComponent<MeshRenderer>();
            MeshRenderer.sharedMaterial = Resources.Load<Material>(MaterialResourcePath + "BillboardCrossTransparentCutoutMaterial");
            WaitingForUpdate = false;
        }

        public void Update()
        {
            // If there is no task, there is nothing to update
            if(_generatorTask == null)
            {
                return;
            }

            // If the task is not completed, there is nothing to do yet
            if(!_generatorTask.Completed)
            {
                return;
            }

            if(Mesh == null)
            {
                return;
            }

            // Task is completed, recreate the mesh
            Mesh.Clear();
            Mesh.vertices = _generatorTask.Vertices;
            Mesh.normals = _generatorTask.Normals;
            Mesh.uv = _generatorTask.UV;
            Mesh.uv2 = _generatorTask.UV2;
            Mesh.triangles = _generatorTask.Triangles;
            Mesh.tangents = _generatorTask.Tangents;
            Mesh.Optimize();
            Mesh.RecalculateBounds();
            _generatorTask = null;

            WaitingForUpdate = false;
        }

        public void OnDestroy()
        {
            _generatorTask = null;
        }

        public void SetMeshData(BillboardCrossMeshData meshData)
        {
            _billboardData = new BillboardCrossMeshData(meshData);
            name = _billboardData.Name;
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            if(_billboardData == null)
            {
                return;
            }

            // Set voxels and queue this mesh up to be generated
            _generatorTask = new BillboardCrossGeneratorTask(_billboardData);
            VoxelMeshGeneratorThread.Generator.EnqueueTask(_generatorTask);
            WaitingForUpdate = true;
        }

    }
}
