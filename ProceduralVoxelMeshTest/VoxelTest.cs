// Copyright 2015 afuzzyllama. All Rights Reserved.

using System;
using NUnit.Framework;
using ProceduralVoxelMesh;
using UnityEngine;

namespace ProceduralVoxelMeshTest
{
    [TestFixture]
    class VoxelTest
    {
        [Test]
        public void VoxelCreateSuccessTest()
        {
            // ReSharper disable once NotAccessedVariable
            var newVoxel = new Voxel(true);
            newVoxel = new Voxel(true, Color.black);
            newVoxel = new Voxel(false, Color.black);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void VoxelCreateFailTest()
        {
            var newVoxel = new Voxel(false, null);   
        }

        [Test]
        public void VoxelPropertiesSuccessTest()
        {
            var newVoxel = new Voxel(true);
            Assert.IsTrue(newVoxel.Empty);

            newVoxel = new Voxel(false, Color.blue);
            Assert.IsFalse(newVoxel.Empty);
            Assert.AreEqual(newVoxel.Color, Color.blue);
        }

        [Test]
        [ExpectedException(typeof (InvalidOperationException))]
        public void VoxelPropertiesFailTest()
        {
            var newVoxel = new Voxel(true);
            Color emptyColor = newVoxel.Color;
        }
    }
}
