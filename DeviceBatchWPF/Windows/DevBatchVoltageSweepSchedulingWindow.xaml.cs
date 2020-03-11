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
using DeviceBatchWPF.Scheduling;

namespace DeviceBatchWPF.Windows
{
    /// <summary>
    /// Interaction logic for DevBatchVoltageSweepSchedulingWindow.xaml
    /// </summary>
    public partial class DevBatchVoltageSweepSchedulingWindow : Window
    {
        public DevBatchVoltageSweepSchedulingWindow(EquipmentSchedulingViewModel ESVM)
        {
            _ESVM = ESVM;
            DataContext = _ESVM;
            InitializeComponent();
        }
        EquipmentSchedulingViewModel _ESVM;
        private void Calendar_DayChanged(object sender, DayChangedEventArgs e)
        {
            //save the text edits to persistant storage
        }
    }
}
