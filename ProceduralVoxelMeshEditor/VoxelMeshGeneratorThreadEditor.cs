// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using UnityEngine;
using UnityEditor;

namespace PixelsForGlory.ProceduralVoxelMeshEditor
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
