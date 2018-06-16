using SpiceSharpRunner.Windows.Common;
using System;
using System.Windows;

namespace SpiceSharpRunner.Windows.ViewModels
{
    public class NetlistWindowViewModel : ViewModelBase, IContent
    {
        public NetlistWindowViewModel(string initialTitle)
        {
            Title = initialTitle;
            CloseCommand = new Command(CloseWindow);
        }

        private string _netlist;
        public string Netlist
        {
            get
            {
                return _netlist;
            }

            set
            {
                _netlist = value;
                RaisePropertyChanged("NetList");
            }
        }

        private string _title;

        public string Title {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
                
            }
        }

        public event EventHandler Closing;

        public Command CloseCommand { get; }

        public bool IsResizable { get; set; } = true;

        public bool CanClose
        {
            get { return true; }
        }

        private void CloseWindow(object p)
        {
            if (this.CanClose)
            {
                this.Closing?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
