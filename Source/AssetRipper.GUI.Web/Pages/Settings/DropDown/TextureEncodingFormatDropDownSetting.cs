using AssetRipper.Export.Configuration;

namespace AssetRipper.GUI.Web.Pages.Settings.DropDown;

public sealed class TextureEncodingFormatDropDownSetting : DropDownSetting<TextureEncodingFormat>
{
	public static TextureEncodingFormatDropDownSetting Instance { get; } = new();

	public override string Title => "Texture Export Format";

	protected override IReadOnlyList<TextureEncodingFormat> Values { get; } = new TextureEncodingFormat[]
	{
		TextureEncodingFormat.Png,
		TextureEncodingFormat.Tga,
		TextureEncodingFormat.Bmp
	};

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

	protected override string GetDisplayName(TextureEncodingFormat value)
	{
		return value switch
		{
			TextureEncodingFormat.Png => "PNG",
			TextureEncodingFormat.Tga => "TGA",
			TextureEncodingFormat.Bmp => "BMP",
			_ => value.ToString()
		};
	}
}