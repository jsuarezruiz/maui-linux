using Microsoft.Maui.Graphics;
using UIKit;

namespace Microsoft.Maui.Handlers
{
	public partial class ViewHandler<TVirtualView, TNativeView> : INativeViewHandler
	{
		UIView? INativeViewHandler.NativeView => WrappedNativeView;
		UIView? INativeViewHandler.ContainerView => ContainerView;

		protected new UIView? WrappedNativeView =>
			(UIView?)base.WrappedNativeView;

		public new WrapperView? ContainerView
		{
			get => (WrapperView?)base.ContainerView;
			protected set => base.ContainerView = value;
		}

		UIViewController? INativeViewHandler.ViewController => null;

		public override void NativeArrange(Rectangle rect)
		{
			var nativeView = WrappedNativeView;

			if (nativeView == null)
				return;

			// We set Center and Bounds rather than Frame because Frame is undefined if the CALayer's transform is 
			// anything other than the identity (https://developer.apple.com/documentation/uikit/uiview/1622459-transform)
			nativeView.Center = new CoreGraphics.CGPoint(rect.Center.X, rect.Center.Y);
			nativeView.Bounds = new CoreGraphics.CGRect(0, 0, rect.Width, rect.Height);

			nativeView.UpdateBackgroundLayerFrame();
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			var nativeView = WrappedNativeView;

			if (nativeView == null)
			{
				return new Size(widthConstraint, heightConstraint);
			}

			var explicitWidth = VirtualView.Width;
			var explicitHeight = VirtualView.Height;
			var hasExplicitWidth = explicitWidth >= 0;
			var hasExplicitHeight = explicitHeight >= 0;

			var sizeThatFits = nativeView.SizeThatFits(new CoreGraphics.CGSize((float)widthConstraint, (float)heightConstraint));

			var size = new Size(
				sizeThatFits.Width == float.PositiveInfinity ? double.PositiveInfinity : sizeThatFits.Width,
				sizeThatFits.Height == float.PositiveInfinity ? double.PositiveInfinity : sizeThatFits.Height);

			if (double.IsInfinity(size.Width) || double.IsInfinity(size.Height))
			{
				nativeView.SizeToFit();
				size = new Size(nativeView.Frame.Width, nativeView.Frame.Height);
			}

			return new Size(hasExplicitWidth ? explicitWidth : size.Width,
				hasExplicitHeight ? explicitHeight : size.Height);
		}

		protected override void SetupContainer()
		{
			if (NativeView == null || ContainerView != null)
				return;

			var oldParent = (UIView?)NativeView.Superview;

			var oldIndex = oldParent?.IndexOfSubview(NativeView);
			NativeView.RemoveFromSuperview();

			ContainerView ??= new WrapperView(NativeView.Bounds);
			ContainerView.AddSubview(NativeView);

			if (oldIndex is int idx && idx >= 0)
				oldParent?.InsertSubview(ContainerView, idx);
			else
				oldParent?.AddSubview(ContainerView);
		}

		protected override void RemoveContainer()
		{
			if (NativeView == null || ContainerView == null || NativeView.Superview != ContainerView)
				return;

			var oldParent = (UIView?)ContainerView.Superview;

			var oldIndex = oldParent?.IndexOfSubview(ContainerView);
			ContainerView.RemoveFromSuperview();

			ContainerView = null;

			if (oldIndex is int idx && idx >= 0)
				oldParent?.InsertSubview(NativeView, idx);
			else
				oldParent?.AddSubview(NativeView);
		}
	}
}