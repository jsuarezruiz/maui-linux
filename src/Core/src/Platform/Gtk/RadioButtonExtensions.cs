using System;
using Gtk;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Platform
{
	public static class RadioButtonExtensions
	{
		public static void UpdateIsChecked(this RadioButton platformRadioButton, IRadioButton radioButton)
		{
			platformRadioButton.Active = radioButton.IsChecked;
		}

		private static readonly string[] _backgroundColorKeys =
		{
			"RadioButtonBackground",
			"RadioButtonBackgroundPointerOver",
			"RadioButtonBackgroundPressed",
			"RadioButtonBackgroundDisabled"
		};

		public static void UpdateBackground(this RadioButton platformRadioButton, IRadioButton button)
		{
			// Ref: https://github.com/jsuarezruiz/maui-linux/blob/ce1c06f1f05422a70c150276a82ad88d3b572198/src/Core/src/Platform/Gtk/ViewExtensions.cs#L15-L69

			var color = button.Background?.BackgroundColor;

			if (button.Background is { } paint)
				color = paint.ToColor();

			var gradientCss = button.Background.ToCss();

			var disposePixbuf = false;
			var pixbuf = gradientCss == null ? button.Background?.ToPixbuf(out disposePixbuf) : default;

			// create a temporary file 
			var picCss = default(string);
			var tempFile = pixbuf?.TempFileFor();

			// use the tempfile as url in css
			if (tempFile is not null)
				picCss = $"url('{tempFile}')";

			if (color is null && (gradientCss is null || picCss is null))
				return;

			if (picCss is not null)
				platformRadioButton.SetStyleValue(picCss, "background-image");

			if (gradientCss is not null)
				platformRadioButton.SetStyleValue(gradientCss, "background");
			else
				platformRadioButton.SetBackgroundColor(color);

			// Gtk.CssProvider translates the file of url() into Base64, so the file can safely deleted:
			tempFile?.Dispose();

			if (disposePixbuf)
				pixbuf?.Dispose();
		}

		[MissingMapper]
		public static void UpdateTextColor(this RadioButton platformRadioButton, ITextStyle button) { }

		public static void UpdateContent(this RadioButton platformRadioButton, IRadioButton radioButton)
		{
			_ = radioButton.Handler?.MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			if (radioButton is { PresentedContent: IView view })
				platformRadioButton.Label = $"{view.ToPlatform(radioButton.Handler.MauiContext)}";
			else
				platformRadioButton.Label = $"{radioButton.Content}";
		}

		[MissingMapper]
		public static void UpdateStrokeColor(this RadioButton platformRadioButton, IRadioButton radioButton) { }

		[MissingMapper]
		public static void UpdateStrokeThickness(this RadioButton nativeRadioButton, IRadioButton radioButton) { }

		[MissingMapper]
		public static void UpdateCornerRadius(this RadioButton nativeRadioButton, IRadioButton radioButton) { }
	}
}