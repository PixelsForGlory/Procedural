// Copyright 2015 afuzzyllama. All Rights Reserved.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralVoxelMesh
{
    /// <summary>
    /// Color voxel mesh implementation.  Plays nice with Unity3D.
    /// </summary>
    [Serializable]
    public class ColorVoxelMesh : VoxelMesh<ColorVoxel>
    {
        public override void Start()
        {
            base.Start();
            MeshRenderer.sharedMaterial = Resources.Load<Material>("ColorVoxelMaterial");
        }
    }

    /// <summary>
    /// Texture voxel mesh implementation.  Plays nice with Unity3D.
    /// </summary>
    [Serializable]
    public class TextureVoxelMesh : VoxelMesh<TextureVoxel>
    {
        public override void Start()
        {
            base.Start();
            MeshRenderer.sharedMaterial = Resources.Load<Material>("TextureVoxelMaterial");
        }
    }

    /// <summary>
    /// Voxel mesh component
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [ExecuteInEditMode]
    [Serializable]
    public partial class VoxelMesh<T> : MonoBehaviour where T : IVoxel, new()
    {
        /// <summary>
        /// UniqueId based on System.Guid.  Creates a persistant unique identifier.
        /// </summary>
        public string UniqueId;

        /// <summary>
        /// 3-dimensional voxel volume represented in 1-dimensional list.  Lists play nice with serialization and allows for persisting data.
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private List<T> _voxels;

        /// <summary>
        /// Width length of voxel list
        /// </summary>
        [SerializeField]
        private int _wLength;

        public int Width => _wLength;

        /// <summary>
        /// Height length of voxel list
        /// </summary>
        [SerializeField]
        private int _hLength;

        public int Height => _hLength;

        /// <summary>
        /// Depth length of voxel list
        /// </summary>
        [SerializeField]
        private int _dLength;

        public int Depth => _dLength;

        /// <summary>
        /// Observers of this mesh who want to know when the mesh has finished updating
        /// </summary>
        private List<IVoxelMeshObserver> _observers; 

        /// <summary>
        /// Get voxel data from a single point
        /// </summary>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="d">depth</param>
        /// <returns></returns>
        public T GetVoxel(int w, int h, int d)
        {
            return _voxels[Utilities.GetIndex(w, h, d, _wLength, _hLength, _dLength)];
        }

        /// <summary>
        /// Sets a single voxel in the volume
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="d"></param>
        /// <param name="voxel"></param>
        public void SetVoxel(int w, int h, int d, T voxel)
        {
            _voxels[Utilities.GetIndex(w, h, d, _wLength, _hLength, _dLength)] = voxel;
            UpdateMesh();
        }

        /// <summary>
        /// Sets voxel volume from 3-dimensional array
        /// </summary>
        /// <param name="voxels"></param>
        public void SetVoxels(T[,,] voxels)
        {
            // Copy the new voxels into the class
            _voxels = new List<T>();
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
                        _voxels.Add(new T());
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
                        _voxels[Utilities.GetIndex(w, h, d, _wLength, _hLength, _dLength)] = (T)voxels[w, h, d].DeepCopy();
                    }
                }
            }

            UpdateMesh();
        }

        /// <summary>
        /// Trigger update on mesh in editor
        /// </summary> 
        public void UpdateMesh()
        {
            // Set voxels and queue this mesh up to be generated
            _generatorTask = new VoxelMeshGeneratorTask<T>(_voxels, _wLength, _hLength, _dLength);
            VoxelMeshGeneratorThread.Generator.EnqueueTask(_generatorTask);
        }

        /// <summary>
        /// If there is a mesh to be generated, this is a reference to that task
        /// </summary>
        private VoxelMeshGeneratorTask<T> _generatorTask;

        // References to asset components
        protected MeshFilter MeshFilter;
        protected Mesh Mesh;
        protected MeshCollider MeshCollider;
        protected MeshRenderer MeshRenderer;

        public void Awake()
        {
            _observers = new List<IVoxelMeshObserver>();
        }

        public virtual void Start()
        {
            if(UniqueId == null)
            {
                UniqueId = Guid.NewGuid().ToString();
            }

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
            MeshCollider = GetComponent<MeshCollider>();

            MeshRenderer = GetComponent<MeshRenderer>();
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
            Mesh.Clear();
            Mesh.vertices = _generatorTask.Vertices.ToArray();
            Mesh.normals = _generatorTask.Normals.ToArray();
            Mesh.colors = _generatorTask.Colors.ToArray();
            Mesh.uv = _generatorTask.UV.ToArray();
            Mesh.uv2 = _generatorTask.UV2.ToArray();
            Mesh.uv3 = _generatorTask.UV3.ToArray();
            Mesh.triangles = _generatorTask.Triangles.ToArray();

            TangentSolver(Mesh);

            Mesh.Optimize();
            Mesh.RecalculateBounds();
            MeshCollider.sharedMesh = Mesh;
            _generatorTask = null;

            foreach(IVoxelMeshObserver observer in _observers)
            {
                observer.Notify();
            }
        }

        public void OnDestroy()
        {
            _generatorTask = null;
        }

        public void RegisterObserver(IVoxelMeshObserver observer)
        {
            _observers.Add(observer);
        }

        public void UnregisterObserver(IVoxelMeshObserver observer)
        {
            _observers.Remove(observer);
        }
    }
}
