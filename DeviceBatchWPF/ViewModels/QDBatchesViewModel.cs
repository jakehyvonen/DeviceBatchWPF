using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using EFDeviceBatchCodeFirst;
using DeviceBatchGenerics.Support;
using DeviceBatchGenerics.Support.Bases;
using DeviceBatchGenerics.ViewModels.EntityVMs;
using DeviceBatchWPF.Windows;

namespace DeviceBatchWPF.ViewModels
{
    public class QDBatchesViewModel : CrudVMBase
    {
        public QDBatchesViewModel()
        {
            FillQDBatches();
        }
        #region Members
        QDBatchesWindow _window;
        QDBatchVM _selectedQDBatch;
        ObservableCollection<QDBatchVM> _visibleQDBatches;
        #endregion
        #region Properties
        public QDBatchVM SelectedQDBatch
        {
            get { return _selectedQDBatch; }
            set
            {
                _selectedQDBatch = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<QDBatchVM> VisibleQDBatches
        {
            get { return _visibleQDBatches; }
            set
            {
                _visibleQDBatches = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region Methods
        private void FillQDBatches()
        {
            var mats = (from a in ctx.Materials.Where(mat => mat is QDBatch)
                        select a).ToList();
            VisibleQDBatches = new ObservableCollection<QDBatchVM>();
            foreach (Material m in mats)
            {
                QDBatch qdb;
                qdb = (QDBatch)m;
                VisibleQDBatches.Add(new QDBatchVM(qdb));
                Debug.WriteLine("Added QDBatch named " + qdb.Name);
            }
        }
        private void UpdateEMLSpreadSheet()
        {
            List<QDBatchVM> qdbs = VisibleQDBatches.ToList();
            SpreadsheetGenerator.GenerateEMLCharacteristics(qdbs);
        }

        #endregion
        #region ICommands
        private RelayCommand _updateQDBatchesSpreadSheet;
        public ICommand UpdateQDBatchesSpreadSheet
        {
            get
            {
                if (_updateQDBatchesSpreadSheet == null)
                {
                    _updateQDBatchesSpreadSheet = new RelayCommand(param => this.UpdateQDBatchesSpreadSheetExecute(param));
                }
                return _updateQDBatchesSpreadSheet;
            }
        }
        public void UpdateQDBatchesSpreadSheetExecute(object o)
        {
            try
            {
                UpdateEMLSpreadSheet();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            MessageBox.Show("Updated EML Characteristics");
        }

        #endregion

    }
}
