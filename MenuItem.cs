using MLS.HA.DeviceController.Common.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHaController {
    public class MenuItem : IPluginMenuItem {

        public string mainMenuName() {
            return "InControl Virtual Device";
        }

        /// <summary>
        /// Gets all sub-menu items.
        /// </summary>
        /// <returns></returns>
        public List<PluginSubMenuItem> subMenus() {
            var subs = new List<PluginSubMenuItem>();

            var menuItem = new PluginSubMenuItem();
            menuItem.menuName = "_Add InControl Virtual Device";
            menuItem.onMenuItemClicked += addDeviceClicked;
            subs.Add(menuItem);

            return subs;
        }

        /// <summary>
        /// Display the Add Dialog
        /// </summary>
        void addDeviceClicked(System.Windows.Window windowOwner) {            
            var frm = new AddHaVirtualDevice();
            frm.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            frm.Owner = windowOwner;
            frm.ShowDialog();
        }
    }
}
