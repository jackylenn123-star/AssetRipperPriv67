// Auto-generated code. Do not modify manually.

using AssetRipper.Export.Configuration;
using AssetRipper.GUI.Web.Pages.Settings.DropDown;
using AssetRipper.Import.Configuration;
using AssetRipper.Processing.Configuration;

namespace AssetRipper.GUI.Web.Pages.Settings;

#nullable enable

partial class SettingsPage
{
	private static void SetProperty(string key, string? value)
	{
		switch (key)
		{
			case nameof(ImportSettings.ScriptContentLevel):
				Configuration.ImportSettings.ScriptContentLevel = TryParseEnum<ScriptContentLevel>(value);
				break;
			case nameof(ImportSettings.StreamingAssetsMode):
				Configuration.ImportSettings.StreamingAssetsMode = TryParseEnum<StreamingAssetsMode>(value);
				break;
			case nameof(ImportSettings.DefaultVersion):
				Configuration.ImportSettings.DefaultVersion = TryParseUnityVersion(value);
				break;
			case nameof(ImportSettings.TargetVersion):
				Configuration.ImportSettings.TargetVersion = TryParseUnityVersion(value);
				break;
			case nameof(ProcessingSettings.BundledAssetsExportMode):
				Configuration.ProcessingSettings.BundledAssetsExportMode = TryParseEnum<BundledAssetsExportMode>(value);
				break;
			case nameof(ExportSettings.AudioExportFormat):
				Configuration.ExportSettings.AudioExportFormat = TryParseEnum<AudioExportFormat>(value);
				break;
			case nameof(ExportSettings.ImageExportFormat):
				Configuration.ExportSettings.ImageExportFormat = TryParseEnum<ImageExportFormat>(value);
				break;
			case nameof(ExportSettings.LightmapTextureExportFormat):
				Configuration.ExportSettings.LightmapTextureExportFormat = TryParseEnum<LightmapTextureExportFormat>(value);
				break;
			case nameof(ExportSettings.ScriptExportMode):
				Configuration.ExportSettings.ScriptExportMode = TryParseEnum<ScriptExportMode>(value);
				break;
			case nameof(ExportSettings.ScriptLanguageVersion):
				Configuration.ExportSettings.ScriptLanguageVersion = TryParseEnum<ScriptLanguageVersion>(value);
				break;
			case nameof(ExportSettings.ShaderExportMode):
				Configuration.ExportSettings.ShaderExportMode = TryParseEnum<ShaderExportMode>(value);
				break;
			case nameof(ExportSettings.SpriteExportMode):
				Configuration.ExportSettings.SpriteExportMode = TryParseEnum<SpriteExportMode>(value);
				break;
			case nameof(ExportSettings.TextExportMode):
				Configuration.ExportSettings.TextExportMode = TryParseEnum<TextExportMode>(value);
				break;
			case nameof(ExportSettings.LanguageCode):
				Configuration.ExportSettings.LanguageCode = value;
				break;
			// New Settings
			case nameof(ExportSettings.MaxTextureSize):
				if (int.TryParse(value, out int maxTextureSize))
					Configuration.ExportSettings.MaxTextureSize = maxTextureSize;
				break;
			case nameof(ExportSettings.TextureDecodeQuality):
				Configuration.ExportSettings.TextureDecodeQuality = TryParseEnum<TextureDecodeQuality>(value);
				break;
			case nameof(ExportSettings.TextureDecodeType):
				Configuration.ExportSettings.TextureDecodeType = TryParseEnum<TextureDecodeType>(value);
				break;
			case nameof(ExportSettings.TextureExportFormat):
				Configuration.ExportSettings.TextureExportFormat = TryParseEnum<TextureEncodingFormat>(value);
				break;
			case nameof(ExportSettings.SpriteMeshType):
				Configuration.ExportSettings.SpriteMeshType = TryParseEnum<ExportSpriteMeshType>(value);
				break;
			case nameof(ExportSettings.JpegQuality):
				if (int.TryParse(value, out int jpegQuality))
					Configuration.ExportSettings.JpegQuality = jpegQuality;
				break;
			case nameof(ExportSettings.PngCompressionLevel):
				if (int.TryParse(value, out int pngCompression))
					Configuration.ExportSettings.PngCompressionLevel = pngCompression;
				break;
			case nameof(ExportSettings.ExportThreads):
				if (int.TryParse(value, out int exportThreads))
					Configuration.ExportSettings.ExportThreads = exportThreads;
				break;
			case nameof(ExportSettings.MemoryLimitMb):
				if (int.TryParse(value, out int memoryLimit))
					Configuration.ExportSettings.MemoryLimitMb = memoryLimit;
				break;
			case nameof(ExportSettings.UnityProjectVersion):
				Configuration.ExportSettings.UnityProjectVersion = value ?? "2022.3.0";
				break;
			case nameof(ExportSettings.PreviousExportPath):
				Configuration.ExportSettings.PreviousExportPath = value ?? "";
				break;
			case nameof(ExportSettings.UnityProjectName):
				Configuration.ExportSettings.UnityProjectName = value ?? "ExportedProject";
				break;
			case nameof(ExportSettings.UnreleasedFolderName):
				Configuration.ExportSettings.UnreleasedFolderName = value ?? "Unreleased";
				break;
			case nameof(ExportSettings.AudioSampleRate):
				if (int.TryParse(value, out int audioSampleRate))
					Configuration.ExportSettings.AudioSampleRate = audioSampleRate;
				break;
		}
	}

	private static readonly Dictionary<string, Action<bool>> booleanProperties = new()
	{
		{ nameof(ImportSettings.IgnoreStreamingAssets), (value) => { Configuration.ImportSettings.IgnoreStreamingAssets = value; } },
		{ nameof(ProcessingSettings.EnablePrefabOutlining), (value) => { Configuration.ProcessingSettings.EnablePrefabOutlining = value; } },
		{ nameof(ProcessingSettings.EnableStaticMeshSeparation), (value) => { Configuration.ProcessingSettings.EnableStaticMeshSeparation = value; } },
		{ nameof(ProcessingSettings.EnableAssetDeduplication), (value) => { Configuration.ProcessingSettings.EnableAssetDeduplication = value; } },
		{ nameof(ProcessingSettings.RemoveNullableAttributes), (value) => { Configuration.ProcessingSettings.RemoveNullableAttributes = value; } },
		{ nameof(ProcessingSettings.PublicizeAssemblies), (value) => { Configuration.ProcessingSettings.PublicizeAssemblies = value; } },
		{ nameof(ExportSettings.ScriptTypesFullyQualified), (value) => { Configuration.ExportSettings.ScriptTypesFullyQualified = value; } },
		{ nameof(ExportSettings.ExportUnreadableAssets), (value) => { Configuration.ExportSettings.ExportUnreadableAssets = value; } },
		{ nameof(ExportSettings.SaveSettingsToDisk), (value) => { Configuration.ExportSettings.SaveSettingsToDisk = value; } },
		{ nameof(ExportSettings.SkipAudioClips), (value) => { Configuration.ExportSettings.SkipAudioClips = value; } },
		{ nameof(ExportSettings.SkipTextures), (value) => { Configuration.ExportSettings.SkipTextures = value; } },
		{ nameof(ExportSettings.SkipMeshes), (value) => { Configuration.ExportSettings.SkipMeshes = value; } },
		{ nameof(ExportSettings.SkipShaders), (value) => { Configuration.ExportSettings.SkipShaders = value; } },
		{ nameof(ExportSettings.SkipFonts), (value) => { Configuration.ExportSettings.SkipFonts = value; } },
		{ nameof(ExportSettings.SkipVideos), (value) => { Configuration.ExportSettings.SkipVideos = value; } },
		// New Material & Texture Reconnection Settings
		{ nameof(ExportSettings.ReconnectTextures), (value) => { Configuration.ExportSettings.ReconnectTextures = value; } },
		{ nameof(ExportSettings.ReconnectMaterialShaders), (value) => { Configuration.ExportSettings.ReconnectMaterialShaders = value; } },
		{ nameof(ExportSettings.ReconstructNormalMaps), (value) => { Configuration.ExportSettings.ReconstructNormalMaps = value; } },
		{ nameof(ExportSettings.RemapToStandardShaders), (value) => { Configuration.ExportSettings.RemapToStandardShaders = value; } },
		{ nameof(ExportSettings.FixSpriteMaterialReferences), (value) => { Configuration.ExportSettings.FixSpriteMaterialReferences = value; } },
		{ nameof(ExportSettings.AutoDetectTextureSlots), (value) => { Configuration.ExportSettings.AutoDetectTextureSlots = value; } },
		// New Texture Decoding Settings
		{ nameof(ExportSettings.FastTextureDecoding), (value) => { Configuration.ExportSettings.FastTextureDecoding = value; } },
		{ nameof(ExportSettings.HandleEncryptedTextures), (value) => { Configuration.ExportSettings.HandleEncryptedTextures = value; } },
		{ nameof(ExportSettings.FallbackToRawTextures), (value) => { Configuration.ExportSettings.FallbackToRawTextures = value; } },
		// New Sprite Settings
		{ nameof(ExportSettings.GenerateSpriteAtlas), (value) => { Configuration.ExportSettings.GenerateSpriteAtlas = value; } },
		{ nameof(ExportSettings.ExtractSpritesSeparately), (value) => { Configuration.ExportSettings.ExtractSpritesSeparately = value; } },
		// New Shader Settings
		{ nameof(ExportSettings.ExportShaderVariants), (value) => { Configuration.ExportSettings.ExportShaderVariants = value; } },
		{ nameof(ExportSettings.DecryptShaders), (value) => { Configuration.ExportSettings.DecryptShaders = value; } },
		{ nameof(ExportSettings.UseDummyShaders), (value) => { Configuration.ExportSettings.UseDummyShaders = value; } },
		{ nameof(ExportSettings.ExtractShaderLodData), (value) => { Configuration.ExportSettings.ExtractShaderLodData = value; } },
		{ nameof(ExportSettings.DecompileShaders), (value) => { Configuration.ExportSettings.DecompileShaders = value; } },
		// New Model Settings
		{ nameof(ExportSettings.FlipModels), (value) => { Configuration.ExportSettings.FlipModels = value; } },
		{ nameof(ExportSettings.GenerateModelMaterials), (value) => { Configuration.ExportSettings.GenerateModelMaterials = value; } },
		{ nameof(ExportSettings.EmbedTexturesInModels), (value) => { Configuration.ExportSettings.EmbedTexturesInModels = value; } },
		// New Audio Settings
		{ nameof(ExportSettings.ConvertAudioToMono), (value) => { Configuration.ExportSettings.ConvertAudioToMono = value; } },
		// New Performance Settings
		{ nameof(ExportSettings.EnableParallelExport), (value) => { Configuration.ExportSettings.EnableParallelExport = value; } },
		{ nameof(ExportSettings.UseStreamingMode), (value) => { Configuration.ExportSettings.UseStreamingMode = value; } },
		// New Unity Project Generation Settings
		{ nameof(ExportSettings.GenerateUnityProject), (value) => { Configuration.ExportSettings.GenerateUnityProject = value; } },
		{ nameof(ExportSettings.IncludeLibraryFolder), (value) => { Configuration.ExportSettings.IncludeLibraryFolder = value; } },
		{ nameof(ExportSettings.CompareWithPreviousExport), (value) => { Configuration.ExportSettings.CompareWithPreviousExport = value; } },
	};

	private static void WriteDropDownForScriptContentLevel(TextWriter writer)
	{
		WriteDropDown(writer, ScriptContentLevelDropDownSetting.Instance, Configuration.ImportSettings.ScriptContentLevel, nameof(ImportSettings.ScriptContentLevel));
	}

	private static void WriteCheckBoxForIgnoreStreamingAssets(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ImportSettings.IgnoreStreamingAssets, nameof(ImportSettings.IgnoreStreamingAssets), disabled);
	}

	private static void WriteDropDownForStreamingAssetsMode(TextWriter writer)
	{
		WriteDropDown(writer, StreamingAssetsModeDropDownSetting.Instance, Configuration.ImportSettings.StreamingAssetsMode, nameof(ImportSettings.StreamingAssetsMode));
	}

	private static void WriteCheckBoxForEnablePrefabOutlining(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ProcessingSettings.EnablePrefabOutlining, nameof(ProcessingSettings.EnablePrefabOutlining), disabled);
	}

	private static void WriteCheckBoxForEnableStaticMeshSeparation(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ProcessingSettings.EnableStaticMeshSeparation, nameof(ProcessingSettings.EnableStaticMeshSeparation), disabled);
	}

	private static void WriteCheckBoxForEnableAssetDeduplication(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ProcessingSettings.EnableAssetDeduplication, nameof(ProcessingSettings.EnableAssetDeduplication), disabled);
	}

	private static void WriteCheckBoxForRemoveNullableAttributes(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ProcessingSettings.RemoveNullableAttributes, nameof(ProcessingSettings.RemoveNullableAttributes), disabled);
	}

	private static void WriteCheckBoxForPublicizeAssemblies(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ProcessingSettings.PublicizeAssemblies, nameof(ProcessingSettings.PublicizeAssemblies), disabled);
	}

	private static void WriteDropDownForBundledAssetsExportMode(TextWriter writer)
	{
		WriteDropDown(writer, BundledAssetsExportModeDropDownSetting.Instance, Configuration.ProcessingSettings.BundledAssetsExportMode, nameof(ProcessingSettings.BundledAssetsExportMode));
	}

	private static void WriteDropDownForAudioExportFormat(TextWriter writer)
	{
		WriteDropDown(writer, AudioExportFormatDropDownSetting.Instance, Configuration.ExportSettings.AudioExportFormat, nameof(ExportSettings.AudioExportFormat));
	}

	private static void WriteDropDownForImageExportFormat(TextWriter writer)
	{
		WriteDropDown(writer, ImageExportFormatDropDownSetting.Instance, Configuration.ExportSettings.ImageExportFormat, nameof(ExportSettings.ImageExportFormat));
	}

	private static void WriteDropDownForLightmapTextureExportFormat(TextWriter writer)
	{
		WriteDropDown(writer, LightmapTextureExportFormatDropDownSetting.Instance, Configuration.ExportSettings.LightmapTextureExportFormat, nameof(ExportSettings.LightmapTextureExportFormat));
	}

	private static void WriteDropDownForScriptExportMode(TextWriter writer)
	{
		WriteDropDown(writer, ScriptExportModeDropDownSetting.Instance, Configuration.ExportSettings.ScriptExportMode, nameof(ExportSettings.ScriptExportMode));
	}

	private static void WriteDropDownForScriptLanguageVersion(TextWriter writer)
	{
		WriteDropDown(writer, ScriptLanguageVersionDropDownSetting.Instance, Configuration.ExportSettings.ScriptLanguageVersion, nameof(ExportSettings.ScriptLanguageVersion));
	}

	private static void WriteCheckBoxForScriptTypesFullyQualified(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.ScriptTypesFullyQualified, nameof(ExportSettings.ScriptTypesFullyQualified), disabled);
	}

	private static void WriteDropDownForShaderExportMode(TextWriter writer)
	{
		WriteDropDown(writer, ShaderExportModeDropDownSetting.Instance, Configuration.ExportSettings.ShaderExportMode, nameof(ExportSettings.ShaderExportMode));
	}

	private static void WriteDropDownForSpriteExportMode(TextWriter writer)
	{
		WriteDropDown(writer, SpriteExportModeDropDownSetting.Instance, Configuration.ExportSettings.SpriteExportMode, nameof(ExportSettings.SpriteExportMode));
	}

	private static void WriteDropDownForTextExportMode(TextWriter writer)
	{
		WriteDropDown(writer, TextExportModeDropDownSetting.Instance, Configuration.ExportSettings.TextExportMode, nameof(ExportSettings.TextExportMode));
	}

	private static void WriteCheckBoxForExportUnreadableAssets(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.ExportUnreadableAssets, nameof(ExportSettings.ExportUnreadableAssets), disabled);
	}

	private static void WriteCheckBoxForSaveSettingsToDisk(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.SaveSettingsToDisk, nameof(ExportSettings.SaveSettingsToDisk), disabled);
	}

	private static void WriteCheckBoxForSkipAudioClips(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.SkipAudioClips, nameof(ExportSettings.SkipAudioClips), disabled);
	}

	private static void WriteCheckBoxForSkipTextures(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.SkipTextures, nameof(ExportSettings.SkipTextures), disabled);
	}

	private static void WriteCheckBoxForSkipMeshes(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.SkipMeshes, nameof(ExportSettings.SkipMeshes), disabled);
	}

	private static void WriteCheckBoxForSkipShaders(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.SkipShaders, nameof(ExportSettings.SkipShaders), disabled);
	}

	private static void WriteCheckBoxForSkipFonts(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.SkipFonts, nameof(ExportSettings.SkipFonts), disabled);
	}

	private static void WriteCheckBoxForSkipVideos(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.SkipVideos, nameof(ExportSettings.SkipVideos), disabled);
	}

	// Material & Texture Settings
	private static void WriteCheckBoxForReconnectTextures(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.ReconnectTextures, nameof(ExportSettings.ReconnectTextures), disabled);
	}

	private static void WriteCheckBoxForReconnectMaterialShaders(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.ReconnectMaterialShaders, nameof(ExportSettings.ReconnectMaterialShaders), disabled);
	}

	private static void WriteCheckBoxForReconstructNormalMaps(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.ReconstructNormalMaps, nameof(ExportSettings.ReconstructNormalMaps), disabled);
	}

	private static void WriteCheckBoxForRemapToStandardShaders(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.RemapToStandardShaders, nameof(ExportSettings.RemapToStandardShaders), disabled);
	}

	private static void WriteCheckBoxForAutoDetectTextureSlots(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.AutoDetectTextureSlots, nameof(ExportSettings.AutoDetectTextureSlots), disabled);
	}

	private static void WriteTextAreaForMaxTextureSize(TextWriter writer)
	{
		new Label(writer).WithClass("form-label").WithFor(nameof(Configuration.ExportSettings.MaxTextureSize)).Close("Max Texture Size");
		new Input(writer)
			.WithType("number")
			.WithClass("form-control")
			.WithId(nameof(Configuration.ExportSettings.MaxTextureSize))
			.WithName(nameof(Configuration.ExportSettings.MaxTextureSize))
			.WithValue(Configuration.ExportSettings.MaxTextureSize.ToString())
			.Close();
	}

	private static void WriteDropDownForTextureDecodeQuality(TextWriter writer)
	{
		WriteDropDown(writer, TextureDecodeQualityDropDownSetting.Instance, Configuration.ExportSettings.TextureDecodeQuality, nameof(ExportSettings.TextureDecodeQuality));
	}

	private static void WriteDropDownForTextureExportFormat(TextWriter writer)
	{
		WriteDropDown(writer, TextureEncodingFormatDropDownSetting.Instance, Configuration.ExportSettings.TextureExportFormat, nameof(ExportSettings.TextureExportFormat));
	}

	// Unity Project Generation
	private static void WriteCheckBoxForGenerateUnityProject(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.GenerateUnityProject, nameof(ExportSettings.GenerateUnityProject), disabled);
	}

	private static void WriteCheckBoxForIncludeLibraryFolder(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.IncludeLibraryFolder, nameof(ExportSettings.IncludeLibraryFolder), disabled);
	}

	private static void WriteTextAreaForUnityProjectVersion(TextWriter writer)
	{
		new Label(writer).WithClass("form-label").WithFor(nameof(Configuration.ExportSettings.UnityProjectVersion)).Close("Unity Project Version");
		new Input(writer)
			.WithType("text")
			.WithClass("form-control")
			.WithId(nameof(Configuration.ExportSettings.UnityProjectVersion))
			.WithName(nameof(Configuration.ExportSettings.UnityProjectVersion))
			.WithValue(Configuration.ExportSettings.UnityProjectVersion)
			.Close();
	}

	private static void WriteTextAreaForUnityProjectName(TextWriter writer)
	{
		new Label(writer).WithClass("form-label").WithFor(nameof(Configuration.ExportSettings.UnityProjectName)).Close("Project Name");
		new Input(writer)
			.WithType("text")
			.WithClass("form-control")
			.WithId(nameof(Configuration.ExportSettings.UnityProjectName))
			.WithName(nameof(Configuration.ExportSettings.UnityProjectName))
			.WithValue(Configuration.ExportSettings.UnityProjectName)
			.Close();
	}
}

	private static void WriteCheckBoxForCompareWithPreviousExport(TextWriter writer, string label, bool disabled = false)
	{
		WriteCheckBox(writer, label, Configuration.ExportSettings.CompareWithPreviousExport, nameof(ExportSettings.CompareWithPreviousExport), disabled);
	}

	private static void WriteTextAreaForPreviousExportPath(TextWriter writer)
	{
		new Label(writer).WithClass("form-label").WithFor(nameof(ExportSettings.PreviousExportPath)).Close("Previous Export Path");
		new Input(writer)
			.WithType("text")
			.WithClass("form-control")
			.WithId(nameof(ExportSettings.PreviousExportPath))
			.WithName(nameof(ExportSettings.PreviousExportPath))
			.WithValue(Configuration.ExportSettings.PreviousExportPath)
			.Close();
	}

	private static void WriteTextAreaForUnreleasedFolderName(TextWriter writer)
	{
		new Label(writer).WithClass("form-label").WithFor(nameof(ExportSettings.UnreleasedFolderName)).Close("Unreleased Folder Name");
		new Input(writer)
			.WithType("text")
			.WithClass("form-control")
			.WithId(nameof(ExportSettings.UnreleasedFolderName))
			.WithName(nameof(ExportSettings.UnreleasedFolderName))
			.WithValue(Configuration.ExportSettings.UnreleasedFolderName)
			.Close();
	}
