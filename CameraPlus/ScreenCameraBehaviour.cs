using System;
using UnityEngine;

namespace CameraPlus
{
    /// <summary>
    /// This is the monobehaviour that goes on the camera that handles
    /// displaying the actual feed from the camera to the screen.
    /// </summary>
    public class ScreenCameraBehaviour : MonoBehaviour
    {
        private Camera _cam;
        private RenderTexture _renderTexture;
        static public Shader shader;
        public Material mat;
        public bool isMainCamera;
        public Config Config;

        public void SetRenderTexture(RenderTexture renderTexture)
        {
            _renderTexture = renderTexture;
        }

        public void SetCameraInfo(Vector2 position, Vector2 size, int layer)
        {
            _cam.pixelRect = new Rect(position, size);
            _cam.depth = layer;
        }

        public void Awake()
        {
            Logger.Log("Created new screen camera behaviour component!");
            DontDestroyOnLoad(gameObject);

            _cam = gameObject.AddComponent<Camera>();
            _cam.clearFlags = CameraClearFlags.SolidColor;
            _cam.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            _cam.cullingMask = 0;
            _cam.stereoTargetEye = StereoTargetEyeMask.None;

            if (!shader)
            {
                try {
                    shader = AssetBundle.LoadFromFile("UserData/CameraPlus/shader.assetbundle").LoadAsset("assets/alphablit.shader") as Shader;
                }
                catch (Exception e)
                {
                    Logger.Log(e.ToString());
                }            
            }

            try
            {
                mat = new Material(shader);
                Logger.Log($"Load AlphaBlit shader successed.");
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
                Logger.Log($"Cannot load AlphaBlit shader.");
            }
         
        }
        
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (_renderTexture == null) return;

            if (isMainCamera || Config.avatar < 2)
            {
                Graphics.Blit(_renderTexture, dest);
            }
            else
            {
                mat.SetTexture("_MainTex", _renderTexture);
                mat.SetTexture("_SecondTex", src);
                Graphics.Blit(_renderTexture, dest, mat);
            }
        }
    }
}
