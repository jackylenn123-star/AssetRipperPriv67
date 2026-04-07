using AssetRipper.Export.Configuration;

namespace AssetRipper.GUI.Web.Pages.Settings.DropDown;

public sealed class TextureDecodeQualityDropDownSetting : DropDownSetting<TextureDecodeQuality>
{
	public static TextureDecodeQualityDropDownSetting Instance { get; } = new();

	public override string Title => "Texture Decode Quality";

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

	protected override IEnumerable<DropDownItem<TextureDecodeQuality>> GetItems()
	{
		yield return new(TextureDecodeQuality.Low, "Low");
		yield return new(TextureDecodeQuality.Medium, "Medium");
		yield return new(TextureDecodeQuality.High, "High");
		yield return new(TextureDecodeQuality.Ultra, "Ultra");
	}
}