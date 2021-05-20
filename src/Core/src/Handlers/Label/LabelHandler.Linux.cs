﻿using System;
using System.Runtime.InteropServices.ComTypes;
using Gtk;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Native.Gtk;
using Pango;

namespace Microsoft.Maui.Handlers
{

	public partial class LabelHandler : ViewHandler<ILabel, Label>
	{

		private static Microsoft.Maui.Graphics.Native.Gtk.TextLayout? _textLayout;

		public Microsoft.Maui.Graphics.Native.Gtk.TextLayout SharedTextLayout => _textLayout ??= new Microsoft.Maui.Graphics.Native.Gtk.TextLayout(
			Microsoft.Maui.Graphics.Native.Gtk.NativeGraphicsService.Instance.SharedContext)
		{
			HeightForWidth = true
		};

		// https://developer.gnome.org/gtk3/stable/GtkLabel.html
		protected override Label CreateNativeView()
		{
			return new Label()
			{
				// Hexpand = true,
				// MaxWidthChars = 1,
				LineWrap = true,
				// nativeView.Lines = 10;
				Halign = Align.Fill,
				Xalign = 0,
				MaxWidthChars = 1
			};
		}

		// note:fix that problem by using
		// Microsoft.Maui.Graphics.Native.Gtk - NativeGraphicsService.GetStringSize

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			if (NativeView is not { } nativeView)
				return default;

			if (VirtualView is not { } virtualView)
				return default;

			var res = base.GetDesiredSize(widthConstraint, heightConstraint);

			lock (SharedTextLayout)
			{
				SharedTextLayout.FontFamily = virtualView.Font.FontFamily;
				SharedTextLayout.TextFlow = TextFlow.ClipBounds;
				SharedTextLayout.PangoFontSize = virtualView.Font.FontSize.ScaledToPango();
				SharedTextLayout.HorizontalAlignment = virtualView.HorizontalTextAlignment.GetHorizontalAlignment();
				SharedTextLayout.LineBreakMode = virtualView.LineBreakMode.GetLineBreakMode();
			}

			var (width,height) = SharedTextLayout.GetPixelSize(NativeView.Text, double.IsInfinity(widthConstraint)?-1:widthConstraint);
			var inkRect = new Pango.Rectangle();
			var logicalRect = new Pango.Rectangle();
			nativeView.Layout.GetLineReadonly(0).GetExtents(ref inkRect, ref logicalRect);

			// nativeView.SetSizeRequest((int)ts.Width,(int)ts.Height);
			// nativeView.QueueResize();
			// nativeView.QueueComputeExpand();
			// res.Width = ts.Width + nativeView.MarginStart + nativeView.MarginEnd;
			// res.Height = ts.Height + nativeView.MarginTop + nativeView.MarginBottom;
			return new Size(width,height);

			
		}

		public static void MapText(LabelHandler handler, ILabel label)
		{
			handler.NativeView?.UpdateText(label);
		}

		public static void MapTextColor(LabelHandler handler, ILabel label)
		{
			handler.NativeView?.UpdateTextColor(label.TextColor);
		}

		public static void MapFont(LabelHandler handler, ILabel label)
		{
			handler.MapFont(label);
		}

		public static void MapHorizontalTextAlignment(LabelHandler handler, ILabel label)
		{
			handler.NativeView?.UpdateTextAlignment(label);
		}

		public static void MapLineBreakMode(LabelHandler handler, ILabel label)
		{
			handler.NativeView?.UpdateLineBreakMode(label);
		}

		public static void MapMaxLines(LabelHandler handler, ILabel label)
		{
			handler.NativeView?.UpdateMaxLines(label);
		}

		public static void MapPadding(LabelHandler handler, ILabel label)
		{
			handler.NativeView.WithPadding(label.Padding);

		}

		[MissingMapper]
		public static void MapCharacterSpacing(LabelHandler handler, ILabel label)
		{ }

		[MissingMapper]
		public static void MapTextDecorations(LabelHandler handler, ILabel label)
		{ }

		[MissingMapper]
		public static void MapLineHeight(LabelHandler handler, ILabel label)
		{
			// there is no LineHeight for label in gtk3:
			// https://gitlab.gnome.org/GNOME/gtk/-/issues/2379
		}

	}

}