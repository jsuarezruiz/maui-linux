﻿#nullable enable
using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;

namespace Microsoft.Maui
{
	public static class SliderExtensions
	{
		static void UpdateIncrement(this MauiSlider nativeSlider, ISlider slider)
		{
			double stepping = Math.Min((slider.Maximum - slider.Minimum) / 1000, 1);
			nativeSlider.StepFrequency = stepping;
			nativeSlider.SmallChange = stepping;
		}

		public static void UpdateMinimum(this MauiSlider nativeSlider, ISlider slider)
		{
			nativeSlider.Minimum = slider.Minimum;
			nativeSlider.UpdateIncrement(slider);
		}

		public static void UpdateMaximum(this MauiSlider nativeSlider, ISlider slider)
		{
			nativeSlider.Maximum = slider.Maximum;
			nativeSlider.UpdateIncrement(slider);
		}

		public static void UpdateValue(this MauiSlider nativeSlider, ISlider slider)
		{
			if (nativeSlider.Value != slider.Value)
				nativeSlider.Value = slider.Value;
		}

		public static void UpdateMinimumTrackColor(this MauiSlider nativeSlider, ISlider slider, Brush? defaultForegroundColor)
		{
			if (slider.MinimumTrackColor.IsDefault())
				nativeSlider.Foreground = defaultForegroundColor;
			else
				nativeSlider.Foreground = slider.MinimumTrackColor.ToNative();
		}

		public static void UpdateMaximumTrackColor(this MauiSlider nativeSlider, ISlider slider, Brush? defaultBackgroundColor)
		{
			if (slider.MaximumTrackColor.IsDefault())
				nativeSlider.Background = defaultBackgroundColor;
			else
				nativeSlider.Background = slider.MaximumTrackColor.ToNative();
		}

		public static async Task UpdateThumbImageSourceAsync(this MauiSlider nativeSlider, ISlider slider, IImageSourceServiceProvider? provider)
		{
			var thumbImage = slider.ThumbImageSource;

			if (thumbImage == null)
			{
				nativeSlider.ThumbImageSource = null;
				return;
			}

			if (provider == null)
				return;

			var service = provider.GetRequiredImageSourceService(thumbImage);
			var thumbImageSource = await service.GetImageSourceAsync(thumbImage);

			nativeSlider.ThumbImageSource = thumbImageSource?.Value;
		}
	}
}