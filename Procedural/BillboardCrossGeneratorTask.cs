// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using System.Collections.Generic;
using UnityEngine;

namespace PixelsForGlory.Procedural
{
    /// <summary>
    /// Task to create a two sided crossing mesh
    /// </summary>
    public class BillboardCrossGeneratorTask : IVoxelMeshGeneratorTask
    {
        public bool Completed { get; private set; }
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public Vector2[] UV;
        public Vector2[] UV2;
        public int[] Triangles;
        public Vector4[] Tangents;

        private readonly BillboardCrossMeshData _billboardData;

        public BillboardCrossGeneratorTask(BillboardCrossMeshData billboardData)
        {
            _billboardData = new BillboardCrossMeshData(billboardData);
            Completed = false;
        }

        public void CreateMesh()
        {
            if(_billboardData.BottomTextureIndex == -1 && _billboardData.MiddleTextureIndex == -1 && _billboardData.TopTextureIndex == -1)
            {
                Vertices = new Vector3[0];
                Normals = new Vector3[0];
                UV = new Vector2[0];
                UV2 = new Vector2[0];
                Triangles = new int[0];
                Tangents = new Vector4[0];
            }

            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uv = new List<Vector2>();
            var uv2 = new List<Vector2>();
            var triangles = new List<int>();

            float currentHeight = 0f;
            if(_billboardData.BottomTextureIndex != -1)
            {
                currentHeight += 1f;
            }

            currentHeight += _billboardData.MiddleCount;

            if(_billboardData.TopTextureIndex != -1)
            {
                currentHeight += 1f;
            }

            currentHeight /= -2f;

            if(_billboardData.BottomTextureIndex != -1)
            {
                AddBillboards(currentHeight, vertices, normals, uv, uv2, triangles, _billboardData.BottomTextureIndex);
                currentHeight += 1f;
            }

            for(int repeat = 0; repeat < _billboardData.MiddleCount; repeat++)
            {
                AddBillboards(currentHeight, vertices, normals, uv, uv2, triangles, _billboardData.MiddleTextureIndex);
                currentHeight += 1f;
            }

            if(_billboardData.TopTextureIndex != -1)
            {
                AddBillboards(currentHeight, vertices, normals, uv, uv2, triangles, _billboardData.TopTextureIndex);
            }

            Triangles = triangles.ToArray();
            Vertices = vertices.ToArray();
            Normals = normals.ToArray();
            UV = uv.ToArray();
            UV2 = uv2.ToArray();

            Tangents = Utilities.TangentSolver(Triangles, Vertices, Normals, UV);

            Completed = true;
        }

        private void AddBillboards(float currentHeight, IList<Vector3> vertices, IList<Vector3> normals, IList<Vector2> uv, IList<Vector2> uv2,  IList<int> triangles, int textureIndex)
        {
            int startingVerticesCount = vertices.Count;

            // +z face
            vertices.Add(new Vector3( 0.5f, currentHeight     , 0f));   // 0
            vertices.Add(new Vector3(-0.5f, currentHeight + 1f, 0f));   // 1
            vertices.Add(new Vector3(-0.5f, currentHeight     , 0f));   // 2
            vertices.Add(new Vector3( 0.5f, currentHeight + 1f, 0f));   // 3

            normals.Add(new Vector3(0f, 0f, 1f));
            normals.Add(new Vector3(0f, 0f, 1f));
            normals.Add(new Vector3(0f, 0f, 1f));
            normals.Add(new Vector3(0f, 0f, 1f));

            uv.Add(new Vector2(0f, 0f));
            uv.Add(new Vector2(1f, 1f));
            uv.Add(new Vector2(1f, 0f));
            uv.Add(new Vector2(0f, 1f));

            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));

            triangles.Add(startingVerticesCount);
            triangles.Add(startingVerticesCount + 1);
            triangles.Add(startingVerticesCount + 2);

            triangles.Add(startingVerticesCount);
            triangles.Add(startingVerticesCount + 3);
            triangles.Add(startingVerticesCount + 1);

            // -z face
            vertices.Add(new Vector3(-0.5f, currentHeight     , 0f));   // 4
            vertices.Add(new Vector3( 0.5f, currentHeight + 1f, 0f));   // 5
            vertices.Add(new Vector3( 0.5f, currentHeight     , 0f));   // 6
            vertices.Add(new Vector3(-0.5f, currentHeight + 1f, 0f));   // 7


            normals.Add(new Vector3(0f, 0f, -1f));
            normals.Add(new Vector3(0f, 0f, -1f));
            normals.Add(new Vector3(0f, 0f, -1f));
            normals.Add(new Vector3(0f, 0f, -1f));

            uv.Add(new Vector2(0f, 0f));
            uv.Add(new Vector2(1f, 1f));
            uv.Add(new Vector2(1f, 0f));
            uv.Add(new Vector2(0f, 1f));

            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));

            triangles.Add(startingVerticesCount + 4);
            triangles.Add(startingVerticesCount + 5);
            triangles.Add(startingVerticesCount + 6);

            triangles.Add(startingVerticesCount + 4);
            triangles.Add(startingVerticesCount + 7);
            triangles.Add(startingVerticesCount + 5);


            // +x face
            vertices.Add(new Vector3(0f, currentHeight     , -0.5f));   // 8
            vertices.Add(new Vector3(0f, currentHeight + 1f,  0.5f));   // 9
            vertices.Add(new Vector3(0f, currentHeight     ,  0.5f));   // 10
            vertices.Add(new Vector3(0f, currentHeight + 1f, -0.5f));   // 11

            normals.Add(new Vector3(1f, 0f, 0f));
            normals.Add(new Vector3(1f, 0f, 0f));
            normals.Add(new Vector3(1f, 0f, 0f));
            normals.Add(new Vector3(1f, 0f, 0f));

            uv.Add(new Vector2(0f, 0f));
            uv.Add(new Vector2(1f, 1f));
            uv.Add(new Vector2(1f, 0f));
            uv.Add(new Vector2(0f, 1f));

            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));

            triangles.Add(startingVerticesCount + 8);
            triangles.Add(startingVerticesCount + 9);
            triangles.Add(startingVerticesCount + 10);

            triangles.Add(startingVerticesCount + 8);
            triangles.Add(startingVerticesCount + 11);
            triangles.Add(startingVerticesCount + 9);

            // -x face
            vertices.Add(new Vector3(0f, currentHeight     ,  0.5f));   // 12
            vertices.Add(new Vector3(0f, currentHeight + 1f, -0.5f));   // 13
            vertices.Add(new Vector3(0f, currentHeight     , -0.5f));   // 14
            vertices.Add(new Vector3(0f, currentHeight + 1f,  0.5f));   // 15

            normals.Add(new Vector3(-1f, 0f, 0f));
            normals.Add(new Vector3(-1f, 0f, 0f));
            normals.Add(new Vector3(-1f, 0f, 0f));
            normals.Add(new Vector3(-1f, 0f, 0f));

            uv.Add(new Vector2(0f, 0f));
            uv.Add(new Vector2(1f, 1f));
            uv.Add(new Vector2(1f, 0f));
            uv.Add(new Vector2(0f, 1f));

            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));
            uv2.Add(new Vector2(textureIndex / 256f, 0f));

            triangles.Add(startingVerticesCount + 12);
            triangles.Add(startingVerticesCount + 13);
            triangles.Add(startingVerticesCount + 14);

            triangles.Add(startingVerticesCount + 12);
            triangles.Add(startingVerticesCount + 15);
            triangles.Add(startingVerticesCount + 13);
        }
    }
}