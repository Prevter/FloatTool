/*
- Copyright(C) 2022 Prevter
-
- This program is free software: you can redistribute it and/or modify
- it under the terms of the GNU General Public License as published by
- the Free Software Foundation, either version 3 of the License, or
- (at your option) any later version.
-
- This program is distributed in the hope that it will be useful,
- but WITHOUT ANY WARRANTY; without even the implied warranty of
- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
- GNU General Public License for more details.
-
- You should have received a copy of the GNU General Public License
- along with this program. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FloatTool.Theme
{
    public partial class NumericBox : UserControl
    {
        public event EventHandler<int> ValueChanged;

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { TrySetValue(value); }
        }

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(NumericBox),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(NumericBox),
                new PropertyMetadata(0));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(NumericBox),
                new PropertyMetadata(100));


        public NumericBox()
        {
            InitializeComponent();
        }

        private void TrySetValue(int value)
        {
            if (value >= Minimum && value <= Maximum)
                SetValue(ValueProperty, value);
            else if (value < Minimum)
                SetValue(ValueProperty, Minimum);
            else if (value > Maximum)
                SetValue(ValueProperty, Maximum);
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            Value++;
            ValueChanged?.Invoke(this, Value);
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            Value--;
            ValueChanged?.Invoke(this, Value);
        }

        private static bool IsTextAllowed(string text)
        {
            return Array.TrueForAll(text.ToCharArray(),
                delegate (char c)
                {
                    return char.IsDigit(c);
                }
            );
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }

                Value = int.Parse(text);
                ValueChanged?.Invoke(this, Value);
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void PreviewTextInputHandler(Object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsTextAllowed(inputBox.Text) || string.IsNullOrEmpty(inputBox.Text))
                return;

            Value = int.Parse(inputBox.Text);
            ValueChanged?.Invoke(this, Value);
        }
    }
}
