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
using DeviceBatchWPF.ViewModels;

namespace DeviceBatchWPF.Windows
{
    /// <summary>
    /// Interaction logic for QDBatchesWindow.xaml
    /// </summary>
    public partial class QDBatchesWindow : Window
    {
        public QDBatchesWindow()
        {
            InitializeComponent();
            QDVM = new QDBatchesViewModel();
            this.DataContext = QDVM;
        }
        public QDBatchesViewModel QDVM;
    }
}
