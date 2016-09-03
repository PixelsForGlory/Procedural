// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using UnityEngine;
using UnityEditor;

namespace PixelsForGlory.Procedural.Editor
{
    /// <summary>
    /// Class to start the generator thread by default on Editor startup.
    /// </summary>
    [InitializeOnLoad]
    public class VoxelMeshGeneratorThreadEditor : MonoBehaviour
    {
        static VoxelMeshGeneratorThreadEditor()
        {
            Procedural.VoxelMeshGeneratorThread.StartThread();
            EditorApplication.update += Procedural.VoxelMeshGeneratorThread.UpdateMeshesInEditor;
        }
    }
}
