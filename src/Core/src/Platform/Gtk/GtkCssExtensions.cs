using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Native.Gtk;
using Pango;

namespace Microsoft.Maui
{

	public static class GtkCssExtensions
	{

		public static string CssMainNode(this Gtk.Widget nativeView)
		{
			var mainNode = string.Empty;

			switch (nativeView)
			{
				case Gtk.ProgressBar:

				case Gtk.ComboBox box:

				default:
					mainNode = nativeView.StyleContext.Path.ToString().Split(':')[0];

					break;
			}

			return mainNode;
		}

		/// <summary>
		/// seems that CssParser doesn't support base64:
		/// https://github.com/GNOME/gtk/blob/gtk-3-22/gtk/gtkcssparser.c
		/// _gtk_css_parser_read_url
		/// </summary>
		public static string CssImage(this Gdk.Pixbuf nativeImage)
		{
			var puf = nativeImage.SaveToBuffer(ImageFormat.Png.ToImageExtension());

			return $"url('data:image/png;base64,{Convert.ToBase64String(puf)}')";
		}

		[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
		public static void Realize(Gtk.CssProvider p)
		{
			var l = p.ToString().Length;

			if (l != 0)
			{
				// ReSharper disable once EmptyStatement
				;
			}
		}

		public static void SetStyleValueNode(this Gtk.Widget widget, string value, string mainNode, string attr, string? subNode = null)
		{
			if (string.IsNullOrEmpty(value))
				return;

			using var p = new Gtk.CssProvider();

			subNode = subNode != null ? $" > {subNode} " : subNode;

			p.LoadFromData($"{mainNode}{subNode}{{{attr}:{value}}}");
			widget.StyleContext.AddProvider(p, Gtk.StyleProviderPriority.User);

			if (value.StartsWith("url"))
				Realize(p);
		}

		public static void SetStyleValue(this Gtk.Widget widget, string value, string attr, string? subNode = null)
			=> widget.SetStyleValueNode(value, widget.CssMainNode(), attr, subNode);

	}

}