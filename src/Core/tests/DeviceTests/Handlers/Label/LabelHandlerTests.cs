using System;
using System.Threading.Tasks;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	[Category(TestCategory.Label)]
	public partial class LabelHandlerTests : HandlerTestBase<LabelHandler, LabelStub>
	{
		[Fact(DisplayName = "Background Color Initializes Correctly")]
		public async Task BackgroundColorInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				BackgroundColor = Colors.Blue,
				Text = "Test"
			};

			await ValidateNativeBackgroundColor(label, Colors.Blue);
		}

		[Fact(DisplayName = "Text Initializes Correctly")]
		public async Task TextInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				Text = "Test"
			};

			await ValidatePropertyInitValue(label, () => label.Text, GetNativeText, label.Text);
		}

		[Fact(DisplayName = "Text Color Initializes Correctly")]
		public async Task TextColorInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				Text = "Test",
				TextColor = Colors.Red
			};

			await ValidatePropertyInitValue(label, () => label.TextColor, GetNativeTextColor, label.TextColor);
		}

		[Theory(DisplayName = "Font Size Initializes Correctly")]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(20)]
		[InlineData(100)]
		public async Task FontSizeInitializesCorrectly(int fontSize)
		{
			var label = new LabelStub()
			{
				Text = "Test",
				Font = Font.OfSize("Arial", fontSize)
			};

			await ValidatePropertyInitValue(label, () => label.Font.FontSize, GetNativeUnscaledFontSize, label.Font.FontSize);
		}

		[Theory(DisplayName = "Font Attributes Initialize Correctly")]
		[InlineData(FontAttributes.None, false, false)]
		[InlineData(FontAttributes.Bold, true, false)]
		[InlineData(FontAttributes.Italic, false, true)]
		[InlineData(FontAttributes.Bold | FontAttributes.Italic, true, true)]
		public async Task FontAttributesInitializeCorrectly(FontAttributes attributes, bool isBold, bool isItalic)
		{
			var label = new LabelStub()
			{
				Text = "Test",
				Font = Font.OfSize("Arial", 10).WithAttributes(attributes)
			};

			await ValidatePropertyInitValue(label, () => label.Font.FontAttributes.HasFlag(FontAttributes.Bold), GetNativeIsBold, isBold);
			await ValidatePropertyInitValue(label, () => label.Font.FontAttributes.HasFlag(FontAttributes.Italic), GetNativeIsItalic, isItalic);
		}

		[Fact(DisplayName = "CharacterSpacing Initializes Correctly")]
		public async Task CharacterSpacingInitializesCorrectly()
		{
			var label = new LabelStub()
			{
				Text = "Test CharacterSpacing",
				CharacterSpacing = 4.0
			};

			await ValidatePropertyInitValue(label, () => label.CharacterSpacing, GetNativeCharacterSpacing, label.CharacterSpacing);
		}

		[Theory(DisplayName = "CharacterSpacing Updates Correctly")]
		[InlineData(0, 0)]
		[InlineData(0, 5)]
		[InlineData(5, 0)]
		[InlineData(5, 5)]
		[InlineData(5, 10)]
		[InlineData(10, 5)]
		public async Task CharacterSpacingUpdatesCorrectly(double setValue, double unsetValue)
		{
			var label = new LabelStub
			{
				Text = "This is TEXT!"
			};

			await ValidatePropertyUpdatesValue(
				label,
				nameof(ILabel.CharacterSpacing),
				GetNativeCharacterSpacing,
				setValue,
				unsetValue);
		}

		[Theory(DisplayName = "Updating Font Does Not Affect CharacterSpacing")]
		[InlineData(10, 20)]
		[InlineData(20, 10)]
		public async Task FontDoesNotAffectCharacterSpacing(double initialSize, double newSize)
		{
			var label = new LabelStub
			{
				Text = "This is TEXT!",
				CharacterSpacing = 5,
				Font = Font.SystemFontOfSize(initialSize)
			};

			await ValidateUnrelatedPropertyUnaffected(
				label,
				GetNativeCharacterSpacing,
				nameof(ILabel.Font),
				() => label.Font = Font.SystemFontOfSize(newSize));
		}

		[Theory(DisplayName = "Updating Text Does Not Affect CharacterSpacing")]
		[InlineData("Short", "Longer Text")]
		[InlineData("Long thext here", "Short")]
		public async Task TextDoesNotAffectCharacterSpacing(string initialText, string newText)
		{
			var label = new LabelStub
			{
				Text = initialText,
				CharacterSpacing = 5,
			};

			await ValidateUnrelatedPropertyUnaffected(
				label,
				GetNativeCharacterSpacing,
				nameof(ILabel.Text),
				() => label.Text = newText);
		}

		[Theory(DisplayName = "Updating Font Does Not Affect HorizontalTextAlignment")]
		[InlineData(10, 20)]
		[InlineData(20, 10)]
		public async Task FontDoesNotAffectHorizontalTextAlignment(double initialSize, double newSize)
		{
			var label = new LabelStub
			{
				Text = "This is TEXT!",
				HorizontalTextAlignment = TextAlignment.Center,
				Font = Font.SystemFontOfSize(initialSize),
			};

			await ValidateUnrelatedPropertyUnaffected(
				label,
				GetNativeHorizontalTextAlignment,
				nameof(ILabel.Font),
				() => label.Font = Font.SystemFontOfSize(newSize));
		}

		[Theory(DisplayName = "Updating Text Does Not Affect HorizontalTextAlignment")]
		[InlineData("Short", "Longer Text")]
		[InlineData("Long thext here", "Short")]
		public async Task TextDoesNotAffectHorizontalTextAlignment(string initialText, string newText)
		{
			var label = new LabelStub
			{
				Text = initialText,
				HorizontalTextAlignment = TextAlignment.Center,
			};

			await ValidateUnrelatedPropertyUnaffected(
				label,
				GetNativeHorizontalTextAlignment,
				nameof(ILabel.Text),
				() => label.Text = newText);
		}

		[Theory(DisplayName = "Updating LineHeight Does Not Affect HorizontalTextAlignment")]
		[InlineData(1, 2)]
		[InlineData(2, 1)]
		public async Task LineHeightDoesNotAffectHorizontalTextAlignment(double initialSize, double newSize)
		{
			var label = new LabelStub
			{
				Text = "This is TEXT!",
				HorizontalTextAlignment = TextAlignment.Center,
				LineHeight = initialSize,
			};

			await ValidateUnrelatedPropertyUnaffected(
				label,
				GetNativeHorizontalTextAlignment,
				nameof(ILabel.LineHeight),
				() => label.LineHeight = newSize);
		}

		[Theory(DisplayName = "Updating TextDecorations Does Not Affect HorizontalTextAlignment")]
		[InlineData(TextDecorations.None, TextDecorations.Underline)]
		[InlineData(TextDecorations.Underline, TextDecorations.Strikethrough)]
		[InlineData(TextDecorations.Underline, TextDecorations.None)]
		public async Task TextDecorationsDoesNotAffectHorizontalTextAlignment(TextDecorations initialDecorations, TextDecorations newDecorations)
		{
			var label = new LabelStub
			{
				Text = "This is TEXT!",
				HorizontalTextAlignment = TextAlignment.Center,
				TextDecorations = initialDecorations,
			};

			await ValidateUnrelatedPropertyUnaffected(
				label,
				GetNativeHorizontalTextAlignment,
				nameof(ILabel.TextDecorations),
				() => label.TextDecorations = newDecorations);
		}

	}
}