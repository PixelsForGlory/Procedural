// Copyright (C) afuzzyllama. All rights reserved
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace PixelsForGlory.Procedural
{
    /// <summary>
    /// Faces of a cube
    /// </summary>
    [Flags]
    public enum FaceType
    {
        None = 0,
        XPositive = 1,
        XNegative = 2,
        YPositive = 4,
        YNegative = 8,
        ZPositive = 16,
        ZNegative = 32
    }
}