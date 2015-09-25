// Copyright 2015 afuzzyllama. All Rights Reserved.

using System;
using System.Collections.Generic;
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
        /// <summary>
        /// UniqueId based on System.Guid.  Creates a persistant unique identifier.
        /// </summary>
        public string UniqueId;

        /// <summary>
        /// 3-dimensional voxel volume represented in 1-dimensional list.  Lists play nice with serialization and allows for persisting data.
        /// </summary>
        [SerializeField]
        private List<Voxel> _voxels;

        /// <summary>
        /// Width length of voxel list
        /// </summary>
        [SerializeField]
        private int _wLength;

        /// <summary>
        /// Height length of voxel list
        /// </summary>
        [SerializeField]
        private int _hLength;

        /// <summary>
        /// Depth length of voxel list
        /// </summary>
        [SerializeField]
        private int _dLength;

        /// <summary>
        /// Get voxel data from a single point
        /// </summary>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="d">depth</param>
        /// <returns></returns>
        public Voxel GetVoxel(int w, int h, int d)
        {
            return _voxels[Utilities.GetIndex(w, h, d, _wLength, _hLength, _dLength)];
        }
        
        /// <summary>
        /// Sets voxel volume from 3-dimensional array
        /// </summary>
        /// <param name="voxels"></param>
        public void SetVoxels(Voxel[,,] voxels)
        {
            // Copy the new voxels into the class
            _voxels = new List<Voxel>();
            _wLength = voxels.GetLength(0);
            _hLength = voxels.GetLength(1);
            _dLength = voxels.GetLength(2);

            // Resize list
            for(int w = 0; w < _wLength; ++w)
            {
                for(int h = 0; h < _hLength; ++h)
                {
                    for(int d = 0; d < _dLength; ++d)
                    {
                        _voxels.Add(new Voxel());
                    }
                }
            }

            // Copy voxels
            for(int w = 0; w < _wLength; ++w)
            {
                for(int h = 0; h < _hLength; ++h)
                {
                    for(int d = 0; d < _dLength; ++d)
                    {
                        _voxels[Utilities.GetIndex(w, h, d, _wLength, _hLength, _dLength)] = voxels[w, h, d];
                    }
                }
            }

            UpdateMesh();
        }

        // Trigger update on mesh in editor
        public void UpdateMesh()
        {
            // Set voxels and queue this mesh up to be generated
            _generatorTask = new VoxelMeshGeneratorTask(_voxels, _wLength, _hLength, _dLength);
            VoxelMeshGeneratorThread.Generator.EnqueueTask(_generatorTask);
        }

        // If there is a mesh to be generated, this is a reference to that task
        private VoxelMeshGeneratorTask _generatorTask;

        // References to asset components
        private MeshFilter _meshFilter;
        private Mesh _mesh;
        private MeshCollider _meshCollider;
        private MeshRenderer _meshRenderer;
        
        public void Start()
        {
            if(UniqueId == null)
            {
                UniqueId = Guid.NewGuid().ToString();
            }

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
            _mesh.triangles = _generatorTask.Triangles.ToArray();

            TangentSolver(_mesh);

            _mesh.Optimize();
            _mesh.RecalculateBounds();
            _meshCollider.sharedMesh = _mesh;
            _generatorTask = null;
        }

        public void OnDestroy()
        {
            _generatorTask = null;
        }
    }
}
