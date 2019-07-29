using System;
using System.Reflection;
using AppBase0500;

namespace ApStudio5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBaseApp
    {
        /// <summary>
        /// Main entry point inits base and plugs in App
        /// </summary>
        public MainWindow() : base(Assembly.GetExecutingAssembly().Location)
        {
            var app = "IC.AP0500.AppVM";

            try
            {
                InitializeComponent();

                PluginApp(app);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}
