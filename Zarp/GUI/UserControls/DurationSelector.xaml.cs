using System.Windows;
using System.Windows.Controls;

namespace Zarp.GUI.UserControls
{
    internal partial class DurationSelector : UserControl
    {
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(Duration), typeof(int), typeof(DurationSelector), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDurationChanged));

        public int Duration
        {
            get => (int)GetValue(DurationProperty);
            set
            {
                if (value >= 0)
                {
                    SetValue(DurationProperty, value);
                }
            }
        }
        private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DurationSelector)d).OnDurationChanged();
        }

        private void OnDurationChanged()
        {
            if (!_SuppressPropertyChangedCallback)
            {
                _SuppressTextChangedEvent = true;

                Hours.Text = (Duration / 60).ToString();
                Minutes.Text = (Duration % 60).ToString();

                _SuppressTextChangedEvent = false;
            }
        }

        public static readonly RoutedEvent DurationChangedEvent = EventManager.RegisterRoutedEvent(nameof(DurationChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DurationSelector));

        public event RoutedEventHandler DurationChanged
        {
            add => AddHandler(DurationChangedEvent, value);
            remove => RemoveHandler(DurationChangedEvent, value);
        }

        private bool _SuppressPropertyChangedCallback;
        private bool _SuppressTextChangedEvent;

        public DurationSelector()
        {
            InitializeComponent();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_SuppressTextChangedEvent)
            {
                return;
            }

            try
            {
                int hours = int.Parse(Hours.Text);
                int minutes = int.Parse(Minutes.Text);

                if (hours < 0 || hours >= 100 || minutes < 0 || minutes >= 60)
                {
                    return;
                }

                _SuppressPropertyChangedCallback = true;

                SetValue(DurationProperty, hours * 60 + minutes);
                RaiseEvent(new RoutedEventArgs(DurationChangedEvent));

                _SuppressPropertyChangedCallback = false;
            }
            catch { }
        }
    }
}
