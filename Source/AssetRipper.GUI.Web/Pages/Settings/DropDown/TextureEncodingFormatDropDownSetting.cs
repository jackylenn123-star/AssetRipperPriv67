using AssetRipper.Export.Configuration;

namespace AssetRipper.GUI.Web.Pages.Settings.DropDown;

public sealed class TextureEncodingFormatDropDownSetting : DropDownSetting<TextureEncodingFormat>
{
	public static TextureEncodingFormatDropDownSetting Instance { get; } = new();

	public override string Title => "Texture Export Format";

	protected override string? GetDescription(TextureEncodingFormat value)
	{
		return value switch
		{
			TextureEncodingFormat.Png => "PNG format - Lossless, recommended",
			TextureEncodingFormat.Tga => "TGA format - Uncompressed, good for editing",
			TextureEncodingFormat.Bmp => "BMP format - Basic uncompressed format",
			_ => null
		};
	}

	protected override IEnumerable<DropDownItem<TextureEncodingFormat>> GetItems()
	{
		yield return new(TextureEncodingFormat.Png, "PNG");
		yield return new(TextureEncodingFormat.Tga, "TGA");
		yield return new(TextureEncodingFormat.Bmp, "BMP");
	}
}