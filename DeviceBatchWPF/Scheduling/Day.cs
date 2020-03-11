using System;
using System.Collections.Generic;
using System.Linq;
using EFDeviceBatchCodeFirst;
using DeviceBatchGenerics.Support.Bases;

namespace DeviceBatchWPF.Scheduling
{
    public class Day : CrudVMBase
    {
        //public event PropertyChangedEventHandler PropertyChanged;
        #region Construction
        public Day(DateTime date, Boolean isenabled, Boolean istargetmonth)
        {
            Date = date;
            IsEnabled = isenabled;
            IsTargetMonth = istargetmonth;
            Initialize();
        }

        #endregion
        #region Members
        private DateTime date;
        private string notes;
        private bool enabled = true;
        private bool isTargetMonth;
        private bool isToday;
        private List<EquipmentTask> _equipmentTaskList;
        #endregion
        #region Properties
        public bool IsToday
        {
            get { return isToday; }
            set
            {
                isToday = value;
                OnPropertyChanged();
            }
        }

        public bool IsTargetMonth
        {
            get { return isTargetMonth; }
            set
            {
                isTargetMonth = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                OnPropertyChanged();
            }
        }

        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged();
            }
        }
        public List<EquipmentTask> EquipmentTaskList
        {
            get { return _equipmentTaskList; }
            set
            {
                _equipmentTaskList = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region Methods
        private void Initialize()
        {
            EquipmentTaskList = ctx.EquipmentTasks
                .Where(x => x.ScheduledDate == Date)
                .Where(x => x.IsCompleted == false)
                .ToList();
            /*
            foreach (EquipmentTask ET in EquipmentTaskList)
            {
                Debug.WriteLine("Found ET for batch: " + ET.DeviceBatch.Name);
            }
            
            var dumbList = ctx.EquipmentTasks.ToList();
            foreach (EquipmentTask task in dumbList)
            {
                Debug.WriteLine("Sanity check for: " + task.DeviceBatch.Name);
            }
            */
        }
        #endregion
    }

}
