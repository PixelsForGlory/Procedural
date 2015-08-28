using UnityEngine;
using UnityEditor;
using System.Threading;

namespace ProceduralVoxelMesh
{
    /// <summary>
    /// Singleton class that handles creating the generator thread at start up.
    /// Uses static constructor to start in the editor or the Start method in game.
    /// </summary>
    [InitializeOnLoad]
    [ExecuteInEditMode]
    public class VoxelMeshGeneratorThread : MonoBehaviour
    {
        private static Thread _generatorThread;

        private static VoxelMeshGenerator _generator;
        public static VoxelMeshGenerator Generator => _generator;

        // ReSharper doesn't realize that this constructor is not empty
        // ReSharper disable once EmptyConstructor
        static VoxelMeshGeneratorThread()
        {
#if UNITY_EDITOR
		StartThread();
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
            _generator.Shutdown();
#endif
            // Kind of messy. The thread will shutdown after going out of scope in the editor :(
        }

        private static void StartThread()
        {
            _generator = new VoxelMeshGenerator();
            // ReSharper disable once RedundantDelegateCreation
            _generatorThread = new Thread(new ThreadStart(_generator.Run));
            _generatorThread.Start();
        }


    }
}