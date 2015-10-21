// Copyright 2015 afuzzyllama. All Rights Reserved.
using System;
using NUnit.Framework;
using ProceduralVoxelMesh;
// ReSharper disable NotAccessedVariable
// ReSharper disable JoinDeclarationAndInitializer
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable SuspiciousTypeConversion.Global

namespace ProceduralVoxelMeshTest
{
    [TestFixture]
    class UtilitiesTest
    {

        [Test]
        public void GetIndexTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Utilities.GetIndex(-1, 0, 0, 2, 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => Utilities.GetIndex(0, -1, 0, 2, 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => Utilities.GetIndex(0, 0, -1, 2, 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => Utilities.GetIndex(2, 0, 0, 2, 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => Utilities.GetIndex(0, 2, 0, 2, 2, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => Utilities.GetIndex(0, 0, 2, 2, 2, 2));
            Assert.AreEqual(Utilities.GetIndex(1, 1, 1, 2, 2, 2), 1 + 1 * 2 + 1 * 2 * 2);
        }

        [Test]
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
