using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Threading;

namespace ProceduralVoxelMesh
{
    /// <summary>
    /// Singleton class that handles creating the generator thread at start up.
    /// Uses static constructor to start in the editor or the Start method in game.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class VoxelMeshGeneratorThread : MonoBehaviour
    {
        private static Thread _generatorThread;

        private static VoxelMeshGenerator _generator;
        public static VoxelMeshGenerator Generator
        {
            get { return _generator; }
            private set { _generator = value; }
        } 

        // ReSharper doesn't realize that this constructor is not empty
        static VoxelMeshGeneratorThread()
        {
#if UNITY_EDITOR
		    StartThread();
            EditorApplication.update += UpdateMeshesInEditor;
#endif
        }

        public void Start()
        {
#if !UNITY_EDITOR
            StartThread();
#endif
            transform.hideFlags = HideFlags.HideInInspector;
        }
        public void OnApplicationQuit()
        {
#if !UNITY_EDITOR
            Generator.Shutdown();
#endif
            // Kind of messy. The thread will shutdown after going out of scope in the editor :(
        }

        public static void UpdateMeshesInEditor()
        {
            VoxelMesh[] voxelMeshes = FindObjectsOfType<VoxelMesh>();

            foreach(VoxelMesh voxelMesh in voxelMeshes)
            {
                voxelMesh.Update();
            }

        }

        private static void StartThread()
        {
            if (Generator == null)
            {
                Generator = new VoxelMeshGenerator();
                // ReSharper disable once RedundantDelegateCreation
                _generatorThread = new Thread(new ThreadStart(_generator.Run));
                _generatorThread.Start();
            }
        }

    }
}