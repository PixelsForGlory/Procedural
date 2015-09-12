﻿// Copyright 2015 afuzzyllama. All Rights Reserved.
using UnityEngine;

namespace ProceduralVoxelMesh
{
    /// <summary>
    /// Voxel mesh component
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [ExecuteInEditMode]
    public partial class VoxelMesh : MonoBehaviour
    {
        // Reference to current voxel configuration
        public Voxel[,,] Voxels { get; private set; }

        // Trigger update on mesh in editor
        public bool UpdateMesh { get; private set; }

        // If there is a mesh to be generated, this is a reference to that task
        private VoxelMeshGeneratorTask _generatorTask;

        // References to asset components
        private MeshFilter _meshFilter;
        private Mesh _mesh;
        private MeshCollider _meshCollider;
        private MeshRenderer _meshRenderer;
        
        public void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
            if(_mesh == null)
            {
                // This is written this way to prevent leaking of meshes in the editor
                if(_meshFilter.sharedMesh == null)
                {
                    _mesh = new Mesh();
                    _meshFilter.sharedMesh = _mesh;
                }
                else
                {
                    _mesh = _meshFilter.sharedMesh;
                }
            }
            _meshCollider = GetComponent<MeshCollider>();

            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.sharedMaterial = Resources.Load<Material>("VoxelMaterial");

            UpdateMesh = false;
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
            
            // Task is completed, recreate the mesh
            _mesh.Clear();
            _mesh.vertices = _generatorTask.Vertices.ToArray();
            _mesh.normals = _generatorTask.Normals.ToArray();
            _mesh.uv = _generatorTask.UV0.ToArray();
            //_mesh.uv2 = _generatorTask.UV1.ToArray(); // Not implemented for now
            _mesh.colors = _generatorTask.Colors.ToArray();
            _mesh.triangles = _generatorTask.Triangles.ToArray();

            TangentSolver(_mesh);

            _mesh.Optimize();
            _mesh.RecalculateBounds();
            _meshCollider.sharedMesh = _mesh;
            _generatorTask = null;

            UpdateMesh = false;
        }

        public void OnDestroy()
        {
            _generatorTask = null;
        }

        /// <summary>
        /// Set voxels and queue this mesh up to be generated
        /// </summary>
        /// <param name="newVoxels"></param>
        public void SetVoxels(Voxel[,,] newVoxels)
        {
            _generatorTask = new VoxelMeshGeneratorTask(newVoxels, newVoxels.GetLength(0), newVoxels.GetLength(1), newVoxels.GetLength(2));
            VoxelMeshGeneratorThread.Generator.EnqueueTask(_generatorTask);

            // Copy the new voxels into the class
            Voxels = new Voxel[newVoxels.GetLength(0), newVoxels.GetLength(1), newVoxels.GetLength(2)];
            for(int w = 0; w < newVoxels.GetLength(0); ++w)
            {
                for(int h = 0; h < newVoxels.GetLength(1); ++h)
                {
                    for(int d = 0; d < newVoxels.GetLength(2); ++d)
                    {
                        Voxels[w, h, d] = newVoxels[w, h, d];
                    }
                }
            }

            UpdateMesh = true;
        }

    }
}
