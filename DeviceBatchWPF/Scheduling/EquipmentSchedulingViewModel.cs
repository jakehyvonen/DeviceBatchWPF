using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using EFDeviceBatchCodeFirst;
using DeviceBatchGenerics.Support;
using DeviceBatchGenerics.Support.Bases;
using DeviceBatchWPF.Scheduling.Controls;
using DeviceBatchGenerics.ViewModels.EntityVMs;

namespace DeviceBatchWPF.Scheduling
{
    public class EquipmentSchedulingViewModel : CrudVMBase
    {

        #region Construction
        public EquipmentSchedulingViewModel()
        {
            //assume that we are working with the batch voltage sweep system
            TheEquipmentResource = ctx.EquipmentResources.Where(x => x.Name == "Batch Voltage Sweep System").First();
            Initialize();
            ActiveUserControl = new MasterVoltageSweepSchedulingControl();
        }
        public EquipmentSchedulingViewModel(DeviceBatchVM DBVM)
        {
            TheDeviceBatchViewModel = DBVM;
            //assume that we are working with the batch voltage sweep system
            TheEquipmentResource = ctx.EquipmentResources.Where(x => x.Name == "Batch Voltage Sweep System").First();
            Initialize();
            ActiveUserControl = new DevBatchSweepSchedulingControl();
        }
        public EquipmentSchedulingViewModel(EquipmentResource ER)
        {
            TheEquipmentResource = ER;
            Initialize();
        }
        #endregion
        #region Members
        UserControl _activeUserControl;
        EquipmentTask _selectedTask;
        EquipmentTask _editableTask;
        int _selectedTaskId = 0;
        EquipmentResource _theEquipmentResource;
        DeviceBatchVM _theDBVM;
        Scheduling.Calendar _jarlooCalendar;
        string _selectedMonth;
        int _leCurrentYear = DateTime.Now.Year;
        ObservableCollection<string> _months;
        ObservableCollection<EquipmentTask> _devBatchVoltageSweeps;
        DateTime _voltageSweepsPeriodStartDate = DateTime.Now.AddDays(1);
        int _voltageSweepIntervalDays = 7;
        int _numberOfVoltageSweeps = 4;
        #endregion
        #region Properties
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
        public EquipmentTask SelectedTask
        {
            get
            {
                return _selectedTask;
            }
            set
            {
                _selectedTask = null;
                OnPropertyChanged();
                _selectedTask = value;
                if (_selectedTask != null)
                {
                    _selectedTaskId = _selectedTask.EquipmentTaskId;
                    Debug.WriteLine("_selectedTaskId is: " + _selectedTaskId);
                    //EquipmentTask newSelection = ctx.EquipmentTasks.Where(x => x.Id == _selectedTask.Id).First();
                    //_selectedTask = newSelection;
                    OnPropertyChanged();
                    EditableTask = ctx.EquipmentTasks.Where(x => x.EquipmentTaskId == _selectedTaskId).First();
                }
                //Debug.WriteLine("Selected task for DeviceBatch: " + _selectedTask.DeviceBatch.Name);
            }
        }
        public EquipmentTask EditableTask
        {
            get
            {

                if (_selectedTaskId != 0) _editableTask = ctx.EquipmentTasks.Where(x => x.EquipmentTaskId == _selectedTaskId).First();
                return _editableTask;
            }
            set
            {
                _editableTask = value;
                OnPropertyChanged();
            }
        }
        public EquipmentResource TheEquipmentResource
        {
            get { return _theEquipmentResource; }
            set
            {
                _theEquipmentResource = value;
                OnPropertyChanged();
            }
        }
        public DeviceBatchVM DBVM
        {
            get { return _theDBVM; }
            set
            {
                _theDBVM = value;
                OnPropertyChanged();
            }
        }
        public DeviceBatchVM TheDeviceBatchViewModel
        {
            get { return _theDBVM; }
            set
            {
                _theDBVM = value;
                OnPropertyChanged();
            }
        }
        public Scheduling.Calendar JarlooCalendar
        {
            get { return _jarlooCalendar; }
            set
            {
                _jarlooCalendar = value;
                OnPropertyChanged();
            }
        }
        public string SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                _selectedMonth = value;
                OnPropertyChanged();
                UpdateCalendar();
            }
        }
        public ObservableCollection<string> Months
        {
            get { return _months; }
            set
            {
                _months = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<EquipmentTask> DevBatchVoltageSweeps
        {
            get { return _devBatchVoltageSweeps; }
            set
            {
                _devBatchVoltageSweeps = value;
                OnPropertyChanged();
            }
        }
        public DateTime VoltageSweepsPeriodStartDate
        {
            get { return _voltageSweepsPeriodStartDate; }
            set
            {
                _voltageSweepsPeriodStartDate = value;
                OnPropertyChanged();
            }
        }
        public int VoltageSweepIntervalDays
        {
            get { return _voltageSweepIntervalDays; }
            set
            {
                _voltageSweepIntervalDays = value;
                OnPropertyChanged();
            }
        }
        public int NumberOfVoltageSweeps
        {
            get { return _numberOfVoltageSweeps; }
            set
            {
                _numberOfVoltageSweeps = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region Methods
        private void Initialize()
        {
            JarlooCalendar = new Scheduling.Calendar();
            Months = new ObservableCollection<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            //Debug.WriteLine("Month integer is: " + DateTime.Now.Month.ToString("00"));
            SelectedMonth = Months[Convert.ToInt32(DateTime.Now.Month.ToString("00")) - 1];
            if (DBVM != null)
            {
                UpdateDevBatchVoltageSweepsCollection();
            }
        }
        private void UpdateDevBatchVoltageSweepsCollection()
        {
            var batchScheduledSweeps = ctx.EquipmentTasks
                   .Where(x => x.EquipmentTaskId == _theDBVM.TheDeviceBatch.DeviceBatchId)
                   .Where(x => x.IsCompleted == false)
                   .OrderBy(x => x.ScheduledDate)
                   .ToList();
            DevBatchVoltageSweeps = new ObservableCollection<EquipmentTask>(batchScheduledSweeps);
        }
        private void UpdateCalendar()
        {
            string dumbString = string.Concat(" 1, ", _leCurrentYear);
            DateTime targetDate = DateTime.Parse(SelectedMonth + dumbString);
            Debug.WriteLine("targetDate is:" + targetDate);
            JarlooCalendar.BuildCalendar(targetDate);
        }
        #endregion
        #region Commands
        private RelayCommand _AddNewVoltageSweepTasks;
        public ICommand AddNewVoltageSweepTasks
        {
            get
            {
                if (_AddNewVoltageSweepTasks == null)
                {
                    _AddNewVoltageSweepTasks = new RelayCommand(param => this.AddNewVoltageSweepTasksExecute(param));
                }
                return _AddNewVoltageSweepTasks;
            }
        }
        public void AddNewVoltageSweepTasksExecute(object o)
        {
            try
            {
                for (int i = 0; i < NumberOfVoltageSweeps; i++)
                {
                    var newTask = new EquipmentTask();
                    ctx.EquipmentTasks.Add(newTask);
                    newTask.EquipmentResource = ctx.EquipmentResources.Where(x => x.Name == "Batch Voltage Sweep System").First();
                    var devBatchId = DBVM.TheDeviceBatch.DeviceBatchId;
                    newTask.Employee = ctx.Employees.Where(x => x.EmployeeId == DBVM.TheDeviceBatch.Employee.EmployeeId).First();
                    newTask.IsCompleted = false;
                    if (i == 0) newTask.ScheduledDate = VoltageSweepsPeriodStartDate.Date;
                    if (i >= 1) newTask.ScheduledDate = VoltageSweepsPeriodStartDate.AddDays(i * VoltageSweepIntervalDays).Date;
                    newTask.DeviceBatch.DeviceBatchId = devBatchId;
                    //newTask.DeviceBatch = DBVM.TheDeviceBatch;
                    //DBVM.TheDeviceBatch.EquipmentTasks.Add(newTask);
                    DevBatchVoltageSweeps.Add(newTask);
                    //ctx.EquipmentTasks.Add(newTask);
                }
                ctx.SaveChanges();
                UpdateCalendar();
                MessageBox.Show("Scheduled " + NumberOfVoltageSweeps + " New Voltage Sweep(s)");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _DeleteTask;
        public ICommand DeleteTask
        {
            get
            {
                if (_DeleteTask == null)
                {
                    _DeleteTask = new RelayCommand(param => this.DeleteTaskExecute(param));
                }
                return _DeleteTask;
            }
        }
        public void DeleteTaskExecute(object o)
        {
            try
            {
                EquipmentTask task = (EquipmentTask)o;
                if (MessageBox.Show(
                    String.Concat("Delete Test for Batch ", task.DeviceBatch.Name, "?"),
                    "Delete Layer",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    task = ctx.EquipmentTasks.Where(x => x.EquipmentTaskId == task.EquipmentTaskId).First();
                    if (DevBatchVoltageSweeps != null) DevBatchVoltageSweeps.Remove(task);
                    if (DBVM != null) DBVM.TheDeviceBatch.EquipmentTasks.Remove(task);
                    ctx.EquipmentTasks.Remove(task);
                    ctx.SaveChanges();
                    UpdateCalendar();
                    //MessageBox.Show("Deleted task");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _SaveChanges;
        public ICommand SaveChanges
        {
            get
            {
                if (_SaveChanges == null)
                {
                    _SaveChanges = new RelayCommand(param => this.SaveChangesExecute(param));
                }
                return _SaveChanges;
            }
        }
        public void SaveChangesExecute(object o)
        {
            try
            {
                ctx.SaveChanges();
                UpdateCalendar();
                MessageBox.Show("Saved Scheduling Changes");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _GoToDevBatchSchedulingControl;
        public ICommand GoToDevBatchSchedulingControl
        {
            get
            {
                if (_GoToDevBatchSchedulingControl == null)
                {
                    _GoToDevBatchSchedulingControl = new RelayCommand(param => this.GoToDevBatchSchedulingControlExecute(param));
                }
                return _GoToDevBatchSchedulingControl;
            }
        }
        public void GoToDevBatchSchedulingControlExecute(object o)
        {
            try
            {
                DBVM = new DeviceBatchVM(EditableTask.DeviceBatch);
                UpdateDevBatchVoltageSweepsCollection();
                ActiveUserControl = new DevBatchSweepSchedulingControl();
                UpdateCalendar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _GoToMasterVoltageSweepSchedulingControl;
        public ICommand GoToMasterVoltageSweepSchedulingControl
        {
            get
            {
                if (_GoToMasterVoltageSweepSchedulingControl == null)
                {
                    _GoToMasterVoltageSweepSchedulingControl = new RelayCommand(param => this.GoToMasterVoltageSweepSchedulingControlExecute(param));
                }
                return _GoToMasterVoltageSweepSchedulingControl;
            }
        }
        public void GoToMasterVoltageSweepSchedulingControlExecute(object o)
        {
            try
            {
                _selectedTaskId = 0;
                ActiveUserControl = new MasterVoltageSweepSchedulingControl();
                UpdateCalendar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _GoToNextMonth;
        public ICommand GoToNextMonth
        {
            get
            {
                if (_GoToNextMonth == null)
                {
                    _GoToNextMonth = new RelayCommand(param => this.GoToNextMonthExecute(param));
                }
                return _GoToNextMonth;
            }
        }
        public void GoToNextMonthExecute(object o)
        {
            try
            {
                string dumbString = string.Concat(" 1, ", _leCurrentYear);
                int selectedMonthInt = Convert.ToInt32(DateTime.Parse(SelectedMonth + dumbString).Month.ToString("00"));
                Debug.WriteLine("selectedMonthInt is: " + selectedMonthInt);
                if (selectedMonthInt == 12)
                {
                    _leCurrentYear++;
                    Debug.WriteLine("the Current Year is: " + _leCurrentYear);
                    SelectedMonth = Months[0];
                }
                else SelectedMonth = Months[selectedMonthInt];//don't need to add 1 to selectedMonthInt because Months[] is zero-indexed
                UpdateCalendar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private RelayCommand _GoToPreviousMonth;
        public ICommand GoToPreviousMonth
        {
            get
            {
                if (_GoToPreviousMonth == null)
                {
                    _GoToPreviousMonth = new RelayCommand(param => this.GoToPreviousMonthExecute(param));
                }
                return _GoToPreviousMonth;
            }
        }
        public void GoToPreviousMonthExecute(object o)
        {
            try
            {
                string dumbString = string.Concat(" 1, ", _leCurrentYear);
                int selectedMonthInt = Convert.ToInt32(DateTime.Parse(SelectedMonth + dumbString).Month.ToString("00"));
                Debug.WriteLine("selectedMonthInt is: " + selectedMonthInt);
                if (selectedMonthInt == 1)
                {
                    _leCurrentYear--;
                    Debug.WriteLine("the Current Year is: " + _leCurrentYear);
                    SelectedMonth = Months[11];
                }
                else SelectedMonth = Months[selectedMonthInt - 2];
                UpdateCalendar();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        #endregion

    }

}
