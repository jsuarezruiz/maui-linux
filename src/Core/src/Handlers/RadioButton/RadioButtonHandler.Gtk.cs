using Gtk;
using System;

namespace Microsoft.Maui.Handlers
{
	public partial class RadioButtonHandler : ViewHandler<IRadioButton, RadioButton>
	{
		static RadioButton baseRadioButton = new("base");

		protected override RadioButton CreatePlatformView()
		{
			return new RadioButton(baseRadioButton, "foo");
		}

		protected override void ConnectHandler(RadioButton platformView)
		{
			platformView.Toggled += OnClicked;
			// base.ConnectHandler(platformView);
		}

		protected override void DisconnectHandler(RadioButton platformView)
		{
			platformView.Toggled -= OnClicked;
			// base.DisconnectHandler(platformView);
		}

		[MissingMapper]
		public static void MapBackground(IRadioButtonHandler handler, IRadioButton radioButton) { }

		public static void MapIsChecked(IRadioButtonHandler handler, IRadioButton radioButton)
		{
			if (handler.PlatformView is RadioButton rb)
				rb.UpdateIsChecked(radioButton);
			// handler.PlatformView?.UpdateIsChecked(radioButton);
		}

		public static void MapContent(IRadioButtonHandler handler, IRadioButton radioButton)
		{
			if (handler.PlatformView is RadioButton rb)
				rb.UpdateContent(radioButton);
			// handler.PlatformView?.UpdateContent(radioButton);
		}

		public static void MapTextColor(IRadioButtonHandler handler, ITextStyle textStyle)
		{
			handler.PlatformView?.UpdateTextColor(textStyle.TextColor);
		}

		[MissingMapper]
		public static void MapCharacterSpacing(IRadioButtonHandler handler, ITextStyle textStyle) { }

		public static void MapFont(IRadioButtonHandler handler, ITextStyle textStyle)
		{
			var fontManager = handler.GetRequiredService<IFontManager>();
			handler.PlatformView?.UpdateFont(textStyle, fontManager);
		}

		[MissingMapper]
		public static void MapStrokeColor(IRadioButtonHandler handler, IRadioButton radioButton) { }

		[MissingMapper]
		public static void MapStrokeThickness(IRadioButtonHandler handler, IRadioButton radioButton) { }

		[MissingMapper]
		public static void MapCornerRadius(IRadioButtonHandler handler, IRadioButton radioButton) { }

		void OnClicked(object? sender, EventArgs e)
		{
			if (sender is RadioButton platformView && VirtualView != null)
				VirtualView.IsChecked = platformView.Active;
		}
	}
}