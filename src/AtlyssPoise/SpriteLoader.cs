using System.IO;
using UnityEngine;

namespace AtlyssPoise
{
    public static class SpriteLoader
    {
        public static Sprite LoadSpriteFromFile(string filename, int pixelsPerUnit = 100)
        {
            // Get the folder the .dll is in
            string pluginFolder = Path.GetDirectoryName(typeof(SpriteLoader).Assembly.Location);
            string fullPath = Path.Combine(pluginFolder, filename);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"[Poise] Sprite not found at path: {fullPath}");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(fullPath);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(fileData);
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        }
    }
}