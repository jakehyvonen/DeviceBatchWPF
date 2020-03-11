using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using EFDeviceBatchCodeFirst;
using DeviceBatchGenerics.Support;
using DeviceBatchGenerics.Support.Bases;
using DeviceBatchWPF.Windows;

namespace DeviceBatchWPF.ViewModels
{
    public class DevTemplateBuilderVM : CrudVMBase
    {
        public DevTemplateBuilderVM()
        {
            PopulateDicts();
            NewDeviceTemplate = new DeviceTemplate();
            NewDevice = new Device();
            NewDeviceTemplate.Device = NewDevice;
            NewDeviceTemplate.Name = "Unnamed Template";
            NewDeviceTemplate.EmissionType = "Bottom Emitting";
            FillDeviceProperties();
            SelectedDepositionMethod = DepositionMethodsDict["Spincoating"];
            OpenWindow();
        }
        public event Action DeviceTemplateBuilt;
        #region Members
        Dictionary<string, DepositionMethod> _depositionMethodsDict;
        Dictionary<string, PhysicalRole> _physicalRolesDict;
        DepositionMethod _selectedDepositionMethod = new DepositionMethod();
        PhysicalRole _selectedPhysicalRole = new PhysicalRole();
        DevTemplateBuilderWindow _window = new DevTemplateBuilderWindow();
        DeviceTemplate _newDeviceTemplate;
        Device _newDevice;
        ObservableCollection<Layer> _newDeviceLayersCollection = new ObservableCollection<Layer>();
        #endregion
        #region Properties
        public Dictionary<string, DepositionMethod> DepositionMethodsDict
        {
            get
            {
                return _depositionMethodsDict;
            }
            set
            {
                _depositionMethodsDict = value;
                OnPropertyChanged();
            }
        }
        public Dictionary<string, PhysicalRole> PhysicalRolesDict
        {

            get { return _physicalRolesDict; }
            set
            {
                _physicalRolesDict = value;
                OnPropertyChanged();
            }
        }
        public DepositionMethod SelectedDepositionMethod
        {
            get
            {
                return _selectedDepositionMethod;
            }
            set
            {
                _selectedDepositionMethod = value;
                OnPropertyChanged();
            }
        }
        public PhysicalRole SelectedPhysicalRole
        {
            get
            {
                return _selectedPhysicalRole;
            }
            set
            {
                _selectedPhysicalRole = value;
                OnPropertyChanged();
            }
        }
        public DeviceTemplate NewDeviceTemplate
        {
            get
            {
                return _newDeviceTemplate;
            }
            set
            {
                _newDeviceTemplate = value;
                OnPropertyChanged();
                Debug.WriteLine("Changed NewDeviceTemplate");
            }
        }
        public Device NewDevice
        {
            get
            {
                return _newDevice;
            }
            set
            {
                _newDevice = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Layer> NewDeviceLayersCollection
        {
            get
            {
                return _newDeviceLayersCollection;
            }
            set
            {
                _newDeviceLayersCollection = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region Methods
        private void PopulateDicts()
        {
            DepositionMethodsDict = new Dictionary<string, DepositionMethod>();
            var depMethods = ctx.DepositionMethods.ToList();
            foreach (DepositionMethod dm in depMethods)
            {
                DepositionMethodsDict.Add(dm.Name, dm);
            }
            PhysicalRolesDict = new Dictionary<string, PhysicalRole>();
            var pRoles = ctx.PhysicalRoles.ToList();
            foreach (PhysicalRole p in pRoles)
            {
                PhysicalRolesDict.Add(p.LongName, p);
            }
        }
        private void AddLayerToDevice(Layer l)
        {
            l.PhysicalRole = ctx.PhysicalRoles.Where(p => p.LongName == SelectedPhysicalRole.LongName).Single();
            l.PositionIndex = NewDevice.NumberOfLayers ?? default(int);
            NewDevice.Layers.Add(l);
            NewDevice.NumberOfLayers += 1;
            NewDeviceLayersCollection.Add(l);
            UpdateDeviceTemplateStructure();
            Debug.WriteLine("Added layer of type: " + l.GetType().ToString());
            Debug.WriteLine("Deposition method is now: " + l.DepositionMethod.Name);
        }
        private void AddAnode()
        {
            var ito = new Layer();
            SelectedPhysicalRole = PhysicalRolesDict["Anode"];
            ito.DepositionMethod = ctx.DepositionMethods.Where(d => d.Name == "TCO Substrate").Single();
            AddLayerToDevice(ito);
            SelectedPhysicalRole = PhysicalRolesDict["Hole Injection Layer"];
        }
        private void FillDeviceProperties()
        {
            NewDevice.Label = "MMDD-CMMDDYY-#";
            NewDevice.NumberOfLayers = 0;
            Pixel pixelA = new Pixel();
            pixelA.Site = "SiteA";
            NewDevice.Pixels.Add(pixelA);
            Pixel pixelB = new Pixel();
            pixelB.Site = "SiteB";
            NewDevice.Pixels.Add(pixelB);
            Pixel pixelC = new Pixel();
            pixelC.Site = "SiteC";
            NewDevice.Pixels.Add(pixelC);
            Pixel pixelD = new Pixel();
            pixelD.Site = "SiteD";
            NewDevice.Pixels.Add(pixelD);
            AddAnode();
            UpdateDeviceTemplateStructure();
        }
        private void UpdateDeviceTemplateStructure()
        {
            NewDeviceTemplate.Structure = "";
            foreach (var l in NewDevice.Layers)
            {
                var layertype = l.GetType().ToString();
                Debug.WriteLine("New " + layertype + "layer has physical role " + l.PhysicalRole.LongName);
                NewDeviceTemplate.Structure = string.Concat(NewDeviceTemplate.Structure, l.PhysicalRole.ShortName, @"/");
            }
        }
        private Layer ConstructNewLayerFromSelectedDepositionMethod()
        {
            var newLayer = new Layer();
            newLayer.DepositionMethod = SelectedDepositionMethod;
            return newLayer;
        }
        public void OpenWindow()
        {
            _window.Show();
            _window.DataContext = this;
        }
        public DeviceTemplate TransferNewTemplate()
        {
            return NewDeviceTemplate;
        }
        #endregion
        #region ICommands
        private RelayCommand _addSelectedLayer;
        public ICommand AddSelectedLayer
        {
            get
            {
                if (_addSelectedLayer == null)
                {
                    _addSelectedLayer = new RelayCommand(param => this.AddSelectedLayerExecute(param));
                }
                return _addSelectedLayer;
            }
        }
        public void AddSelectedLayerExecute(object o)
        {
            AddLayerToDevice(ConstructNewLayerFromSelectedDepositionMethod());
            //PopulateDicts();
        }
        private RelayCommand _deleteLastLayer;
        public ICommand DeleteLastLayer
        {
            get
            {
                if (_deleteLastLayer == null)
                {
                    _deleteLastLayer = new RelayCommand(param => this.DeleteLastLayerExecute(param));
                }
                return _deleteLastLayer;
            }
        }
        public void DeleteLastLayerExecute(object o)
        {
            NewDeviceLayersCollection.Remove(NewDevice.Layers.Last());
            NewDevice.Layers.Remove(NewDevice.Layers.Last());
            UpdateDeviceTemplateStructure();
        }
        private RelayCommand _addNewDeviceTemplate;
        public ICommand AddNewDeviceTemplate
        {
            get
            {
                if (_addNewDeviceTemplate == null)
                {
                    _addNewDeviceTemplate = new RelayCommand(param => this.AddNewDeviceTemplateExecute(param));
                }
                return _addNewDeviceTemplate;
            }
        }
        public void AddNewDeviceTemplateExecute(object o)
        {
            try
            {
                ctx.DeviceTemplates.Add(NewDeviceTemplate);
                ctx.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.InnerException.ToString());
            }
            if (DeviceTemplateBuilt != null) DeviceTemplateBuilt();
            MessageBox.Show("Added New Template");
            _window.Close();
        }
        #endregion
    }

}
