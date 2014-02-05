using MLS.HA.DeviceController.Common;
using MLS.HA.DeviceController.Common.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VirtualHaController {
    /// <summary>
    /// Interaction logic for AddHaVirtualDevice.xaml
    /// </summary>
    public partial class AddHaVirtualDevice : PluginGuiWindow {
        public AddHaVirtualDevice() {
            InitializeComponent();
        }

        private void btnAdd_click(object sender, RoutedEventArgs e) {
            var deviceType = rdoSwitch.IsChecked == true ? DeviceType.StandardSwitch : DeviceType.DimmerSwitch;

            // Get the device's provider id; for simpliciy, this will be a derivation of the current time
            var providerId = DateTime.Now.ToString("yyMMddHHmmss");

            var deviceId = base.addDevice(DeviceProviderTypes.PluginDevice, VirtualHaController.getControllerName(), deviceType, providerId, txtDeviceName.Text);
            
            Dispatcher.Invoke(new Action(() => {
                Close();
            }));
        }

        private void btnCancel_click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
