using AssetRipper.Export.Configuration;
using AssetRipper.IO.Files;
using AssetRipper.Import.Logging;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace AssetRipper.Export.UnityProjects.PostExport;

public class CompareExportsPostExporter : IPostExporter
{
	public void DoPostExport(GameData gameData, FullConfiguration options, FileSystem fileSystem)
	{
		var settings = options.ExportSettings;
		if (!settings.CompareWithPreviousExport || string.IsNullOrEmpty(settings.PreviousExportPath))
		{
			return;
		}

		Logger.Info(LogCategory.Export, "========================================");
		Logger.Info(LogCategory.Export, "Comparing exports...");
		Logger.Info(LogCategory.Export, "========================================");

		string currentExportPath = options.ProjectRootPath;
		string previousExportPath = settings.PreviousExportPath;
		string unreleasedFolder = Path.Combine(Path.GetDirectoryName(currentExportPath) ?? "", settings.UnreleasedFolderName);

		if (!Directory.Exists(previousExportPath))
		{
			Logger.Warning(LogCategory.Export, "Previous export path does not exist: " + previousExportPath);
			return;
		}

		// Build hash maps for both exports
		Dictionary<string, string> previousHashes = BuildFileHashMap(previousExportPath);
		Dictionary<string, string> currentHashes = BuildFileHashMap(currentExportPath);

		Logger.Info(LogCategory.Export, "Previous export: " + previousHashes.Count + " files");
		Logger.Info(LogCategory.Export, "Current export: " + currentHashes.Count + " files");

		// Find new files (in current but not in previous)
		List<string> newFiles = new List<string>();
		foreach (var kvp in currentHashes)
		{
			if (!previousHashes.ContainsKey(kvp.Key))
			{
				newFiles.Add(kvp.Key);
			}
			else if (previousHashes[kvp.Key] != kvp.Value)
			{
				// File exists but content is different
				newFiles.Add(kvp.Key);
			}
		}

		Logger.Info(LogCategory.Export, "Found " + newFiles.Count + " new/changed files");

		if (newFiles.Count > 0)
		{
			// Create unreleased folder
			if (Directory.Exists(unreleasedFolder))
			{
				Directory.Delete(unreleasedFolder, true);
			}
			Directory.CreateDirectory(unreleasedFolder);

			// Copy new files to unreleased folder, maintaining folder structure
			int copiedCount = 0;
			foreach (string relativePath in newFiles)
			{
				string sourceFile = Path.Combine(currentExportPath, relativePath);
				string destFile = Path.Combine(unreleasedFolder, relativePath);

				if (File.Exists(sourceFile))
				{
					string destDir = Path.GetDirectoryName(destFile);
					if (!Directory.Exists(destDir))
					{
						Directory.CreateDirectory(destDir);
					}

					File.Copy(sourceFile, destFile, true);
					copiedCount++;
					Logger.Info(LogCategory.Export, "  NEW: " + relativePath);
				}
			}

			Logger.Info(LogCategory.Export, "========================================");
			Logger.Info(LogCategory.Export, "Created Unreleased folder with " + copiedCount + " new/changed files");
			Logger.Info(LogCategory.Export, "Location: " + unreleasedFolder);
			Logger.Info(LogCategory.Export, "========================================");
		}
		else
		{
			Logger.Info(LogCategory.Export, "No new files found - exports are identical");
		}
	}

	private Dictionary<string, string> BuildFileHashMap(string exportPath)
	{
		Dictionary<string, string> hashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		if (!Directory.Exists(exportPath))
		{
			return hashes;
		}

		// Get all files recursively
		string[] allFiles = Directory.GetFiles(exportPath, "*", SearchOption.AllDirectories);

		foreach (string filePath in allFiles)
		{
			// Get relative path from export root
			string relativePath = Path.GetRelativePath(exportPath, filePath);

			// Skip .meta files and settings files
			if (relativePath.EndsWith(".meta") || relativePath.Contains("ProjectSettings"))
			{
				continue;
			}

			try
			{
				// Compute hash for this file
				string hash = ComputeFileHash(filePath);
				hashes[relativePath] = hash;
			}
			catch (Exception ex)
			{
				Logger.Warning(LogCategory.Export, "Could not hash file " + relativePath + ": " + ex.Message);
			}
		}

		return hashes;
	}

	private string ComputeFileHash(string filePath)
	{
		using (var md5 = MD5.Create())
		{
			using (var stream = File.OpenRead(filePath))
			{
				byte[] hashBytes = md5.ComputeHash(stream);
				return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
			}
		}
	}

	public string Name
	{
		get { return "Compare Exports Post Exporter"; }
	}
}