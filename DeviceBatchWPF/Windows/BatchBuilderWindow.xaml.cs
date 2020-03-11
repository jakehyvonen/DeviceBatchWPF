using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DeviceBatchWPF.ViewModels;

namespace DeviceBatchWPF.Windows
{
    /// <summary>
    /// Interaction logic for BatchBuilderWindow.xaml
    /// </summary>
    public partial class BatchBuilderWindow : Window
    {
        public BatchBuilderWindow()
        {
            InitializeComponent();
            BBVM = new BatchBuilderViewModel();
            this.DataContext = BBVM;
            BBVM.ParentWindow = this;
        }
        public BatchBuilderWindow(BatchBuilderViewModel vm)
        {
            InitializeComponent();
            BBVM = vm;
            DataContext = vm;
            vm.ParentWindow = this;
        }
        public BatchBuilderViewModel BBVM;
        private void Window_Closed(object sender, EventArgs e)
        {
            BBVM.CheckToRemoveTemporaryDevices();
            this.Close();
        }
    }
}
