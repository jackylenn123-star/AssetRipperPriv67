using AssetRipper.Export.Configuration;

namespace AssetRipper.GUI.Web.Pages.Settings.DropDown;

public sealed class TextureDecodeQualityDropDownSetting : DropDownSetting<TextureDecodeQuality>
{
	public static TextureDecodeQualityDropDownSetting Instance { get; } = new();

	public override string Title => "Texture Decode Quality";

	protected override IReadOnlyList<TextureDecodeQuality> Values { get; } = new TextureDecodeQuality[]
	{
		TextureDecodeQuality.Low,
		TextureDecodeQuality.Medium,
		TextureDecodeQuality.High,
		TextureDecodeQuality.Ultra
	};

	protected override string? GetDescription(TextureDecodeQuality value)
	{
		return value switch
		{
			TextureDecodeQuality.Low => "Fastest but lowest quality",
			TextureDecodeQuality.Medium => "Balanced quality and speed",
			TextureDecodeQuality.High => "Best quality, standard speed",
			TextureDecodeQuality.Ultra => "Maximum quality, slower",
			_ => null
		};
	}

	protected override string GetDisplayName(TextureDecodeQuality value)
	{
		return value switch
		{
			TextureDecodeQuality.Low => "Low",
			TextureDecodeQuality.Medium => "Medium",
			TextureDecodeQuality.High => "High",
			TextureDecodeQuality.Ultra => "Ultra",
			_ => value.ToString()
		};
	}
}