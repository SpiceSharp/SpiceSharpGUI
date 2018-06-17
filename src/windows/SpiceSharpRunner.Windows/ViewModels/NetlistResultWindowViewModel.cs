using SpiceSharp.Components;
using SpiceSharp.Runner.Windows.ViewModels;
using SpiceSharp.Simulations;
using SpiceSharpParser.ModelsReaders.Netlist.Spice;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Evaluation.CustomFunctions;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls.Plots;
using SpiceSharpRunner.Windows.Common;
using SpiceSharpRunner.Windows.Controls;
using SpiceSharpRunner.Windows.Logic;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SpiceSharpRunner.Windows.ViewModels
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
        public string Title { get; set; }

        public bool CanClose => true;

        public bool IsResizable => true;

        public string Netlist { get; set; }

        private TabsViewModel _plots;
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

        private ObservableCollection<UIElement> _prints;
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

        private TreeViewModel _internals;
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

        private string _status;
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

        private bool _plotsEnabled;
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

        private string _stats;
        public string Stats
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

        private string _logs;
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
        public SpiceEvaluatorMode Mode { get; internal set; }

        public NetlistResultWindowViewModel(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
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

                var model = SpiceHelper.GetSpiceSharpNetlist(Netlist, Mode);

                AppendStats($"Simulations found: {model.Simulations.Count}");

                foreach (BaseSimulation simulation in model.Simulations)
                {
                    RunSimulation(model, simulation);
                }

                // Generate plots
                if (model.Plots.Count > 0)
                {
                    PlotsEnabled = true;

                    AppendStats($"Creating plots: {model.Plots.Count}");

                    if (model.Plots.Count > 0)
                    {
                        foreach (var plot in model.Plots)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                Plots.Items.Add(new TabItem() { Header = plot.Name, Content = new PlotControl() { Plot = plot, YEnabled = SpiceHelper.IsPlotPositive(plot) } });
                            });
                        }
                    }
                }

                AppendStats($"Prints found: {model.Prints.Count}");
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
                        Logs += ("Warning: " + warning + "\n");
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

        private void RunSimulation(SpiceNetlistReaderResult model, BaseSimulation simulation)
        {
            // Setup for Internals tab
            simulation.FinalizeSimulationExport += (arg, e) => {
                Dispatcher.Invoke(() =>
                {
                    TreeViewItem simulationItem = new TreeViewItem() { Header = simulation.Name };
                    TreeViewItem objects = new TreeViewItem() { Header = "Objects" };
                    simulationItem.Items.Add(objects);

                    var enumerator = model.Circuit.Objects.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var entity = enumerator.Current;
                        TreeViewItem item = new TreeViewItem() { Header = (string.Format("{0}     -    ({1})", entity.Name, entity)) };

                        if (entity is Component c)
                        {
                            for (var i = 0; i < c.PinCount; i++)
                            {
                                var nodeId = c.GetNode(i);
                                TreeViewItem nodeItem = new TreeViewItem() { Header = (string.Format("Node: {0}", nodeId)) };
                                item.Items.Add(nodeItem);
                            }
                        }

                        objects.Items.Add(item);
                    }

                    TreeViewItem variables = new TreeViewItem() { Header = "Variables" };
                    simulationItem.Items.Add(variables);

                    foreach (var variable in simulation.Nodes.GetVariables())
                    {
                        TreeViewItem item = new TreeViewItem { Header = string.Format("{0}     -     ({1})", variable.Name, variable.UnknownType) };
                        variables.Items.Add(item);
                    }

                    TreeViewItem parameters = new TreeViewItem() { Header = "Parameters" };
                    simulationItem.Items.Add(parameters);

                    foreach (var parameter in model.Evaluator.GetParameterNames())
                    {
                        TreeViewItem item = new TreeViewItem { Header = string.Format("{0}     -     ({1})", parameter, model.Evaluator.GetParameterValue(parameter, simulation)) };
                        parameters.Items.Add(item);
                    }

                    this.Internals.Items.Add(new TreeItem() { Content = simulationItem });
                });
            };

            AppendStats("---");
            AppendStats($"Running simulation: { simulation.Name }");
            AppendStats($"Plots found: {model.Plots.Count}");

            simulation.Run(model.Circuit);
           
            AppendStats($"Finished executing simulation {simulation.Name} ({simulation.GetType()})");
            AppendStats($"Number of itrations: {simulation.Statistics.Iterations}");
            AppendStats($"Solve time: {simulation.Statistics.SolveTime.ElapsedMilliseconds} ms");
            AppendStats($"Load time: {simulation.Statistics.LoadTime.ElapsedMilliseconds} ms");
            AppendStats($"Reorder time: {simulation.Statistics.ReorderTime.ElapsedMilliseconds} ms");
            AppendStats($"Behavior creation time: {simulation.Statistics.BehaviorCreationTime.ElapsedMilliseconds} ms");
            AppendStats($"Timepoints calculated: {simulation.Statistics.TimePoints}");
            AppendStats($"Transient iterations: {simulation.Statistics.TransientIterations}");
            AppendStats($"Transient time: {simulation.Statistics.TransientTime.ElapsedMilliseconds} ms");
            AppendStats($"Accepted timepoints: {simulation.Statistics.Accepted}");
            AppendStats($"Rejected timepoints: {simulation.Statistics.Rejected}");
            AppendStats("---");
        }

        private void AppendStats(string text)
        {
            Stats += (text + "\n");
        }

        /// <summary>
        /// Runs
        /// </summary>
        public void Run()
        {
            Task.Run(() =>
            {
                RunSimulations();
            });

            /*
            RunOnGUIThread(() =>
            {
                PlotsTab.IsEnabled = false;
                LogsTab.IsEnabled = false;
            });

            Stopwatch mainWatch = new Stopwatch();
            Stopwatch secondaryWatch = new Stopwatch();
            try
            {
                mainWatch.Start();

                // Reading netlist 
                secondaryWatch.Reset();
                secondaryWatch.Start();
                var spiceSharpModel = SpiceHelper.GetSpiceSharpNetlist(Netlist);
                secondaryWatch.Stop();

                RunOnGUIThread(() =>
                {
                    AppendStats($"Translating Netlist object model to Spice# time: { secondaryWatch.ElapsedMilliseconds} ms");
                });

              
                // Prints 
                RunOnGUIThread(() =>
                {
                    AppendStats($"Prints found: {spiceSharpModel.Prints.Count}");
                });

                if (spiceSharpModel.Prints.Count > 0)
                {
                    foreach (var print in spiceSharpModel.Prints)
                    {
                        RunOnGUIThread(() =>
                        {
                            this.PrintsTab.IsEnabled = true;
                            PrintControl control = new PrintControl(print);
                            control.DataBind();
                            this.PrintsPanel.Children.Add(control);
                        });
                    }
                }

                // Warnings
                foreach (var warning in spiceSharpModel.Warnings)
                {
                    RunOnGUIThread(() =>
                    {
                        txtLogs.Text += ("Warning: " + warning + "\n");
                    });
                }
                mainWatch.Stop();

                // Successfull finish
                RunOnGUIThread(() =>
                {
                    this.LogsTab.IsEnabled = true;
                    this.lblStatus.Text = "Status: Finished";
                    AppendStats($"---\nFinished executing netlist in {mainWatch.ElapsedMilliseconds} ms");
                });

            }
            catch (Exception ex)
            {
                // Error finish
                RunOnGUIThread(() =>
                {
                    txtLogs.Text += "Exception occurred: " + ex.ToString();
                    this.lblStatus.Text = "Status: Error (see 'Logs' tab)";
                    AppendStats($"---\nFinished executing netlist in {mainWatch.ElapsedMilliseconds} ms");
                    this.LogsTab.IsSelected = true;
                    this.LogsTab.IsEnabled = true;
                });
            }
            */
        }

        /*
        private void RunSimulation(Stopwatch secondaryWatch, SpiceNetlistReaderResult connectorResult, BaseSimulation simulation)
        {
            secondaryWatch.Reset();
            secondaryWatch.Start();

            simulation.FinalizeSimulationExport += (arg, e) => {
                // Circuit object tab
                RunOnGUIThread(() =>
                {
                    TreeViewItem simulationItem = new TreeViewItem() { Header = simulation.Name };
                    TreeViewItem objects = new TreeViewItem() { Header = "Objects" };
                    simulationItem.Items.Add(objects);

                    var enumerator = connectorResult.Circuit.Objects.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var entity = enumerator.Current;
                        TreeViewItem item = new TreeViewItem() { Header = (string.Format("{0}     -    ({1})", entity.Name, entity)) };

                        if (entity is Component c)
                        {
                            for (var i = 0; i < c.PinCount; i++)
                            {
                                var nodeId = c.GetNode(i);
                                TreeViewItem nodeItem = new TreeViewItem() { Header = (string.Format("Node: {0}", nodeId)) };
                                item.Items.Add(nodeItem);
                            }
                        }

                        objects.Items.Add(item);
                    }

                    TreeViewItem variables = new TreeViewItem() { Header = "Variables" };
                    simulationItem.Items.Add(variables);

                    foreach (var variable in simulation.Nodes.GetVariables())
                    {
                        TreeViewItem item = new TreeViewItem { Header = string.Format("{0}     -     ({1})", variable.Name, variable.UnknownType) };
                        variables.Items.Add(item);
                    }

                    TreeViewItem parameters = new TreeViewItem() { Header = "Parameters" };
                    simulationItem.Items.Add(parameters);

                    foreach (var parameter in connectorResult.Evaluator.GetParameterNames())
                    {
                        TreeViewItem item = new TreeViewItem { Header = string.Format("{0}     -     ({1})", parameter, connectorResult.Evaluator.GetParameterValue(parameter, simulation)) };
                        parameters.Items.Add(item);
                    }

                    this.CircuitElementsTreeView.Items.Add(simulationItem);
                });
            };
            simulation.Run(connectorResult.Circuit);
            secondaryWatch.Stop();

            // Stats
            RunOnGUIThread(() =>
            {
                AppendStats("---");
                AppendStats($"Finished executing simulation {simulation.Name} ({simulation.GetType()}) in {secondaryWatch.ElapsedMilliseconds} ms");

                AppendStats($"Number of itrations: {simulation.Statistics.Iterations}");
                AppendStats($"Solve time: {simulation.Statistics.SolveTime.ElapsedMilliseconds} ms");
                AppendStats($"Load time: {simulation.Statistics.LoadTime.ElapsedMilliseconds} ms");
                AppendStats($"Reorder time: {simulation.Statistics.ReorderTime.ElapsedMilliseconds} ms");
                AppendStats($"Behavior creation time: {simulation.Statistics.BehaviorCreationTime.ElapsedMilliseconds} ms");
                AppendStats($"Timepoints calculated: {simulation.Statistics.TimePoints}");
                AppendStats($"Transient iterations: {simulation.Statistics.TransientIterations}");
                AppendStats($"Transient time: {simulation.Statistics.TransientTime.ElapsedMilliseconds} ms");
                AppendStats($"Accepted timepoints: {simulation.Statistics.Accepted}");
                AppendStats($"Rejected timepoints: {simulation.Statistics.Rejected}");
                AppendStats("---");
            });
        }

        private void RunOnGUIThread(Action action)
        {
            this.Dispatcher.Invoke(() => action());
        }

        private void AppendStats(string text)
        {
            this.txtStats.Text += (text + "\n");
        }

    */
    }
}
