using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using SpiceSharp.Simulations;
using SpiceSharpGUI.Windows.Common;
using SpiceSharpGUI.Windows.Controls;
using SpiceSharpGUI.Windows.Logic;
using SpiceSharpParser.Common;
using SpiceSharpParser.ModelReaders.Netlist.Spice;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Exporters;

namespace SpiceSharpGUI.Windows.ViewModels
{
    public class TabItem
    {
        public string Header { get; set; }

        public UIElement Content { get; set; }
    }

    public class TabsViewModel
    {
        public ObservableCollection<TabItem> Items { get; set; } = new ObservableCollection<TabItem>();
        public TabItem Selected { get; set; }
    }

    public class TreeItem
    {
        public UIElement Content { get; set; }
    }

    public class TreeViewModel
    {
        public ObservableCollection<TreeItem> Items { get; set; } = new ObservableCollection<TreeItem>();
    }

    public class NetlistResultWindowViewModel : ViewModelBase, IContent
    {
        private TabsViewModel _plots;
        private ObservableCollection<UIElement> _prints;
        private TreeViewModel _internals;
        private string _status;
        private bool _plotsEnabled;
        private ObservableCollection<SimulationStatistics> _stats = new ObservableCollection<SimulationStatistics>();
        private ObservableCollection<SummarySimulationStatistics> _summaryStats = new ObservableCollection<SummarySimulationStatistics>();
        private string _logs;

        public NetlistResultWindowViewModel(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public string Title { get; set; }

        public bool CanClose => true;

        public bool IsResizable => true;

        public string Netlist { get; set; }

        public TabsViewModel Plots
        {
            get
            {
                return _plots;
            }

            set
            {
                _plots = value;
                RaisePropertyChanged("Plots");
            }
        }

        public ObservableCollection<UIElement> Prints
        {
            get
            {
                return _prints;
            }

            set
            {
                _prints = value;
                RaisePropertyChanged("Prints");
            }
        }

        public TreeViewModel Internals
        {
            get
            {
                return _internals;
            }

            set
            {
                _internals = value;
                RaisePropertyChanged("Internals");
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        public bool PlotsEnabled
        {
            get
            {
                return _plotsEnabled;
            }

            set
            {
                _plotsEnabled = value;
                RaisePropertyChanged("PlotsEnabled");
            }
        }

        public ObservableCollection<SimulationStatistics> Stats
        {
            get
            {
                return _stats;
            }

            set
            {
                _stats = value;
                RaisePropertyChanged("Stats");
            }
        }

        public ObservableCollection<SummarySimulationStatistics> SummaryStats
        {
            get
            {
                return _summaryStats;
            }

            set
            {
                _summaryStats = value;
                RaisePropertyChanged("SummaryStats");
            }
        }


        public string Logs
        {
            get
            {
                return _logs;
            }

            set
            {
                _logs = value;
                RaisePropertyChanged("Logs");
            }
        }

        public Dispatcher Dispatcher { get; }

        public int MaxDegreeOfParallelism { get; set; }

        public int? RandomSeed { get; set; }

        public bool HasTitle { get; set; }

        public string NetlistPath { get; set; }

        public Encoding Encoding { get; set; }

        /// <summary>
        /// Runs
        /// </summary>
        public void Run()
        {
            Task.Run(() =>
            {
                RunSimulations();
            });
        }

        private void RunSimulations()
        {
            try
            {
                Status = "Status: Running simulations";
                PlotsEnabled = false;

                Internals = new TreeViewModel();
                Plots = new TabsViewModel();
                Prints = new ObservableCollection<UIElement>();

                var parseResult = SpiceHelper.GetSpiceSharpNetlist(Netlist, RandomSeed, HasTitle, Encoding);

                if (parseResult != null)
                {
                    var model = new SpiceSharpParser.SpiceSharpReader().Read(parseResult.FinalModel);


                    if (model == null)
                    {
                        Logs += $"Errors in lexing: {parseResult.ValidationResult.HasError}\n";
                    }
                    else
                    {
                        SaveExportsToFile(model);
                        Logs += $"Simulations found: {model.Simulations.Count}\n";
                        Logs += "Errors and warnings: \n";

                        foreach (var log in model.ValidationResult)
                        {
                            Logs += log.Message + ", line =" + log.LineInfo.LineNumber + "\n";
                        }

                        int simulationNo = 0;

                        Stopwatch simulationsWatch = new Stopwatch();
                        simulationsWatch.Start();
                        System.Threading.Tasks.Parallel.ForEach<ISimulationWithEvents>(
                            model.Simulations,
                            new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, simulation => RunSimulation(model, (Simulation)simulation, Interlocked.Increment(ref simulationNo)));
                        simulationsWatch.Stop();

                        // Generate summary statistics
                        Dispatcher.Invoke(() =>
                        {
                            var summary = new SummarySimulationStatistics();

                            foreach (var stat in Stats)
                            {
                                summary.BehaviorCreationTime += stat.BehaviorCreationTime;
                                summary.FinishTime += stat.FinishTime;
                                summary.ExecutionTime += stat.ExecutionTime;
                                summary.SetupTime += stat.SetupTime;
                                summary.ValidationTime += stat.ValidationTime;
                            }
                            summary.TotalSimulationsTime = simulationsWatch.ElapsedMilliseconds;

                            SummaryStats.Add(summary);
                        });

                        // Generate plots
                        if (model.XyPlots.Count > 0)
                        {
                            PlotsEnabled = true;

                            Logs += $"Creating plots: {model.XyPlots.Count}\n";

                            if (model.XyPlots.Count > 0)
                            {
                                foreach (var plot in model.XyPlots)
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        Plots.Items.Add(new TabItem() { Header = plot.Name, Content = new XyPlotControl() { Plot = plot } });
                                    });
                                }
                            }
                        }

                        // Generate plots
                        if (model.MonteCarloResult.Enabled)
                        {
                            PlotsEnabled = true;

                            Logs += $"Creating monte carlo plot\n";

                            Dispatcher.Invoke(() =>
                            {
                                var plot = new HistogramPlotControl() { Data = model.MonteCarloResult };
                                plot.DataBind();
                                Plots.Items.Add(new TabItem() { Header = "Monte Carlo", Content = plot });
                            });
                        }

                        Logs += $"Prints found: {model.Prints.Count}\n";

                        if (model.Prints.Count > 0)
                        {
                            foreach (var print in model.Prints)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    PrintControl control = new PrintControl(print);
                                    control.DataBind();
                                    Prints.Add(control);
                                });
                            }
                        }
                    }
                    Status = "Status: Finished";
                }
            }
            catch (Exception ex)
            {
                Logs += ex.ToString();
                Status = "Status: Error";
            }
        }

        private void SaveExportsToFile(SpiceSharpModel model)
        {
            Dictionary<Export, List<string>> results = new Dictionary<Export, List<string>>();
            foreach (var export in model.Exports)
            {
                results[export] = new List<string>();
                export.Simulation.EventExportData += (sender, e) => {
                    try
                    {
                        if (export.Simulation is DC)
                        {
                            //TODO: Add

                            //results[export].Add( $"{e.};{export.Extract()}");
                        }

                        if (export.Simulation is OP)
                        {
                            results[export].Add($"{export.Extract()}");
                        }

                        if (export.Simulation is Transient t)
                        {
                            results[export].Add($"{t.Time};{export.Extract()}");
                        }

                        if (export.Simulation is AC f)
                        {
                            results[export].Add($"{f.Frequency};{export.Extract()}");
                        }
                    }
                    catch
                    {
                    }
                };
            }

            foreach (var export in results.Keys)
            {
                export.Simulation.EventAfterExecute += (sender, args) =>
                {
                    var exportTime = Environment.TickCount;
                    var outputPath = Path.Combine(Directory.GetCurrentDirectory(),
                        $"{Path.GetFileName(NetlistPath)}.{export.Simulation.Name}_{export.Name}_{exportTime}.RES");

                    if (results[export].Any())
                    {
                        File.WriteAllLines(outputPath, results[export]);
                    }
                };
            }
        }

        private void RunSimulation(SpiceSharpModel model, Simulation simulation, int index)
        {
            var simulationStats = new SimulationStatistics()
            {
                SimulationNo = index,
                SimulationName = simulation.Name
            };

            var codes = simulation.Run(model.Circuit, -1);

            if (simulation is ISimulationWithEvents sim)
            {
                codes = sim.InvokeEvents(codes);
            }

            codes.ToArray(); // evaluate

            simulationStats.BehaviorCreationTime = simulation.Statistics.BehaviorCreationTime.ElapsedMilliseconds;
            simulationStats.ExecutionTime = simulation.Statistics.ExecutionTime.ElapsedMilliseconds;
            simulationStats.FinishTime = simulation.Statistics.FinishTime.ElapsedMilliseconds;
            simulationStats.SetupTime = simulation.Statistics.SetupTime.ElapsedMilliseconds;
            simulationStats.ValidationTime = simulation.Statistics.ValidationTime.ElapsedMilliseconds;

            Dispatcher.Invoke(() =>
            {
                Stats.Add(simulationStats);
            });
        }
    }
}
