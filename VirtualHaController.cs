using MLS.HA.DeviceController.Common;
using MLS.HA.DeviceController.Common.Device;
using MLS.HA.DeviceController.Common.HaControllerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHaController {
    public class VirtualHaController : HaController, IHaController {

        private const string PRIOR_STATE = "vhaPriorState";

        List<HaDevice> localDevices;

        public string controllerName {
            get { return getControllerName(); }
            set { }
        }

        public VirtualHaController() {
            // If this were a real controller, we'd probably set up a thread to actively poll 
            // each of the localDevices and update the level based on the actual device's level.
            
            // NOTE: We never want to add devices to the localDevices list from here. Instead,
            // do that in the trackDevice method.
            
            // NOTE: If you want to discover devices from here, you can call raiseDiscoveredDevice,
            // which tells InControl about the device. InControl will in-turn call trackDevice.
            // raiseDiscoveredDevice(DeviceProviderTypes.PluginDevice, controllerName, DeviceType.StandardSwitch, 1, "Sample Virtual Device");

            localDevices = new List<HaDevice>();
        }

        /// <summary>
        /// Called by InControl when a new device should be tracked locally.
        /// </summary>
        /// <param name="haDevice"></param>
        /// <returns></returns>
        public HaDevice trackDevice(HaDeviceDto haDevice) {
            // InControl will call this every time it starts up. It'll call it for each device it currently knows about.
            // We should use the data passed in to create an HaDevice and track it in our localDevices list.
            // Then return that new HaDevice back to InControl.

            var newDevice = new HaDevice();

            // Grab some of the other values and set them in our local device
            newDevice.providerDeviceId = haDevice.uniqueName;
            newDevice.deviceId = haDevice.deviceId;
            newDevice.name = haDevice.deviceName;
            
            // Our devices always start at level unless there's a value set in the database saving its state
            newDevice.level = 0;

            // See if the device has a value saved in the database for it's initial level
            try {
                var savedStateRaw = base.getDeviceMetadata(newDevice.deviceId, PRIOR_STATE);
                if (savedStateRaw != null) {
                    double savedValue = 0;
                    if (double.TryParse(savedStateRaw, out savedValue)) {
                        newDevice.level = savedValue;
                    }
                }
            } catch { }
            
            // Add the device to our local list
            localDevices.Add(newDevice);

            return newDevice;
        }

        /// <summary>
        /// Gets a device tracked by this controller using the deviceId.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public HaDevice getHaDevice(Guid deviceId) {
            // Find the device in our local list and return it
            return localDevices.Where(d => d.deviceId == deviceId).FirstOrDefault();
        }

        /// <summary>
        /// Gets a device tracked by this controller using the providerId.
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public HaDevice getHaDevice(object providerId) {
            // Find the device in our local list and return it
            return localDevices.Where(d => d.providerDeviceId.ToString() == providerId.ToString()).FirstOrDefault();
        }

        /// <summary>
        /// Gets all devices tracked by this controller.
        /// </summary>
        /// <returns></returns>
        public List<HaDevice> getHaDevices() {
            // Return all local devices
            return localDevices;
        }

        /// <summary>
        /// Sets the level of our local device.
        /// </summary>
        /// <param name="providerDeviceId"></param>
        /// <param name="newLevel"></param>
        public void setLevel(object providerDeviceId, int newLevel) {
            // Get local device
            var device = getHaDevice(providerDeviceId);

            // Save the device level to the meta data so that it can be retrieved
            try {
                base.setDeviceMetadata(device.deviceId, PRIOR_STATE, newLevel.ToString());
            } catch { }

            // Set the level
            device.level = (byte)newLevel;
        }

        /// <summary>
        /// Sets the power of the local device.
        /// </summary>
        /// <param name="providerDeviceId"></param>
        /// <param name="powered"></param>
        public void setPower(object providerDeviceId, bool powered) {
            // Get the local device
            var device = getHaDevice(providerDeviceId);

            // Set the device as powered (255) or off (0)
            device.level = powered ? (byte)255 : (byte)0;

            // Save the device level to the meta data so that it can be retrieved
            try {
                base.setDeviceMetadata(device.deviceId, PRIOR_STATE, device.level.ToString());
            } catch { }
        }

        #region Less important stuff
        public void stop() {
            // Does nothing
        }

        public MLS.HA.DeviceController.Common.ControllerTestResult testController() {
            return new MLS.HA.DeviceController.Common.ControllerTestResult() {
                result = true
            };
        }

        public void executeSpecialCommand(object providerDeviceId, MLS.HA.DeviceController.Common.SpecialCommand command, object value) {
        }

        public void finishedTracking() {
        }

        public MLS.HA.DeviceController.Common.HaDeviceDetails getDeviceDetails(object providerDeviceId) {
            return new MLS.HA.DeviceController.Common.HaDeviceDetails() {
            };
        }
        #endregion


        internal static string getControllerName() {
            return "VirtualHaController"; 
        }
    }
}
