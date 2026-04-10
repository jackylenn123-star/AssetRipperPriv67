using AssetRipper.Import.Logging;
using System.IO;

namespace AssetRipper.Export.UnityProjects;

public static class UnityProjectGenerator
{
	public static void GenerateUnityProject(string exportPath, ExportSettings settings)
	{
		if (!settings.GenerateUnityProject)
		{
			return;
		}

		Logger.Info(LogCategory.Export, "Generating Unity project...");

		// Create Unity project structure
		string projectName = settings.UnityProjectName;
		string unityProjectPath = Path.Combine(Path.GetDirectoryName(exportPath) ?? ".", projectName);

		CreateUnityProjectStructure(unityProjectPath, settings);

		// Copy exported assets to the Unity project's Assets folder
		string assetsDestPath = Path.Combine(unityProjectPath, "Assets");
		CopyAssetsRecursive(exportPath, assetsDestPath);

		Logger.Info(LogCategory.Export, $"Unity project generated at: {unityProjectPath}");
	}

	private static void CreateUnityProjectStructure(string projectPath, ExportSettings settings)
	{
		// Create required Unity project folders
		string[] folders = new[]
		{
			"Assets",
			"ProjectSettings",
			"Packages",
			"Logs",
			"Temp"
		};

		if (settings.IncludeLibraryFolder)
		{
			folders = folders.Append("Library").ToArray();
		}

		foreach (string folder in folders)
		{
			string folderPath = Path.Combine(projectPath, folder);
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
		}

		// Create ProjectSettings/ProjectVersion.txt
		string projectVersionContent = $"m_EditorVersion: {settings.UnityProjectVersion}\nm_EditorVersionWithRevision: {settings.UnityProjectVersion} (xxxxxxxxxx)";
		File.WriteAllText(Path.Combine(projectPath, "ProjectSettings", "ProjectVersion.txt"), projectVersionContent);

		// Create Packages/manifest.json
		string manifestContent = @"{
	""dependencies"": {
		""com.unity.collab-proxy"": ""2.2.0"",
		""com.unity.feature.development"": ""1.0.0"",
		""com.unity.ide.rider"": ""3.0.27"",
		""com.unity.ide.visualstudio"": ""2.0.22"",
		""com.unity.ide.vscode"": ""1.2.5"",
		""com.unity.test-framework"": ""1.3.9"",
		""com.unity.textmeshpro"": ""3.0.6"",
		""com.unity.timeline"": ""1.8.6"",
		""com.unity.ugui"": ""1.0.0"",
		""com.unity.modules.ai"": ""1.0.0"",
		""com.unity.modules.androidjni"": ""1.0.0"",
		""com.unity.modules.animation"": ""1.0.0"",
		""com.unity.modules.assetbundle"": ""1.0.0"",
		""com.unity.modules.audio"": ""1.0.0"",
		""com.unity.modules.cloth"": ""1.0.0"",
		""com.unity.modules.director"": ""1.0.0"",
		""com.unity.modules.imageconversion"": ""1.0.0"",
		""com.unity.modules.imgui"": ""1.0.0"",
		""com.unity.modules.jsonserialize"": ""1.0.0"",
		""com.unity.modules.particlesystem"": ""1.0.0"",
		""com.unity.modules.physics"": ""1.0.0"",
		""com.unity.modules.physics2d"": ""1.0.0"",
		""com.unity.modules.screencapture"": ""1.0.0"",
		""com.unity.modules.terrain"": ""1.0.0"",
		""com.unity.modules.terrainphysics"": ""1.0.0"",
		""com.unity.modules.tilemap"": ""1.0.0"",
		""com.unity.modules.ui"": ""1.0.0"",
		""com.unity.modules.uielements"": ""1.0.0"",
		""com.unity.modules.umbra"": ""1.0.0"",
		""com.unity.modules.unityanalytics"": ""1.0.0"",
		""com.unity.modules.unitywebrequest"": ""1.0.0"",
		""com.unity.modules.unitywebrequestassetbundle"": ""1.0.0"",
		""com.unity.modules.unitywebrequestaudio"": ""1.0.0"",
		""com.unity.modules.unitywebrequesttexture"": ""1.0.0"",
		""com.unity.modules.unitywebrequestwww"": ""1.0.0"",
		""com.unity.modules.vehicles"": ""1.0.0"",
		""com.unity.modules.video"": ""1.0.0"",
		""com.unity.modules.vr"": ""1.0.0"",
		""com.unity.modules.wind"": ""1.0.0"",
		""com.unity.modules.xr"": ""1.0.0""
	}
}";
		File.WriteAllText(Path.Combine(projectPath, "Packages", "manifest.json"), manifestContent);

		// Create ProjectSettings/EditorBuildSettings.asset

		// Create ProjectSettings/GraphicsSettings.asset
		CreateGraphicsSettings(projectPath);
		string editorBuildSettings = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1045 &1
EditorBuildSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Scenes:
  m_configObjects: {}
";
		File.WriteAllText(Path.Combine(projectPath, "ProjectSettings", "EditorBuildSettings.asset"), editorBuildSettings);

		// Create ProjectSettings/QualitySettings.asset
		string qualitySettings = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!47 &1
QualitySettings:
  m_ObjectHideFlags: 0
  serializedVersion: 5
  m_CurrentQuality: 5
  m_QualitySettings:
  - serializedVersion: 2
    name: Very Low
    pixelLightCount: 0
    shadows: 0
    shadowResolution: 0
    shadowProjection: 1
    shadowCascades: 1
    shadowDistance: 20
    shadowNearPlaneOffset: 3
    shadowCascade2Split: 0.33333334
    shadowCascade4Split: {x: 0.06666667, y: 0.2, z: 0.46666667}
    shadowmaskMode: 0
    skinWeights: 2
    textureQuality: 0
    anisotropicTextures: 0
    antiAliasing: 0
    softParticles: 0
    softVegetation: 0
    realtimeReflectionProbes: 0
    billboardsFaceCameraPosition: 0
    vSyncCount: 0
    lodBias: 0.3
    maximumLODLevel: 0
    streamingMipmapsActive: 0
    streamingMipmapsAddAllCameras: 1
    streamingMipmapsMemoryBudget: 512
    streamingMipmapsRenderersPerFrame: 512
    streamingMipmapsMaxLevelReduction: 2
    streamingMipmapsMaxFileIORequests: 1024
    particleRaycastBudget: 16
    asyncUploadTimeSlice: 2
    asyncUploadBufferSize: 16
    asyncUploadPersistentBuffer: 1
    resolutionScalingFixedDPIFactor: 1
    customRenderPipeline: {fileID: 0}
    excludedTargetPlatforms: []
  - serializedVersion: 2
    name: Low
    pixelLightCount: 0
    shadows: 0
    shadowResolution: 0
    shadowProjection: 1
    shadowCascades: 1
    shadowDistance: 20
    shadowNearPlaneOffset: 3
    shadowCascade2Split: 0.33333334
    shadowCascade4Split: {x: 0.06666667, y: 0.2, z: 0.46666667}
    shadowmaskMode: 0
    skinWeights: 2
    textureQuality: 0
    anisotropicTextures: 0
    antiAliasing: 0
    softParticles: 0
    softVegetation: 0
    realtimeReflectionProbes: 0
    billboardsFaceCameraPosition: 0
    vSyncCount: 1
    lodBias: 0.4
    maximumLODLevel: 0
    streamingMipmapsActive: 0
    streamingMipmapsAddAllCameras: 1
    streamingMipmapsMemoryBudget: 512
    streamingMipmapsRenderersPerFrame: 512
    streamingMipmapsMaxLevelReduction: 2
    streamingMipmapsMaxFileIORequests: 1024
    particleRaycastBudget: 64
    asyncUploadTimeSlice: 2
    asyncUploadBufferSize: 16
    asyncUploadPersistentBuffer: 1
    resolutionScalingFixedDPIFactor: 1
    customRenderPipeline: {fileID: 0}
    excludedTargetPlatforms: []
  - serializedVersion: 2
    name: Medium
    pixelLightCount: 1
    shadows: 1
    shadowResolution: 0
    shadowProjection: 1
    shadowCascades: 1
    shadowDistance: 40
    shadowNearPlaneOffset: 3
    shadowCascade2Split: 0.33333334
    shadowCascade4Split: {x: 0.06666667, y: 0.2, z: 0.46666667}
    shadowmaskMode: 0
    skinWeights: 2
    textureQuality: 0
    anisotropicTextures: 1
    antiAliasing: 0
    softParticles: 0
    softVegetation: 0
    realtimeReflectionProbes: 0
    billboardsFaceCameraPosition: 0
    vSyncCount: 1
    lodBias: 0.7
    maximumLODLevel: 0
    streamingMipmapsActive: 0
    streamingMipmapsAddAllCameras: 1
    streamingMipmapsMemoryBudget: 512
    streamingMipmapsRenderersPerFrame: 512
    streamingMipmapsMaxLevelReduction: 2
    streamingMipmapsMaxFileIORequests: 1024
    particleRaycastBudget: 256
    asyncUploadTimeSlice: 2
    asyncUploadBufferSize: 16
    asyncUploadPersistentBuffer: 1
    resolutionScalingFixedDPIFactor: 1
    customRenderPipeline: {fileID: 0}
    excludedTargetPlatforms: []
  - serializedVersion: 2
    name: High
    pixelLightCount: 2
    shadows: 2
    shadowResolution: 1
    shadowProjection: 1
    shadowCascades: 2
    shadowDistance: 70
    shadowNearPlaneOffset: 3
    shadowCascade2Split: 0.33333334
    shadowCascade4Split: {x: 0.06666667, y: 0.2, z: 0.46666667}
    shadowmaskMode: 1
    skinWeights: 2
    textureQuality: 0
    anisotropicTextures: 1
    antiAliasing: 2
    softParticles: 0
    softVegetation: 1
    realtimeReflectionProbes: 1
    billboardsFaceCameraPosition: 1
    vSyncCount: 1
    lodBias: 1
    maximumLODLevel: 0
    streamingMipmapsActive: 0
    streamingMipmapsAddAllCameras: 1
    streamingMipmapsMemoryBudget: 512
    streamingMipmapsRenderersPerFrame: 512
    streamingMipmapsMaxLevelReduction: 2
    streamingMipmapsMaxFileIORequests: 1024
    particleRaycastBudget: 1024
    asyncUploadTimeSlice: 2
    asyncUploadBufferSize: 16
    asyncUploadPersistentBuffer: 1
    resolutionScalingFixedDPIFactor: 1
    customRenderPipeline: {fileID: 0}
    excludedTargetPlatforms: []
  - serializedVersion: 2
    name: Ultra
    pixelLightCount: 4
    shadows: 2
    shadowResolution: 2
    shadowProjection: 1
    shadowCascades: 4
    shadowDistance: 150
    shadowNearPlaneOffset: 3
    shadowCascade2Split: 0.33333334
    shadowCascade4Split: {x: 0.06666667, y: 0.2, z: 0.46666667}
    shadowmaskMode: 1
    skinWeights: 4
    textureQuality: 0
    anisotropicTextures: 2
    antiAliasing: 4
    softParticles: 1
    softVegetation: 1
    realtimeReflectionProbes: 1
    billboardsFaceCameraPosition: 1
    vSyncCount: 1
    lodBias: 1
    maximumLODLevel: 0
    streamingMipmapsActive: 0
    streamingMipmapsAddAllCameras: 1
    streamingMipmapsMemoryBudget: 512
    streamingMipmapsRenderersPerFrame: 512
    streamingMipmapsMaxLevelReduction: 2
    streamingMipmapsMaxFileIORequests: 1024
    particleRaycastBudget: 4096
    asyncUploadTimeSlice: 2
    asyncUploadBufferSize: 16
    asyncUploadPersistentBuffer: 1
    resolutionScalingFixedDPIFactor: 1
    customRenderPipeline: {fileID: 0}
    excludedTargetPlatforms: []
  m_PerPlatformDefaultQuality: {}
";
		File.WriteAllText(Path.Combine(projectPath, "ProjectSettings", "QualitySettings.asset"), qualitySettings);

		// Create ProjectSettings/TagManager.asset
		string tagManager = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!78 &1
TagManager:
  serializedVersion: 2
  tags: []
  layers:
  - Default
  - TransparentFX
  - Ignore Raycast
  - 
  - Water
  - UI
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  - 
  m_SortingLayers:
  - name: Default
    uniqueID: 0
    locked: 0
";
		File.WriteAllText(Path.Combine(projectPath, "ProjectSettings", "TagManager.asset"), tagManager);

		Logger.Info(LogCategory.Export, $"Created Unity project structure at: {projectPath}");
	}

	private static void CopyAssetsRecursive(string sourcePath, string destPath)
	{
		if (!Directory.Exists(sourcePath))
		{
			Logger.Warning(LogCategory.Export, $"Source path does not exist: {sourcePath}");
			return;
		}

		Directory.CreateDirectory(destPath);

		foreach (string file in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
		{
			string relativePath = Path.GetRelativePath(sourcePath, file);
			string destFile = Path.Combine(destPath, relativePath);
			string destDir = Path.GetDirectoryName(destFile);

			if (!Directory.Exists(destDir))
			{
				Directory.CreateDirectory(destDir);
			}

			File.Copy(file, destFile, true);
			Logger.Info(LogCategory.Export, $"Copied: {relativePath}");
		}

		Logger.Info(LogCategory.Export, $"Copied all assets to: {destPath}");
	}
}
	private static void CreateGraphicsSettings(string projectPath)
	{
		// Allow larger texture sizes in Unity
		string graphicsSettings = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!30 &1
GraphicsSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 14
  m_Deferred:
    m_Shader: {fileID: 69, guid: 0000000000000000f000000000000000, type: 0}
  m_DeferredReflections:
    m_Shader: {fileID: 74, guid: 0000000000000000f000000000000000, type: 0}
  m_DeferredEmissive:
    m_Shader: {fileID: 73, guid: 0000000000000000f000000000000000, type: 0}
  m_DeferredAmbient:
    m_Shader: {fileID: 72, guid: 0000000000000000f000000000000000, type: 0}
  m_Skybox:
    m_Shader: {fileID: 71, guid: 0000000000000000f000000000000000, type: 0}
  m_DefaultRenderingLayerMask: 1
  m_LightmapKeepDirts: 1
  m_LightmapsKeepNormals: 1
  m_LightmapsKeepIndirects: 1
  m_LightProbeProxyVolumeKeepTypes: 0
  m_SRPDefaultSettings: {}
  m_AllowEnlightenSupportForBuild: 1
  m_ExcludedRuntimePlatforms: []
  m_TemplateAsset: {fileID: 0}
";
		File.WriteAllText(Path.Combine(projectPath, "ProjectSettings", "GraphicsSettings.asset"), graphicsSettings);
		
		// Also create QualitySettings with higher texture limits
		string qualitySettings = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!47 &1
QualitySettings:
  m_ObjectHideFlags: 0
  serializedVersion: 5
  m_CurrentQuality: 5
  m_QualitySettings:
  - serializedVersion: 2
    name: Low
    pixelLightCount: 0
    textureQuality: 0
    anisotropicTextures: 0
    antiAliasing: 0
    vSyncCount: 0
    lodBias: 0.3
    maximumLODLevel: 0
  - serializedVersion: 2
    name: Medium
    pixelLightCount: 1
    textureQuality: 0
    anisotropicTextures: 1
    antiAliasing: 0
    vSyncCount: 1
    lodBias: 0.7
    maximumLODLevel: 0
  - serializedVersion: 2
    name: High
    pixelLightCount: 2
    textureQuality: 0
    anisotropicTextures: 2
    antiAliasing: 2
    vSyncCount: 1
    lodBias: 1
    maximumLODLevel: 0
  m_PerPlatformDefaultQuality: {}
";
		File.WriteAllText(Path.Combine(projectPath, "ProjectSettings", "QualitySettings.asset"), qualitySettings);
		
		Logger.Info(LogCategory.Export, "Created GraphicsSettings with higher texture limits");
	}
