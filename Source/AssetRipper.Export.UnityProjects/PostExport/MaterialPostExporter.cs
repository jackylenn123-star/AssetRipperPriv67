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
		string materialsPath = Path.Combine(exportPath, "Assets");
		if (!Directory.Exists(materialsPath))
		{
			return;
		}

		int fixedCount = 0;
		foreach (string matFile in Directory.GetFiles(materialsPath, "*.mat", SearchOption.AllDirectories))
		{
			try
			{
				if (FixMaterial(matFile, options.ExportSettings))
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

	private bool FixMaterial(string matFile, ExportSettings settings)
	{
		string[] lines = File.ReadAllLines(matFile);
		bool modified = false;

		// Find and replace shader to Standard if requested (preserves everything else)
		if (settings.RemapToStandardShaders)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].Contains("m_Shader:"))
				{
					// Replace only the shader line, keep everything else intact
					lines[i] = "  m_Shader: {fileID: 46, guid: 0000000000000000f000000000000000, type: 0}";
					modified = true;
					break;
				}
			}
		}

		// Reconnect texture references
		if (settings.ReconnectTextures)
		{
			modified = FixTextureReferences(matFile, lines, settings) || modified;
		}

		if (modified)
		{
			File.WriteAllLines(matFile, lines);
		}

		return modified;
	}

	private bool FixTextureReferences(string matFile, string[] lines, ExportSettings settings)
	{
		bool modified = false;
		string materialDir = Path.GetDirectoryName(matFile) ?? "";
		string materialName = Path.GetFileNameWithoutExtension(matFile);

		for (int i = 0; i < lines.Length; i++)
		{
			string line = lines[i];

			if (line.Contains("m_Texture:") && !line.Contains("{fileID: 0}"))
			{
				int propNameLine = FindPropertyNameLine(lines, i);
				if (propNameLine >= 0)
				{
					string propName = ExtractPropertyName(lines, propNameLine, i);
					if (!string.IsNullOrEmpty(propName))
					{
						string texturePath = FindMatchingTexture(materialDir, materialName, propName);
						if (!string.IsNullOrEmpty(texturePath))
						{
							lines[i] = UpdateTextureReference(lines[i], texturePath);
							modified = true;
						}
					}
				}
			}
		}

		return modified;
	}

	private int FindPropertyNameLine(string[] lines, int textureRefLine)
	{
		for (int i = Math.Max(0, textureRefLine - 10); i < textureRefLine; i++)
		{
			if (lines[i].Contains("--- !u!") && lines[i].Contains(", serializedVersion:"))
			{
				return i;
			}
		}
		return -1;
	}

	private string ExtractPropertyName(string[] lines, int startLine, int endLine)
	{
		for (int i = startLine; i < endLine; i++)
		{
			if (lines[i].Contains("_MainTex") || lines[i].Contains("_BaseMap") ||
				lines[i].Contains("_Albedo") || lines[i].Contains("_BumpMap") ||
				lines[i].Contains("_Normal") || lines[i].Contains("_Metallic") ||
				lines[i].Contains("_Smoothness") || lines[i].Contains("_Occlusion"))
			{
				Match match = Regex.Match(lines[i], @"(\w+)");
				if (match.Success)
				{
					return match.Groups[1].Value;
				}
			}
		}
		return "";
	}

	private string FindMatchingTexture(string materialDir, string materialName, string propertyName)
	{
		if (!Directory.Exists(materialDir))
			return "";

		string[] patterns = new[]
		{
			$"{materialName}_{propertyName}.png",
			$"{materialName}_{propertyName}.tga",
			$"{propertyName}.png",
			$"Texture_{propertyName}.png"
		};

		foreach (string pattern in patterns)
		{
			string texPath = Path.Combine(materialDir, pattern);
			if (File.Exists(texPath))
				return texPath;
		}

		string parentDir = Directory.GetParent(materialDir)?.FullName;
		if (!string.IsNullOrEmpty(parentDir))
		{
			string texturesDir = Path.Combine(parentDir, "Textures");
			if (Directory.Exists(texturesDir))
			{
				foreach (string pattern in patterns)
				{
					string texPath = Path.Combine(texturesDir, pattern);
					if (File.Exists(texPath))
						return texPath;
				}
			}
		}

		return "";
	}

	private string UpdateTextureReference(string line, string texturePath)
	{
		string guid = GenerateGuidFromPath(texturePath);
		return $"  m_Texture: {{fileID: 2800000, guid: {guid}, type: 3}}";
	}

	private string GenerateGuidFromPath(string path)
	{
		string normalizedPath = path.Replace("\\", "/").ToLower();
		int hash = normalizedPath.GetHashCode();
		return hash.ToString("x8") + "0000000000000000";
	}

	public string Name => "Material Post Exporter";
}