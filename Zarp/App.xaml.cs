using System;
using System.Windows;
using System.Windows.Input;

namespace Zarp
{
    public partial class App : Application
    {
        Core.Zarp app;

        public App()
        {
            app = new Core.Zarp();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            app.Save();
            base.OnExit(e);
        }
    }
}
