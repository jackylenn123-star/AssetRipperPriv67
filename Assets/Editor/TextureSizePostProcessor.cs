using UnityEngine;
using UnityEditor;

namespace AssetRipperFixes
{
    // Auto-fix textures 4096+ to use 8192 max size (preserves original)
    public class TextureSizePostProcessor : AssetPostprocessor
    {
        void OnPostprocessTexture(Texture2D texture)
        {
            // Only fix if texture is larger than 4096px (but preserve original)
            if (texture.width > 4096 || texture.height > 4096)
            {
                TextureImporter importer = assetImporter as TextureImporter;
                if (importer != null && importer.maxTextureSize < 8192)
                {
                    importer.maxTextureSize = 8192;
                    importer.SaveAndReimport();
                    Debug.Log($"[AutoFix] Set max size to 8192 for {assetPath} ({texture.width}x{texture.height})");
                }
            }
        }
    }
    
    [MenuItem("Assets/Fix Large Textures (4096+ -> 8192)")]
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
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null && (tex.width > 4096 || tex.height > 4096) && importer.maxTextureSize < 8192)
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