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

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            Value++;
            ValueChanged?.Invoke(this, Value);
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
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

        private void inputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsTextAllowed(inputBox.Text) || string.IsNullOrEmpty(inputBox.Text))
                return;

            Value = int.Parse(inputBox.Text);
            ValueChanged?.Invoke(this, Value);
        }
    }
}
