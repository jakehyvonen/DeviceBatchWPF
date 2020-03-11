using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeviceBatchWPF.Scheduling
{
    /// <summary>
    /// Interaction logic for EquipmentSchedulingWindow.xaml
    /// </summary>
    public partial class EquipmentSchedulingWindow : Window
    {
        public EquipmentSchedulingWindow(EquipmentSchedulingViewModel ESVM)
        {
            _ESVM = ESVM;
            DataContext = _ESVM;
            InitializeComponent();
            //cboMonth.SelectionChanged += (o, e) => RefreshCalendar();
        }
        EquipmentSchedulingViewModel _ESVM;
        /*
        private void RefreshCalendar()
        {
            if (cboMonth.SelectedItem == null) return;
            int month = cboMonth.SelectedIndex + 1;
            DateTime targetDate = new DateTime(DateTime.Now.Year, month, 1);
            Calendar.BuildCalendar(targetDate);
        }
        */
        private void Calendar_DayChanged(object sender, DayChangedEventArgs e)
        {
            //save the text edits to persistant storage
        }
    }
}
