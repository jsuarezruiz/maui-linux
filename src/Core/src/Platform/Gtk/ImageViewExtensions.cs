﻿#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using Gtk;

namespace Microsoft.Maui
{
	public static class ImageViewExtensions
	{
		public static void Clear(this ImageView imageView)
		{
			
		}

		public static void UpdateAspect(this ImageView imageView, IImage image)
		{
		}

		public static void UpdateIsAnimationPlaying(this ImageView imageView, IImageSourcePart image)
		{
			if (image.IsAnimationPlaying)
			{
				;
			}
			else
			{
				;
			}
		}

		public static async Task<IImageSourceServiceResult<Gdk.Pixbuf>?> UpdateSourceAsync(this ImageView imageView, IImageSourcePart image, IImageSourceServiceProvider services, CancellationToken cancellationToken = default)
		{
			imageView.Clear();

			image.UpdateIsLoading(false);

			var imageSource = image.Source;
			if (imageSource == null)
				return null;

			var events = image as IImageSourcePartEvents;

			events?.LoadingStarted();
			image.UpdateIsLoading(true);

			try
			{
				var service = services.GetRequiredImageSourceService(imageSource);

				var scale = imageView.ScaleFactor;
				var result = await service.GetImageAsync(imageSource, (float)scale, cancellationToken);
				var uiImage = result?.Value;

				var applied = !cancellationToken.IsCancellationRequested && imageSource == image.Source;

				// only set the image if we are still on the same one
				if (applied)
				{
					imageView.Image = uiImage;

					imageView.UpdateIsAnimationPlaying(image);
				}

				events?.LoadingCompleted(applied);

				return result;
			}
			catch (OperationCanceledException)
			{
				// no-op
				events?.LoadingCompleted(false);
			}
			catch (Exception ex)
			{
				events?.LoadingFailed(ex);
			}
			finally
			{
				// only mark as finished if we are still working on the same image
				if (imageSource == image.Source)
				{
					image.UpdateIsLoading(false);
				}
			}

			return null;
		}
	}
}