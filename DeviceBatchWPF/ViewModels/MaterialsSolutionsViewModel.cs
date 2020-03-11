using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Data;
using System.Data.Entity.Validation;
using EFDeviceBatchCodeFirst;
using DeviceBatchGenerics.Support;
using DeviceBatchGenerics.Support.Bases;
using DeviceBatchWPF.Windows;
using DeviceBatchWPF.Controls;

namespace DeviceBatchWPF.ViewModels
{
    public class MaterialsSolutionsViewModel : CrudVMBase
    {
        public MaterialsSolutionsViewModel()
        {
            FillAllEntities();
            PopulateDictionaries();
            SetDefaultValues();
            ConstructNewEntities();
            //ConstructNewMaterialToAdd();
            ConstructNewQDBatchToAdd();
            SolutionToAdd = new Solution();
            solWindow = new SolutionsWindow();
        }
        #region Members
        Dictionary<string, System.Windows.Controls.UserControl> _controlsDict;
        Dictionary<string, PhysicalRole> _physicalRolesDict;
        Dictionary<string, DepositionMethod> _depositionMethodsDict;
        System.Windows.Controls.UserControl _activeUserControl;
        PhysicalRole _selectedRole;
        DepositionMethod _selectedMethod;
        Material _materialToAdd;
        ObservableCollection<Material> _visibleMaterials;
        QDBatch _QDBatchToAdd;
        ObservableCollection<QDBatch> _visibleQDBatches;
        Solution _solutionToAdd;
        ObservableCollection<Solution> _visibleSolutions;
        SolutionsWindow solWindow;
        List<string> _QDColorsList = new List<string>() { "Red", "Green", "Blue" };
        ListCollectionView _materialsView;
        ListCollectionView _solutionsView;
        bool _editingEnabled = true;
        LayerTemplate _layerTemplateToAdd;
        #endregion
        #region Properties
        public Dictionary<string, DepositionMethod> DepositionMethodsDict
        {

            get { return _depositionMethodsDict; }
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
        public Dictionary<string, System.Windows.Controls.UserControl> ControlsDict
        {

            get { return _controlsDict; }
            set
            {
                _controlsDict = value;
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
                if (MaterialToAdd != null && LayerTemplateToAdd != null)
                {
                    MaterialToAdd.PhysicalRole = value;
                    LayerTemplateToAdd.Layer.PhysicalRole = value;
                    ReloadActiveUserControl();
                }
                OnRoleChanged();
                OnPropertyChanged();
                Debug.WriteLine("SelectedRole Changed");
            }
        }
        public DepositionMethod SelectedMethod
        {
            get
            {
                return _selectedMethod;
            }
            set
            {
                _selectedMethod = value;
                if (MaterialToAdd != null && LayerTemplateToAdd != null)
                {
                    MaterialToAdd.DepositionMethod = value;
                    LayerTemplateToAdd.Layer.DepositionMethod = value;
                    ReloadActiveUserControl();
                    Debug.WriteLine("LayerTemplateToAdd.Layer.DepositionMethod.Name = " + LayerTemplateToAdd.Layer.DepositionMethod.Name);
                }
                OnPropertyChanged();
            }
        }
        public System.Windows.Controls.UserControl ActiveUserControl
        {
            get { return _activeUserControl; }
            set
            {
                {
                    _activeUserControl = value;
                    OnPropertyChanged();
                    Debug.WriteLine("ActiveUserControl changed");
                }
            }
        }
        public Material MaterialToAdd
        {
            get { return _materialToAdd; }
            set
            {
                _materialToAdd = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Material> VisibleMaterials
        {
            get { return _visibleMaterials; }
            set
            {
                _visibleMaterials = value;
                OnPropertyChanged();
            }
        }
        public QDBatch QDBatchToAdd
        {
            get { return _QDBatchToAdd; }
            set
            {
                _QDBatchToAdd = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<QDBatch> VisibleQDBatches
        {
            get { return _visibleQDBatches; }
            set
            {
                _visibleQDBatches = value;
                OnPropertyChanged();
            }
        }
        public Solution SolutionToAdd
        {
            get { return _solutionToAdd; }
            set
            {
                _solutionToAdd = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Solution> VisibleSolutions
        {
            get { return _visibleSolutions; }
            set
            {
                _visibleSolutions = value;
                OnPropertyChanged();
            }
        }
        public List<string> QDColorsList
        {
            get { return _QDColorsList; }
            set
            {
                _QDColorsList = value;
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
        public bool EditingEnabled
        {
            get { return _editingEnabled; }
            set
            {
                _editingEnabled = value;
                OnPropertyChanged();
            }
        }
        public LayerTemplate LayerTemplateToAdd
        {
            get
            {
                return _layerTemplateToAdd;
            }
            set
            {
                _layerTemplateToAdd = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region Methods
        private void SetDefaultValues()
        {
            ActiveUserControl = ControlsDict["Materials"];
            var newSelectedRole = SelectedRole;
            if (SelectedRole == null)
                newSelectedRole = ctx.PhysicalRoles.Where(p => p.ShortName == "HTL").Single();
            var newSelectedMethod = SelectedMethod;
            if (SelectedMethod == null)
                newSelectedMethod = ctx.DepositionMethods.Where(d => d.Name == "Spincoating").Single();
            SelectedRole = newSelectedRole;
            SelectedMethod = newSelectedMethod;
        }
        private void ConstructNewMaterialToAdd()
        {
            MaterialToAdd = new Material();
            MaterialToAdd.Name = "Unnamed material";
            ctx.Materials.Add(MaterialToAdd);
            LayerTemplateToAdd.Layer.Material = MaterialToAdd;
        }
        private void ConstructNewQDBatchToAdd()
        {
            QDBatchToAdd = new QDBatch();
            QDBatchToAdd.Color = "Red";
            QDBatchToAdd.PhysicalRole = ctx.PhysicalRoles.Where(p => p.ShortName == "EML").Single();
            QDBatchToAdd.DepositionMethod = ctx.DepositionMethods.Where(d => d.Name == "Spincoating").Single();
            //ctx.Materials.Add(QDBatchToAdd);
        }
        private void ConstructNewEntities()
        {
            try
            {
                //due to DB schematic restrictions, construction must be done in this order to allow for usage of LayerTemplates
                var newLayer = new Layer();
                Debug.WriteLine("SelectedMethod is " + SelectedMethod.Name);
                Debug.WriteLine("SelectedRole is " + SelectedRole.LongName);
                newLayer.DepositionMethod = SelectedMethod;
                newLayer.PhysicalRole = SelectedRole;
                newLayer.Comment = "Template";
                ctx.Layers.Add(newLayer);
                //ctx.SaveChanges();
                MaterialToAdd = new Material();
                MaterialToAdd.Name = "Unnamed material";
                MaterialToAdd.DepositionMethod = SelectedMethod;
                MaterialToAdd.PhysicalRole = SelectedRole;
                ctx.Materials.Add(MaterialToAdd);
                //ctx.SaveChanges();
                LayerTemplateToAdd = new LayerTemplate();
                LayerTemplateToAdd.Layer = newLayer;
                LayerTemplateToAdd.Material = MaterialToAdd;
                ctx.LayerTemplates.Add(LayerTemplateToAdd);
                //ctx.SaveChanges();
                ConstructNewQDBatchToAdd();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors: ", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                MessageBox.Show(e.ToString());
            }
        }
        public void RemoveNullEntities()
        {
            //DB schematic is flawed: saving Materials adds an "Unnamed material" every time so this is a workaround until schematic revision
            var nullMats = ctx.Materials.Where(x => x.Name == "Unnamed material").ToList();
            foreach (Material m in nullMats)
            {
                ctx.LayerTemplates.Remove(m.LayerTemplate);
                ctx.Materials.Remove(m);
            }
            ctx.SaveChanges();
        }
        private void RemoveTemporaryEntities()
        {
            //remove temporary entities to avoid saving them along with edits. This needs to be reworked at some point
            ctx.LayerTemplates.Remove(LayerTemplateToAdd);
            try
            {
                ctx.Layers.Remove(MaterialToAdd.Layers.First());
            }
            catch { }
            ctx.Materials.Remove(MaterialToAdd);
            ctx.SaveChanges();
        }
        private void UpdateInconsistentPhysicalRoles()
        {
            List<Material> moddedMats = ctx.Materials.Where(x => x.PhysicalRole != x.LayerTemplate.Layer.PhysicalRole).ToList();
            foreach (Material mat in moddedMats)
            {
                Debug.WriteLine("Found a modified material: " + mat.Name);
                try
                {
                    mat.LayerTemplate.Layer.PhysicalRole = mat.PhysicalRole;
                }
                catch
                { }
            }
        }
        private void FillAllEntities()
        {
            FillMaterials();
            FillQDBatches();
            FillSolutions();
        }
        private void PopulateDictionaries()
        {
            ControlsDict = new Dictionary<string, System.Windows.Controls.UserControl>();
            ControlsDict.Add("Materials", new MaterialsControl());
            ControlsDict.Add("QD Batches", new QDBatchesControl());
            ControlsDict.Add("Solutions", new SolutionsControl());
            PhysicalRolesDict = new Dictionary<string, PhysicalRole>();
            List<PhysicalRole> pRoles = ctx.PhysicalRoles.ToList();
            pRoles.OrderBy(x => x.LongName);
            foreach (PhysicalRole p in pRoles)
            {
                PhysicalRolesDict.Add(p.LongName, p);
            }
            DepositionMethodsDict = new Dictionary<string, DepositionMethod>();
            var depMethods = ctx.DepositionMethods.ToList();
            foreach (DepositionMethod d in depMethods)
            {
                DepositionMethodsDict.Add(d.Name, d);
            }
        }
        private void FillMaterials()
        {
            //ctx.Database.Connection.Open();
            var q = (from a in ctx.Materials.OrderByDescending(mat => mat.MaterialId)
                     select a).ToList();
            this.VisibleMaterials = new ObservableCollection<Material>(q);
            this.MaterialsView = new ListCollectionView(VisibleMaterials);
        }
        private void FillQDBatches()
        {
            var mats = (from a in ctx.Materials.OrderByDescending(mat => mat.MaterialId)
                        select a).ToList();
            ObservableCollection<QDBatch> newQDBatchCollection = new ObservableCollection<QDBatch>();
            foreach (Material m in mats)
            {
                if (m is QDBatch)
                {
                    QDBatch qq;
                    qq = (QDBatch)m;
                    newQDBatchCollection.Add(qq);
                }
            }
            this.VisibleQDBatches = newQDBatchCollection;
        }
        private void FillSolutions()
        {
            //ctx.Database.Connection.Open();
            var q = (from a in ctx.Solutions.OrderByDescending(mat => mat.SolutionId)
                     select a).ToList();
            this.VisibleSolutions = new ObservableCollection<Solution>(q);
            this.SolutionsView = new ListCollectionView(VisibleSolutions);
        }
        private void OnRoleChanged()
        {
            try
            {
                if (MaterialsView != null)
                    MaterialsView.Filter = o => ((Material)o).PhysicalRole.LongName == SelectedRole.LongName;
                if (SolutionsView != null)
                    SolutionsView.Filter = o => ((Solution)o).Material.PhysicalRole.LongName == SelectedRole.LongName;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void OnDepositionMethodChanged()
        {


        }
        private void ReloadActiveUserControl()
        {
            //this is a reeeeeeaally stupid way to trigger ContentTemplateSelector, but it works
            var placeHolder = ActiveUserControl;
            ActiveUserControl = null;
            ActiveUserControl = placeHolder;
        }
        private void PromptForCreateSolution(Material mat)
        {
            var dialogResult = MessageBox.Show(
                string.Concat("Create solution for ", mat.Name, "?"),
                "Create solution?",
                MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Yes)
            {
                CreateSolutionExecute(mat);
            }

        }
        #endregion
        #region ICommands
        private RelayCommand _SaveEdits;
        public ICommand SaveEdits
        {
            get
            {
                if (_SaveEdits == null)
                {
                    _SaveEdits = new RelayCommand(param => this.SaveEditsExecute());
                }
                return _SaveEdits;
            }
        }
        public void SaveEditsExecute()
        {
            try
            {
                RemoveNullEntities();
                UpdateInconsistentPhysicalRoles();
                ctx.SaveChanges();
                MessageBox.Show("Changes Saved");
                ConstructNewEntities();
                UpdateInconsistentPhysicalRoles();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors: ", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                MessageBox.Show(e.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _AddMaterial;
        public ICommand AddMaterial
        {
            get
            {
                if (_AddMaterial == null)
                {
                    _AddMaterial = new RelayCommand(param => this.AddMaterialExecute());
                }
                return _AddMaterial;
            }
        }
        public void AddMaterialExecute()
        {
            try
            {
                if (MaterialToAdd.PhysicalRole.ShortName == "EML")
                    MessageBox.Show("Please change to QD Batch view to add EMLs");
                else
                {
                    //ctx.Materials.Add(MaterialToAdd);
                    ctx.SaveChanges();
                    if (MaterialToAdd.DepositionMethod.Name == "Spincoating" | MaterialToAdd.DepositionMethod.Name == "Inkjet Printing")
                        PromptForCreateSolution(MaterialToAdd);
                    ActiveUserControl = ControlsDict["Materials"];
                    ConstructNewEntities();
                    //ConstructNewMaterialToAdd();
                    FillMaterials();
                    OnRoleChanged();
                }
            }
            catch (DbEntityValidationException e)
            {
                MessageBox.Show(e.ToString());
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors: ", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }
        }
        private RelayCommand _DeleteMaterial;
        public ICommand DeleteMaterial
        {
            get
            {
                if (_DeleteMaterial == null)
                {
                    _DeleteMaterial = new RelayCommand(param => this.DeleteMaterialExecute(param));
                }
                return _DeleteMaterial;
            }
        }
        public void DeleteMaterialExecute(object o)
        {
            try
            {
                Material myMaterial = new Material();
                if (o is Material)
                {
                    myMaterial = (Material)o;
                    if (MessageBox.Show(string.Concat("Are you sure you want to delete ", myMaterial.Name, "?"), "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        RemoveTemporaryEntities();
                        var q = ctx.Materials.First(i => i.MaterialId == myMaterial.MaterialId);
                        ctx.Materials.Remove(q);
                        ctx.SaveChanges();
                        FillAllEntities();
                        ConstructNewEntities();
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private RelayCommand _AddQDBatch;
        public ICommand AddQDBatch
        {
            get
            {
                if (_AddQDBatch == null)
                {
                    _AddQDBatch = new RelayCommand(param => this.AddQDBatchExecute());
                }
                return _AddQDBatch;
            }
        }
        public void AddQDBatchExecute()
        {
            try
            {
                Debug.WriteLine("Adding QDBatch: " + QDBatchToAdd.Name);
                ctx.Materials.Add(QDBatchToAdd);
                LayerTemplateToAdd.Material = QDBatchToAdd;
                LayerTemplateToAdd.Layer.Material = QDBatchToAdd;
                ctx.SaveChanges();
                PromptForCreateSolution(QDBatchToAdd);
                ActiveUserControl = ControlsDict["QD Batches"];
                ConstructNewEntities();
                FillAllEntities();
                ReloadActiveUserControl();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _DeleteQDBatch;
        public ICommand DeleteQDBatch
        {
            get
            {
                if (_DeleteQDBatch == null)
                {
                    _DeleteQDBatch = new RelayCommand(param => this.DeleteQDBatchExecute(param));
                }
                return _DeleteQDBatch;
            }
        }
        public void DeleteQDBatchExecute(object o)
        {
            try
            {
                QDBatch myQDBatch = new QDBatch();
                if (o is QDBatch)
                {
                    myQDBatch = (QDBatch)o;
                    if (MessageBox.Show(string.Concat("Are you sure you want to delete ", myQDBatch.Name, "?"), "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var q = ctx.Materials.First(i => i.MaterialId == myQDBatch.MaterialId);
                        ctx.Materials.Remove(q);
                        ctx.SaveChanges();
                        FillAllEntities();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        private RelayCommand _CreateSolution;
        public ICommand CreateSolution
        {
            get
            {
                if (_CreateSolution == null)
                {
                    _CreateSolution = new RelayCommand(param => this.CreateSolutionExecute(param));
                }
                return _CreateSolution;
            }
        }
        public void CreateSolutionExecute(object o)
        {
            try
            {
                Material myMaterial = new Material();
                if (o is Material)
                {
                    myMaterial = (Material)o;
                    Debug.WriteLine("Creating solution from: " + myMaterial.Name);
                    //Debug.WriteLine("thing casted properly");
                }
                SolutionToAdd.Material = myMaterial;
                SelectedRole = myMaterial.PhysicalRole;
                solWindow = new SolutionsWindow();
                solWindow.DataContext = this;
                solWindow.Show();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _AddSolution;
        public ICommand AddSolution
        {
            get
            {
                if (_AddSolution == null)
                {
                    _AddSolution = new RelayCommand(param => this.AddSolutionExecute());
                }
                return _AddSolution;
            }
        }
        public void AddSolutionExecute()
        {
            try
            {
                ctx.Solutions.Add(SolutionToAdd);
                ctx.SaveChanges();
                MessageBox.Show(string.Concat("Added new solution of " + SolutionToAdd.Material.Name));

                SolutionToAdd = new Solution();
                FillSolutions();
                solWindow.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _DeleteSolution;
        public ICommand DeleteSolution
        {
            get
            {
                if (_DeleteSolution == null)
                {
                    _DeleteSolution = new RelayCommand(param => this.DeleteSolutionExecute(param));
                }
                return _DeleteSolution;
            }
        }
        public void DeleteSolutionExecute(object o)
        {
            try
            {
                Solution mySolution = new Solution();
                if (o is Solution)
                {
                    mySolution = (Solution)o;
                    if (MessageBox.Show(string.Concat("Are you sure you want to delete ", mySolution.Material.Name, "?"), "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var q = ctx.Solutions.First(i => i.SolutionId == mySolution.SolutionId);
                        ctx.Solutions.Remove(q);
                        ctx.SaveChanges();
                        FillAllEntities();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        #endregion

    }

}
