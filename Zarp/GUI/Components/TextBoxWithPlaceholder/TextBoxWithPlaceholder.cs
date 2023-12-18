using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Zarp.GUI.Components
{
    /// <summary>
    /// Interaction logic for TextBoxWithPlaceholder.xaml
    /// </summary>
    public class TextBoxWithPlaceholder : TextBox
    {
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(TextBoxWithPlaceholder), new PropertyMetadata(null, OnPlaceholderTextChanged));

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        private static void OnPlaceholderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBoxWithPlaceholder)d).OnPlaceholderTextChanged();
        }

        protected void OnPlaceholderTextChanged()
        {
            if (!IsFocused && string.IsNullOrEmpty(Text))
            {
                base.Text = PlaceholderText;
            }
        }

        public static new readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBoxWithPlaceholder), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public new string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBoxWithPlaceholder)d).OnTextChanged();
        }

        private void OnTextChanged()
        {
            if (!IsFocused && string.IsNullOrEmpty(Text))
            {
                base.Text = PlaceholderText;
            }
            else
            {
                base.Text = Text;
            }
        }

        public TextBoxWithPlaceholder()
        {
            TextChanged += OnBaseTextChanged;
            GotKeyboardFocus += OnGotKeyboardFocus;
            LostKeyboardFocus += OnLostKeyboardFocus;
            Initialized += OnInitialized;
        }

        private void OnInitialized(object? sender, EventArgs e)
        {
            base.Text = PlaceholderText;
        }

        private void OnBaseTextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsKeyboardFocused)
            {
                Text = base.Text;
            }
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                base.Text = string.Empty;
            }
        }

        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
            {
                base.Text = PlaceholderText;
            }
        }
    }
}