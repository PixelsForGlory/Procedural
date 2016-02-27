using UnityEngine;
using UnityEditor;

namespace ProceduralVoxelMeshEditor
{
    /// <summary>
    /// Class to start the generator thread by default on Editor startup.
    /// </summary>
    [InitializeOnLoad]
    public class VoxelMeshGeneratorThreadEditor : MonoBehaviour
    {
        static VoxelMeshGeneratorThreadEditor()
        {
            ProceduralVoxelMesh.VoxelMeshGeneratorThread.StartThread();
            EditorApplication.update += ProceduralVoxelMesh.VoxelMeshGeneratorThread.UpdateMeshesInEditor;
        }
    }
}
