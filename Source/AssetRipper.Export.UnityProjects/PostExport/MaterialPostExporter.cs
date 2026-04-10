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
		var textureMap = BuildTextureMap(exportPath);
		var spriteAtlasMap = BuildSpriteAtlasMap(exportPath);

		Logger.Info(LogCategory.Export, "Found " + textureMap.Count + " textures");

		foreach (string matFile in Directory.GetFiles(exportPath, "*.mat", SearchOption.AllDirectories))
		{
			try
			{
				if (FixMaterial(matFile, settings, textureMap, spriteAtlasMap))
				{
					fixedCount++;
				}
			}
			catch (Exception ex)
			{
				Logger.Warning(LogCategory.Export, "Failed to fix material: " + ex.Message);
			}
		}

		Logger.Info(LogCategory.Export, "Fixed " + fixedCount + " materials");
	}

	private Dictionary<string, string> BuildTextureMap(string exportPath)
	{
		Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (string texFile in Directory.GetFiles(exportPath, "*.png", SearchOption.AllDirectories))
		{
			map[Path.GetFileNameWithoutExtension(texFile)] = texFile;
		}
		return map;
	}

	private Dictionary<string, List<SpriteRect>> BuildSpriteAtlasMap(string exportPath)
	{
		Dictionary<string, List<SpriteRect>> map = new Dictionary<string, List<SpriteRect>>(StringComparer.OrdinalIgnoreCase);
		return map;
	}

	private bool FixMaterial(string matFile, ExportSettings settings, Dictionary<string, string> textureMap, Dictionary<string, List<SpriteRect>> spriteAtlasMap)
	{
		string[] lines = File.ReadAllLines(matFile);
		bool modified = false;
		string materialName = Path.GetFileNameWithoutExtension(matFile);

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

		if (settings.ReconnectTextures)
		{
			string matchedTex = FindMatchingTexture(materialName, textureMap);
			if (!string.IsNullOrEmpty(matchedTex))
			{
				modified = SetBaseMapTexture(lines, matchedTex) || modified;
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
		if (textureMap.TryGetValue(materialName, out string tex))
			return tex;

		string clean = materialName.Replace("_Mat", "").Replace("_Material", "").Trim();
		if (textureMap.TryGetValue(clean, out tex))
			return tex;

		string lower = materialName.ToLower();
		foreach (var kvp in textureMap)
		{
			string keyLower = kvp.Key.ToLower();
			if (keyLower.Contains(lower) || lower.Contains(keyLower))
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
		string texRef = "  m_Texture: {fileID: 2800000, guid: " + guid + ", type: 3}";

		for (int i = 0; i < lines.Length; i++)
		{
			if (lines[i].Contains("_BaseMap") || lines[i].Contains("_MainTex"))
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

		return modified;
	}

	private string GenerateGuidFromPath(string path)
	{
		string normalizedPath = path.Replace("\\", "/").ToLower();
		int hash = normalizedPath.GetHashCode();
		uint uHash = (uint)Math.Abs(hash);
		return uHash.ToString("x8") + "0000000000000000";
	}

	public string Name
	{
		get { return "Material Post Exporter"; }
	}

	private class SpriteRect
	{
		public int X, Y, Width, Height;
		public string Name;
	}
}