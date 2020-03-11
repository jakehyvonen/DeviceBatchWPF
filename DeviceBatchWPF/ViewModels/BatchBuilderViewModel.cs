using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.Entity.Validation;
using System.Windows.Data;
using System.IO;
using System.Windows;
using OfficeOpenXml;
using EFDeviceBatchCodeFirst;
using DeviceBatchWPF.Controls.BatchBuilding;
using DeviceBatchWPF.Scheduling;
using DeviceBatchGenerics.Support;
using DeviceBatchGenerics.Support.Bases;
using DeviceBatchGenerics.ViewModels.EntityVMs;
using DeviceBatchWPF.Windows;


namespace DeviceBatchWPF.ViewModels
{
    public class BatchBuilderViewModel : CrudVMBase
    {
        //Presently, this class contains all logic for creating a DeviceBatch either from scratch or from a previous DeviceBatch
        //It is horribly disorganized, uncommented, and bloated and I apologize ahead of time to whomever has to clean up after me
        #region Construction
        /// <summary>
        /// Constructs a new DeviceBatch and adds it to the context + sets the control to assign the employee and choose build method
        /// </summary>
        public BatchBuilderViewModel()
        {
            FillEmployees();
            DeviceBatchToAdd = new DeviceBatch();
            DeviceBatchToAdd.FabDate = DateTime.Now;
            DeviceBatchToAdd.Size = 0;
            DeviceBatchToAdd.Name = "Unnamed Batch";
            DeviceBatchToAdd.Notes = " ";
            DeviceBatchToAdd.FilePath = @"\";
            ctx.DeviceBatches.Add(DeviceBatchToAdd);
            DeviceViewSource = new CollectionViewSource();
            DeviceViewSource.Source = DeviceBatchToAdd.Devices;
            ActiveUserControl = new SelectBuildMethodControl();
        }
        public BatchBuilderViewModel(DeviceBatchVM dbVM)
        {
            DeviceBatchToAdd = ctx.DeviceBatches.Where(x => x.DeviceBatchId == dbVM.TheDeviceBatch.DeviceBatchId).First();
            //DeviceBatchToAdd = dbVM.TheDeviceBatch;
            _devicesHaveBeenConstructed = true;
            _windowIsClosedBeforeCompletion = false;//make sure we don't accidentally delete the device batch
            ConstructAndPopulateCollections(true);//not actually buildingFromTemplates but lazy
            DeviceViewSource = new CollectionViewSource();
            DeviceViewSource.Source = DeviceBatchToAdd.Devices;
            //ctx.ChangeTracker.DetectChanges();
        }
        #endregion
        #region Members
        DevTemplateBuilderVM _devTBVM;
        DeviceBatch _deviceBatchToAdd;
        UserControl _activeUserControl;
        DateTime _beginSearchDate = DateTime.Now.AddDays(-90);
        DateTime _endSearchDate = DateTime.Now;
        ObservableCollection<DeviceBatch> _visibleDeviceBatches;
        ObservableCollection<DeviceTemplate> _deviceTemplatesObservableCollection;
        ObservableCollection<DeviceTemplate> _templatesToAddCollection;
        ObservableCollection<Material> _materialsToAddCollection;
        ObservableCollection<Material> _visibleMaterialsCollection;
        ObservableCollection<Solution> _solutionsToAddCollection;
        ObservableCollection<Solution> _visibleSolutionsCollection;
        ObservableCollection<PhysicalRole> _physicalRolesCollection;
        ObservableCollection<Employee> _employeesCollection;
        ObservableCollection<LayerTemplate> _layerTemplatesCollection;
        ObservableCollection<string> _QDBatchesCollection = new ObservableCollection<string>();
        CollectionViewSource _deviceViewSource;
        PhysicalRole _selectedRole;
        DeviceTemplate _selectedDeviceTemplate;
        LayerTemplate _selectedLayerTemplate;
        Solution _selectedSolution;
        Material _selectedMaterial;
        Layer _selectedLayer;
        Device _selectedDevice;
        Employee _selectedEmployee;
        ListCollectionView _solutionsView;
        ListCollectionView _materialsView;
        bool _windowIsClosedBeforeCompletion = true;
        bool _buildingFromPreviousBatch = false;
        bool _devicesHaveBeenConstructed = false;
        #endregion
        #region Properties
        public event EventHandler<DeviceBatchCreatedEventArgs> DeviceBatchCreated;
        public BatchBuilderWindow ParentWindow { get; set; }
        public ListCollectionView SolutionsView
        {
            get
            {
                return _solutionsView;
            }
            set
            {
                _solutionsView = value;
                OnPropertyChanged();
            }
        }
        public ListCollectionView MaterialsView
        {
            get
            {
                return _materialsView;
            }
            set
            {
                _materialsView = value;
                OnPropertyChanged();
            }
        }
        public CollectionViewSource DeviceViewSource
        {
            get
            {
                _deviceViewSource.Source = DeviceBatchToAdd.Devices;
                return _deviceViewSource;
            }
            set
            {
                _deviceViewSource = value;
                OnPropertyChanged();
                Debug.WriteLine("Thing changed");
            }
        }
        public Material SelectedMaterial
        {
            get
            {
                return _selectedMaterial;
            }
            set
            {
                _selectedMaterial = value;
                OnPropertyChanged();
            }
        }
        public Solution SelectedSolution
        {
            get
            {
                return _selectedSolution;
            }
            set
            {
                _selectedSolution = value;
                OnPropertyChanged();
            }
        }
        public PhysicalRole SelectedRole
        {
            get
            {
                return _selectedRole;
            }
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
                OnRoleChanged();
            }
        }
        public DeviceTemplate SelectedDeviceTemplate
        {
            get
            {
                return _selectedDeviceTemplate;
            }
            set
            {
                _selectedDeviceTemplate = value;
                OnPropertyChanged();
            }
        }
        public LayerTemplate SelectedLayerTemplate
        {
            get
            {
                return _selectedLayerTemplate;
            }
            set
            {
                _selectedLayerTemplate = value;
                OnPropertyChanged();
            }
        }
        public Layer SelectedLayer
        {
            get
            {
                return _selectedLayer;
            }
            set
            {
                _selectedLayer = value;
                OnPropertyChanged();
                Debug.WriteLine("Selected Layer with Material: " + _selectedLayer.Material.Name);
            }
        }
        public Device SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged();
                Debug.WriteLine("Selected Device with Label: " + _selectedDevice.Label);
            }
        }
        public Employee SelectedEmployee
        {
            get
            {
                return _selectedEmployee;
            }
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<PhysicalRole> PhysicalRolesCollection
        {
            get { return _physicalRolesCollection; }
            set
            {
                _physicalRolesCollection = value;
                OnPropertyChanged();
            }
        }
        public DeviceBatch DeviceBatchToAdd
        {
            get { return _deviceBatchToAdd; }
            set
            {
                _deviceBatchToAdd = value;
                UpdateDeviceBatchToAdd();
                OnPropertyChanged();
                Debug.WriteLine("DeviceBatchToAdd changed");
            }
        }
        public UserControl ActiveUserControl
        {
            get { return _activeUserControl; }
            set
            {
                {
                    _activeUserControl = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<DeviceTemplate> DeviceTemplates
        {
            get { return _deviceTemplatesObservableCollection; }
            set
            {
                _deviceTemplatesObservableCollection = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<DeviceTemplate> TemplatesToAdd
        {
            get { return _templatesToAddCollection; }
            set
            {
                _templatesToAddCollection = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Material> MaterialsToAdd
        {
            get { return _materialsToAddCollection; }
            set
            {
                _materialsToAddCollection = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Solution> SolutionsToAdd
        {
            get { return _solutionsToAddCollection; }
            set
            {
                _solutionsToAddCollection = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<DeviceBatch> VisibleDeviceBatches
        {
            get
            {
                return _visibleDeviceBatches;
            }
            set
            {
                _visibleDeviceBatches = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Employee> VisibleEmployees
        {
            get
            {
                return _employeesCollection;
            }
            set
            {
                _employeesCollection = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<LayerTemplate> LayerTemplatesCollection
        {
            get
            {
                return _layerTemplatesCollection;
            }
            set
            {
                _layerTemplatesCollection = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> QDBatchesCollection
        {
            get { return _QDBatchesCollection; }
            set
            {
                _QDBatchesCollection = value;
                OnPropertyChanged();
            }
        }

        public DateTime BeginSearchDate
        {
            get { return _beginSearchDate; }
            set
            {
                _beginSearchDate = value;
                OnPropertyChanged();
            }
        }
        public DateTime EndSearchDate
        {
            get { return _endSearchDate; }
            set
            {
                _endSearchDate = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region Methods
        /// <summary>
        /// If the window is closed before the DeviceBatch is completed, delete it
        /// </summary>
        public void CheckToRemoveTemporaryDevices()
        {
            try
            {
                if (_windowIsClosedBeforeCompletion)
                {
                    ctx.DeviceBatches.Remove(DeviceBatchToAdd);
                    ctx.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        /// <summary>
        /// Adds a new DeviceTemplate to the collection after it is built from a DeviceTemplateBuilderWindow
        /// </summary>
        private void AcceptNewDevTemplateFromBuilder()
        {
            TemplatesToAdd.Add(_devTBVM.NewDeviceTemplate);
            OnPropertyChanged();
            FillDeviceTemplates();
            Debug.WriteLine("Added NewDeviceTemplate with a Device that has " + _devTBVM.NewDeviceTemplate.Device.NumberOfLayers + " layers");
        }
        /// <summary>
        /// Fill all observableCollections depending on selected build method
        /// </summary>
        /// <param name="isBuildingFromTemplates"></param>
        private void ConstructAndPopulateCollections(bool isBuildingFromTemplates)
        {
            var q = (from a in ctx.PhysicalRoles
                     select a).ToList();
            PhysicalRolesCollection = new ObservableCollection<PhysicalRole>(q);
            TemplatesToAdd = new ObservableCollection<DeviceTemplate>();
            MaterialsToAdd = new ObservableCollection<Material>();
            SolutionsToAdd = new ObservableCollection<Solution>();
            LayerTemplatesCollection = new ObservableCollection<LayerTemplate>();
            if (isBuildingFromTemplates && !_devicesHaveBeenConstructed)
            {
                DeviceTemplates = new ObservableCollection<DeviceTemplate>();
                FillDeviceTemplates();
                FillSolutions();
                FillPhysicalRoles();
                FillMaterials();
                AddTypicalMaterials();
            }
            if (!isBuildingFromTemplates && !_devicesHaveBeenConstructed)
            {
                VisibleDeviceBatches = new ObservableCollection<DeviceBatch>();
                FillDeviceBatchesLast30Days();
            }
            if (isBuildingFromTemplates && _devicesHaveBeenConstructed)
            {
                Debug.WriteLine("thing worked");
                UpdateCollectionsToAdd();
                UpdateLayerTemplatesCollection();
                FillSolutions();
                FillPhysicalRoles();
                FillMaterials();
                //AddTypicalMaterials();
            }
        }
        /// <summary>
        /// Cycle through devices and add each material/solution to collections
        /// </summary>
        private void UpdateCollectionsToAdd()
        {
            SolutionsToAdd = new ObservableCollection<Solution>();
            MaterialsToAdd = new ObservableCollection<Material>();
            foreach (Device d in DeviceBatchToAdd.Devices)
            {
                //assume that all devices will be modified because ChangeTracker (or me?) is dum
                foreach (Layer l in d.Layers)
                {
                    if (l.Solution != null && !SolutionsToAdd.Contains(l.Solution))
                        SolutionsToAdd.Add(l.Solution);
                    else
                    {
                        if (l.Material != null && !MaterialsToAdd.Contains(l.Material))
                            MaterialsToAdd.Add(l.Material);
                    }
                }
            }
        }
        /// <summary>
        /// populate VisibleSolutionsCollection from the database
        /// </summary>
        private void FillSolutions()
        {
            var q = (from a in ctx.Solutions
                     select a).ToList();
            this._visibleSolutionsCollection = new ObservableCollection<Solution>(q);
            this.SolutionsView = new ListCollectionView(_visibleSolutionsCollection);
        }
        /// <summary>
        /// populate PhysicalRolesCollection from the database 
        /// (the PhysicalRole entity should probably be replaced with a static enum at some point)
        /// </summary>
        private void FillPhysicalRoles()
        {
            try
            {
                var excludedRolesByShortName = new List<string>() { "AN", "CAT", "HBL", "ENC", "EIL" };
                var q = ctx.PhysicalRoles
                    .Where(p => !excludedRolesByShortName.Contains(p.ShortName));
                PhysicalRolesCollection = new ObservableCollection<PhysicalRole>(q);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        /// <summary>
        /// populate VisibleMaterialsCollection from the database
        /// </summary>
        private void FillMaterials()
        {
            //select materials that are not in solution and add them to the MaterialsView
            var q = ctx.Materials.Where(d => d.DepositionMethod.Name == "Thermal Evaporation" | d.DepositionMethod.Name == "Manual Pipetting" | d.DepositionMethod.Name == "Sputtering" | d.DepositionMethod.Name == "TCO Substrate")
                .ToList();
            this._visibleMaterialsCollection = new ObservableCollection<Material>(q);
            this.MaterialsView = new ListCollectionView(_visibleMaterialsCollection);
        }
        /// <summary>
        /// populate DeviceTemplates ObservableCollection from the database
        /// </summary>
        private void FillDeviceTemplates()
        {
            var q = (from a in ctx.DeviceTemplates
                     select a).ToList();
            this.DeviceTemplates = new ObservableCollection<DeviceTemplate>(q);
        }
        /// <summary>
        /// populate VisibleEmployees from the database
        /// </summary>
        private void FillEmployees()
        {
            var q = (from a in ctx.Employees
                     select a).ToList();
            foreach (Employee e in q)
            {
                Debug.WriteLine("Found an employee with first name:" + e.FirstName);
            }
            this.VisibleEmployees = new ObservableCollection<Employee>(q);
        }
        /// <summary>
        /// Add ITO, Aluminum, and AA 349 to MaterialsToAdd as they are almost always used in device manufacturing
        /// </summary>
        private void AddTypicalMaterials()
        {
            var q = ctx.Materials.Where(m => m.Name == "ITO").Single();
            MaterialsToAdd.Add(q);
            q = ctx.Materials.Where(m => m.Name == "Aluminum").Single();
            MaterialsToAdd.Add(q);
            q = ctx.Materials.Where(m => m.Name == "AA 349").Single();
            MaterialsToAdd.Add(q);
        }
        /// <summary>
        /// populate VisibleDeviceBatches with entities from the past 30 days
        /// </summary>
        private void FillDeviceBatchesLast30Days()
        {
            DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var q = ctx.DeviceBatches.Where(p => p.FabDate > thirtyDaysAgo)
                .ToList();
            foreach (DeviceBatch d in q)
            {
                VisibleDeviceBatches.Add(d);
            }
        }
        /// <summary>
        /// Make a new directory based upon supplied information in the NPI NAS (Synology ds418j)
        /// </summary>
        private void CreateSaveDirectory()
        {
            try
            {
                string monthString = DeviceBatchToAdd.FabDate.Month.ToString();
                if (Int32.Parse(monthString) < 10)
                    monthString = string.Concat("0", monthString);
                monthString = string.Concat(monthString, "-", DeviceBatchToAdd.FabDate.ToString("MMMM"));
                DeviceBatchToAdd.FilePath = string.Concat(
                    @"Z:\Data (LJV, Lifetime)\Device Batches\",
                    DeviceBatchToAdd.FabDate.Year, @"\",
                    monthString, @"\",
                    DeviceBatchToAdd.Name
                    /*
                    DeviceBatchToAdd.FabDate.ToString("MMddyy"),
                    " ",
                    SelectedEmployee.FirstName.Substring(0, 1),
                    SelectedEmployee.LastName.Substring(0, 1),
                    " (",
                    DeviceBatchToAdd.Name,
                    ")"
                    */
                    )
                ;
                Debug.WriteLine("Save directory filepath is: " + DeviceBatchToAdd.FilePath);
                if (!File.Exists(DeviceBatchToAdd.FilePath))
                    Directory.CreateDirectory(DeviceBatchToAdd.FilePath);
            }
            catch (Exception e)
            {
                string msgString = string.Concat("Is the NAS connected?", "\r\n", "\r\n", e.ToString());
                MessageBox.Show(msgString);
            }
        }
        /// <summary>
        /// Make new Devices with layer structures according to the selected DeviceTemplates and update particular properties
        /// </summary>
        private void CloneTemplatesAndAddToDevBatch()
        {
            MessageBox.Show("This functionality is presently not available");
            /*
            try
            {
                foreach (DeviceTemplate dt in TemplatesToAdd.ToList())
                {
                    for (int i = 0; i < dt.NumberOfCopies; i++)
                    {
                        Debug.WriteLine("Adding DeviceTemplate with structure: " + dt.Structure);
                        Device devToAdd = new Device();
                        devToAdd = ctx.Devices
                            .AsNoTracking()
                            .Include("Layers")
                            .Include("Pixels")
                            .FirstOrDefault(d => d.Id == dt.Device.Id);
                        devToAdd.DeviceTemplate = null;

                        ctx.Devices.Add(devToAdd);
                        DeviceBatchToAdd.Devices.Add(devToAdd);
                        DeviceBatchToAdd.Size++;
                        Debug.WriteLine("DeviceBatchToAdd.Size = " + DeviceBatchToAdd.Size);
                        devToAdd.BatchIndex = DeviceBatchToAdd.Size;
                        ctx.SaveChanges();
                        //ctx.Entry(devToAdd).Reload();
                    }
                    TemplatesToAdd.Remove(dt);
                }
                DeviceViewSource = new CollectionViewSource();
                DeviceViewSource.Source = DeviceBatchToAdd.Devices;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            */
        }
        /// <summary>
        /// If there is only one material for a particular physical role, assign it to all layers with that role
        /// </summary>
        private void AssignMaterialsWithUniquePhysicalRolesToLayers()
        {
            //first, find all physical roles used in DeviceTemplates collection
            HashSet<PhysicalRole> layerRolesUsed = new HashSet<PhysicalRole>();
            //find physical roles list from templates if building from templates (not sure if this is actually necessary)
            if (!_buildingFromPreviousBatch)
            {
                foreach (DeviceTemplate dt in DeviceTemplates)
                {
                    foreach (Layer l in dt.Device.Layers)
                    {
                        layerRolesUsed.Add(l.PhysicalRole);
                    }
                }
            }
            else//find physical roles from each device
            {
                foreach (Device d in DeviceBatchToAdd.Devices)
                {
                    foreach (Layer l in d.Layers)
                    {
                        layerRolesUsed.Add(l.PhysicalRole);
                    }
                }
            }
            //add materials and solutions with unique physical roles to lists
            List<Material> uniqueMatList = new List<Material>();
            List<Solution> uniqueSolList = new List<Solution>();
            foreach (PhysicalRole p in layerRolesUsed)
            {
                int roleCounter = 0;
                foreach (Material m in MaterialsToAdd)
                {
                    if (m.PhysicalRole == p)
                        roleCounter++;
                }
                if (roleCounter == 1)
                {
                    foreach (Material m in MaterialsToAdd)
                    {
                        if (m.PhysicalRole == p)
                        {
                            uniqueMatList.Add(m);
                            Debug.WriteLine("Added to uniqueMatList: " + m.Name);
                        }
                    }
                }
            }
            foreach (PhysicalRole p in layerRolesUsed)
            {
                int roleCounter = 0;
                foreach (Solution s in SolutionsToAdd)
                {
                    if (s.Material.PhysicalRole == p)
                        roleCounter++;
                }
                if (roleCounter == 1)
                {
                    foreach (Solution s in SolutionsToAdd)
                    {
                        if (s.Material.PhysicalRole == p)
                        {
                            uniqueSolList.Add(s);
                            Debug.WriteLine("Added to uniqueSolList: " + s.Material.Name);
                        }
                    }
                }
            }
            //assign unique materials and solutions to device layers
            foreach (Material m in uniqueMatList)
            {
                foreach (Device d in DeviceBatchToAdd.Devices)
                {
                    foreach (Layer l in d.Layers)
                    {
                        if (m.PhysicalRole.LongName == l.PhysicalRole.LongName)
                        {
                            Debug.WriteLine("Layer with Id " + l.LayerId + " has physical role " + l.PhysicalRole.LongName);
                            if (m.LayerTemplate.Layer != null)
                            {
                                Reflection.CopyLayerProperties(m.LayerTemplate.Layer, l);
                                l.LayerTemplate = null;
                                l.Material = m;
                            }
                        }
                    }
                }
            }
            foreach (Solution s in uniqueSolList)
            {
                foreach (Device d in DeviceBatchToAdd.Devices)
                {
                    foreach (Layer l in d.Layers)
                    {
                        if (s.Material.PhysicalRole.LongName == l.PhysicalRole.LongName)
                        {
                            if (s.Material.LayerTemplate.Layer != null)
                            {
                                Reflection.CopyLayerProperties(s.Material.LayerTemplate.Layer, l);
                                l.LayerTemplate = null;
                                l.Solution = s;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Populate "Label" property for Devices in DeviceBatchToAdd
        /// </summary>
        private void UpdateDeviceQDBatchAndLabels()
        {
            string fabDateString = string.Concat(DeviceBatchToAdd.FabDate.ToString("MMdd"), "-");
            foreach (Device d in DeviceBatchToAdd.Devices)
            {
                d.NumberOfScans = 0;

                foreach (Layer l in d.Layers)
                {
                    if (l.PhysicalRole.ShortName == "EML")
                    {
                        try
                        {
                            if (l.Solution != null)
                                d.QDBatch = (QDBatch)l.Solution.Material;
                        }
                        catch (Exception e)
                        {
                            //MessageBox.Show("Make sure only QD Batches are assigned to emissive layers" + Environment.NewLine + Environment.NewLine + e.ToString());
                        }
                    }
                }
                if (d.QDBatch != null)
                    d.Label = string.Concat(fabDateString, d.QDBatch.Name, "-", d.BatchIndex);
                else
                    d.Label = string.Concat(fabDateString, "CMMDDYY-", d.BatchIndex);
                Debug.WriteLine("Assigned new device label: " + d.Label);
            }
            //ActiveUserControl = new AssignMaterialsAndLayerPropertiesControl();
        }
        /// <summary>
        /// Associate the MaterialsToAdd and SolutionsToAdd collections with DeviceBatchToAdd
        /// </summary>
        /*
        private void AddMatsAndSolsToDeviceBatch()
        {
            //make sure that the DeviceBatch doesn't already possess each material, then add it if that's true
            foreach (Material m in MaterialsToAdd)
            {
                bool mIsInDBMatList = false;
                foreach (Material dbm in DeviceBatchToAdd.Materials)
                {
                    if (m.Id == dbm.Id)
                        mIsInDBMatList = true;
                }
                if (!mIsInDBMatList)
                    DeviceBatchToAdd.Materials.Add(m);
            }
            foreach (Solution s in SolutionsToAdd)
            {
                bool sIsInDBMatList = false;
                foreach (Solution dbs in DeviceBatchToAdd.Solutions)
                {
                    if (s.Id == dbs.Id)
                        sIsInDBMatList = true;
                }
                if (!sIsInDBMatList)
                    DeviceBatchToAdd.Solutions.Add(s);
            }
        }
        */
        /// <summary>
        /// Add all LayerTemplates to the ObservableCollection and set SelectedLayerTemplate to FirstOrDefault
        /// </summary>
        private void UpdateLayerTemplatesCollection()
        {
            foreach (Material m in MaterialsToAdd)
            {
                if (!LayerTemplatesCollection.Contains(m.LayerTemplate))
                {
                    m.LayerTemplate.Layer.Material = m;
                    LayerTemplatesCollection.Add(m.LayerTemplate);
                }
            }
            foreach (Solution s in SolutionsToAdd)
            {
                if (!LayerTemplatesCollection.Contains(s.Material.LayerTemplate))
                {
                    s.Material.LayerTemplate.Layer.Solution = s;
                    LayerTemplatesCollection.Add(s.Material.LayerTemplate);
                }
            }
            SelectedLayerTemplate = LayerTemplatesCollection.FirstOrDefault();
        }
        /// <summary>
        /// Filter SolutionsView to only display Solutions with the selected PhysicalRole
        /// </summary>
        private void OnRoleChanged()
        {
            SolutionsView.Filter = o => ((Solution)o).Material.PhysicalRole.LongName == SelectedRole.LongName;
        }

        /// <summary>
        /// A workaround to get the UI to update. Should be fixed at some point
        /// </summary>
        private void ReloadActiveUserControl()
        {
            //this is a reeeeeeaally stupid way to trigger ContentTemplateSelector, but it works
            var placeHolder = ActiveUserControl;
            ActiveUserControl = null;
            ActiveUserControl = placeHolder;
        }
        private void UpdateDeviceBatchName()
        {
            string updatedName = "";
            if (SelectedEmployee != null)
            {
                updatedName = string.Concat(DeviceBatchToAdd.FabDate.ToString("MMddyy"),
                        " ",
                        SelectedEmployee.FirstName.Substring(0, 1),
                        SelectedEmployee.LastName.Substring(0, 1));
            }
            /*
            if (DeviceBatchToAdd.Devices.Count > 0)
            {
                var qdBatches = new HashSet<string>();
                foreach (Device d in DeviceBatchToAdd.Devices)
                {
                    qdBatches.Add(d.QDBatch.Name);
                }
                foreach (string qdbname in qdBatches)
                {
                    updatedName = string.Concat(updatedName, " ", qdbname, ",");
                }
                updatedName = updatedName.Substring(1, updatedName.LastIndexOf(",") - 1);//remove the first space and last comma
            }
            //updatedName = string.Concat(" (", updatedName, ")");//add parentheses
            */
            DeviceBatchToAdd.Name = updatedName;
        }
        /// <summary>
        /// Update DeviceBatchToAdd properties and entities after the user makes modifications
        /// </summary>
        private void UpdateDeviceBatchToAdd()
        {
            foreach (Device d in DeviceBatchToAdd.Devices)
            {
                d.Layers.OrderBy(l => l.PositionIndex);
            }
            UpdateDeviceQDBatchAndLabels();
            UpdateDeviceBatchName();
            DeviceBatchToAdd.Devices.OrderBy(d => d.BatchIndex);
            DeviceViewSource = new CollectionViewSource();
            DeviceViewSource.Source = DeviceBatchToAdd.Devices;
            DeviceViewSource.SortDescriptions.Add(new SortDescription("BatchIndex", ListSortDirection.Ascending));
            DeviceViewSource.View.Refresh();
            ctx.SaveChanges();
            ReloadActiveUserControl();
        }
        private void PopulatePropertiesFromSpreadsheet()
        {
            DeviceBatchToAdd.Devices.Clear();
            DeviceBatchToAdd.FabDate = TryToAssignFabDate(DeviceBatchToAdd.SpreadSheetPath);
            foreach (Tuple<Device, string> d in ReadEMLsOnlyFromSpreadSheet(DeviceBatchToAdd.SpreadSheetPath))
            {
                DeviceBatchToAdd.Devices.Add(d.Item1);
                TryToAssignQDBatchFromString(d.Item1, d.Item2);
            }
            UpdateDeviceQDBatchAndLabels();
            UpdateDeviceBatchName();
            //RaisePropertyChanged("DeviceBatchToAdd");

        }
        private List<Tuple<Device, string>> ReadEMLsOnlyFromSpreadSheet(string filePath)
        {
            List<Tuple<Device, string>> devicesFromSpreadsheet = new List<Tuple<Device, string>>();//the string will record whatever text looks like a QD Batch
            //List<Device> devicesFromSpreadsheet = new List<Device>();
            var spreadsheetFile = new FileInfo(filePath);
            using (var spreadsheet = new ExcelPackage(spreadsheetFile))
            {
                var worksheet = spreadsheet.Workbook.Worksheets.First();//assume that everything is on the first worksheet;
                int columnIndex = 1;
                int EMLColumnIndex = -1;
                string cellText = "";
                bool foundEMLColumn = false;
                while (!foundEMLColumn)
                {
                    cellText = worksheet.Cells[1, columnIndex].Text.ToUpper();
                    Debug.WriteLine("Column header is: " + cellText + " at index: " + columnIndex);
                    if (cellText == "EML" | cellText == "EMISSIVE LAYER" | cellText.Contains("QD"))
                    {
                        EMLColumnIndex = columnIndex;
                        Debug.WriteLine("It looks like the EML column index is: " + EMLColumnIndex);
                        break;
                    }
                    columnIndex++;
                    if (columnIndex > 77)
                    {
                        System.Windows.Forms.MessageBox.Show("Looks like there is no EML column");
                        break;
                    }
                }
                int presentBatchIndexForDevice = 0;
                int rowCounter = 2; //to account for the case where devices aren't numbered sequentially or starting at 1, count with a different int
                List<Tuple<int, Device>> indexedDeviceList = new List<Tuple<int, Device>>();
                while (presentBatchIndexForDevice >= 0)
                {
                    cellText = worksheet.Cells[rowCounter, 1].Text;
                    if (cellText != "")
                    {
                        presentBatchIndexForDevice = Convert.ToInt32(cellText);
                        if (presentBatchIndexForDevice > 0)
                        {
                            var newTuple = new Tuple<int, Device>(rowCounter, new Device());
                            newTuple.Item2.BatchIndex = presentBatchIndexForDevice;
                            Pixel pixelA = new Pixel();
                            pixelA.Site = "SiteA";
                            newTuple.Item2.Pixels.Add(pixelA);
                            Pixel pixelB = new Pixel();
                            pixelB.Site = "SiteB";
                            newTuple.Item2.Pixels.Add(pixelB);
                            Pixel pixelC = new Pixel();
                            pixelC.Site = "SiteC";
                            newTuple.Item2.Pixels.Add(pixelC);
                            Pixel pixelD = new Pixel();
                            pixelD.Site = "SiteD";
                            newTuple.Item2.Pixels.Add(pixelD);
                            indexedDeviceList.Add(newTuple);
                        }
                    }
                    else
                    {
                        presentBatchIndexForDevice = -1;
                    }
                    Debug.WriteLine("1st column cell contains: " + cellText + " at index " + rowCounter);
                    rowCounter++;

                }
                foreach (Tuple<int, Device> indexedDev in indexedDeviceList)
                {
                    indexedDev.Item2.Label = "unlabeled";
                    string possibleQDBatch = null;
                    cellText = worksheet.Cells[indexedDev.Item1, EMLColumnIndex].Text;
                    if (cellText != "")
                    {
                        var newLayer = new Layer();
                        newLayer.PositionIndex = EMLColumnIndex;
                        newLayer.SpreadSheetCellText = cellText;
                        possibleQDBatch = ParseCellTextToMaterialName(cellText);
                        Debug.WriteLine("Looks like there's a QDBatch for device in row: " + indexedDev.Item1 + " named: " + possibleQDBatch);
                        newLayer.PhysicalRole = ctx.PhysicalRoles.Where(x => x.ShortName == "EML").First();
                        indexedDev.Item2.Layers.Add(newLayer);
                        indexedDev.Item2.NumberOfLayers++;
                    }
                    devicesFromSpreadsheet.Add(new Tuple<Device, string>(indexedDev.Item2, possibleQDBatch));
                }
            }
            return devicesFromSpreadsheet;
        }

        private List<Tuple<Device, string>> ReadDFSSpreadSheet(string filePath)
        {
            List<Tuple<Device, string>> devicesFromSpreadsheet = new List<Tuple<Device, string>>();//the string will record whatever text looks like a QD Batch
            //List<Device> devicesFromSpreadsheet = new List<Device>();
            var spreadsheetFile = new FileInfo(filePath);
            using (var spreadsheet = new ExcelPackage(spreadsheetFile))
            {
                var worksheet = spreadsheet.Workbook.Worksheets.First();//assume that everything is on the first worksheet;
                int commentColumnIndex = 1;
                int EMLColumnIndex = -1;
                string cellText = "";
                List<Tuple<int, string>> textRolesList = new List<Tuple<int, string>>();//usually each layer has a PhysicalRole but for now we will only track whatever text the user includes in the column header
                while (!cellText.Contains("COMMENT"))
                {
                    cellText = worksheet.Cells[1, commentColumnIndex].Text.ToUpper();
                    Debug.WriteLine("Column header is: " + cellText + " at index: " + commentColumnIndex);
                    if (!cellText.Contains("#") && !cellText.Contains("LABEL") && !cellText.Contains("EQE"))//ignore these columns
                        textRolesList.Add(new Tuple<int, string>(commentColumnIndex, cellText));
                    if (cellText == "EML" | cellText == "EMISSIVE LAYER" | cellText.Contains("QD"))
                    {
                        EMLColumnIndex = commentColumnIndex;
                        Debug.WriteLine("It looks like the EML column index is: " + EMLColumnIndex);
                    }
                    commentColumnIndex++;
                    if (commentColumnIndex > 77)
                    {
                        System.Windows.Forms.MessageBox.Show("Looks like there is no Comment column");
                        break;
                    }
                }
                string[] columnHeaders = new string[commentColumnIndex - 1];
                for (int i = 1; i < commentColumnIndex; i++)
                {
                    columnHeaders[i - 1] = worksheet.Cells[1, i].Text;
                }
                int presentBatchIndexForDevice = 0;
                int rowCounter = 2; //to account for the case where devices aren't numbered sequentially or starting at 1, count with a different int
                List<Tuple<int, Device>> indexedDeviceList = new List<Tuple<int, Device>>();
                while (presentBatchIndexForDevice >= 0)
                {
                    cellText = worksheet.Cells[rowCounter, 1].Text;
                    if (cellText != "")
                    {
                        presentBatchIndexForDevice = Convert.ToInt32(cellText);
                        if (presentBatchIndexForDevice > 0)
                        {
                            var newTuple = new Tuple<int, Device>(rowCounter, new Device());
                            newTuple.Item2.BatchIndex = presentBatchIndexForDevice;
                            Pixel pixelA = new Pixel();
                            pixelA.Site = "SiteA";
                            newTuple.Item2.Pixels.Add(pixelA);
                            Pixel pixelB = new Pixel();
                            pixelB.Site = "SiteB";
                            newTuple.Item2.Pixels.Add(pixelB);
                            Pixel pixelC = new Pixel();
                            pixelC.Site = "SiteC";
                            newTuple.Item2.Pixels.Add(pixelC);
                            Pixel pixelD = new Pixel();
                            pixelD.Site = "SiteD";
                            newTuple.Item2.Pixels.Add(pixelD);
                            indexedDeviceList.Add(newTuple);
                        }
                    }
                    else
                    {
                        presentBatchIndexForDevice = -1;
                    }
                    Debug.WriteLine("1st column cell contains: " + cellText + " at index " + rowCounter);
                    rowCounter++;

                }
                foreach (Tuple<int, Device> indexedDev in indexedDeviceList)
                {
                    indexedDev.Item2.Label = "unlabeled";
                    string possibleQDBatch = null;
                    foreach (Tuple<int, string> columnRole in textRolesList)
                    {
                        cellText = worksheet.Cells[indexedDev.Item1, columnRole.Item1].Text;
                        int layerPositionIndex = 0;
                        if (cellText != "")
                        {
                            layerPositionIndex++;
                            var newLayer = new Layer();
                            newLayer.PositionIndex = layerPositionIndex;
                            newLayer.SpreadSheetCellText = cellText;
                            TryToAssignPhysicalRoleFromString(newLayer, columnRole.Item2);
                            if (columnRole.Item1 == EMLColumnIndex)
                            {
                                possibleQDBatch = ParseCellTextToMaterialName(cellText);
                                Debug.WriteLine("Looks like there's a QDBatch for device in row: " + indexedDev.Item1 + " named: " + possibleQDBatch);
                                newLayer.PhysicalRole = ctx.PhysicalRoles.Where(x => x.ShortName == "EML").First();
                            }
                            else
                            {
                                var matName = ParseCellTextToMaterialName(cellText);
                                if (matName != "")
                                {
                                    TryToAssignDepositionMethodFromString(newLayer, matName);
                                    TryToAssignMaterialFromString(newLayer, matName);
                                }
                            }
                            indexedDev.Item2.Layers.Add(newLayer);
                            indexedDev.Item2.NumberOfLayers++;
                        }
                    }
                    devicesFromSpreadsheet.Add(new Tuple<Device, string>(indexedDev.Item2, possibleQDBatch));
                }
            }
            return devicesFromSpreadsheet;
        }
        private string ParseCellTextToMaterialName(string text)
        {
            string possibleMaterialName = "";
            int firstSpaceIndex = text.IndexOf(" ");
            //Debug.WriteLine("firstSpaceIndex is: " + firstSpaceIndex);
            int firstCommaIndex = text.IndexOf(",");
            //Debug.WriteLine("firstCommaIndex is: " + firstCommaIndex);
            if (firstSpaceIndex < firstCommaIndex && firstSpaceIndex > 0)
                possibleMaterialName = text.Substring(0, firstSpaceIndex);
            if (firstCommaIndex < firstSpaceIndex && firstCommaIndex > 0)
                possibleMaterialName = text.Substring(0, firstCommaIndex);
            if (firstCommaIndex == -1 && firstSpaceIndex > 0)
                possibleMaterialName = text.Substring(0, firstSpaceIndex);
            if (firstSpaceIndex == -1 && firstCommaIndex > 0)
                possibleMaterialName = text.Substring(0, firstCommaIndex);
            Debug.WriteLine("Looks like there's a material for device named: " + possibleMaterialName);
            return possibleMaterialName;
        }
        private void TryToAssignPhysicalRoleFromString(Layer l, string s)
        {
            PhysicalRole roleToAssign = ctx.PhysicalRoles.Where(x => x.ShortName == "UN").First();//unassigned role
            PhysicalRole roleFoundInDB = null;

            roleFoundInDB = ctx.PhysicalRoles.Where(x => x.ShortName == s | x.LongName == s).FirstOrDefault();

            if (roleFoundInDB != null)
            {
                roleToAssign = roleFoundInDB;
                Debug.WriteLine("Assigned role: " + roleToAssign.ShortName);
            }
            l.PhysicalRole = roleToAssign;
        }
        private void TryToAssignDepositionMethodFromString(Layer l, string s)
        {
            DepositionMethod methodToAssign = ctx.DepositionMethods.Where(x => x.Name == "Unassigned Method").First();
            //add logic here to check DB for material name deposition method in the future
            l.DepositionMethod = methodToAssign;
        }
        private void TryToAssignMaterialFromString(Layer l, string possibleMaterial)
        {
            var matFromDB = ctx.Materials.Where(x => x.Name == possibleMaterial).FirstOrDefault();
            if (matFromDB != null)
            {
                l.Material = matFromDB;
            }
            else//add a new material
            {
                Material newMaterial = new Material();
                try
                {
                    newMaterial.Name = possibleMaterial;
                    newMaterial.PhysicalRole = l.PhysicalRole;
                    newMaterial.DepositionMethod = l.DepositionMethod;
                    ctx.Materials.Add(newMaterial);
                    ctx.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following error: ", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Debug.WriteLine("- property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    Debug.WriteLine(e.ToString());
                }
            }
        }
        private void TryToAssignQDBatchFromString(Device dev, string possibleQDBatch)
        {
            Debug.WriteLine("Trying to assign QDBatch with name: " + possibleQDBatch);
            var qdBatchFromDB = ctx.Materials.Where(x => x.Name == possibleQDBatch).FirstOrDefault();
            if (qdBatchFromDB != null)
            {
                dev.QDBatch = (QDBatch)qdBatchFromDB;
            }
            else//add a new material 
            {
                QDBatch newQDBatch = new QDBatch();
                try
                {
                    newQDBatch.Name = possibleQDBatch;
                    newQDBatch.PhysicalRole = ctx.PhysicalRoles.Where(x => x.ShortName == "EML").First();
                    newQDBatch.DepositionMethod = ctx.DepositionMethods.Where(x => x.Name == "Spincoating").First();
                    ctx.Materials.Add(newQDBatch);
                    ctx.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following error: ", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Debug.WriteLine("- property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    Debug.WriteLine(e.ToString());
                }
                dev.QDBatch = newQDBatch;
            }
        }
        public DateTime TryToAssignFabDate(string filePath)
        {
            DateTime devbatchFabDateTime = DateTime.Now;
            try
            {
                int yearIndex = filePath.IndexOf(@"\", filePath.IndexOf(@"\", filePath.IndexOf(@"\") + 1) + 1) + 1;//assume directory structure never changes
                string yearSubString = filePath.Substring(yearIndex, 4);
                Debug.WriteLine("yearSubString is: " + yearSubString);
                int monthIndex = filePath.LastIndexOf(@"\") + 1;
                string monthSubString = filePath.Substring(monthIndex, 2);
                Debug.WriteLine("monthSubString is: " + monthSubString);
                int dayIndex = monthIndex + 2;
                string daySubString = filePath.Substring(dayIndex, 2);
                Debug.WriteLine("daySubString is: " + daySubString);
                devbatchFabDateTime = new DateTime(Convert.ToInt32(yearSubString), Convert.ToInt32(monthSubString), Convert.ToInt32(daySubString));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return devbatchFabDateTime;
        }
        public static void TryToFindDeviceBatchDirectory()
        {

        }
        #endregion
        #region ICommands
        private RelayCommand _updateDeviceBatchToAdd;
        public ICommand UpdateDeviceBatchToAddCommand
        {
            get
            {
                if (_updateDeviceBatchToAdd == null)
                {
                    _updateDeviceBatchToAdd = new RelayCommand(param => this.UpdateDeviceBatchToAddExecute(param));
                }
                return _updateDeviceBatchToAdd;
            }
        }
        public void UpdateDeviceBatchToAddExecute(object o)
        {
            UpdateDeviceBatchToAdd();
        }
        /// <summary>
        /// All ICommands associated with controls used in the DeviceBatch building process.
        /// This BatchBuilderViewModel should be broken into multiple classes at some point to improve maintainability
        /// </summary>
        private RelayCommand _goToNextLayerTemplate;
        public ICommand GoToNextLayerTemplate
        {
            get
            {
                if (_goToNextLayerTemplate == null)
                {
                    _goToNextLayerTemplate = new RelayCommand(param => this.GoToNextLayerTemplateExecute(param));
                }
                return _goToNextLayerTemplate;
            }
        }
        public void GoToNextLayerTemplateExecute(object o)
        {
            if (LayerTemplatesCollection.IndexOf(SelectedLayerTemplate) + 1 == LayerTemplatesCollection.Count())
                SelectedLayerTemplate = LayerTemplatesCollection[0];
            else
                SelectedLayerTemplate = LayerTemplatesCollection[LayerTemplatesCollection.IndexOf(SelectedLayerTemplate) + 1];
            //RaisePropertyChanged("SelectedLayerTemplate");
        }
        private RelayCommand _goToPreviousLayerTemplate;
        public ICommand GoToPreviousLayerTemplate
        {
            get
            {
                if (_goToPreviousLayerTemplate == null)
                {
                    _goToPreviousLayerTemplate = new RelayCommand(param => this.GoToPreviousLayerTemplateExecute(param));
                }
                return _goToPreviousLayerTemplate;
            }
        }
        public void GoToPreviousLayerTemplateExecute(object o)
        {
            if (LayerTemplatesCollection.IndexOf(SelectedLayerTemplate) == 0)
                SelectedLayerTemplate = LayerTemplatesCollection[LayerTemplatesCollection.Count() - 1];
            else
                SelectedLayerTemplate = LayerTemplatesCollection[LayerTemplatesCollection.IndexOf(SelectedLayerTemplate) - 1];
            //RaisePropertyChanged("SelectedLayerTemplate");
        }
        private RelayCommand _deleteDevice;
        public ICommand DeleteDevice
        {
            get
            {
                if (_deleteDevice == null)
                {
                    _deleteDevice = new RelayCommand(param => this.DeleteDeviceExecute(param));
                }
                return _deleteDevice;
            }
        }
        public void DeleteDeviceExecute(object o)
        {
            try
            {
                Device myDevice = new Device();
                if (o is Device)
                {
                    myDevice = (Device)o;
                    var batchIndex = myDevice.BatchIndex;
                    DeviceBatchToAdd.Devices.Remove(myDevice);
                    DeviceBatchToAdd.Size--;
                    foreach (Device d in DeviceBatchToAdd.Devices)
                    {
                        if (d.BatchIndex > batchIndex)
                            d.BatchIndex--;
                    }
                    ctx.SaveChanges();

                    //RaisePropertyChanged("DeviceBatchToAdd");
                    UpdateDeviceBatchToAdd();
                    ReloadActiveUserControl();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            /* can't do this if using DeviceViewSource
            if (SelectedDevice != null)
            {
                var batchIndex = SelectedDevice.BatchIndex;
                DeviceBatchToAdd.Devices.Remove(SelectedDevice);
                DeviceBatchToAdd.Size--;
                foreach (Device d in DeviceBatchToAdd.Devices)
                {
                    if (d.BatchIndex > batchIndex)
                        d.BatchIndex--;
                }
                ctx.SaveChanges();
                DeviceViewSource.SortDescriptions.Add(new SortDescription("BatchIndex", ListSortDirection.Descending));
                DeviceViewSource.View.Refresh();
                RaisePropertyChanged("DeviceBatchToAdd");
                UpdateDeviceBatchToAdd();
            }
            */
        }
        private RelayCommand _moveSelectedDeviceUpOneRow;
        public ICommand MoveSelectedDeviceUpOneRow
        {
            get
            {
                if (_moveSelectedDeviceUpOneRow == null)
                {
                    _moveSelectedDeviceUpOneRow = new RelayCommand(param => this.MoveSelectedDeviceUpOneRowExecute(param));
                }
                return _moveSelectedDeviceUpOneRow;
            }
        }
        public void MoveSelectedDeviceUpOneRowExecute(object o)
        {
            Debug.WriteLine("thing works");
            var thisDevice = (Device)o;
            var originalIndex = thisDevice.BatchIndex;
            if (originalIndex > 1) //make sure that the user isn't trying to break things
            {
                var deviceInRowAboveSelected = DeviceBatchToAdd.Devices.Where(d => d.BatchIndex == originalIndex - 1).First();
                thisDevice.BatchIndex--;
                deviceInRowAboveSelected.BatchIndex++;
                ctx.SaveChanges();

                //RaisePropertyChanged("DeviceBatchToAdd");
                UpdateDeviceBatchToAdd();
                Debug.WriteLine("Moved device up one row");
            }
        }
        private RelayCommand _moveSelectedDeviceDownOneRow;
        public ICommand MoveSelectedDeviceDownOneRow
        {
            get
            {
                if (_moveSelectedDeviceDownOneRow == null)
                {
                    _moveSelectedDeviceDownOneRow = new RelayCommand(param => this.MoveSelectedDeviceDownOneRowExecute(param));
                }
                return _moveSelectedDeviceDownOneRow;
            }
        }
        public void MoveSelectedDeviceDownOneRowExecute(object o)
        {
            var thisDevice = (Device)o;
            var originalIndex = thisDevice.BatchIndex;
            if (originalIndex < DeviceBatchToAdd.Devices.Count) //make sure that the user isn't trying to break things
            {
                var deviceInRowAboveSelected = DeviceBatchToAdd.Devices.Where(d => d.BatchIndex == originalIndex + 1).First();
                thisDevice.BatchIndex++;
                deviceInRowAboveSelected.BatchIndex--;
                ctx.SaveChanges();

                //RaisePropertyChanged("DeviceBatchToAdd");
                UpdateDeviceBatchToAdd();
            }
        }
        private RelayCommand _goBackToDevTemplatesSelect;
        public ICommand GoBackToDevTemplatesSelect
        {
            get
            {
                if (_goBackToDevTemplatesSelect == null)
                {
                    _goBackToDevTemplatesSelect = new RelayCommand(param => this.GoBackToDevTemplatesSelectExecute(param));
                }
                return _goBackToDevTemplatesSelect;
            }
        }
        public void GoBackToDevTemplatesSelectExecute(object o)
        {
            TemplatesToAdd = new ObservableCollection<DeviceTemplate>();
            ActiveUserControl = new DevTemplatesSelectControl();
            _devicesHaveBeenConstructed = false;

        }
        private RelayCommand _goBackToBatchMaterialsSelect;
        public ICommand GoBackToBatchMaterialsSelect
        {
            get
            {
                if (_goBackToBatchMaterialsSelect == null)
                {
                    _goBackToBatchMaterialsSelect = new RelayCommand(param => this.GoBackToBatchMaterialsSelectExecute(param));
                }
                return _goBackToBatchMaterialsSelect;
            }
        }
        public void GoBackToBatchMaterialsSelectExecute(object o)
        {
            ActiveUserControl = new BatchMaterialsSelectControl();
        }
        private RelayCommand _UpdateDevices;
        public ICommand UpdateDevices
        {
            get
            {
                if (_UpdateDevices == null)
                {
                    _UpdateDevices = new RelayCommand(param => this.UpdateDevicesExecute(param));
                }
                return _UpdateDevices;
            }
        }
        public void UpdateDevicesExecute(object o)
        {
            UpdateDeviceQDBatchAndLabels();
        }
        private RelayCommand _AddDevicesFromTemplates;
        public ICommand AddDevicesFromTemplates
        {
            get
            {
                if (_AddDevicesFromTemplates == null)
                {
                    _AddDevicesFromTemplates = new RelayCommand(param => this.AddDevicesFromTemplatesExecute(param));
                }
                return _AddDevicesFromTemplates;
            }
        }
        public void AddDevicesFromTemplatesExecute(object o)
        {
            if (!_devicesHaveBeenConstructed)
            {
                UpdateLayerTemplatesCollection();
                CloneTemplatesAndAddToDevBatch();
                //AssignMaterialsWithUniquePhysicalRolesToLayers();
                _devicesHaveBeenConstructed = true;
            }
            ActiveUserControl = new BatchMaterialsSelectControl();
        }
        private RelayCommand _GoToAssignMaterialsAndProperties;
        public ICommand GoToAssignMaterialsAndProperties
        {
            get
            {
                if (_GoToAssignMaterialsAndProperties == null)
                {
                    _GoToAssignMaterialsAndProperties = new RelayCommand(param => this.GoToAssignMaterialsAndPropertiesExecute(param));
                }
                return _GoToAssignMaterialsAndProperties;
            }
        }
        public void GoToAssignMaterialsAndPropertiesExecute(object o)
        {
            UpdateLayerTemplatesCollection();
            AssignMaterialsWithUniquePhysicalRolesToLayers();
            ActiveUserControl = new AssignMaterialsAndLayerPropertiesControl();
            UpdateDeviceBatchToAdd();
        }
        private RelayCommand _AddSelectedMaterial;
        public ICommand AddSelectedMaterial
        {
            get
            {
                if (_AddSelectedMaterial == null)
                {
                    _AddSelectedMaterial = new RelayCommand(param => this.AddSelectedMaterialExecute(param));
                }
                return _AddSelectedMaterial;
            }
        }
        public void AddSelectedMaterialExecute(object o)
        {
            try
            {
                MaterialsToAdd.Add(SelectedMaterial);
                //RaisePropertyChanged("MaterialsToAdd");
            }
            catch (Exception e)
            {
                MessageBox.Show("Is a material selected?" + Environment.NewLine + Environment.NewLine + e.ToString());
            }
        }
        private RelayCommand _DeleteSelectedMaterial;
        public ICommand DeleteSelectedMaterial
        {
            get
            {
                if (_DeleteSelectedMaterial == null)
                {
                    _DeleteSelectedMaterial = new RelayCommand(param => this.DeleteSelectedMaterialExecute(param));
                }
                return _DeleteSelectedMaterial;
            }
        }
        public void DeleteSelectedMaterialExecute(object o)
        {
            try
            {
                MaterialsToAdd.Remove(SelectedMaterial);
                //DeviceBatchToAdd.Materials.Remove(SelectedMaterial);
                //RaisePropertyChanged("MaterialsToAdd");
            }
            catch (Exception e)
            {
                MessageBox.Show("Is a material selected?" + Environment.NewLine + Environment.NewLine + e.ToString());

            }
        }
        private RelayCommand _AddSelectedSolution;
        public ICommand AddSelectedSolution
        {
            get
            {
                if (_AddSelectedSolution == null)
                {
                    _AddSelectedSolution = new RelayCommand(param => this.AddSelectedSolutionExecute(param));
                }
                return _AddSelectedSolution;
            }
        }
        public void AddSelectedSolutionExecute(object o)
        {
            try
            {
                SolutionsToAdd.Add(SelectedSolution);
                //RaisePropertyChanged("SolutionsToAdd");
            }
            catch (Exception e)
            {
                MessageBox.Show("Is a solution selected?" + Environment.NewLine + Environment.NewLine + e.ToString());

            }
        }
        private RelayCommand _DeleteSelectedSolution;
        public ICommand DeleteSelectedSolution
        {
            get
            {
                if (_DeleteSelectedSolution == null)
                {
                    _DeleteSelectedSolution = new RelayCommand(param => this.DeleteSelectedSolutionExecute(param));
                }
                return _DeleteSelectedSolution;
            }
        }
        public void DeleteSelectedSolutionExecute(object o)
        {
            try
            {
                SolutionsToAdd.Remove(SelectedSolution);
                //DeviceBatchToAdd.Solutions.Remove(SelectedSolution);
                //RaisePropertyChanged("SolutionsToAdd");
            }
            catch (Exception e)
            {
                MessageBox.Show("Is a solution selected?" + Environment.NewLine + Environment.NewLine + e.ToString());

            }
        }
        private RelayCommand _AddSelectedTemplate;
        public ICommand AddSelectedTemplate
        {
            get
            {
                if (_AddSelectedTemplate == null)
                {
                    _AddSelectedTemplate = new RelayCommand(param => this.AddSelectedTemplateExecute(param));
                }
                return _AddSelectedTemplate;
            }
        }
        public void AddSelectedTemplateExecute(object o)
        {
            try
            {
                if (SelectedDeviceTemplate != null)
                {
                    TemplatesToAdd.Add(SelectedDeviceTemplate);
                    //RaisePropertyChanged("TemplatesToAdd");
                }
                else
                {
                    MessageBox.Show("Please select a DeviceTemplate");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Is a template selected?" + Environment.NewLine + Environment.NewLine + e.ToString());

            }
        }
        private RelayCommand _AssignSelectedEmployee;
        public ICommand AssignSelectedEmployee
        {
            get
            {
                if (_AssignSelectedEmployee == null)
                {
                    _AssignSelectedEmployee = new RelayCommand(param => this.AssignSelectedEmployeeExecute(param));
                }
                return _AssignSelectedEmployee;
            }
        }
        public void AssignSelectedEmployeeExecute(object o)
        {
            try
            {
                DeviceBatchToAdd.Employee = SelectedEmployee;
                //RaisePropertyChanged("DeviceBatchToAdd");
                MessageBox.Show("Assigned " + SelectedEmployee.FirstName + " as DeviceBatch manufacturer");
            }
            catch (Exception e)
            {
                MessageBox.Show("Is an employee selected?" + Environment.NewLine + Environment.NewLine + e.ToString());

            }
        }
        private RelayCommand _DeleteSelectedTemplate;
        public ICommand DeleteSelectedTemplate
        {
            get
            {
                if (_DeleteSelectedTemplate == null)
                {
                    _DeleteSelectedTemplate = new RelayCommand(param => this.DeleteSelectedTemplateExecute(param));
                }
                return _DeleteSelectedTemplate;
            }
        }
        public void DeleteSelectedTemplateExecute(object o)
        {
            try
            {
                TemplatesToAdd.Remove(SelectedDeviceTemplate);
                //RaisePropertyChanged("TemplatesToAdd");
            }
            catch (Exception e)
            {
                MessageBox.Show("Is a template selected?" + Environment.NewLine + Environment.NewLine + e.ToString());

            }
        }
        private RelayCommand _BuildNewDeviceTemplate;
        public ICommand BuildNewDeviceTemplate
        {
            get
            {
                if (_BuildNewDeviceTemplate == null)
                {
                    _BuildNewDeviceTemplate = new RelayCommand(param => this.BuildNewDeviceTemplateExecute(param));
                }
                return _BuildNewDeviceTemplate;
            }
        }
        public void BuildNewDeviceTemplateExecute(object o)
        {
            _devTBVM = new DevTemplateBuilderVM();
            _devTBVM.DeviceTemplateBuilt += () => AcceptNewDevTemplateFromBuilder();
        }
        private RelayCommand _ChooseToBuildBatchFromTemplates;
        public ICommand ChooseToBuildBatchFromTemplates
        {
            get
            {
                if (_ChooseToBuildBatchFromTemplates == null)
                {
                    _ChooseToBuildBatchFromTemplates = new RelayCommand(param => this.ChooseToBuildBatchFromTemplatesExecute(param));
                }
                return _ChooseToBuildBatchFromTemplates;
            }
        }
        public void ChooseToBuildBatchFromTemplatesExecute(object o)
        {
            if (DeviceBatchToAdd.Employee != null)
            {
                ConstructAndPopulateCollections(true);
                //ActiveUserControl = new BatchBuilderControl1();
                ActiveUserControl = new DevTemplatesSelectControl();
            }
            else
            {
                MessageBox.Show("Please select an employee");
            }
        }
        private RelayCommand _ChooseToBuildBatchFromPrevious;
        public ICommand ChooseToBuildBatchFromPrevious
        {
            get
            {
                if (_ChooseToBuildBatchFromPrevious == null)
                {
                    _ChooseToBuildBatchFromPrevious = new RelayCommand(param => this.ChooseToBuildBatchFromPreviousExecute(param));
                }
                return _ChooseToBuildBatchFromPrevious;
            }
        }
        public void ChooseToBuildBatchFromPreviousExecute(object o)
        {
            if (DeviceBatchToAdd.Employee != null)
            {
                _buildingFromPreviousBatch = true;
                ConstructAndPopulateCollections(false);
                FilterVisibleDeviceBatches();
                ActiveUserControl = new DevBatchSelectControl();
            }
            else
            {
                MessageBox.Show("Please select an employee");
            }
        }
        private RelayCommand _ChooseToBuildFromSpreadsheet;
        public ICommand ChooseToBuildFromSpreadsheet
        {
            get
            {
                if (_ChooseToBuildFromSpreadsheet == null)
                {
                    _ChooseToBuildFromSpreadsheet = new RelayCommand(param => this.ChooseToBuildFromSpreadsheetExecute(param));
                }
                return _ChooseToBuildFromSpreadsheet;
            }
        }
        public void ChooseToBuildFromSpreadsheetExecute(object o)
        {
            if (DeviceBatchToAdd.Employee != null)
            {

                ActiveUserControl = new SpreadsheetBuildControl();
            }
            else
            {
                MessageBox.Show("Please select an employee");
            }
        }
        private RelayCommand _SelectSpreadSheet;
        public ICommand SelectSpreadSheet
        {
            get
            {
                if (_SelectSpreadSheet == null)
                {
                    _SelectSpreadSheet = new RelayCommand(param => this.SelectSpreadSheetExecute(param));
                }
                return _SelectSpreadSheet;
            }
        }
        public void SelectSpreadSheetExecute(object o)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.InitialDirectory = @"Z:\Data (LJV, Lifetime)\Device Batches\";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DeviceBatchToAdd.SpreadSheetPath = dialog.FileName;
                Debug.WriteLine("Selected file: " + DeviceBatchToAdd.SpreadSheetPath);
                PopulatePropertiesFromSpreadsheet();
            }
        }
        private RelayCommand _SaveDeviceBatchFromSpreadsheet;
        public ICommand SaveDeviceBatchFromSpreadsheet
        {
            get
            {
                if (_SaveDeviceBatchFromSpreadsheet == null)
                {
                    _SaveDeviceBatchFromSpreadsheet = new RelayCommand(param => this.SaveDeviceBatchFromSpreadsheetExecute(param));
                }
                return _SaveDeviceBatchFromSpreadsheet;
            }
        }
        public void SaveDeviceBatchFromSpreadsheetExecute(object o)
        {
            DeviceBatchToAdd.Size = DeviceBatchToAdd.Devices.Count;
            CreateSaveDirectory();
            ctx.SaveChanges();
            MessageBox.Show("Saved Device Batch");
            /*
            if (MessageBox.Show(
                 "Schedule Voltage Sweep Testing?",
                 "Schedule Testing?",
                 MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                EquipmentSchedulingViewModel ESVM = new EquipmentSchedulingViewModel(new DeviceBatchVM(DeviceBatchToAdd));
                EquipmentSchedulingWindow ESW = new EquipmentSchedulingWindow(ESVM);
                ESW.Show();
            }
            */
            if (DeviceBatchCreated != null)
            {
                var e = new DeviceBatchCreatedEventArgs { batchId = DeviceBatchToAdd.DeviceBatchId };
                DeviceBatchCreated(this, e);
            }
            _windowIsClosedBeforeCompletion = false;
            ParentWindow.Close();
        }
        private RelayCommand _copyLayerPropertiesForSelectedTemplate;
        public ICommand CopyLayerPropertiesForSelectedTemplate
        {
            get
            {
                if (_copyLayerPropertiesForSelectedTemplate == null)
                {
                    _copyLayerPropertiesForSelectedTemplate = new RelayCommand(param => this.CopyLayerPropertiesForSelectedTemplateExecute(param));
                }
                return _copyLayerPropertiesForSelectedTemplate;
            }
        }
        public void CopyLayerPropertiesForSelectedTemplateExecute(object o)
        {
            foreach (Device d in DeviceBatchToAdd.Devices)
            {
                foreach (Layer l in d.Layers)
                {
                    if (l.Solution != null && SelectedLayerTemplate.Layer.Solution != null && l.Solution.Material.Name == SelectedLayerTemplate.Layer.Solution.Material.Name)
                    {
                        Reflection.CopyLayerProperties(SelectedLayerTemplate.Layer, l);
                    }
                    else if (l.Material != null && SelectedLayerTemplate.Layer.Material != null && l.Material.Name == SelectedLayerTemplate.Layer.Material.Name)
                    {
                        Reflection.CopyLayerProperties(SelectedLayerTemplate.Layer, l);
                    }
                }
            }
            ReloadActiveUserControl();
        }
        private RelayCommand _DuplicateDevice;
        public ICommand DuplicateDevice
        {
            get
            {
                if (_DuplicateDevice == null)
                {
                    _DuplicateDevice = new RelayCommand(param => this.DuplicateDeviceExecute(param));
                }
                return _DuplicateDevice;
            }
        }
        public void DuplicateDeviceExecute(object o)
        {
            try
            {
                Device thisDevice = (Device)o;
                int thisDeviceBatchIndex = thisDevice.BatchIndex;
                QDBatch thisDeviceQDBatch = thisDevice.QDBatch;
                //DB schematic is flawed: the QDBatch entity is unnecessary and needs to be removed but that requires a complete overhaul of codebase
                //instead of using QDBatches all logic can be replaced by selecting materials with PhysicalRole "Emissive Layer/EML"
                List<Tuple<int, Material, Solution>> DeviceLayersMaterialsSolution = new List<Tuple<int, Material, Solution>>();//<PositionIndex, Material, Solution>

                foreach (Layer l in thisDevice.Layers)
                {
                    if (l.Solution == null && l.Material != null)
                    {
                        DeviceLayersMaterialsSolution.Add(new Tuple<int, Material, Solution>(l.PositionIndex, l.Material, null));
                    }
                    if (l.Solution != null)
                    {
                        DeviceLayersMaterialsSolution.Add(new Tuple<int, Material, Solution>(l.PositionIndex, null, l.Solution));
                    }
                }
                Device newDevice = ctx.Devices.AsNoTracking()
                    .Include("Layers")
                    .Include("Pixels")
                    .Where(x => x.DeviceId == thisDevice.DeviceId)
                    .Single();
                newDevice.NumberOfScans = 0;
                newDevice.DeviceLJVScanSummaries = null;
                newDevice.BatchIndex = thisDeviceBatchIndex + 1;
                newDevice.QDBatch = thisDeviceQDBatch;
                foreach (Layer l in newDevice.Layers)
                {
                    l.PhysicalRole = ctx.PhysicalRoles.Where(x => x.LongName == l.PhysicalRole.LongName).First();//make sure we don't add copies of every PhysicalRole
                    if (l.Solution == null && l.Material != null)//remap materials to layers to avoid duplicating materials
                    {
                        Material thisMaterial = DeviceLayersMaterialsSolution.Where(x => x.Item1 == l.PositionIndex).First().Item2;
                        l.Material = ctx.Materials.Where(x => x.MaterialId == thisMaterial.MaterialId).First();
                    }
                    if (l.Solution != null)//remap solutions to layers to avoid duplicating solutions
                    {
                        Solution thisSolution = DeviceLayersMaterialsSolution.Where(x => x.Item1 == l.PositionIndex).First().Item3;
                        l.Solution = ctx.Solutions.Where(x => x.SolutionId == thisSolution.SolutionId).First();
                    }
                }
                foreach (Device d in DeviceBatchToAdd.Devices)//increase the batchIndex for each subsequent device so that newDevice is inserted after the selected device instead of at the end
                {
                    if (d.BatchIndex > thisDeviceBatchIndex)
                        d.BatchIndex++;
                }
                ctx.Devices.Add(newDevice);
                DeviceBatchToAdd.Devices.Add(newDevice);
                DeviceBatchToAdd.Size++;
                UpdateDeviceBatchToAdd();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _DeleteLayer;
        public ICommand DeleteLayer
        {
            get
            {
                if (_DeleteLayer == null)
                {
                    _DeleteLayer = new RelayCommand(param => this.DeleteLayerExecute(param));
                }
                return _DeleteLayer;
            }
        }
        public void DeleteLayerExecute(object o)
        {
            try
            {
                Layer thisLayer = (Layer)o;
                if (MessageBox.Show(
                    String.Concat("Delete ", thisLayer.PhysicalRole.ShortName, " for Device ", thisLayer.Device.BatchIndex, "?"),
                    "Delete Layer",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int thisLayerPositionIndex = thisLayer.PositionIndex;
                    Device thisDevice = thisLayer.Device;
                    thisDevice.Layers.Remove(thisLayer);
                    foreach (Layer l in thisDevice.Layers)
                    {
                        if (l.PositionIndex > thisLayerPositionIndex)
                            l.PositionIndex--;
                    }
                    UpdateDeviceBatchToAdd();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _SaveAndCommitDeviceBatch;
        public ICommand SaveAndCommitDeviceBatch
        {
            get
            {
                if (_SaveAndCommitDeviceBatch == null)
                {
                    _SaveAndCommitDeviceBatch = new RelayCommand(param => this.SaveAndCommitDeviceBatchExecute(param));
                }
                return _SaveAndCommitDeviceBatch;
            }
        }
        public void SaveAndCommitDeviceBatchExecute(object o)
        {
            try
            {
                if (!_devicesHaveBeenConstructed)
                {
                    UpdateDeviceBatchToAdd();
                    CreateSaveDirectory();
                    SpreadsheetGenerator.GenerateDeviceFabricationSpecification(DeviceBatchToAdd);
                    ctx.SaveChanges();
                    MessageBox.Show("Added New Device Batch");
                }
                if (_devicesHaveBeenConstructed)
                {
                    UpdateDeviceBatchToAdd();
                    CreateSaveDirectory();
                    SpreadsheetGenerator.GenerateDeviceFabricationSpecification(DeviceBatchToAdd);
                    Debug.WriteLine("Detected changes: " + ctx.ChangeTracker.HasChanges());
                    ctx.SaveChanges();
                    MessageBox.Show("Updated Device Batch");
                }
                if (MessageBox.Show(
                   "Schedule Voltage Sweep Testing?",
                   "Schedule Testing?",
                   MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    EquipmentSchedulingViewModel ESVM = new EquipmentSchedulingViewModel(new DeviceBatchVM(DeviceBatchToAdd));
                    EquipmentSchedulingWindow ESW = new EquipmentSchedulingWindow(ESVM);
                    ESW.Show();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.InnerException.ToString());
            }

            _windowIsClosedBeforeCompletion = false;
            ParentWindow.Close();
        }
        #endregion
        #region Build from previous batch stuff
        private void FilterVisibleDeviceBatches()
        {
            var q = ctx.DeviceBatches
                .Where(p => p.FabDate >= BeginSearchDate && p.FabDate <= EndSearchDate)
                .OrderByDescending(p => p.FabDate)
                .ToList();
            VisibleDeviceBatches = new ObservableCollection<DeviceBatch>(q);
        }
        private RelayCommand _FilterDevBatches;
        public ICommand FilterDevBatches
        {
            get
            {
                if (_FilterDevBatches == null)
                {
                    _FilterDevBatches = new RelayCommand(param => this.FilterDevBatchesExecute(param));
                }
                return _FilterDevBatches;
            }
        }
        public void FilterDevBatchesExecute(object o)
        {
            FilterVisibleDeviceBatches();
        }
        private RelayCommand _UseSelectedBatch;
        public ICommand UseSelectedBatch
        {
            get
            {
                if (_UseSelectedBatch == null)
                {
                    _UseSelectedBatch = new RelayCommand(param => this.UseSelectedBatchExecute(param));
                }
                return _UseSelectedBatch;
            }
        }
        public void UseSelectedBatchExecute(object o)
        {
            ctx.DeviceBatches.Remove(DeviceBatchToAdd);
            DeviceBatch myDeviceBatch;
            if (o is DeviceBatch)
            {
                myDeviceBatch = (DeviceBatch)o;
                HashSet<Material> uniqueMatsInPreviousBatch = new HashSet<Material>();
                HashSet<Solution> uniqueSolsInPreviousBatch = new HashSet<Solution>();
                List<Tuple<int, QDBatch>> DeviceQDBatches = new List<Tuple<int, QDBatch>>(); //need to find these values first because AsNoTracking duplicates the QDBatch
                //DB schematic is flawed: the QDBatch entity is unnecessary and needs to be removed but that requires a complete overhaul of codebase
                //instead of using QDBatches all logic can be replaced by selecting materials with PhysicalRole "Emissive Layer/EML"
                List<Tuple<int, int, Material, Solution>> DeviceLayersMaterialsSolution = new List<Tuple<int, int, Material, Solution>>();//<BatchIndex, PositionIndex, Material, Solution>
                foreach (Device d in myDeviceBatch.Devices)
                {
                    foreach (Layer l in d.Layers)
                    {
                        if (l.Solution == null && l.Material != null)
                        {

                            uniqueMatsInPreviousBatch.Add(l.Material);
                            DeviceLayersMaterialsSolution.Add(new Tuple<int, int, Material, Solution>(d.BatchIndex, l.PositionIndex, l.Material, null));
                        }
                        if (l.Solution != null)
                        {
                            uniqueSolsInPreviousBatch.Add(l.Solution);
                            DeviceLayersMaterialsSolution.Add(new Tuple<int, int, Material, Solution>(d.BatchIndex, l.PositionIndex, null, l.Solution));
                            if (l.Solution.Material.MaterialId == d.QDBatch.MaterialId)
                                DeviceQDBatches.Add(new Tuple<int, QDBatch>(d.BatchIndex, d.QDBatch));
                        }
                    }
                }
                DeviceBatchToAdd = ctx.DeviceBatches.AsNoTracking()
                   .Include("Devices")
                   .Include("Devices.Layers")
                   .Include("Devices.Pixels")
                   .Where(x => x.DeviceBatchId == myDeviceBatch.DeviceBatchId).Single();
                foreach (Device d in DeviceBatchToAdd.Devices)
                {
                    d.NumberOfScans = 0;
                    d.DeviceLJVScanSummaries = null;
                    d.QDBatch = DeviceQDBatches.Where(x => x.Item1 == d.BatchIndex).First().Item2;//assign the QDBatch from the list by BatchIndex
                    foreach (Layer l in d.Layers)
                    {
                        l.PhysicalRole = ctx.PhysicalRoles.Where(x => x.LongName == l.PhysicalRole.LongName).First();//make sure we don't add copies of every PhysicalRole
                        if (l.Solution == null && l.Material != null)//remap materials to layers to avoid duplicating materials
                        {
                            Material thisMaterial = DeviceLayersMaterialsSolution.Where(x => x.Item1 == d.BatchIndex).Where(x => x.Item2 == l.PositionIndex).First().Item3;
                            l.Material = ctx.Materials.Where(x => x.MaterialId == thisMaterial.MaterialId).First();
                        }
                        if (l.Solution != null)//remap solutions to layers to avoid duplicating solutions
                        {
                            Solution thisSolution = DeviceLayersMaterialsSolution.Where(x => x.Item1 == d.BatchIndex).Where(x => x.Item2 == l.PositionIndex).First().Item4;
                            l.Solution = ctx.Solutions.Where(x => x.SolutionId == thisSolution.SolutionId).First();
                        }
                    }
                }
                DeviceBatchToAdd.FabDate = DateTime.Now;
                DeviceBatchToAdd.Employee = SelectedEmployee;
                ctx.DeviceBatches.Add(DeviceBatchToAdd);
                myDeviceBatch = null;//don't unintentionally save extra shit to DB...this doesn't actually do anything?
                ctx.SaveChanges();
                foreach (Material m in uniqueMatsInPreviousBatch)
                {
                    MaterialsToAdd.Add(m);
                    Debug.WriteLine("Found unique material with name: " + m.Name);
                }
                foreach (Solution s in uniqueSolsInPreviousBatch)
                {
                    SolutionsToAdd.Add(s);
                    Debug.WriteLine("Found unique solution with name: " + s.Label);
                }
                FillMaterials();
                FillSolutions();
                ActiveUserControl = new BatchMaterialsSelectControl();
                UpdateDeviceBatchToAdd();
            }
        }
        #endregion
    }
    public class DeviceBatchCreatedEventArgs : EventArgs
    {
        public int batchId { get; set; }
    }
}
