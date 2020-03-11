using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceBatchGenerics.ViewModels.EntityVMs;
using DeviceBatchWPF.Windows;

namespace DeviceBatchWPF.ViewModels
{
    public class WPFDeviceVM : DeviceVM
    {
        public AssignLifetimeDataToPixelWindow ParentWindow { get; set; }
        public override void CommitNewLifetimeEntityToDeviceAndDBExecute(object o)
        {
            base.CommitNewLifetimeEntityToDeviceAndDBExecute(o);
            ParentWindow.Close();
        }
    }
}
