// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PixelsForGlory.ProceduralVoxelMesh;
using UnityEngine;
// ReSharper disable NotAccessedVariable
// ReSharper disable JoinDeclarationAndInitializer
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable ObjectCreationAsStatement

namespace ProceduralVoxelMeshTest
{
    [TestClass]
    public class TextureVoxelTest
    {
        [ClassInitialize]
        public static void TextureVoxelTestSetup(TestContext context)
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

        [TestMethod]
        public void CreateTest()
        {
            TextureVoxel voxel;
            voxel = new TextureVoxel();
            Assert.IsTrue(voxel.Empty);

            try
            {
                new TextureVoxel(-1);
            }
            catch(ArgumentException)
            {
            }
            catch(Exception)
            {
                Assert.Fail();
            }

            try
            {
                new TextureVoxel(1);
            }
            catch (ArgumentException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                new TextureVoxel(0, -1);
            }
            catch (ArgumentException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                new TextureVoxel(0, 1); 
            }
            catch (ArgumentException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            
            // ReSharper disable once RedundantArgumentDefaultValue
            voxel = new TextureVoxel(0, 0);
            Assert.IsFalse(voxel.Empty);
            Assert.AreEqual(voxel.TextureMapIndex, 0);
            Assert.AreEqual(voxel.DetailMapIndex, 0);
        }

        [TestMethod]
        public void EmptyPropertyTest()
        {
            var voxel = new TextureVoxel();
            voxel.Empty = true;
            Assert.AreEqual(voxel.Empty, true);

            try
            {
                voxel.Empty = false;
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

        }
        
        [TestMethod]
        public void TextureMapIndexPropertyTest()
        {
            int index;

            var voxel = new TextureVoxel();

            try
            {
                // ReSharper disable once RedundantAssignment
                index = voxel.TextureMapIndex;
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                voxel.TextureMapIndex = -1;
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                voxel.TextureMapIndex = 1;
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            voxel.TextureMapIndex = 0;
            Assert.AreEqual(voxel.TextureMapIndex, 0);
        }

        [TestMethod]
        public void DetailMapIndexPropertyTest()
        {
            int index;

            var voxel = new TextureVoxel();

            try
            {
                // ReSharper disable once RedundantAssignment
                index = voxel.DetailMapIndex;
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                voxel.DetailMapIndex = 0;
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            
            voxel.TextureMapIndex = 0;

            try
            {
                voxel.DetailMapIndex = -1;
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                voxel.DetailMapIndex = 1;
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            
            voxel.DetailMapIndex = 0;
            Assert.AreEqual(voxel.DetailMapIndex, 0);
        }


        [TestMethod]
        public void EqualsTest()
        {
            var voxelOne = new TextureVoxel();
            // ReSharper disable once RedundantArgumentDefaultValue
            var voxelTwo = new TextureVoxel(0, 0);
            Assert.IsFalse(voxelOne.Equals(voxelTwo));

            // ReSharper disable once RedundantArgumentDefaultValue
            voxelOne = new TextureVoxel(0, 0);
            Assert.IsTrue(voxelOne.Equals(voxelTwo));

            int notVoxel = 1;
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.IsFalse(voxelOne.Equals(notVoxel));

            var color = Color.black;
            Assert.IsFalse(voxelOne.Equals(color));
        }

        [TestMethod]
        public void GetHashCodeTest()
        {
            var voxelOne = new TextureVoxel();
            // ReSharper disable once RedundantArgumentDefaultValue
            var voxelTwo = new TextureVoxel(0, 0);

            Assert.IsFalse(voxelOne.GetHashCode() == voxelTwo.GetHashCode());

            // ReSharper disable once RedundantArgumentDefaultValue
            voxelOne = new TextureVoxel(0, 0);

            Assert.IsTrue(voxelOne.GetHashCode() == voxelTwo.GetHashCode());
        }
    }
}
