// copyright 2015 afuzzyllama

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using ProceduralVoxelMesh;

namespace Assets
{
    public class Test : MonoBehaviour
    {
        private Queue<string> _testsToRunList;
        private string _currentTest;
        

        private GameObject _testObject;
        private int _screenshotCount;
        private bool _ready;
        private float _timeAcc;

	    public static void StartTest()
	    {
            #if UNITY_EDITOR
		    EditorApplication.ExecuteMenuItem("Edit/Play");
            #endif
	    }

        public void Start()
        {
            GameObject voxelMeshGeneratorObject = GameObject.Find("VoxelMeshGenerator");
            voxelMeshGeneratorObject.AddComponent<VoxelMeshGeneratorThread>();

            _testsToRunList = new Queue<string>();
            _testsToRunList.Enqueue("ColorVoxelMesh");
            _testsToRunList.Enqueue("TextureVoxelMesh");
            _currentTest = string.Empty;

            TextureVoxel.TextureVoxelMap.Clear();
            TextureVoxel.TextureVoxelMap.Add(
                new TextureVoxelSetup()
                {
                    XNegativeTextureIndex = 1,
                    XPositiveTextureIndex = 1,
                    YNegativeTextureIndex = 3,
                    YPositiveTextureIndex = 2,
                    ZNegativeTextureIndex = 1,
                    ZPositiveTextureIndex = 1
                });

            TextureVoxel.TextureVoxelMap.Add(
                new TextureVoxelSetup()
                {
                    XNegativeTextureIndex = 4,
                    XPositiveTextureIndex = 4,
                    YNegativeTextureIndex = 4,
                    YPositiveTextureIndex = 4,
                    ZNegativeTextureIndex = 4,
                    ZPositiveTextureIndex = 4
                });
        }

	    public void Update()
        {
            // Go to the next test
	        if(_screenshotCount == 6 && _testsToRunList.Count > 0)
	        {
                GetNextTest();
                ResetTest();
            }
            // All tests complete
            else if(_screenshotCount == 6 && _testsToRunList.Count == 0)
	        {
                #if UNITY_EDITOR
                EditorApplication.Exit(0);
                #else
                Application.Quit();     
                #endif
                return;
            }
            // Start first test
            else if(_currentTest == string.Empty)
            {
                GetNextTest();
                ResetTest();
            }

            if(!_ready && _timeAcc >= 0.66f)
            {
                switch(_screenshotCount)
                {
                    case 1:
                        _testObject.transform.Rotate(Vector3.up, 90.0f);
                        break;
                    case 2:
                        _testObject.transform.Rotate(Vector3.up, 90.0f);
                        break;
                    case 3:
                        _testObject.transform.Rotate(Vector3.up, 90.0f);
                        break;
                    case 4:
                        _testObject.transform.Rotate(Vector3.up, 90.0f);
                        _testObject.transform.Rotate(Vector3.left, 90.0f);
                        break;
                    case 5:
                        _testObject.transform.Rotate(Vector3.left, 180.0f);
                        break;
                }

                _ready = true;
                _timeAcc = 0.0f;
            }
        }

	    public void LateUpdate()
	    {
		    _timeAcc += Time.deltaTime;
		
		    if(_ready && _timeAcc >= 0.33f)
		    {
		        string screenshotName = string.Format("{0}_{1}.png", _currentTest, _screenshotCount.ToString());

                Debug.Log(string.Format("Take screenshot: {0}", screenshotName));

			    int resWidth = 1024; 
			    int resHeight = 768;
			    RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
			    Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);

			    Camera.main.targetTexture = rt;
			    Camera.main.Render();

			    RenderTexture.active = rt;

			    screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
			    Camera.main.targetTexture = null;
			    RenderTexture.active = null; // JC: added to avoid errors
			    Destroy(rt);

			    byte[] bytes = screenShot.EncodeToPNG();

			    System.IO.File.WriteAllBytes(screenshotName, bytes);

			    _screenshotCount++;
			    _ready = false;
		    }
	    }

        private void SetupColorVoxelTest()
        {
            _testObject = new GameObject("Test Object");
            ColorVoxelMesh voxelMesh = _testObject.AddComponent<ColorVoxelMesh>();

            int width = 16;
            int height = 16;
            int depth = 16;

            ColorVoxel[,,] voxels = new ColorVoxel[width, height, depth];
            for(int w = 0; w < width; ++w)
            {
                for(int h = 0; h < height; ++h)
                {
                    for(int d = 0; d < depth; ++d)
                    {
                        voxels[w, h, d] = new ColorVoxel(new Color(w / 16.0f, h / 16.0f, d / 16.0f));
                    }
                }
            }
            var voxelData = new ColorVoxelMeshData("Test", width, height, depth, 1f, voxels);
            voxelMesh.SetVoxelData(voxelData);
        }

        public void SetupTextureVoxelTest()
        {
            _testObject = new GameObject("Test Object");
            _testObject.transform.localScale = new Vector3(8.0f, 8.0f, 8.0f);
            TextureVoxelMesh voxelMesh = _testObject.AddComponent<TextureVoxelMesh>();


            int width = 2;
            int height = 2;
            int depth = 2;

            TextureVoxel[,,] voxels = new TextureVoxel[width, height, depth];
            for(int w = 0; w < width; ++w)
            {
                for(int h = 0; h < height; ++h)
                {
                    for(int d = 0; d < depth; ++d)
                    {
                        if(w == d && h == d)
                        {
                            voxels[w, h, d] = new TextureVoxel(0, 1);
                        }
                        else
                        {
                            voxels[w, h, d] = new TextureVoxel(0);
                        }

                    }
                }
            }

            var voxelData = new TextureVoxelMeshData("Test", width, height, depth, 1f, voxels);
            voxelMesh.SetVoxelData(voxelData);
        }
    
        private void GetNextTest()
        {
            if(_testObject != null)
            {
                DestroyImmediate(_testObject);
            }

            _currentTest = _testsToRunList.Dequeue();
            
            switch(_currentTest)
            {
                case "ColorVoxelMesh":
                    SetupColorVoxelTest();
                    break;
                case "TextureVoxelMesh":
                    SetupTextureVoxelTest();
                    break;
                default:
                    Debug.LogError("Test not found");
                    break;
            }
        }

        private void ResetTest()
        {
            _screenshotCount = 0;
            _ready = true;
            _timeAcc = 0.0f;
        }
    }
}
