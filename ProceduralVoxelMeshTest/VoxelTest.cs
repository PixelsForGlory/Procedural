// Copyright 2015 afuzzyllama. All Rights Reserved.
using System;
using System.Collections.Generic;
using NUnit.Framework;
using ProceduralVoxelMesh;
using UnityEngine;
// ReSharper disable NotAccessedVariable
// ReSharper disable JoinDeclarationAndInitializer
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable SuspiciousTypeConversion.Global

namespace ProceduralVoxelMeshTest
{
    [TestFixture]
    class VoxelTest
    {
        [Test]
        public void CreateTest()
        {
            var voxel = new Voxel();
        }

        [Test]
        public void EmptyPropertyTest()
        {
            var voxel = new Voxel();
            bool empty = true;
            Assert.Throws<NotImplementedException>(() =>  empty = voxel.Empty);
            Assert.Throws<NotImplementedException>(() => voxel.Empty = empty);
        }

        [Test]
        public void AddVoxelToMeshTest()
        {
            var voxel = new Voxel();
            Assert.Throws<NotImplementedException>(() => voxel.AddVoxelToMesh(FaceType.None, 0, 0, new List<Color>(), new List<Vector2>(), new List<Vector2>(), new List<Vector2>()));
        }

    }
}
