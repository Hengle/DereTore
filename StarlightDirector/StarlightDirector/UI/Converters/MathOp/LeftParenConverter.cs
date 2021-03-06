﻿using System;
using System.Globalization;
using System.Windows;

namespace StarlightDirector.UI.Converters.MathOp {
    public sealed class LeftParenConverter : OpSymbolConverterBase {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }

    }
}
