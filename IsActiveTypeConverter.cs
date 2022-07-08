using System;
using System.Globalization;
using System.Windows.Data;

namespace Witcher3SaveToggle;

class IsActiveToString : IValueConverter {
    #region Constructors
    /// <summary>
    /// The default constructor
    /// </summary>
    public IsActiveToString() { }
    #endregion

    #region IValueConverter Members
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        if (value == null && value is not bool) return null;
        else return (bool)value ? "Active" : "Inactive";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        if (value == null && value is not string) return null;
        else return (string)value == "Active";
    }
    #endregion
}

