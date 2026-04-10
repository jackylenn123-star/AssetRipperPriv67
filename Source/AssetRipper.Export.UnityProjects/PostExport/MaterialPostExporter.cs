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

		// Get all textures
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
		
		// Collect all textures (no size checking - include atlases)
		foreach (string texFile in Directory.GetFiles(exportPath, "*.png", SearchOption.AllDirectories))
		{
			map[Path.GetFileNameWithoutExtension(texFile)] = texFile;
		}
		foreach (string texFile in Directory.GetFiles(exportPath, "*.tga", SearchOption.AllDirectories))
		{
			map[Path.GetFileNameWithoutExtension(texFile)] = texFile;
		}
		foreach (string texFile in Directory.GetFiles(exportPath, "*.jpg", SearchOption.AllDirectories))
		{
			map[Path.GetFileNameWithoutExtension(texFile)] = texFile;
		}

		return map;
	}

	private Dictionary<string, List<SpriteRect>> BuildSpriteAtlasMap(string exportPath)
	{
		var map = new Dictionary<string, List<SpriteRect>>(StringComparer.OrdinalIgnoreCase);
		
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
						SpriteRect? rect = ParseSpriteRect(metaLines, i);
						if (rect.HasValue)
						{
							sprites.Add(rect.Value);
						}
					}
				}
				
				if (sprites.Count > 0)
				{
					map[Path.GetFileNameWithoutExtension(texPath)] = sprites;
					Logger.Info(LogCategory.Export, $"Found {sprites.Count} sprites in {Path.GetFileNameWithoutExtension(texPath)}");
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
		int x = 0, y = 0, w = 0, h = 0;
		string name = "";
		
		for (int i = startIndex; i < Math.Min(startIndex + 50, lines.Length); i++)
		{
			if (lines[i].Contains("rect:"))
			{
				Match match = Regex.Match(lines[i], @"rect:\s*\{");
				if (match.Success)
				{
					// Try to find numeric values in nearby lines
					for (int j = i; j < i + 10 && j < lines.Length; j++)
					{
						if (lines[j].Contains("x:")) 
						{
							var m = Regex.Match(lines[j], @"x:\s*(\d+)");
							if (m.Success) x = int.Parse(m.Groups[1].Value);
						}
						if (lines[j].Contains("y:")) 
						{
							var m = Regex.Match(lines[j], @"y:\s*(\d+)");
							if (m.Success) y = int.Parse(m.Groups[1].Value);
						}
						if (lines[j].Contains("width:")) 
						{
							var m = Regex.Match(lines[j], @"width:\s*(\d+)");
							if (m.Success) w = int.Parse(m.Groups[1].Value);
						}
						if (lines[j].Contains("height:")) 
						{
							var m = Regex.Match(lines[j], @"height:\s*(\d+)");
							if (m.Success) h = int.Parse(m.Groups[1].Value);
						}
					}
				}
			}
			if (lines[i].Contains("name:"))
			{
				Match match = Regex.Match(lines[i], @"name:\s*""([^""]+)""");
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

		// Step 2: Reconnect textures
		if (settings.ReconnectTextures)
		{
			// Try sprite atlas first
			bool connected = TryConnectSpriteAtlas(lines, materialName, spriteAtlasMap);
			
			if (!connected)
			{
				string matchedTex = FindMatchingTexture(materialName, textureMap);
				if (!string.IsNullOrEmpty(matchedTex))
				{
					modified = SetBaseMapTexture(lines, matchedTex) || modified;
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

	private bool TryConnectSpriteAtlas(string[] lines, string materialName, Dictionary<string, List<SpriteRect>> spriteAtlasMap)
	{
		foreach (var kvp in spriteAtlasMap)
		{
			if (kvp.Key.Contains(materialName) || materialName.Contains(kvp.Key))
			{
				if (kvp.Value.Count > 0)
				{
					var sprite = kvp.Value[0];
					Logger.Info(LogCategory.Export, $"  Found sprite atlas {kvp.Key} with sprite at ({sprite.X},{sprite.Y}) {sprite.Width}x{sprite.Height}");
				}
				return false; // Let it fall back to regular texture matching
			}
		}
		return false;
	}

	private string FindMatchingTexture(string materialName, Dictionary<string, string> textureMap)
	{
		// Exact match
		if (textureMap.TryGetValue(materialName, out string tex))
			return tex;

		// Variations
		string[] variations = new[]
		{
			materialName.Replace("_Mat", "").Replace("_Material", ""),
			materialName.Replace("Mat_", "").Replace("Material_", ""),
			materialName.Replace("_diffuse", "").Replace("_albedo", ""),
		};

		foreach (var v in variations)
		{
			if (textureMap.TryGetValue(v, out tex))
				return tex;
		}

		// Partial match
		string lowerName = materialName.ToLower();
		foreach (var kvp in textureMap)
		{
			string keyLower = kvp.Key.ToLower();
			if (keyLower.Contains(lowerName) || lowerName.Contains(keyLower))
			{
				if (!keyLower.Contains("normal") && !keyLower.Contains("spec"))
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

		// Try _BaseMap
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

		// Try _MainTex
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