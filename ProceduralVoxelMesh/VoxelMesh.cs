// Copyright 2015-2016 afuzzyllama. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelsForGlory.ProceduralVoxelMesh
{
    /// <summary>
    /// Color voxel mesh implementation.  Plays nice with Unity3D.
    /// </summary>
    [Serializable]
    public class ColorVoxelMesh : VoxelMesh<ColorVoxel>
    {
        [SerializeField]
        public static string MaterialResourcePath = string.Empty;

        [SerializeField]
        private ColorVoxelMeshData _voxelData;

        public override string UniqueId => _voxelData.UniqueId;
        public override string Name => _voxelData.Name;
        public override int Width => _voxelData.Width;
        public override int Height => _voxelData.Height;
        public override int Depth => _voxelData.Depth;
        public override IList<ColorVoxel> Voxels => _voxelData.Voxels;
        public override VoxelMeshData<ColorVoxel> VoxelData => _voxelData;

        public override void SetVoxelData(VoxelMeshData<ColorVoxel> voxelData)
        {
            _voxelData = new ColorVoxelMeshData(voxelData.UniqueId, voxelData.Name, voxelData.Width, voxelData.Height, voxelData.Depth, voxelData.Voxels);
            name = voxelData.Name;
            UpdateMesh();
        }

        public override void Start()
        {
            base.Start();
            MeshRenderer.sharedMaterial = Resources.Load<Material>(MaterialResourcePath + "ColorVoxelMaterial");
        }
    }

    /// <summary>
    /// Color voxel mesh transparent implementation.  Plays nice with Unity3D.
    /// </summary>
    [Serializable]
    public class ColorVoxelTransparentMesh : VoxelMesh<ColorVoxel>
    {
        [SerializeField]
        public static string MaterialResourcePath = string.Empty;

        [SerializeField]
        private ColorVoxelMeshData _voxelData;

        public override string UniqueId => _voxelData.UniqueId;
        public override string Name => _voxelData.Name;
        public override int Width => _voxelData.Width;
        public override int Height => _voxelData.Height;
        public override int Depth => _voxelData.Depth;
        public override IList<ColorVoxel> Voxels => _voxelData.Voxels;
        public override VoxelMeshData<ColorVoxel> VoxelData => _voxelData;

        public override void SetVoxelData(VoxelMeshData<ColorVoxel> voxelData)
        {
            _voxelData = new ColorVoxelMeshData(voxelData.UniqueId, voxelData.Name, voxelData.Width, voxelData.Height, voxelData.Depth, voxelData.Voxels);
            name = voxelData.Name;
            UpdateMesh();
        }

        public override void Start()
        {
            base.Start();
            MeshRenderer.sharedMaterial = Resources.Load<Material>(MaterialResourcePath + "ColorVoxelTransparentMaterial");
        }
    }

    /// <summary>
    /// Texture voxel mesh implementation.  Plays nice with Unity3D.
    /// </summary>
    [Serializable]
    public class TextureVoxelMesh : VoxelMesh<TextureVoxel>
    {
        [SerializeField]
        public static string MaterialResourcePath = string.Empty;

        [SerializeField]
        private TextureVoxelMeshData _voxelData;

        public override string UniqueId => _voxelData.UniqueId;
        public override string Name => _voxelData.Name;
        public override int Width => _voxelData.Width;
        public override int Height => _voxelData.Height;
        public override int Depth => _voxelData.Depth;
        public override IList<TextureVoxel> Voxels => _voxelData.Voxels;
        public override VoxelMeshData<TextureVoxel> VoxelData => _voxelData;

        public override void SetVoxelData(VoxelMeshData<TextureVoxel> voxelData)
        {
            _voxelData = new TextureVoxelMeshData(voxelData.UniqueId, voxelData.Name, voxelData.Width, voxelData.Height, voxelData.Depth, voxelData.Voxels);
            name = voxelData.Name;
            UpdateMesh();
        }

        public override void Start()
        {
            base.Start();
            MeshRenderer.sharedMaterial = Resources.Load<Material>(MaterialResourcePath + "TextureVoxelMaterial");
        }
    }

    /// <summary>
    /// Texture voxel mesh implementation.  Plays nice with Unity3D.
    /// </summary>
    [Serializable]
    public class TextureVoxelTransparentMesh : VoxelMesh<TextureVoxel>
    {
        [SerializeField]
        public static string MaterialResourcePath = string.Empty;

        [SerializeField]
        private TextureVoxelMeshData _voxelData;

        public override string UniqueId => _voxelData.UniqueId;
        public override string Name => _voxelData.Name;
        public override int Width => _voxelData.Width;
        public override int Height => _voxelData.Height;
        public override int Depth => _voxelData.Depth;
        public override IList<TextureVoxel> Voxels => _voxelData.Voxels;
        public override VoxelMeshData<TextureVoxel> VoxelData => _voxelData;

        public override void SetVoxelData(VoxelMeshData<TextureVoxel> voxelData)
        {
            _voxelData = new TextureVoxelMeshData(voxelData.UniqueId, voxelData.Name, voxelData.Width, voxelData.Height, voxelData.Depth, voxelData.Voxels);
            name = voxelData.Name;
            UpdateMesh();
        }

        public override void Start()
        {
            base.Start();
            MeshRenderer.sharedMaterial = Resources.Load<Material>(MaterialResourcePath + "TextureVoxelTransparentMaterial");
        }
    }

    /// <summary>
    /// Voxel mesh component
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [ExecuteInEditMode]
    public abstract class VoxelMesh<T> : MonoBehaviour, ISerializationCallbackReceiver where T : IVoxel, new()
    {
        /// <summary>
        /// UniqueId based on System.Guid
        /// </summary>
        public abstract string UniqueId { get; }

        /// <summary>
        /// Name of voxel mesh
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Width of the voxel mesh
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        /// Height of the voxel mesh
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        /// Depth of the voxel mesh
        /// </summary>
        public abstract int Depth { get; }

        public abstract IList<T> Voxels { get; }

        public abstract VoxelMeshData<T> VoxelData { get; }

        private int _levelOfDetail;

        public bool WaitingForUpdate
        {
            get;
            private set;
        }

        /// <summary>
        /// Level of detail to render.  2^(LOD)
        /// </summary>
        public int LevelOfDetail
        {
            get
            {
                return _levelOfDetail;
            }

            set
            {
                _levelOfDetail = value;
                UpdateMesh();
            }
        }

        /// <summary>
        /// Observers of this mesh who want to know when the mesh has finished updating
        /// </summary>
        protected List<IVoxelMeshObserver> Observers;

        /// <summary>
        /// Get voxel data from a single point
        /// </summary>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="d">depth</param>
        /// <returns></returns>
        public T GetVoxel(int w, int h, int d)
        {
            return Voxels[Utilities.GetIndex(w, h, d, Width, Height, Depth)];
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
            Voxels[Utilities.GetIndex(w, h, d, Width, Height, Depth)] = voxel;
            UpdateMesh();
        }

        /// <summary>
        /// Sets voxel data
        /// </summary>
        /// <param name="voxelData">Voxel data this mesh will contain</param>
        public abstract void SetVoxelData(VoxelMeshData<T> voxelData);

        /// <summary>
        /// Trigger update on mesh in editor
        /// </summary> 
        public void UpdateMesh()
        {
            if(VoxelData == null)
            {
                return;
            }
            // Set voxels and queue this mesh up to be generated
            _generatorTask = new VoxelMeshGeneratorTask<T>(Voxels, LevelOfDetail, Width, Height, Depth);
            VoxelMeshGeneratorThread.Generator.EnqueueTask(_generatorTask);
            WaitingForUpdate = true;
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
            Observers = new List<IVoxelMeshObserver>();
        }

        public virtual void Start()
        {
            MeshFilter = GetComponent<MeshFilter>();
            if (Mesh == null)
            {
                // This is written this way to prevent leaking of meshes in the editor
                if (MeshFilter.sharedMesh == null)
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

            WaitingForUpdate = false;
        }

        public void Update()
        {
            // If there is no task, there is nothing to update
            if (_generatorTask == null)
            {
                return;
            }

            // If the task is not completed, there is nothing to do yet
            if (!_generatorTask.Completed)
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
            Mesh.colors = _generatorTask.Colors;
            Mesh.uv = _generatorTask.UV;
            Mesh.uv2 = _generatorTask.UV2;
            Mesh.uv3 = _generatorTask.UV3;
            Mesh.triangles = _generatorTask.Triangles;
            Mesh.tangents = _generatorTask.Tangents;
            Mesh.Optimize();
            Mesh.RecalculateBounds();
            MeshCollider.sharedMesh = Mesh;
            _generatorTask = null;

            WaitingForUpdate = false;

            foreach (IVoxelMeshObserver observer in Observers)
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
            Observers.Add(observer);
        }

        public void UnregisterObserver(IVoxelMeshObserver observer)
        {
            Observers.Remove(observer);
        }

        public void OnBeforeSerialize()
        {
            Observers.Clear();
        }

        public void OnAfterDeserialize()
        {
            Observers = new List<IVoxelMeshObserver>();
        }
    }
}