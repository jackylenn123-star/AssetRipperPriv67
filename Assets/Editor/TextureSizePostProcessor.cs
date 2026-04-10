using UnityEngine;
using UnityEditor;

namespace AssetRipperFixes
{
    // Auto-fix large textures when imported
    public class TextureSizePostProcessor : AssetPostprocessor
    {
        void OnPostprocessTexture(Texture2D texture)
        {
            if (texture.width > 4096 || texture.height > 4096)
            {
                TextureImporter importer = assetImporter as TextureImporter;
                if (importer != null)
                {
                    importer.maxTextureSize = 8192;
                    importer.SaveAndReimport();
                    Debug.Log($"[AutoFix] Set max texture size to 8192 for {assetPath} ({texture.width}x{texture.height})");
                }
            }
        }
    }
    
    // Menu item to fix all textures manually
    public class FixTexturesMenu
    {
        [MenuItem("Assets/Fix Large Textures")]
        public static void FixAllTextures()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture");
            int fixedCount = 0;
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                
                if (importer != null)
                {
                    TextureImporterTextureSettings settings = new TextureImporterTextureSettings();
                    importer.ReadTextureSettings(settings);
                    
                    // Check actual texture size
                    Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    if (tex != null && (tex.width > 4096 || tex.height > 4096))
                    {
                        importer.maxTextureSize = 8192;
                        importer.SaveAndReimport();
                        Debug.Log($"Fixed: {path} ({tex.width}x{tex.height})");
                        fixedCount++;
                    }
                }
            }
            
            Debug.Log($"Fixed {fixedCount} large textures");
        }
    }
}