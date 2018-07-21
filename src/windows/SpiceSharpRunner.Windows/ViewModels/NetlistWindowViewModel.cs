using SpiceSharpRunner.Windows.Common;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace SpiceSharpRunner.Windows.ViewModels
{
    public class NetlistWindowViewModel : ViewModelBase, IContent
    {
        public Dispatcher Dispatcher { get; }
        public ObservableCollection<IContent> Windows { get; }

        public NetlistWindowViewModel(string path, Dispatcher dispatcher, ObservableCollection<IContent> windows)
        {
            Windows = windows;
            Dispatcher = dispatcher;
            Path = path;
            Title = path != null ? "Netlist: " + path : "Netlist: unsaved";
            CloseCommand = new Command(CloseWindow);
            RunSimulation = new Command(Run);
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

        public int MaxDegreeOfParallelism { get; set; } = 1;

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

        public Command RunSimulation { get; }

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


        private void Run(object obj)
        {
            NetlistResultWindowViewModel netlistWindow = new NetlistResultWindowViewModel(Dispatcher);
            netlistWindow.Title = "Results - " + Title;
            netlistWindow.Netlist = Netlist;
            netlistWindow.Mode = (SpiceSharpParser.ModelsReaders.Netlist.Spice.Evaluation.CustomFunctions.SpiceEvaluatorMode)SelectedMode;
            netlistWindow.MaxDegreeOfParallelism = MaxDegreeOfParallelism;
            netlistWindow.Run();
            this.Windows.Add(netlistWindow);
        }
    }
}
