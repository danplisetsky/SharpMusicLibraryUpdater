using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SharpMusicLibraryUpdater.App.Models;

namespace SharpMusicLibraryUpdater.App.Resources
{
    public class NegateBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !(bool)value;
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !(bool)value;
    }

    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool markAsSeen)
                return markAsSeen ? Brushes.Black : Brushes.White;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
    }

    public class IsIgnoredAndNewAlbumsToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is bool isIgnored && isIgnored)
                return Brushes.White;
            if (values[1] is List<NewAlbum> newAlbums)
                return newAlbums.Any(al => !al.MarkAsSeen) ? Brushes.Green : Brushes.Red;
            return DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => new[] { DependencyProperty.UnsetValue };
    }

    public class IsIgnoredAndNewAlbumsToStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is bool isIgnored && isIgnored)
                return String.Empty;
            if (values[1] is List<NewAlbum> newAlbums)
            {
                int count = newAlbums.Count(al => !al.MarkAsSeen);
                return count == 0
                    ? "No new albums available"
                    : count == 1
                        ? "1 new album available"
                        : $"{count} new albums available";
            }
            return DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => new[] { DependencyProperty.UnsetValue };
    }
}
