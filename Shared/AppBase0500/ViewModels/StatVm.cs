using System.Windows.Shell;

namespace AppBase0500
{
    public class StatVm : NotifyBase
    {
        public WindowStat UI;
        public int Index { get; set; } = -1;

        private double left = 0;
        public double Left
        {
            get => this.left;
            set => SetProperty(ref this.left, value);
        }
        private double top = 0;
        public double Top
        {
            get => this.top;
            set => SetProperty(ref this.top, value);
        }
        private string title = "";
        public string Title
        {
            get => this.title;
            set => SetProperty(ref this.title, value);
        }

        private string status = "";
        public string Status
        {
            get => this.status;
            set => SetProperty(ref this.status, value);
        }

        private double progress = 0;
        public double Progress
        {
            get => this.progress;
            set
            {
                SetProperty(ref this.progress, value);
                this.ProgressValue = value / 100;
            }
        }


        private double progressValue = 0;
        public double ProgressValue
        {
            get => this.progressValue;
            set => SetProperty(ref this.progressValue, value);
        }

        private TaskbarItemProgressState progressState = TaskbarItemProgressState.Normal;
        public TaskbarItemProgressState ProgressState
        {
            get => this.progressState;
            set
            {
                SetProperty(ref this.progressState, value);
                this.IsIndeterminate = (value == TaskbarItemProgressState.Indeterminate);
            }
        }

        private bool isIndeterminate = false;
        public bool IsIndeterminate
        {
            get => this.isIndeterminate;
            set => SetProperty(ref this.isIndeterminate, value);
        }



        public StatVm()
        {
            this.UI = new WindowStat
            {
                DataContext = this
            };

        }

    }
}
