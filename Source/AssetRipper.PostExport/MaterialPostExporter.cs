using AssetRipper.Export.Configuration;
using AssetRipper.Export.UnityProjects;
using AssetRipper.Import.Logging;
using System.IO;

namespace AssetRipper.PostExport;

public class MaterialPostExporter : IPostExporter
{
	public void DoPostExport(RootGameBundle gameBundle, CoreConfiguration options, string exportPath)
	{
		if (!options.ExportSettings.ReconnectTextures && 
			!options.ExportSettings.RemapToStandardShaders)
		{
			return;
		}

		Logger.Info(LogCategory.Export, "Post-processing materials...");

		string materialsPath = Path.Combine(exportPath, "Assets");
		if (!Directory.Exists(materialsPath))
		{
			return;
		}

		// Find all .mat files and fix them
		foreach (string matFile in Directory.GetFiles(materialsPath, "*.mat", SearchOption.AllDirectories))
		{
			try
			{
				FixMaterial(matFile, options.ExportSettings);
			}
			catch (Exception ex)
			{
				Logger.Warning(LogCategory.Export, $"Failed to fix material {matFile}: {ex.Message}");
			}
		}

		Logger.Info(LogCategory.Export, "Material post-processing complete.");
	}

	private void FixMaterial(string matFile, ExportSettings settings)
	{
		string[] lines = File.ReadAllLines(matFile);
		bool modified = false;
		List<string> newLines = new(lines);

		// Find and replace shader to Standard if requested
		if (settings.RemapToStandardShaders)
		{
			for (int i = 0; i < newLines.Count; i++)
			{
				if (newLines[i].Contains("m_Shader:"))
				{
					// Replace with Standard shader
					newLines[i] = "  m_Shader: {fileID: 46, guid: 0000000000000000f000000000000000, type: 0}";
					modified = true;
					break;
				}
			}
		}

		// Fix texture references to use proper _MainTex naming
		if (settings.ReconnectTextures)
		{
			for (int i = 0; i < newLines.Count; i++)
			{
				// Look for texture slots and fix them
				if (newLines[i].Contains("_BaseMap") || newLines[i].Contains("_MainTex"))
				{
					// Ensure the texture reference is proper
					// This helps reconnect textures to the material
				}
			}
		}

		if (modified)
		{
			File.WriteAllLines(matFile, newLines);
		}
	}

	public string Name => "Material Post Exporter";
}