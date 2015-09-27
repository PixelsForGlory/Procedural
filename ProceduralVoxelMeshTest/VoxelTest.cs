// Copyright 2015 afuzzyllama. All Rights Reserved.

using System;
using NUnit.Framework;
using ProceduralVoxelMesh;
using UnityEngine;

namespace ProceduralVoxelMeshTest
{
    [TestFixture]
    public class VoxelTest
    {
        [Test]
        public void VoxelCreateSuccessTest()
        {
            // ReSharper disable once NotAccessedVariable
            var newVoxel = new Voxel(true);
            
            // ReSharper disable RedundantAssignment
            newVoxel = new Voxel(true, Color.black);
            newVoxel = new Voxel(false, Color.black);
            // ReSharper restore RedundantAssignment
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void VoxelCreateFailTest()
        {
            // ReSharper disable once UnusedVariable
            var newVoxel = new Voxel(false);   
        }

        [Test]
        public void VoxelPropertiesSuccessTest()
        {
            var newVoxel = new Voxel(true);
            Assert.IsTrue(newVoxel.Empty);

            newVoxel = new Voxel(false, Color.blue, 0.25f, 0.5f, 0.75f);
            Assert.IsFalse(newVoxel.Empty);
            Assert.AreEqual(newVoxel.Color, Color.blue);
            Assert.AreEqual(newVoxel.Metallic, 0.25f);
            Assert.AreEqual(newVoxel.Smoothness, 0.5f);
            Assert.AreEqual(newVoxel.Emission, 0.75f);
        }

        [Test]
        public void VoxelPropertiesFailTest()
        {
            var newVoxel = new Voxel(true);

            // ReSharper disable NotAccessedVariable
            Color color;
            Assert.Throws<InvalidOperationException>(() => color = newVoxel.Color);

            float metallic;
            Assert.Throws<InvalidOperationException>(() => metallic = newVoxel.Metallic);

            float smoothness;
            Assert.Throws<InvalidOperationException>(() => smoothness = newVoxel.Smoothness);

            float emission;
            Assert.Throws<InvalidOperationException>(() => emission = newVoxel.Emission);

            newVoxel = new Voxel(false, Color.black);
            Assert.Throws<InvalidOperationException>(() => newVoxel.Metallic = -1.0f);
            Assert.Throws<InvalidOperationException>(() => newVoxel.Metallic = 2.0f);
            Assert.Throws<InvalidOperationException>(() => newVoxel.Smoothness = -1.0f);
            Assert.Throws<InvalidOperationException>(() => newVoxel.Smoothness = 2.0f);
            Assert.Throws<InvalidOperationException>(() => newVoxel.Emission = -1.0f);
            Assert.Throws<InvalidOperationException>(() => newVoxel.Emission = 2.0f);

            // ReSharper restore NotAccessedVariable
        }
    }
}
