using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace AppBase0500
{
    public class WindowBaseApp : Window
    {

        //Basics
        protected string assembly = string.Empty;

        private readonly string userLang = "en-us"; //zh-TW
        public string[] Languages = { "en-us", "zh-tw" };
        //private int lang = 0;
        private readonly string userTheme = "Light";
        private readonly string[] Themes = { "Light", "Dark" };
        //private int theme = 0;
        private readonly Dictionary<string, WindowState> state = new Dictionary<string, WindowState>();
        protected string pathApp = string.Empty;

        private readonly int workspaceWidth = 0, workspaceHeight = 0;


        private readonly WindowState lastState;
        private readonly WindowStyle lastStyle;

        private readonly ResourceDictionary rd = new ResourceDictionary();

        //Tests
        public UserContainer Container = new UserContainer();


        public AppLoadContext? c = new AppLoadContext();
        public WeakReference wr = new WeakReference(null);
        private dynamic? d = null;


        public WindowBaseApp(string assembly)
        {
            int l = 0;  //for line error tracking

            try
            {
                this.TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo();

                this.assembly = assembly;
                #region Test UI Language
                string selectedLanguage = "zh-tw";
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(selectedLanguage);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(selectedLanguage);
                #endregion
                this.userLang = Thread.CurrentThread.CurrentUICulture.Name.ToLower();

                string selectedTheme = "Light";
                this.userTheme = selectedTheme;

                l++; //1


                this.lastState = this.WindowState;
                this.lastStyle = this.WindowStyle;

                l++; //3
                //Host State
                this.state = new Dictionary<string, WindowState>
                {
                    { WindowState.Maximized.ToString().ToLower(), WindowState.Maximized },
                    { WindowState.Minimized.ToString().ToLower(), WindowState.Minimized },
                    { WindowState.Normal.ToString().ToLower(), WindowState.Normal }
                };

                var fi = new FileInfo(assembly);
                var f = fi.Name.Replace(fi.Extension, ".xds");

                this.pathApp = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
                var p = Path.Combine(this.pathApp, "xDB", "Temp");
                Util.CreateFolder(p);

                GetAllConfig();

                var wa = System.Windows.Forms.Screen.GetWorkingArea(new System.Drawing.Point());
                //this.workspaceHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
                //this.workspaceWidth = wa.Location.X + System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
                //this.Window_Closing += Window_Closing();
                //this.SizeChanged += Window_SizeChanged;

                ProcessCommands();

                this.Container = new UserContainer();
                this.Container.btnLoad.Click += (s, e) => LoadPlugin();
                this.Container.btnUnLoad.Click += (s, e) => UnLoadPlugin();
                this.Content = this.Container;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "WindowBase Init Error");
            }
        }



        /// <summary>
        /// Get SrvBase040x.exe.config to config service parts
        /// </summary>
        private void GetAllConfig()
        {
            //string f = Assembly.GetExecutingAssembly().Location;
            //string f = Assembly.GetCallingAssembly().Location;
            Trace.WriteLine($"ExecutingAssembly: {this.assembly}");
            var c = ConfigurationManager.OpenExeConfiguration(this.assembly);
            var settings = c.AppSettings.Settings;
            foreach (string key in settings.AllKeys)
            {
                //this.iBus1.Pins[key].Value = settings[key].Value;
                Trace.WriteLine($"Key: {key}={settings[key].Value}");
            }
            //settings.Add("Test","abc");
            //c.Save();
        }

        private void ProcessCommands()
        {
            string[] sa;
            //string code;
            var cmdArgs = Environment.GetCommandLineArgs();

            int cnt = cmdArgs.Length;
            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    sa = cmdArgs[i].Trim('/').Split('=');
                    if (sa.Length > 1)
                    {
                        //Trace.WriteLineIf(this.traceSwitch.TraceInfo, string.Format("Arg{0}: {1}={2}", i, sa[0], sa[1]), "Winbase");
                        //this.iBus1.Pins[sa[0]].Value = sa[1].Trim('"'); //remove quote marks from command line parameter
                        /*switch (sa[0])
                        {
                            case "Host.App":
                                if (i < cnt)
                                {
                                    i++;
                                    this.startupSettings = this.cmdArgs[i];
                                }
                                break;
                            case "/taskapp":
                                if (i < cnt)
                                {
                                    i++;
                                    this.startupTasks = this.cmdArgs[i];
                                }
                                break;
                        }*/
                    }
                }
            }

        }



        protected void PluginApp(string app)
        {
            //this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
        }

        private void LoadPlugin()
        {
            Assembly p;
            Type t;
            try
            {
                this.c = new AppLoadContext();
                this.wr = new WeakReference(this.c, trackResurrection: true);
                this.c.Unloading += (o) => App_Unloading(o);

                var f = Path.Combine(this.pathApp, "P01.dll");
                p = this.c.LoadFromAssemblyPath(f);

                t = p.GetType("P01.AppVM", true);
                this.d = Activator.CreateInstance(t, null);
                this.d.Ap = this;
                //this.main.Children.Add(this.instance.GetUI());

                //dynamic d = PluginAny.Plugin(this.pathApp, "P01.AppVM");
                UIElement ui = this.d.GetUI();
                this.Container.main.Children.Add(ui);
            }
            catch (System.Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine("");
            }
        }

        private void UnLoadPlugin()
        {
            try
            {
                this.Container.main.Children.Clear();
                this.d = null;
                if (this.c != null)
                {
                    this.c.Unload();
                    for (int i = 0; this.wr.IsAlive && (i < 10); i++)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                    Trace.WriteLine($"Weak Ref. IsAlive={this.wr.IsAlive}");
                    this.c = null;
                }
            }
            catch (System.Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine("");
            }
        }
        private void App_Unloading(System.Runtime.Loader.AssemblyLoadContext obj)
        {
            Trace.WriteLine($"INF Unloading {obj.Name}");
        }

        //Status VMs
        #region Status VMs
        public Dictionary<int, StatVm> dStat = new Dictionary<int, StatVm>();
        public void AddStatVM(StatVm vmStat)
        {
            int i = 0;
            try
            {
                while (this.dStat.ContainsKey(i) && i < 100)
                {
                    i++;
                }
                vmStat.Index = i;
                this.dStat[i] = vmStat;
                vmStat.Left = 0;// Monitors[0].Screen.WorkingArea.Width - vmStat.UI.Width + 7;
                if (i > 0)
                {
                    vmStat.Top = (vmStat.UI.Height - 9) * i;
                }
            }
            catch (Exception ex)
            {
                //i = -1;
                ShowError(ex);
            }
        }
        public void RemoveStatVM(StatVm? vmStat)
        {
            try
            {
                if (vmStat != null)
                {
                    vmStat.UI.Hide();
                    if (this.dStat.ContainsKey(vmStat.Index))
                    {
                        this.dStat.Remove(vmStat.Index);
                    }
                    vmStat = null;
                }
            }
            catch (Exception ex)
            {
                TraceError(ex);
            }
        }

        #endregion


        //Helpers
        public void ShowError(Exception ex, string caller = "")
        {
            MessageBox.Show(ex.Message, caller, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public void TraceError(Exception ex)
        {
            Trace.WriteLine($"Error {ex.Message}");
        }
    }
}
