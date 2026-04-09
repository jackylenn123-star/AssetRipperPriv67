using AssetRipper.Export.Configuration;
using AssetRipper.IO.Files;
using AssetRipper.Import.Logging;
using System.IO;
using System.Text.RegularExpressions;

namespace AssetRipper.Export.UnityProjects.PostExport;

public class MaterialPostExporter : IPostExporter
{
	public void DoPostExport(GameData gameData, FullConfiguration options, FileSystem fileSystem)
	{
		if (!options.ExportSettings.ReconnectTextures && 
			!options.ExportSettings.RemapToStandardShaders)
		{
			return;
		}

		Logger.Info(LogCategory.Export, "Post-processing materials...");

		string exportPath = options.ProjectRootPath;
		if (!Directory.Exists(exportPath))
		{
			return;
		}

		int fixedCount = 0;
		
		// Step 1: Slice sprite atlases if enabled
		if (options.ExportSettings.ReconnectTextures)
		{
			SliceAtlases(exportPath, (ExportSettings)options);
		}

		// Step 2: Fix materials
		foreach (string matFile in Directory.GetFiles(exportPath, "*.mat", SearchOption.AllDirectories))
		{
			try
			{
				if (FixMaterial(matFile, (ExportSettings)options, exportPath))
				{
					fixedCount++;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning(LogCategory.Export, $"Failed to fix material {matFile}: {ex.Message}");
			}
		}

		Logger.Info(LogCategory.Export, $"Material post-processing complete. Fixed {fixedCount} materials.");
	}

	private void SliceAtlases(string exportPath, ExportSettings settings)
	{
		// Find all sprite atlas files and try to extract individual sprites
		string[] atlasFiles = Directory.GetFiles(exportPath, "*.spriteatlas", SearchOption.AllDirectories);
		
		foreach (string atlasFile in atlasFiles)
		{
			Logger.Info(LogCategory.Export, $"Processing atlas: {atlasFile}");
			// Atlas slicing logic would go here
			// This requires parsing the atlas file and extracting texture slices
		}

		// Also look for large textures that might be atlases
		string[] textureFiles = Directory.GetFiles(exportPath, "*.png", SearchOption.AllDirectories);
		foreach (string texFile in textureFiles)
		{
			FileInfo fi = new FileInfo(texFile);
			// If texture is large (e.g., 2048x2048 or bigger), it might be an atlas
			if (fi.Length > 1000000) // > 1MB
			{
				Logger.Info(LogCategory.Export, $"Potential atlas found: {texFile} ({fi.Length} bytes)");
				// Could attempt to slice this into smaller pieces
			}
		}
	}

	private bool FixMaterial(string matFile, ExportSettings settings, string exportPath)
	{
		string[] lines = File.ReadAllLines(matFile);
		bool modified = false;
		string materialName = Path.GetFileNameWithoutExtension(matFile);
		string materialDir = Path.GetDirectoryName(matFile) ?? "";

		// Step 1: Remap to Standard shader
		if (settings.RemapToStandardShaders)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].Contains("m_Shader:"))
				{
					lines[i] = "  m_Shader: {fileID: 46, guid: 0000000000000000f000000000000000, type: 0}";
					modified = true;
					break;
				}
			}
		}

		// Step 2: Reconnect textures and set BaseMap to atlas slice
		if (settings.ReconnectTextures)
		{
			// Find the material's folder (look in Materials subfolder)
			string materialsFolder = Path.Combine(exportPath, "Assets", "Materials");
			if (Directory.Exists(materialsFolder))
			{
				string matSpecificFolder = Path.Combine(materialsFolder, materialName);
				if (Directory.Exists(matSpecificFolder))
				{
					materialDir = matSpecificFolder;
				}
			}

			// Try to find a matching texture in the material's folder
			string texturePath = FindMaterialTexture(materialDir, materialName);
			
			if (!string.IsNullOrEmpty(texturePath))
			{
				// Update _BaseMap to reference the texture
				modified = SetBaseMapTexture(lines, texturePath) || modified;
			}
			else
			{
				// Fallback: look in common locations
				texturePath = FindMaterialTextureFallback(exportPath, materialName);
				if (!string.IsNullOrEmpty(texturePath))
				{
					modified = SetBaseMapTexture(lines, texturePath) || modified;
				}
			}
		}

		if (modified)
		{
			File.WriteAllLines(matFile, lines);
		}

		return modified;
	}

	private string FindMaterialTexture(string materialDir, string materialName)
	{
		if (!Directory.Exists(materialDir))
			return "";

		// Look for texture with same name as material
		string[] extensions = { ".png", ".tga", ".jpg", ".jpeg" };
		foreach (string ext in extensions)
		{
			string texPath = Path.Combine(materialDir, materialName + ext);
			if (File.Exists(texPath))
				return texPath;
		}

		// Look for any texture in the folder
		foreach (string ext in extensions)
		{
			string[] files = Directory.GetFiles(materialDir, "*" + ext);
			if (files.Length > 0)
				return files[0];
		}

		return "";
	}

	private string FindMaterialTextureFallback(string exportPath, string materialName)
	{
		// Look in Textures folder
		string texturesFolder = Path.Combine(exportPath, "Assets", "Textures");
		if (!Directory.Exists(texturesFolder))
			texturesFolder = Path.Combine(exportPath, "Textures");

		if (Directory.Exists(texturesFolder))
		{
			string[] extensions = { ".png", ".tga", ".jpg" };
			foreach (string ext in extensions)
			{
				// Try material name
				string texPath = Path.Combine(texturesFolder, materialName + ext);
				if (File.Exists(texPath))
					return texPath;

				// Try removing common prefixes/suffixes
				string searchName = materialName.Replace("Mat_", "").Replace("_Material", "");
				texPath = Path.Combine(texturesFolder, searchName + ext);
				if (File.Exists(texPath))
					return texPath;
			}
		}

		return "";
	}

	private bool SetBaseMapTexture(string[] lines, string texturePath)
	{
		bool modified = false;
		string guid = GenerateGuidFromPath(texturePath);
		string texRef = $"  m_Texture: {{fileID: 2800000, guid: {guid}, type: 3}}";

		for (int i = 0; i < lines.Length; i++)
		{
			// Find _BaseMap or _MainTex properties and update them
			if (lines[i].Contains("_BaseMap") || lines[i].Contains("_MainTex"))
			{
				// Look for the m_Texture line in the next few lines
				for (int j = i; j < Math.Min(i + 10, lines.Length); j++)
				{
					if (lines[j].Contains("m_Texture:"))
					{
						lines[j] = texRef;
						modified = true;
						break;
					}
				}
			}
		}

		return modified;
	}

	private string GenerateGuidFromPath(string path)
	{
		string normalizedPath = path.Replace("\\", "/").ToLower();
		int hash = normalizedPath.GetHashCode();
		return Math.Abs(hash).ToString("x8") + "0000000000000000";
	}

	public string Name => "Material Post Exporter";
}