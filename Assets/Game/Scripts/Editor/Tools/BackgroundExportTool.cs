
using Sirenix.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

public class BackgroundExportTool
{
    private const string BACK_MASK_NAME = "Background";

    private class DevCamera
    {
        private Camera camera;
        private int width;
        private int height;

        public DevCamera(int width, int height)
        {
            camera = GameObject.FindObjectOfType<Camera>();

            this.width = width;
            this.height = height;

            RenderTexture targetTexture = RenderTexture.GetTemporary(width, height);
            camera.targetTexture = targetTexture;

            RenderTexture.active = targetTexture;
        }

        public byte[] Render(int cullingMask, bool isTransparent = false)
        {
            Color color = camera.backgroundColor;
            color.a = isTransparent ? 0 : 1;
            camera.backgroundColor = color;

            camera.cullingMask = cullingMask;

            Texture2D texture = new Texture2D(width, height);
            camera.Render();

            var rect = new Rect(0, 0, width, height);
            texture.ReadPixels(rect, 0, 0);
            texture.Apply();

            return texture.EncodeToPNG();
        }

        public void Clear()
		{
            camera.targetTexture = null;
		}
    }

    [MenuItem("Tools/Take Screenshot")]
    public static void ExportDevBackground()
	{
        var path = "Screenshots/";
        var scene = EditorSceneManager.GetActiveScene();
        var scenePath = scene.path.Split('/', '\\');
        Array.Resize(ref scenePath, scenePath.Length - 1);
        string p = "";
        scenePath.ForEach((x) => p += x + "/");
        path = p  + path;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var devCamera = new DevCamera((int)Handles.GetMainGameViewSize().x, (int)Handles.GetMainGameViewSize().y);

        var bytes = devCamera.Render(-1, true);

        FileStream fileStream = File.Create($"{path}screenshot_{Directory.GetFiles(path).Length}.png");
        fileStream.Write(bytes, 0, bytes.Length);
        fileStream.Close();

        AssetDatabase.Refresh();

        devCamera.Clear();
    }
}