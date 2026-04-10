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

		Logger.Info(LogCategory.Export, "========================================");
		Logger.Info(LogCategory.Export, "Post-processing materials...");
		Logger.Info(LogCategory.Export, "========================================");

		string exportPath = options.ProjectRootPath;
		if (!Directory.Exists(exportPath))
		{
			return;
		}

		int fixedCount = 0;

		// Get all textures first for better matching
		var textureMap = BuildTextureMap(exportPath);
		var spriteAtlasMap = BuildSpriteAtlasMap(exportPath);

		Logger.Info(LogCategory.Export, $"Found {textureMap.Count} textures and {spriteAtlasMap.Count} sprite atlases");

		// Process all materials
		foreach (string matFile in Directory.GetFiles(exportPath, "*.mat", SearchOption.AllDirectories))
		{
			try
			{
				if (FixMaterial(matFile, settings, exportPath, textureMap, spriteAtlasMap))
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
		
		foreach (string texFile in Directory.GetFiles(exportPath, "*.png", SearchOption.AllDirectories))
		{
			string name = Path.GetFileNameWithoutExtension(texFile);
			// Skip very large "atlas" textures (likely 10000+ pixels in one dimension)
			try
			{
				using var img = System.Drawing.Image.FromFile(texFile);
				if (img.Width > 8000 || img.Height > 8000)
				{
					Logger.Info(LogCategory.Export, $"Skipping large texture (possible atlas): {name} ({img.Width}x{img.Height})");
					continue; // Skip atlases
				}
			}
			catch { }
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

		return map;
	}

	private Dictionary<string, List<SpriteRect>> BuildSpriteAtlasMap(string exportPath)
	{
		var map = new Dictionary<string, List<SpriteRect>>(StringComparer.OrdinalIgnoreCase);
		
		// Look for .meta files that might contain sprite sheet data
		foreach (string metaFile in Directory.GetFiles(exportPath, "*.png.meta", SearchOption.AllDirectories))
		{
			string texPath = metaFile.Replace(".meta", "");
			if (!File.Exists(texPath)) continue;
			
			try
			{
				string[] metaLines = File.ReadAllLines(metaFile);
				List<SpriteRect> sprites = new();
				
				for (int i = 0; i < metaLines.Length; i++)
				{
					if (metaLines[i].Contains("spriteMetaData:"))
					{
						// Extract sprite rect from meta
						SpriteRect? rect = ParseSpriteRect(metaLines, i);
						if (rect.HasValue)
						{
							sprites.Add(rect.Value);
						}
					}
				}
				
				if (sprites.Count > 0)
				{
					string name = Path.GetFileNameWithoutExtension(texPath);
					map[name] = sprites;
					Logger.Info(LogCategory.Export, $"Found {sprites.Count} sprites in {name}");
				}
			}
			catch (Exception ex)
			{
				Logger.Warning(LogCategory.Export, $"Error parsing meta for {metaFile}: {ex.Message}");
			}
		}
		
		return map;
	}

	private SpriteRect? ParseSpriteRect(string[] lines, int startIndex)
	{
		// Look for sprite rect data in the meta file
		int x = 0, y = 0, w = 0, h = 0;
		string name = "";
		
		for (int i = startIndex; i < Math.Min(startIndex + 50, lines.Length); i++)
		{
			if (lines[i].Contains("rect:"))
			{
				// Try to extract rect values
				var match = Regex.Match(lines[i], @"rect:\s*\{"[^"]+":\s*(\d+),\s*"y":\s*(\d+),\s*"width":\s*(\d+),\s*"height":\s*(\d+)");
				if (match.Success)
				{
					x = int.Parse(match.Groups[1].Value);
					y = int.Parse(match.Groups[2].Value);
					w = int.Parse(match.Groups[3].Value);
					h = int.Parse(match.Groups[4].Value);
				}
			}
			if (lines[i].Contains("name:"))
			{
				var match = Regex.Match(lines[i], @"name:\s*""([^""]+)""");
				if (match.Success)
				{
					name = match.Groups[1].Value;
				}
			}
		}
		
		if (w > 0 && h > 0)
		{
			return new SpriteRect { X = x, Y = y, Width = w, Height = h, Name = name };
		}
		return null;
	}

	private bool FixMaterial(string matFile, ExportSettings settings, string exportPath, 
		Dictionary<string, string> textureMap, Dictionary<string, List<SpriteRect>> spriteAtlasMap)
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
					break;
				}
			}
		}

		// Step 2: Reconnect textures - try sprite atlas first, then regular texture
		if (settings.ReconnectTextures)
		{
			// Try sprite atlas slice first
			bool connected = ConnectSpriteAtlas(matFile, lines, materialName, spriteAtlasMap);
			
			if (!connected)
			{
				// Fallback to regular texture
				string matchedTex = FindMatchingTexture(materialName, textureMap);
				if (!string.IsNullOrEmpty(matchedTex))
				{
					Logger.Info(LogCategory.Export, $"  Using texture: {matchedTex}");
					modified = SetBaseMapTexture(lines, matchedTex) || modified;
				}
				else
				{
					Logger.Warning(LogCategory.Export, $"  No matching texture found for {materialName}");
				}
			}
			else
			{
				modified = true;
			}
		}

		if (modified)
		{
			File.WriteAllLines(matFile, lines);
		}

		return modified;
	}

	private bool ConnectSpriteAtlas(string matFile, string[] lines, string materialName, Dictionary<string, List<SpriteRect>> spriteAtlasMap)
	{
		// Look for a sprite atlas that might match this material
		foreach (var kvp in spriteAtlasMap)
		{
			// Check if material name matches atlas name or sprite name
			if (kvp.Key.Contains(materialName) || materialName.Contains(kvp.Key))
			{
				Logger.Info(LogCategory.Export, $"  Found matching sprite atlas: {kvp.Key} with {kvp.Value.Count} sprites");
				
				// Get first sprite
				var sprite = kvp.Value[0];
				
				// Update the texture to point to the atlas texture file with sprite rect
				// For now, just log - actual slicing would require image processing
				Logger.Info(LogCategory.Export, $"  Sprite: {sprite.Name} at ({sprite.X}, {sprite.Y}) {sprite.Width}x{sprite.Height}");
				
				// TODO: Would need to either:
				// 1. Slice the atlas image and export individual sprites
				// 2. Or keep the atlas and set sprite mode in the material
				return false; // Return false to try regular texture instead
			}
		}
		return false;
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
				if (!kvp.Key.ToLower().Contains("normal") && !kvp.Key.ToLower().Contains("spec"))
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

		return modified;
	}

	private string GenerateGuidFromPath(string path)
	{
		string normalizedPath = path.Replace("\\", "/").ToLower();
		int hash = normalizedPath.GetHashCode();
		uint uHash = (uint)Math.Abs(hash);
		return uHash.ToString("x8") + "0000000000000000";
	}

	public string Name => "Material Post Exporter";

	private struct SpriteRect
	{
		public int X, Y, Width, Height;
		public string Name;
	}
}