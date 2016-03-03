// Copyright 2015 afuzzyllama. All Rights Reserved.
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProceduralVoxelMesh;
// ReSharper disable NotAccessedVariable
// ReSharper disable JoinDeclarationAndInitializer
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable SuspiciousTypeConversion.Global

namespace ProceduralVoxelMeshTest
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UtilitiesTest
    {

        [TestMethod]
        public void GetIndexTest()
        {
            try
            {
                Utilities.GetIndex(-1, 0, 0, 2, 2, 2);
            }
            catch (ArgumentOutOfRangeException) { }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                Utilities.GetIndex(0, -1, 0, 2, 2, 2);
            }
            catch (ArgumentOutOfRangeException) { }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                Utilities.GetIndex(0, 0, -1, 2, 2, 2);
            }
            catch (ArgumentOutOfRangeException) { }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                Utilities.GetIndex(2, 0, 0, 2, 2, 2);
            }
            catch (ArgumentOutOfRangeException) { }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                Utilities.GetIndex(0, 2, 0, 2, 2, 2);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            try
            {
                Utilities.GetIndex(0, 0, 2, 2, 2, 2);
            }
            catch(ArgumentOutOfRangeException)
            {
            }
            catch(Exception)
            {
                Assert.Fail();
            }

            Assert.AreEqual(Utilities.GetIndex(1, 1, 1, 2, 2, 2), 1 + 1 * 2 + 1 * 2 * 2);
        }

        [TestMethod]
        public void InBoundsTest()
        {
            Assert.IsFalse(Utilities.InBounds(-1, 0, 0, 2, 2, 2));
            Assert.IsFalse(Utilities.InBounds(0, -1, 0, 2, 2, 2));
            Assert.IsFalse(Utilities.InBounds(0, 0, -1, 2, 2, 2));
            Assert.IsFalse(Utilities.InBounds(2, 0, 0, 2, 2, 2));
            Assert.IsFalse(Utilities.InBounds(0, 2, 0, 2, 2, 2));
            Assert.IsFalse(Utilities.InBounds(0, 0, 2, 2, 2, 2));
            Assert.IsTrue(Utilities.InBounds(1, 1, 1, 2, 2, 2));
        }
    }
}
