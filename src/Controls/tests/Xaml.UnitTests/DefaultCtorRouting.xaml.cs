using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Maui.Controls.Build.Tasks;
using Microsoft.Maui.Controls.Core.UnitTests;
using Microsoft.Maui.Controls.Xaml;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace Microsoft.Maui.Controls.Xaml.UnitTests
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class DefaultCtorRouting : ContentPage
	{
		[System.ComponentModel.TypeConverter(typeof(IsCompiledTypeConverter))]
		public bool IsCompiled { get; set; }

		public DefaultCtorRouting()
		{
			InitializeComponent();
		}

		[TestFixture]
		class Tests
		{
			[SetUp]
			public void Setup()
			{
				Device.PlatformServices = new MockPlatformServices();
			}

			[TearDown]
			public void TearDown()
			{
				Device.PlatformServices = null;
			}

			[Test]
			public void ShouldntBeCompiled()
			{
				var p = new DefaultCtorRouting();
				Assert.False(p.IsCompiled);
			}
		}
	}

	[ProvideCompiled("Microsoft.Maui.Controls.XamlC.IsCompiledTypeConverter")]
	class IsCompiledTypeConverter : TypeConverter, ICompiledTypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => true;

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if ((string)value != "IsCompiled?")
				throw new Exception();
			return false;
		}

		public IEnumerable<Instruction> ConvertFromString(string value, ILContext context, BaseNode node)
		{
			if (value != "IsCompiled?")
				throw new Exception();
			yield return Instruction.Create(OpCodes.Ldc_I4_1);
		}
	}
}