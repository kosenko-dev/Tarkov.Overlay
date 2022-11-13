using System;
using Overlay.NET.Common;
using Overlay.NET.Demo.Internals;
using Overlay.NET.Demo.Wpf;

namespace Overlay.NET.Demo {
    /// <summary>
    /// </summary>
    public static class Program {
        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        [STAThread]
        public static void Main() {
            Log.Register("Console", new ConsoleLog());
            Log.Debug("Start initializing");
            
            var wpfDemo = new WpfOverlayExampleDemo();
            wpfDemo.StartDemo();
            Log.WriteLine("Red dot running..");
        }
    }
}