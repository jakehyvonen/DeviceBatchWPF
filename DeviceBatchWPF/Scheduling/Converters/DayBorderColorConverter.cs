using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.Generic;
using EFDeviceBatchCodeFirst;

namespace DeviceBatchWPF.Scheduling.Converters
{
    public class DayBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<EquipmentTask> tasks = (List<EquipmentTask>)value;
            if (tasks.Count == 0) return null;
            if (tasks.Count > 0) return new LinearGradientBrush(Color.FromRgb(220, 74, 56), Color.FromRgb(198, 56, 40), new Point(0.5, 0), new Point(0.5, 1));
            /*
            string notes = (string)value;

            if (string.IsNullOrEmpty(notes)) return null;

            if (notes.Length > 0) return new LinearGradientBrush(Color.FromRgb(220, 74, 56), Color.FromRgb(198, 56, 40), new Point(0.5, 0), new Point(0.5, 1));
            */
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
