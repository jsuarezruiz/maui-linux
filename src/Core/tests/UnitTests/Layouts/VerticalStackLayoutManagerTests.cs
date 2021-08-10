using System.Collections.Generic;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using NSubstitute;
using Xunit;
using static Microsoft.Maui.UnitTests.Layouts.LayoutTestHelpers;

namespace Microsoft.Maui.UnitTests.Layouts
{
	[Category(TestCategory.Core, TestCategory.Layout)]
	public class VerticalStackLayoutManagerTests : StackLayoutManagerTests
	{
		[Theory]
		[InlineData(0, 100, 0, 0)]
		[InlineData(1, 100, 0, 100)]
		[InlineData(1, 100, 13, 100)]
		[InlineData(2, 100, 13, 213)]
		[InlineData(3, 100, 13, 326)]
		[InlineData(3, 100, -13, 274)]
		public void SpacingMeasurement(int viewCount, double viewHeight, int spacing, double expectedHeight)
		{
			var stack = BuildStack(viewCount, 100, viewHeight);
			stack.Spacing.Returns(spacing);

			var manager = new VerticalStackLayoutManager(stack);
			var measuredSize = manager.Measure(100, double.PositiveInfinity);

			Assert.Equal(expectedHeight, measuredSize.Height);
		}

		[Theory("Spacing has no effect when there's only one item")]
		[InlineData(0), InlineData(26), InlineData(-54)]
		public void SpacingArrangementOneItem(int spacing)
		{
			var stack = BuildStack(1, 100, 100);
			stack.Spacing.Returns(spacing);

			var manager = new VerticalStackLayoutManager(stack);

			var measuredSize = manager.Measure(100, double.PositiveInfinity);
			manager.ArrangeChildren(measuredSize);

			var expectedRectangle = new Rectangle(0, 0, 100, 100);
			stack[0].Received().Arrange(Arg.Is(expectedRectangle));
		}

		[Theory("Spacing has an effect when there's more than one item")]
		[InlineData(26), InlineData(-54)]
		public void SpacingArrangementTwoItems(int spacing)
		{
			var stack = BuildStack(2, 100, 100);
			stack.Spacing.Returns(spacing);

			var manager = new VerticalStackLayoutManager(stack);

			var measuredSize = manager.Measure(double.PositiveInfinity, 100);
			manager.ArrangeChildren(measuredSize);

			AssertArranged(stack[0], 0, 0, 100, 100);
			AssertArranged(stack[1], 0, 100 + spacing, 100, 100);
		}

		[Theory]
		[InlineData(150, 100, 100)]
		[InlineData(150, 200, 200)]
		[InlineData(1250, -1, 1250)]
		public void StackAppliesHeight(double viewHeight, double stackHeight, double expectedHeight)
		{
			var view = LayoutTestHelpers.CreateTestView(new Size(100, viewHeight));

			var stack = CreateTestLayout(new List<IView>() { view });

			stack.Height.Returns(stackHeight);

			var manager = new VerticalStackLayoutManager(stack);
			var measurement = manager.Measure(100, double.PositiveInfinity);
			Assert.Equal(expectedHeight, measurement.Height);
		}

		[Fact]
		public void IgnoresCollapsedViews()
		{
			var view = LayoutTestHelpers.CreateTestView(new Size(100, 100));
			var collapsedView = LayoutTestHelpers.CreateTestView(new Size(100, 100));
			collapsedView.Visibility.Returns(Visibility.Collapsed);

			var stack = CreateTestLayout(new List<IView>() { view, collapsedView });

			var manager = new VerticalStackLayoutManager(stack);
			var measure = manager.Measure(100, double.PositiveInfinity);
			manager.ArrangeChildren(measure);

			// View is visible, so we expect it to be measured and arranged
			view.Received().Measure(Arg.Any<double>(), Arg.Any<double>());
			view.Received().Arrange(Arg.Any<Rectangle>());

			// View is collapsed, so we expect it not to be measured or arranged
			collapsedView.DidNotReceive().Measure(Arg.Any<double>(), Arg.Any<double>());
			collapsedView.DidNotReceive().Arrange(Arg.Any<Rectangle>());
		}

		[Fact]
		public void DoesNotIgnoreHiddenViews()
		{
			var view = LayoutTestHelpers.CreateTestView(new Size(100, 100));
			var hiddenView = LayoutTestHelpers.CreateTestView(new Size(100, 100));
			hiddenView.Visibility.Returns(Visibility.Hidden);

			var stack = CreateTestLayout(new List<IView>() { view, hiddenView });

			var manager = new VerticalStackLayoutManager(stack);
			var measure = manager.Measure(100, double.PositiveInfinity);
			manager.ArrangeChildren(measure);

			// View is visible, so we expect it to be measured and arranged
			view.Received().Measure(Arg.Any<double>(), Arg.Any<double>());
			view.Received().Arrange(Arg.Any<Rectangle>());

			// View is hidden, so we expect it to be measured and arranged (since it'll need to take up space)
			hiddenView.Received().Measure(Arg.Any<double>(), Arg.Any<double>());
			hiddenView.Received().Arrange(Arg.Any<Rectangle>());
		}

		IStackLayout BuildPaddedStack(Thickness padding, double viewWidth, double viewHeight)
		{
			var stack = BuildStack(1, viewWidth, viewHeight);
			stack.Padding.Returns(padding);
			return stack;
		}

		[Theory]
		[InlineData(0, 0, 0, 0)]
		[InlineData(10, 10, 10, 10)]
		[InlineData(10, 0, 10, 0)]
		[InlineData(0, 10, 0, 10)]
		[InlineData(23, 5, 3, 15)]
		public void MeasureAccountsForPadding(double left, double top, double right, double bottom)
		{
			var viewWidth = 100d;
			var viewHeight = 100d;
			var padding = new Thickness(left, top, right, bottom);

			var expectedHeight = padding.VerticalThickness + viewHeight;
			var expectedWidth = padding.HorizontalThickness + viewWidth;

			var stack = BuildPaddedStack(padding, viewWidth, viewHeight);

			var manager = new VerticalStackLayoutManager(stack);
			var measuredSize = manager.Measure(double.PositiveInfinity, double.PositiveInfinity);

			Assert.Equal(expectedHeight, measuredSize.Height);
			Assert.Equal(expectedWidth, measuredSize.Width);
		}

		[Theory]
		[InlineData(0, 0, 0, 0)]
		[InlineData(10, 10, 10, 10)]
		[InlineData(10, 0, 10, 0)]
		[InlineData(0, 10, 0, 10)]
		[InlineData(23, 5, 3, 15)]
		public void ArrangeAccountsForPadding(double left, double top, double right, double bottom)
		{
			var viewWidth = 100d;
			var viewHeight = 100d;
			var padding = new Thickness(left, top, right, bottom);

			var stack = BuildPaddedStack(padding, viewWidth, viewHeight);

			var manager = new VerticalStackLayoutManager(stack);
			var measuredSize = manager.Measure(double.PositiveInfinity, double.PositiveInfinity);
			manager.ArrangeChildren(measuredSize);

			AssertArranged(stack[0], padding.Left, padding.Top, viewWidth, viewHeight);
		}
	}
}
