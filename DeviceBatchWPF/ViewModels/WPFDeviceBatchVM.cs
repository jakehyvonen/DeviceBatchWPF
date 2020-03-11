using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceBatchGenerics.Support;
using DeviceBatchGenerics.ViewModels.EntityVMs;
using DeviceBatchGenerics.ViewModels.PlottingVMs;
using DeviceBatchWPF.Controls;
using DeviceBatchWPF.Scheduling;
using DeviceBatchWPF.Windows;
using EFDeviceBatchCodeFirst;
using System.Windows.Forms;

namespace DeviceBatchWPF.ViewModels
{
    public class WPFDeviceBatchVM : DeviceBatchVM
    {
        public WPFDeviceBatchVM(DeviceBatch devBatch) : base(devBatch)
        {
            ActiveUserControl = new DevBatchVizControl();
            //TryToUpdateDataAndSpreadsheetsFromDevBatchPath();
        }
        System.Windows.Window _window;
       
        public void OpenDeviceBatchWindow()
        {
            _window = new DeviceBatchWindow();
            _window.DataContext = this;
            _window.Show();
        }
        private RelayCommand _openAssignLifetimeDataToPixelWindow;
        public ICommand OpenAssignLifetimeDataToPixelWindow
        {
            get
            {
                if (_openAssignLifetimeDataToPixelWindow == null)
                {
                    _openAssignLifetimeDataToPixelWindow = new RelayCommand(param => this.OpenAssignLifetimeDataToPixelWindowExecute(param));
                }
                return _openAssignLifetimeDataToPixelWindow;
            }
        }
        public void OpenAssignLifetimeDataToPixelWindowExecute(object o)
        {
            if (SelectedDeviceVM == null)
                MessageBox.Show("Please select a device by clicking on its row");
            else
            {
                MessageBox.Show("Needs to be implemented. Sorry!!!");

                /*
                AssignLifetimeDataToPixelWindow window = new AssignLifetimeDataToPixelWindow(SelectedDeviceVM);
                window.Title = SelectedDeviceVM.TheDevice.Label;
                window.Show();
                */
            }
        }
        private RelayCommand _openDeviceViewWindow;
        public ICommand OpenDeviceViewWindow
        {
            get
            {
                if (_openDeviceViewWindow == null)
                {
                    _openDeviceViewWindow = new RelayCommand(param => this.OpenDeviceViewWindowExecute(param));
                }
                return _openDeviceViewWindow;
            }
        }
        public void OpenDeviceViewWindowExecute(object o)
        {
            DeviceVM theDeviceVM = new DeviceVM();
            if (o is DeviceVM)
            {
                theDeviceVM = (DeviceVM)o;//cast the object as a Device
            }
            DevicePlotVM dvm = new DevicePlotVM(theDeviceVM.TheDevice);
            DeviceWindow window = new DeviceWindow(dvm);
            //window.Title = theDeviceVM.TheDevice.Label;
            window.Show();
        }
        private RelayCommand _OpenEquipmentSchedulingWindow;
        public ICommand OpenEquipmentSchedulingWindow
        {
            get
            {
                if (_OpenEquipmentSchedulingWindow == null)
                {
                    _OpenEquipmentSchedulingWindow = new RelayCommand(param => this.OpenEquipmentSchedulingWindowExecute(param));
                }
                return _OpenEquipmentSchedulingWindow;
            }
        }
        public void OpenEquipmentSchedulingWindowExecute(object o)
        {
            EquipmentSchedulingViewModel ESVM = new EquipmentSchedulingViewModel(this);
            EquipmentSchedulingWindow ESW = new EquipmentSchedulingWindow(ESVM);
            ESW.Show();
            /*
            try
            {
                EquipmentSchedulingViewModel ESVM = new EquipmentSchedulingViewModel(this);
                EquipmentSchedulingWindow ESW = new EquipmentSchedulingWindow(ESVM);
                ESW.Show();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            */
        }
    }
}

