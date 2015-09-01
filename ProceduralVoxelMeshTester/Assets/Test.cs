using System.Security.Policy;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using ProceduralVoxelMesh;
using Debug = UnityEngine.Debug;


namespace Assets
{
    public class Test : MonoBehaviour
    {
        private GameObject _testObject;
        private int _screenshotCount;
        private bool _ready;
        private float _timeAcc;

        public void Start()
        {
            _testObject = new GameObject("Test Object");
            VoxelMesh voxelMesh = _testObject.AddComponent<VoxelMesh>();

            Voxel[,,] voxels = new Voxel[16,16,16];
            for(int w = 0; w < 16; ++w)
            {
                for(int h = 0; h < 16; ++h)
                {
                    for(int d = 0; d < 16; ++d)
                    {
                        voxels[w,h,d].Color = new Color(w / 16.0f, h / 16.0f, d / 16.0f);
                    }
                }
            }
            voxelMesh.SetVoxels(voxels);

            _screenshotCount = 0;
            _ready = true;
            _timeAcc = 0.0f;
        }

        public void FixedUpdate()
        {
            _timeAcc += Time.deltaTime;
            
            if(_ready && _timeAcc >= 1.0f)
            {
                Debug.Log("Take screenshot: " + _screenshotCount.ToString() + ".png");
                Application.CaptureScreenshot(_screenshotCount.ToString() + ".png");
                _screenshotCount++;
                _ready = false;
            }
        }

        public void Update()
        {
            if(_screenshotCount == 6)
            {
#if UNITY_EDITOR
                EditorApplication.ExecuteMenuItem("Edit/Play");
#else
                Application.Quit();     
#endif
                return;
            }

            if(_timeAcc < 2.0f)
            {
                return;
            }


            if(!_ready)
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
    }

}
