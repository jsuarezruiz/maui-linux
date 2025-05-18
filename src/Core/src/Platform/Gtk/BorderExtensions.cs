using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Platform
{
	public static class BorderExtensions
	{
		public static void UpdateStroke(this BorderView platformView, IBorderStroke border)
		{
			platformView.UpdateBorderColor(border.Stroke.ToColor());
			platformView.UpdateBorderWidth((int)border.StrokeThickness);
			platformView.UpdateDashPattern(border.StrokeDashPattern);
		}


		public static void UpdateStrokeThickness(this BorderView platformView, IBorderStroke border)
		{
			if (platformView is not { })
				return;

			platformView.UpdateBorderWidth((int)border.StrokeThickness);
		}

		public static void UpdateStrokeDashPattern(this BorderView platformView, IBorderStroke border)
		{
			if (platformView is not { })
				return;

			platformView.UpdateDashPattern(border.StrokeDashPattern);
		}

		public static void UpdateStrokeShape(this BorderView platformView, IBorderStroke border)
		{
			if (platformView is not { })
				return;

			platformView.UpdateShape(border.Shape);
		}

		[MissingMapper]
		public static void UpdateStrokeDashOffset(this BorderView platformView, IBorderStroke border) { }

		[MissingMapper]
		public static void UpdateStrokeMiterLimit(this BorderView platformView, IBorderStroke border) { }

		[MissingMapper]
		public static void UpdateStrokeLineCap(this BorderView platformView, IBorderStroke border) { }

		[MissingMapper]
		public static void UpdateStrokeLineJoin(this BorderView platformView, IBorderStroke border) { }
	}
}