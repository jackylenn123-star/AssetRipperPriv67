using AssetRipper.Import.Logging;

namespace AssetRipper.Export.Configuration;

public sealed record class ExportSettings
{
	/// <summary>
	/// The file format that audio clips get exported in. Recommended: Ogg
	/// </summary>
	public AudioExportFormat AudioExportFormat { get; set; } = AudioExportFormat.Default;

	/// <summary>
	/// The file format that images (like textures) get exported in.
	/// </summary>
	public ImageExportFormat ImageExportFormat { get; set; } = ImageExportFormat.Png;

	/// <summary>
	/// The file format that images (like textures) get exported in.
	/// </summary>
	public LightmapTextureExportFormat LightmapTextureExportFormat { get; set; } = LightmapTextureExportFormat.Yaml;

	/// <summary>
	/// How are MonoScripts exported? Recommended: Decompiled
	/// </summary>
	public ScriptExportMode ScriptExportMode { get; set; } = ScriptExportMode.Hybrid;

	/// <summary>
	/// The C# language version of decompiled scripts.
	/// </summary>
	public ScriptLanguageVersion ScriptLanguageVersion { get; set; } = ScriptLanguageVersion.AutoSafe;

	/// <summary>
	/// If true, type references in scripts are fully qualified.
	/// </summary>
	public bool ScriptTypesFullyQualified { get; set; } = false;

	/// <summary>
	/// Script naming convention: Full type name vs short name
	/// </summary>
	public ScriptNamingConvention ScriptNaming { get; set; } = ScriptNamingConvention.Short;

	/// <summary>
	/// Generate PDB files for debugging (when decompiling)
	/// </summary>
	public bool GeneratePdb { get; set; } = false;

	/// <summary>
	/// How to export shaders?
	/// </summary>
	public ShaderExportMode ShaderExportMode { get; set; } = ShaderExportMode.Dummy;

	/// <summary>
	/// Should sprites be exported as a texture? Recommended: Native
	/// </summary>
	public SpriteExportMode SpriteExportMode { get; set; } = SpriteExportMode.Yaml;

	/// <summary>
	/// How are text assets exported?
	/// </summary>
	public TextExportMode TextExportMode { get; set; } = TextExportMode.Parse;

	public bool ExportUnreadableAssets { get; set; } = false;

	/// <summary>
	/// Skip exporting audio clips.
	/// </summary>
	public bool SkipAudioClips { get; set; } = false;

	/// <summary>
	/// Skip exporting textures and images.
	/// </summary>
	public bool SkipTextures { get; set; } = false;

	/// <summary>
	/// Skip exporting meshes.
	/// </summary>
	public bool SkipMeshes { get; set; } = false;

	/// <summary>
	/// Skip exporting shaders.
	/// </summary>
	public bool SkipShaders { get; set; } = false;

	/// <summary>
	/// Skip exporting fonts.
	/// </summary>
	public bool SkipFonts { get; set; } = false;

	/// <summary>
	/// Skip exporting video clips.
	/// </summary>
	public bool SkipVideos { get; set; } = false;

	public bool SaveSettingsToDisk { get; set; }

	public string? LanguageCode { get; set; }

	// ======== Texture & Material Settings ========

	/// <summary>
	/// Max texture size for export (0 = original, 256-8192 for specific sizes)
	/// </summary>
	public int MaxTextureSize { get; set; } = 0;

	/// <summary>
	/// Texture decoding quality. Higher = better quality but slower.
	/// </summary>
	public TextureDecodeQuality TextureDecodeQuality { get; set; } = TextureDecodeQuality.High;

	/// <summary>
	/// Enable mipmap extraction from textures.
	/// </summary>
	public bool ExtractMipmaps { get; set; } = true;

	/// <summary>
	/// Texture encoding format (PNG, TGA, BMP)
	/// </summary>
	public TextureEncodingFormat TextureExportFormat { get; set; } = TextureEncodingFormat.Png;

	/// <summary>
	/// JPEG quality level (1-100)
	/// </summary>
	public int JpegQuality { get; set; } = 90;

	/// <summary>
	/// PNG compression level (0-9)
	/// </summary>
	public int PngCompressionLevel { get; set; } = 6;

	// ======== Material & Texture Reconnection Settings ========

	/// <summary>
	/// Reconnect textures to materials based on shader property names
	/// </summary>
	public bool ReconnectTextures { get; set; } = true;

	/// <summary>
	/// Reconnect materials to their original shaders
	/// </summary>
	public bool ReconnectMaterialShaders { get; set; } = true;

	/// <summary>
	/// Enable normal map reconstruction from heightmaps
	/// </summary>
	public bool ReconstructNormalMaps { get; set; } = true;

	/// <summary>
	/// Remap material shader references to standard Unity shaders
	/// </summary>
	public bool RemapToStandardShaders { get; set; } = false;

	/// <summary>
	/// Fix material texture references on sprite meshes
	/// </summary>
	public bool FixSpriteMaterialReferences { get; set; } = true;

	/// <summary>
	/// Auto-detect and reconnect material texture slots
	/// </summary>
	public bool AutoDetectTextureSlots { get; set; } = true;

	// ======== Texture Decoding Settings ========

	/// <summary>
	/// Texture decoding type: Auto, ForceCPU, ForceGPU
	/// </summary>
	public TextureDecodeType TextureDecodeType { get; set; } = TextureDecodeType.Auto;

	/// <summary>
	/// Enable fast texture decoding (lower quality, faster)
	/// </summary>
	public bool FastTextureDecoding { get; set; } = false;

	/// <summary>
	/// Handle encrypted textures (Xbox, PS4, Switch)
	/// </summary>
	public bool HandleEncryptedTextures { get; set; } = true;

	/// <summary>
	/// Fallback to raw export for unsupported texture formats
	/// </summary>
	public bool FallbackToRawTextures { get; set; } = true;

	// ======== Sprite Settings ========

	/// <summary>
	/// Generate sprite atlas from extracted sprites
	/// </summary>
	public bool GenerateSpriteAtlas { get; set; } = false;

	/// <summary>
	/// Extract sprites as separate files
	/// </summary>
	public bool ExtractSpritesSeparately { get; set; } = false;

	/// <summary>
	/// Sprite mesh type: FullRect or Tight
	/// </summary>
	public ExportSpriteMeshType SpriteMeshType { get; set; } = ExportSpriteMeshType.FullRect;

	// ======== Shader Settings ========

	/// <summary>
	/// Export shader variants (increases file size significantly)
	/// </summary>
	public bool ExportShaderVariants { get; set; } = false;

	/// <summary>
	/// Decrypt encrypted shader files
	/// </summary>
	public bool DecryptShaders { get; set; } = true;

	/// <summary>
	/// Use dummy shader fallback for missing shaders
	/// </summary>
	public bool UseDummyShaders { get; set; } = true;

	/// <summary>
	/// Extract shader LOD data
	/// </summary>
	public bool ExtractShaderLodData { get; set; } = true;

	/// <summary>
	/// Decompile shader programs to readable HLSL
	/// </summary>
	public bool DecompileShaders { get; set; } = false;

	// ======== Model Settings ========

	/// <summary>
	/// Flip model Z-axis for coordinate system conversion
	/// </summary>
	public bool FlipModels { get; set; } = false;

	/// <summary>
	/// Generate material for exported models
	/// </summary>
	public bool GenerateModelMaterials { get; set; } = true;

	/// <summary>
	/// Export models with embedded textures (GLB only)
	/// </summary>
	public bool EmbedTexturesInModels { get; set; } = false;

	// ======== Audio Settings ========

	/// <summary>
	/// Sample rate for audio export (0 = keep original)
	/// </summary>
	public int AudioSampleRate { get; set; } = 0;

	/// <summary>
	/// Convert all audio to mono
	/// </summary>
	public bool ConvertAudioToMono { get; set; } = false;

	// ======== Performance Settings ========

	/// <summary>
	/// Number of export threads (0 = auto-detect)
	/// </summary>
	public int ExportThreads { get; set; } = 0;

	/// <summary>
	/// Enable parallel export for faster processing
	/// </summary>
	public bool EnableParallelExport { get; set; } = true;

	/// <summary>
	/// Memory limit for export in MB (0 = unlimited)
	/// </summary>
	public int MemoryLimitMb { get; set; } = 0;

	/// <summary>
	/// Use streaming mode for large asset bundles
	/// </summary>
	public bool UseStreamingMode { get; set; } = false;

	// ======== Unity Project Generation ========

	/// <summary>
	/// Generate a complete Unity project with the exported assets
	/// </summary>
	public bool GenerateUnityProject { get; set; } = false;

	/// <summary>
	/// Target Unity version for the generated project (e.g., 2022.3.0)
	/// </summary>
	public string UnityProjectVersion { get; set; } = "2022.3.0";

	/// <summary>
	/// Include Library folder (larger but opens faster in Unity)
	/// </summary>
	public bool IncludeLibraryFolder { get; set; } = false;

	/// <summary>
	/// Project name for the generated Unity project
	/// </summary>
	public string UnityProjectName { get; set; } = "ExportedProject";

	public void Log()
	{
		Logger.Info(LogCategory.General, $"{nameof(AudioExportFormat)}: {AudioExportFormat}");
		Logger.Info(LogCategory.General, $"{nameof(ImageExportFormat)}: {ImageExportFormat}");
		Logger.Info(LogCategory.General, $"{nameof(LightmapTextureExportFormat)}: {LightmapTextureExportFormat}");
		Logger.Info(LogCategory.General, $"{nameof(ScriptExportMode)}: {ScriptExportMode}");
		Logger.Info(LogCategory.General, $"{nameof(ScriptLanguageVersion)}: {ScriptLanguageVersion}");
		Logger.Info(LogCategory.General, $"{nameof(ShaderExportMode)}: {ShaderExportMode}");
		Logger.Info(LogCategory.General, $"{nameof(SpriteExportMode)}: {SpriteExportMode}");
		Logger.Info(LogCategory.General, $"{nameof(TextExportMode)}: {TextExportMode}");
		Logger.Info(LogCategory.General, $"{nameof(ExportUnreadableAssets)}: {ExportUnreadableAssets}");
		Logger.Info(LogCategory.General, $"{nameof(SkipAudioClips)}: {SkipAudioClips}");
		Logger.Info(LogCategory.General, $"{ nameof(SkipTextures)}: {SkipTextures}");
		Logger.Info(LogCategory.General, $"{nameof(SkipMeshes)}: {SkipMeshes}");
		Logger.Info(LogCategory.General, $"{nameof(SkipShaders)}: {SkipShaders}");
		Logger.Info(LogCategory.General, $"{nameof(SkipFonts)}: {SkipFonts}");
		Logger.Info(LogCategory.General, $"{nameof(SkipVideos)}: {SkipVideos}");
		Logger.Info(LogCategory.General, $"{nameof(MaxTextureSize)}: {MaxTextureSize}");
		Logger.Info(LogCategory.General, $"{nameof(TextureDecodeQuality)}: {TextureDecodeQuality}");
		Logger.Info(LogCategory.General, $"{nameof(TextureDecodeType)}: {TextureDecodeType}");
		Logger.Info(LogCategory.General, $"{nameof(ReconnectTextures)}: {ReconnectTextures}");
		Logger.Info(LogCategory.General, $"{nameof(ReconnectMaterialShaders)}: {ReconnectMaterialShaders}");
		Logger.Info(LogCategory.General, $"{nameof(ReconstructNormalMaps)}: {ReconstructNormalMaps}");
		Logger.Info(LogCategory.General, $"{nameof(TextureExportFormat)}: {TextureExportFormat}");
		Logger.Info(LogCategory.General, $"{nameof(FastTextureDecoding)}: {FastTextureDecoding}");
		Logger.Info(LogCategory.General, $"{nameof(HandleEncryptedTextures)}: {HandleEncryptedTextures}");
		Logger.Info(LogCategory.General, $"{nameof(SpriteMeshType)}: {SpriteMeshType}");
		Logger.Info(LogCategory.General, $"{nameof(FlipModels)}: {FlipModels}");
		Logger.Info(LogCategory.General, $"{nameof(ExportThreads)}: {ExportThreads}");
		Logger.Info(LogCategory.General, $"{nameof(EnableParallelExport)}: {EnableParallelExport}");
		Logger.Info(LogCategory.General, $"{nameof(MemoryLimitMb)}: {MemoryLimitMb}");
	}
}