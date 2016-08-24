// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using UnityEngine;
using System.Threading;
using ThreadPriority = System.Threading.ThreadPriority;

namespace PixelsForGlory.ProceduralVoxelMesh
{
    /// <summary>
    /// Singleton class that handles creating the generator thread at start up.
    /// </summary>
    public class VoxelMeshGeneratorThread : MonoBehaviour
    {
        private static Thread _generatorThread;

        private static VoxelMeshGenerator _generator;
        public static VoxelMeshGenerator Generator
        {
            get { return _generator; }
            private set { _generator = value; }
        } 
        
        public void Start()
        {
            StartThread();
            transform.hideFlags = HideFlags.HideInInspector;
        }

        public void OnApplicationQuit()
        {
            // If the application is running as the editor, when the user creates the game object
            // to run the thread, we don't want it shutting down the generator.
            if(!Application.isEditor)
            {
                Generator.Shutdown();
            }            
        }

        /// <summary>
        /// Called from the editor application update event. Updates all meshes in the editor.
        /// </summary>
        public static void UpdateMeshesInEditor()
        {
            ColorVoxelMesh[] colorVoxelMeshes = FindObjectsOfType<ColorVoxelMesh>();
            foreach(ColorVoxelMesh voxelMesh in colorVoxelMeshes)
            {
                voxelMesh.Update();
            }

            TextureVoxelMesh[] textureVoxelMeshes = FindObjectsOfType<TextureVoxelMesh>();
            foreach(TextureVoxelMesh voxelMesh in textureVoxelMeshes)
            {
                voxelMesh.Update();
            }

            BillboardCrossMesh[] billboardCrossMeshes = FindObjectsOfType<BillboardCrossMesh>();
            foreach(BillboardCrossMesh voxelMesh in billboardCrossMeshes)
            {
                voxelMesh.Update();
            }


        }

        /// <summary>
        /// Start voxel mesh generator
        /// </summary>
        public static void StartThread()
        {
            if (Generator == null)
            {
                Generator = new VoxelMeshGenerator();
                // ReSharper disable once RedundantDelegateCreation
                _generatorThread = new Thread(new ThreadStart(_generator.Run))
                {
                    Name = "VoxelMeshGeneratorThread",
                    Priority = ThreadPriority.Lowest,
                    IsBackground = true
                };
                
                _generatorThread.Start();
            }
        }

    }
}