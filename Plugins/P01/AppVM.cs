using AppBase0500;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace P01
{
    public class AppVM : NotifyBase
    {
        public WindowBaseApp? Ap;
        public UserList? ui;


        private string text = "";
        public string Text
        {
            get => this.text;
            set => SetProperty(ref this.text, value);
        }

        private string status = "";
        public string Status
        {
            get => this.status;
            set => SetProperty(ref this.status, value);
        }


        ~AppVM()
        {
            this.ui = null;

        }


        public AppVM()
        {
            this.ui = new UserList
            {
                DataContext = this
            };
            this.ui.PreviewMouseLeftButtonDown += (s, e) => ProcesCommands("Mouse", GetCmd(e));
            this.Text = "Hello World";

        }

        private void ProcesCommands(string type, string cmd)
        {
            Trace.WriteLine($"{type}->{cmd}");
            switch (cmd)
            {
                case "DoThis": DoTest(); break;
                default:
                    Trace.WriteLine("WRN Command not found {cmd}");
                    break;
            }
        }

        protected virtual string GetCmd(MouseButtonEventArgs e)
        {
            var cmd = string.Empty;
            if (e.OriginalSource is UIElement ui)
            {
                var btn = ui.TryFindParent<Button>();
                if (btn != null)
                {
                    cmd = btn.Tag.ToString();
                }
                else
                {
                    if (e.OriginalSource is System.Windows.Documents.Run hl)
                    {
                        cmd = hl.Text;
                    }
                }
            }
            return cmd;
        }



        public UIElement? GetUI()
        {
            return this.ui;
        }

        //Quick Test
        private async void DoTest()
        {
            bool IsProcessing = false;
            Stopwatch sw;
            StatVm vmStat;
            if (this.Ap != null)
            {
                vmStat = new StatVm
                {
                    Title = "Refeshing folders",
                    ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal
                };
                try
                {
                    sw = new Stopwatch();
                    sw.Start();
                    IsProcessing = true;
                    vmStat.UI.btnCancel.Click += (s, e) =>
                    {
                        IsProcessing = false;
                    };

                    this.Ap.AddStatVM(vmStat);
                    vmStat.UI.Show();
                    //vmStat.UI.Left = vmStat.Left;
                    vmStat.UI.Top = vmStat.Top;


                    await Task.Run(() =>
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            vmStat.Status = $"Processing {i}";
                            vmStat.Progress = i;
                            i += 4;
                            if (!IsProcessing)
                            {
                                break;
                            }
                            Thread.Sleep(5000);
                        }
                    });
                    Trace.WriteLine($"INF {vmStat.Top} x {vmStat.UI.Top}");
                    vmStat.Progress = 100;
                    IsProcessing = false;
                    this.Status = $"Process completed in {sw.Elapsed}";
                }
                catch (Exception ex)
                {
                    vmStat.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
                    vmStat.Status = $"ProcessFolder Error: {ex.Message}";
                    this.Ap.ShowError(ex);
                    //LogError("Pak.ProcessFolder", ex);
                }
                this.Ap.RemoveStatVM(vmStat);
            }
        }
    }
}
