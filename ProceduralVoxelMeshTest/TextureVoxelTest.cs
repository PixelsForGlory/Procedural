// Copyright 2015 afuzzyllama. All Rights Reserved.
using System;
using NUnit.Framework;
using ProceduralVoxelMesh;
using UnityEngine;
// ReSharper disable NotAccessedVariable
// ReSharper disable JoinDeclarationAndInitializer
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable ObjectCreationAsStatement

namespace ProceduralVoxelMeshTest
{
    [TestFixture]
    class TextureVoxelTest
    {
        [SetUp]
        public void TextureVoxelTestSetup()
        {
            TextureVoxel.TextureVoxelMap.Clear();
            TextureVoxel.TextureVoxelMap.Add
                (
                    new TextureVoxelSetup()
                    {
                        XPositiveTextureIndex = 0,
                        XNegativeTextureIndex = 1,
                        YPositiveTextureIndex = 2,
                        YNegativeTextureIndex = 3,
                        ZPositiveTextureIndex = 4,
                        ZNegativeTextureIndex = 5
                    }
                );
            
        }

        [Test]
        public void CreateTest()
        {
            TextureVoxel voxel;
            voxel = new TextureVoxel();
            Assert.IsTrue(voxel.Empty);

            Assert.Throws<ArgumentException>(() => new TextureVoxel(-1));
            Assert.Throws<ArgumentException>(() => new TextureVoxel(1));
            Assert.Throws<ArgumentException>(() => new TextureVoxel(0, -1));
            Assert.Throws<ArgumentException>(() => new TextureVoxel(0, 1));

            voxel = new TextureVoxel(0, 0);
            Assert.IsFalse(voxel.Empty);
            Assert.AreEqual(voxel.TextureMapIndex, 0);
            Assert.AreEqual(voxel.DetailMapIndex, 0);
        }

        [Test]
        public void EmptyPropertyTest()
        {
            var voxel = new TextureVoxel();
            voxel.Empty = true;
            Assert.AreEqual(voxel.Empty, true);

            Assert.Throws<InvalidOperationException>(() => voxel.Empty = false);
        }
        
        [Test]
        public void TextureMapIndexPropertyTest()
        {
            int index;

            var voxel = new TextureVoxel();
            Assert.Throws<InvalidOperationException>(() => index = voxel.TextureMapIndex);
            Assert.Throws<InvalidOperationException>(() => voxel.TextureMapIndex = -1);
            Assert.Throws<InvalidOperationException>(() => voxel.TextureMapIndex = 1);

            voxel.TextureMapIndex = 0;
            Assert.AreEqual(voxel.TextureMapIndex, 0);
        }

        [Test]
        public void DetailMapIndexPropertyTest()
        {
            int index;

            var voxel = new TextureVoxel();
            Assert.Throws<InvalidOperationException>(() => index = voxel.DetailMapIndex);
            Assert.Throws<InvalidOperationException>(() => voxel.DetailMapIndex = 0);

            voxel.TextureMapIndex = 0;
            Assert.Throws<InvalidOperationException>(() => voxel.DetailMapIndex = -1);
            Assert.Throws<InvalidOperationException>(() => voxel.DetailMapIndex = 1);

            voxel.DetailMapIndex = 0;
            Assert.AreEqual(voxel.DetailMapIndex, 0);
        }


        [Test]
        public void EqualsTest()
        {
            var voxelOne = new TextureVoxel();
            var voxelTwo = new TextureVoxel(0, 0);
            Assert.IsFalse(voxelOne.Equals(voxelTwo));

            voxelOne = new TextureVoxel(0, 0);
            Assert.IsTrue(voxelOne.Equals(voxelTwo));

            voxelTwo = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.IsFalse(voxelOne.Equals(voxelTwo));

            var color = Color.black;
            Assert.IsFalse(voxelOne.Equals(color));
        }

        [Test]
        public void GetHashCodeTest()
        {
            var voxelOne = new TextureVoxel();
            var voxelTwo = new TextureVoxel(0, 0);

            Assert.IsFalse(voxelOne.GetHashCode() == voxelTwo.GetHashCode());

            voxelOne = new TextureVoxel(0, 0);

            Assert.IsTrue(voxelOne.GetHashCode() == voxelTwo.GetHashCode());
        }
    }
}
