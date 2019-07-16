using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SpiceSharp.Components;
using SpiceSharp.Simulations;
using SpiceSharpGUI.Windows.Common;
using SpiceSharpGUI.Windows.Controls;
using SpiceSharpGUI.Windows.Logic;
using SpiceSharpParser.ModelReaders.Netlist.Spice;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Evaluation;
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

        public SpiceExpressionMode Mode { get; set; }

        public int MaxDegreeOfParallelism { get; set; }

        public int? RandomSeed { get; set; }

        public bool HasTitle { get; set; }

        public string NetlistPath { get; set; }

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

                var model = SpiceHelper.GetSpiceSharpNetlist(Netlist, Mode, RandomSeed, HasTitle);

                SaveExportsToFile(model);

                Logs += $"Simulations found: {model.Simulations.Count}\n";
                int simulationNo = 0;

                Stopwatch simulationsWatch = new Stopwatch();
                simulationsWatch.Start();
                Parallel.ForEach<Simulation>(
                    model.Simulations, 
                    new ParallelOptions() { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, simulation => RunSimulation(model, (BaseSimulation)simulation, Interlocked.Increment(ref simulationNo)));
                simulationsWatch.Stop();

                // Generate summary statistics
                Dispatcher.Invoke(() =>
                {
                    var summary = new SummarySimulationStatistics();

                    foreach (var stat in Stats)
                    {
                        summary.Iterations += stat.Iterations;
                        summary.SolveTime += stat.SolveTime;
                        summary.LoadTime += stat.LoadTime;
                        summary.ReorderTime += stat.ReorderTime;
                        summary.BehaviorCreationTime += stat.BehaviorCreationTime;
                        summary.Timepoints += stat.Timepoints;
                        summary.TransientIterations += stat.TransientIterations;
                        summary.TransientTime += stat.TransientTime;
                        summary.AcceptedTimepoints += stat.AcceptedTimepoints;
                        summary.RejectedTimepoints += stat.RejectedTimepoints;
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

                foreach (var warning in model.Warnings)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Logs += "Warning: " + warning + "\n";
                    });
                }

                Status = "Status: Finished";
            }
            catch (Exception ex)
            {
                Logs += ex.ToString();
                Status = "Status: Error";
            }
        }

        private void SaveExportsToFile(SpiceNetlistReaderResult model)
        {
            Dictionary<Export, List<string>> results = new Dictionary<Export, List<string>>();
            foreach (var export in model.Exports)
            {
                results[export] = new List<string>();
                export.Simulation.ExportSimulationData += (sender, e) => {
                    try
                    {
                        if (export.Simulation is DC)
                        {
                            results[export].Add( $"{e.SweepValue};{export.Extract()}");
                        }

                        if (export.Simulation is OP)
                        {
                            results[export].Add($"{export.Extract()}");
                        }

                        if (export.Simulation is Transient)
                        {
                            results[export].Add($"{e.Time};{export.Extract()}");
                        }

                        if (export.Simulation is AC)
                        {
                            results[export].Add($"{e.Frequency};{export.Extract()}");
                        }
                    }
                    catch
                    {
                    }
                };
            }

            foreach (var export in results.Keys)
            {
                export.Simulation.AfterExecute += (sender, args) =>
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

        private void RunSimulation(SpiceNetlistReaderResult model, BaseSimulation simulation, int index)
        {
            var simulationStats = new SimulationStatistics()
            {
                SimulationNo = index,
                SimulationName = simulation.Name
            };

            simulation.Run(model.Circuit);

            simulationStats.Iterations = simulation.Statistics.Get<BaseSimulationStatistics>().Iterations;
            simulationStats.SolveTime = simulation.Statistics.Get<BaseSimulationStatistics>().SolveTime.ElapsedMilliseconds;
            simulationStats.LoadTime = simulation.Statistics.Get<BaseSimulationStatistics>().LoadTime.ElapsedMilliseconds;
            simulationStats.ReorderTime = simulation.Statistics.Get<BaseSimulationStatistics>().ReorderTime.ElapsedMilliseconds;
            simulationStats.BehaviorCreationTime = simulation.Statistics
                .Get<SpiceSharp.Simulations.SimulationStatistics>().BehaviorCreationTime.ElapsedMilliseconds;

                if (simulation is TimeSimulation)
                {
                    simulationStats.Timepoints = simulation.Statistics.Get<TimeSimulationStatistics>().TimePoints;
                    simulationStats.TransientIterations =
                        simulation.Statistics.Get<TimeSimulationStatistics>().TransientIterations;
                    simulationStats.TransientTime = simulation.Statistics.Get<TimeSimulationStatistics>().TransientTime
                        .ElapsedMilliseconds;
                    simulationStats.AcceptedTimepoints = simulation.Statistics.Get<TimeSimulationStatistics>().Accepted;
                    simulationStats.RejectedTimepoints = simulation.Statistics.Get<TimeSimulationStatistics>().Rejected;
                }

            Dispatcher.Invoke(() =>
            {
                Stats.Add(simulationStats);
            });
        }
    }
}
