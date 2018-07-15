using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics.ViewModels;
using ReactiveUI;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Evaluation.CustomFunctions;
using SpiceSharpRunner.Portable.Views;
using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;

namespace SpiceSharpRunner.Portable.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Window Window { get; }

        public MainWindowViewModel(Window window)
        {
            Window = window;
            OpenCommand = ReactiveCommand.CreateFromTask(Open);
            SaveCommand = ReactiveCommand.CreateFromTask(Save);
            RunCommand = ReactiveCommand.Create(Run);
            ExitCommand = ReactiveCommand.Create(Exit);
            AboutCommand = ReactiveCommand.Create(About);
        }

        public ReactiveCommand OpenCommand { get; }
        public ReactiveCommand ExitCommand { get; }
        public ReactiveCommand SaveCommand { get; }
        public ReactiveCommand RunCommand { get; }
        public ReactiveCommand AboutCommand { get; }


        public int SelectedEvaluatorMode
        {
            get;
            set;
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
                this._netlist = value;
                RaisePropertyChanged("Netlist");
            }
        }

        private async Task Open()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AllowMultiple = false;
            dialog.Title = "Opening netlist dialog";
            var files = await dialog.ShowAsync();

            if (files.Length > 0)
            {
                Netlist = File.ReadAllText(files[0]);
            }
        }

        private async Task Save()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Saving netlist dialog";
            var fileName = await dialog.ShowAsync(Window);

            if (fileName != null)
            {
                File.WriteAllText(fileName, Netlist);

                Window.Title = "SpiceSharpRunner.Portable: " + fileName;
            }
        }

        private void Exit()
        {
            Application.Current.Exit();
        }

        private void About()
        {
            Console.WriteLine("Spice simulation environment for Linux and MacOS built with SpiceSharp and SpiceSharpParser. Used third-party libraries: Avalonia, OxyPlot");
        }

        private Unit Run()
        {
            try
            {
                if (Netlist == null)
                {
                    return Unit.Default;
                }

                var parseResult = Logic.SpiceHelper.GetSpiceSharpNetlist(Netlist, (SpiceEvaluatorMode)SelectedEvaluatorMode);
                Logic.SpiceHelper.Run(parseResult);

                if (parseResult.Plots.Count > 0)
                {
                    foreach (var plot in parseResult.Plots)
                    {
                        PlotWindow window = new PlotWindow(plot);
                        window.Show();
                    }
                }

                return Unit.Default;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Unit.Default;
            }
        }
    }
}
