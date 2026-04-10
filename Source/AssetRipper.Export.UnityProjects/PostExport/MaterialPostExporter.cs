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
		var settings = options.ExportSettings;
		if (!settings.ReconnectTextures && !settings.RemapToStandardShaders)
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

		// Get all textures first for better matching
		var textureMap = BuildTextureMap(exportPath);

		// Process all materials
		foreach (string matFile in Directory.GetFiles(exportPath, "*.mat", SearchOption.AllDirectories))
		{
			try
			{
				if (FixMaterial(matFile, settings, exportPath, textureMap))
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

	private Dictionary<string, string> BuildTextureMap(string exportPath)
	{
		var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		
		// Collect all texture files
		foreach (string texFile in Directory.GetFiles(exportPath, "*.png", SearchOption.AllDirectories))
		{
			string name = Path.GetFileNameWithoutExtension(texFile);
			map[name] = texFile;
		}
		foreach (string texFile in Directory.GetFiles(exportPath, "*.tga", SearchOption.AllDirectories))
		{
			string name = Path.GetFileNameWithoutExtension(texFile);
			map[name] = texFile;
		}
		foreach (string texFile in Directory.GetFiles(exportPath, "*.jpg", SearchOption.AllDirectories))
		{
			string name = Path.GetFileNameWithoutExtension(texFile);
			map[name] = texFile;
		}

		Logger.Info(LogCategory.Export, $"Found {map.Count} texture files");
		return map;
	}

	private bool FixMaterial(string matFile, ExportSettings settings, string exportPath, Dictionary<string, string> textureMap)
	{
		string[] lines = File.ReadAllLines(matFile);
		bool modified = false;
		string materialName = Path.GetFileNameWithoutExtension(matFile);
		
		Logger.Info(LogCategory.Export, $"Processing material: {materialName}");

		// Step 1: Remap to Standard shader
		if (settings.RemapToStandardShaders)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].Contains("m_Shader:"))
				{
					lines[i] = "  m_Shader: {fileID: 46, guid: 0000000000000000f000000000000000, type: 0}";
					modified = true;
					Logger.Info(LogCategory.Export, $"  Set to Standard shader");
					break;
				}
			}
		}

		// Step 2: Reconnect textures to _BaseMap
		if (settings.ReconnectTextures)
		{
			// Find matching texture
			string matchedTex = FindMatchingTexture(materialName, textureMap);
			
			if (!string.IsNullOrEmpty(matchedTex))
			{
				Logger.Info(LogCategory.Export, $"  Found texture: {matchedTex}");
				modified = SetBaseMapTexture(lines, matchedTex) || modified;
			}
			else
			{
				Logger.Warning(LogCategory.Export, $"  No matching texture found for {materialName}");
			}
		}

		if (modified)
		{
			File.WriteAllLines(matFile, lines);
		}

		return modified;
	}

	private string FindMatchingTexture(string materialName, Dictionary<string, string> textureMap)
	{
		// Try exact match
		if (textureMap.TryGetValue(materialName, out string tex))
			return tex;

		// Try without common prefixes/suffixes
		string[] variations = new[]
		{
			materialName.Replace("_Mat", "").Replace("_Material", ""),
			materialName.Replace("Mat_", "").Replace("Material_", ""),
			materialName.Replace("_diffuse", "").Replace("_albedo", ""),
			materialName.Replace("Material", ""),
			materialName.Replace("mat", ""),
		};

		foreach (var v in variations)
		{
			if (textureMap.TryGetValue(v, out tex))
				return tex;
			if (textureMap.TryGetValue(v.Trim(), out tex))
				return tex;
		}

		// Try partial match
		string lowerName = materialName.ToLower();
		foreach (var kvp in textureMap)
		{
			if (kvp.Key.ToLower().Contains(lowerName) || lowerName.Contains(kvp.Key.ToLower()))
			{
				if (!kvp.Key.Contains("normal") && !kvp.Key.Contains("spec") && !kvp.Key.Contains("mask"))
					return kvp.Value;
			}
		}

		return "";
	}

	private bool SetBaseMapTexture(string[] lines, string texturePath)
	{
		bool modified = false;
		string guid = GenerateGuidFromPath(texturePath);
		string texRef = $"  m_Texture: {{fileID: 2800000, guid: {guid}, type: 3}}";

		// Find and update _BaseMap property
		for (int i = 0; i < lines.Length; i++)
		{
			if (lines[i].Contains("_BaseMap"))
			{
				Logger.Info(LogCategory.Export, $"    Setting _BaseMap texture...");
				// Find the m_Texture line within this property block
				for (int j = i + 1; j < Math.Min(i + 15, lines.Length); j++)
				{
					if (lines[j].Contains("m_Texture:"))
					{
						lines[j] = texRef;
						modified = true;
						break;
					}
					// Stop if we hit another property
					if (lines[j].Contains("--- !u!") || (lines[j].Contains("m_") && !lines[j].Contains("m_SavedProperties")))
						break;
				}
				break;
			}
		}

		// Also try _MainTex if _BaseMap not found
		if (!modified)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].Contains("_MainTex"))
				{
					for (int j = i + 1; j < Math.Min(i + 15, lines.Length); j++)
					{
						if (lines[j].Contains("m_Texture:"))
						{
							lines[j] = texRef;
							modified = true;
							break;
						}
					}
					break;
				}
			}
		}

		// Last resort: find ANY texture slot and set it
		if (!modified)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].Contains("m_Texture:") && !lines[i].Contains("{fileID: 0}"))
				{
					lines[i] = texRef;
					modified = true;
					break;
				}
			}
		}

		return modified;
	}

	private string GenerateGuidFromPath(string path)
	{
		// Generate a deterministic GUID based on the file path
		string normalizedPath = path.Replace("\\", "/").ToLower();
		
		// Simple hash - in production you'd want something more robust
		int hash = normalizedPath.GetHashCode();
		uint uHash = (uint)Math.Abs(hash);
		
		return uHash.ToString("x8") + "0000000000000000";
	}

	public string Name => "Material Post Exporter";
}