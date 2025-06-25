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
			platformRadioButton.UpdateBackground(button);
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
