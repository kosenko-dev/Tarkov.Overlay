using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Overlay.NET.Common;
using Overlay.NET.Demo.Internals;
using Overlay.NET.Wpf;
using Process.NET.Windows;

namespace Overlay.NET.Demo.Wpf
{
    [RegisterPlugin("WpfOverlayDemo-1", "Jacob Kemple", "WpfOverlayDemo", "0.0", "A basic demo of the WPF overlay.")]
    public class WpfOverlayDemoExample : WpfOverlayPlugin
    {
        public ISettings<DemoOverlaySettings> Settings { get; } = new SerializableSettings<DemoOverlaySettings>();

        // Used to limit update rates via timestamps 
        // This way we can avoid thread issues with wanting to delay updates
        private readonly TickEngine _tickEngine = new TickEngine();
        private Ellipse _ellipse;

        private bool _isDisposed;

        private bool _isSetup;

        // Shapes used in the demo
        private Line _line;
        private Polygon _polygon;
        private Rectangle _rectangle;

        public override void Enable()
        {
            _tickEngine.IsTicking = true;
            base.Enable();
        }

        public override void Disable()
        {
            _tickEngine.IsTicking = false;
            base.Disable();
        }

        public override void Initialize(IWindow targetWindow)
        {
            // Set target window by calling the base method
            base.Initialize(targetWindow);

            OverlayWindow = new OverlayWindow(targetWindow);

            // For demo, show how to use settings
            var current = Settings.Current;
            var type = GetType();

            current.UpdateRate = 1000 / 60;
            current.Author = GetAuthor(type);
            current.Description = GetDescription(type);
            current.Identifier = GetIdentifier(type);
            current.Name = GetName(type);
            current.Version = GetVersion(type);

            // File is made from above info
            Settings.Save();
            Settings.Load();

            // Set up update interval and register events for the tick engine.
            _tickEngine.Interval = Settings.Current.UpdateRate.Milliseconds();
            _tickEngine.PreTick += OnPreTick;
            _tickEngine.Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            // This will only be true if the target window is active
            // (or very recently has been, depends on your update rate)
            if (OverlayWindow.IsVisible)
            {
                OverlayWindow.Update();
            }
        }

        private void OnPreTick(object sender, EventArgs eventArgs)
        {
            // Only want to set them up once.
            if (!_isSetup)
            {
                SetUp();
                _isSetup = true;
            }

            var activated = TargetWindow.IsActivated;
            var visible = OverlayWindow.IsVisible;

            // Ensure window is shown or hidden correctly prior to updating
            if (!activated && visible)
            {
                OverlayWindow.Hide();
            }

            else if (activated && !visible)
            {
                OverlayWindow.Show();
            }
        }

        public override void Update() => _tickEngine.Pulse();

        // Clear objects
        public override void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            if (IsEnabled)
            {
                Disable();
            }

            OverlayWindow?.Hide();
            OverlayWindow?.Close();
            OverlayWindow = null;
            _tickEngine.Stop();
            Settings.Save();

            base.Dispose();
            _isDisposed = true;
        }

        ~WpfOverlayDemoExample()
        {
            Dispose();
        }

        // Red dot shape
        private void SetUp()
        {
            _ellipse = new Ellipse
            {
                Width = 4,
                Height = 4,
                Margin = new Thickness(SystemParameters.PrimaryScreenWidth / 2 - 2,
                    SystemParameters.PrimaryScreenHeight / 2 - 2, 0, 0),
                Stroke =
                    new SolidColorBrush(Color.FromRgb(255, 0, 0)),
                Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0))
            };

            OverlayWindow.Add(_ellipse);
        }
    }
}