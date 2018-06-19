using Avalonia;
using Avalonia.Diagnostics.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace SpiceSharpRunner.Portable
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveCommand ExitCommand { get; }

        public ReactiveCommand<string, Unit> RunCommand { get; }

        public MainWindowViewModel()
        {
            ExitCommand = ReactiveCommand.Create(Exit);
            RunCommand = ReactiveCommand.Create<string, Unit>(Run);
        }

        private void Exit()
        {
            Application.Current.Exit();
        }

        private Unit Run(string netlist)
        {
            try
            {
                if (netlist == null)
                {
                    return Unit.Default;
                }

                var parseResult = Logic.SpiceHelper.GetSpiceSharpNetlist(netlist, SpiceSharpParser.ModelsReaders.Netlist.Spice.Evaluation.CustomFunctions.SpiceEvaluatorMode.Spice3f5);
                Logic.SpiceHelper.Run(parseResult);
                Console.WriteLine("Finished");

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
