using SpiceSharpGUI.Windows.Common;
using SpiceSharpGUI.Windows.Logic;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace SpiceSharpGUI.Windows.ViewModels
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
            RunSimulation = new Command(Run, (obj) => !string.IsNullOrEmpty(Netlist));
            ParseCommand = new Command(Parse, (obj) => !string.IsNullOrEmpty(Netlist));
            ValidateCommand = new Command(Validate, (obj) => !string.IsNullOrEmpty(Netlist));
        }

        public int? RandomSeed { get; set; }

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

        private bool _hasTitle = true;

        public bool HasTitle
        {
            get
            {
                return _hasTitle;
            }
            set
            {
                _hasTitle = value;
                RaisePropertyChanged("HasTitle");
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

        public Command ParseCommand { get; }

        public Command ValidateCommand { get; }

        public Command RunSimulation { get; }

        public bool IsResizable { get; set; } = true;

        public bool CanClose
        {
            get { return !Dirty || MessageBox.Show("Do you want to close? You have unsaved changes.", "SpiceSharpGUI", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes; }
        }

        private void CloseWindow(object p)
        {
            if (this.CanClose)
            {
                this.Closing?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Parse(object obj)
        {
            try
            {
                var model = SpiceHelper.GetSpiceSharpNetlist(Netlist, (SpiceSharpParser.ModelReaders.Netlist.Spice.Evaluation.SpiceExpressionMode)SelectedMode, RandomSeed, HasTitle);
                MessageBox.Show("Parsing was successful", "SpiceSharpGUI", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Parsing failed: " + ex, "SpiceSharpGUI", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Validate(object obj)
        {
            try
            {
                var model = SpiceHelper.GetSpiceSharpNetlist(Netlist, (SpiceSharpParser.ModelReaders.Netlist.Spice.Evaluation.SpiceExpressionMode)SelectedMode, RandomSeed, HasTitle);
                model.Circuit.Validate();
                MessageBox.Show("Validating was successful", "SpiceSharpGUI", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Validating failed: " + ex, "SpiceSharpGUI", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Run(object obj)
        {
            NetlistResultWindowViewModel netlistWindow = new NetlistResultWindowViewModel(Dispatcher);
            netlistWindow.Title = "Results - " + Title;
            netlistWindow.Netlist = Netlist;
            netlistWindow.RandomSeed = RandomSeed;
            netlistWindow.HasTitle = HasTitle;
            netlistWindow.NetlistPath = Path;
            netlistWindow.Mode = (SpiceSharpParser.ModelReaders.Netlist.Spice.Evaluation.SpiceExpressionMode)SelectedMode;
            netlistWindow.MaxDegreeOfParallelism = MaxDegreeOfParallelism;
            netlistWindow.Run();
            this.Windows.Add(netlistWindow);
        }
    }
}
