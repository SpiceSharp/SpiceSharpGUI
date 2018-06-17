using SpiceSharpRunner.Windows.Common;
using System;
using System.Windows;

namespace SpiceSharpRunner.Windows.ViewModels
{
    public class NetlistWindowViewModel : ViewModelBase, IContent
    {
        public NetlistWindowViewModel(string path)
        {
            Path = path;
            Title = path != null ? "Netlist: " + path : "Netlist: unsaved";
            CloseCommand = new Command(CloseWindow);
        }

        private bool _dirty;
        public bool Dirty
        {
            get
            {
                return _dirty;
            }

            set
            {
                _dirty = value;
                if (value)
                {
                    Title = Path != null ? "(*) Netlist: " + Path : "Netlist: unsaved";
                }
                else
                {
                    Title = Path != null ? "Netlist: " + Path : "Netlist: unsaved";
                }

            }
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
                if (value != _netlist)
                {
                
                    if (_netlist == null)
                    {
                        _netlist = value;
                    }
                    else
                    {
                        _netlist = value;
                        RaisePropertyChanged("NetList");
                        Dirty = true;
                    }
                }
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

        private int _selectedMode;

        public int SelectedMode
        {
            get
            {
                return _selectedMode;
            }
            set
            {
                _selectedMode = value;
                RaisePropertyChanged("SelectedMode");
            }
        }

        private string _path;

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                RaisePropertyChanged("Path");
            }
        }

        public event EventHandler Closing;

        public Command CloseCommand { get; }

        public bool IsResizable { get; set; } = true;

        public bool CanClose
        {
            get { return !Dirty || MessageBox.Show("Do you want to close? You have unsaved changes.", "SpiceSharp-Runner", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes; }
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
