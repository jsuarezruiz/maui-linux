using Microsoft.Maui.ApplicationModel;

namespace Microsoft.Maui.Devices.Sensors
{

	partial class OrientationSensorImplementation : IOrientationSensor
	{

		bool PlatformIsSupported => false;

		void PlatformStart(SensorSpeed sensorSpeed) =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

		void PlatformStop() =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

	}

}