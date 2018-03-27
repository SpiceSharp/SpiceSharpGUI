using ICSharpCode.AvalonEdit.Search;
using SpiceSharp.Circuits;
using SpiceSharp.Components;
using SpiceSharp.Runner.Windows.Controls;
using SpiceSharp.Simulations;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SpiceSharp.Runner.Windows
{
    /// <summary>
    /// Spice netlist run result window
    /// </summary>
    public partial class SpiceNetlistResult : Window
    {
        public SpiceNetlistResult()
        {
            InitializeComponent();

            SearchPanel.Install(txtNetlist);
        }
       
        public SpiceNetlistResult(string netlist) : this()
        {
            Netlist = netlist;

            Task.Run(() => Run());
        }

        /// <summary>
        /// Gets the netlist 
        /// </summary>
        public string Netlist { get; }

        private void Run()
        {
            RunOnGUIThread(() =>
            {
                lblStatus.Text = "Status: Running ...";
                txtNetlist.Text = Netlist;
                PlotsTab.IsEnabled = false;
                LogsTab.IsEnabled = false;
            });

            Stopwatch mainWatch = new Stopwatch();
            Stopwatch secondaryWatch = new Stopwatch();
            try
            {
                mainWatch.Start();

                // Tokenization generation
                secondaryWatch.Start();
                var tokens = SpiceHelper.GetTokens(Netlist);
                secondaryWatch.Stop();
                RunOnGUIThread(() =>
                {
                   AppendStats($"Tokenization time: { secondaryWatch.ElapsedMilliseconds} ms");
                });
    
                // Parse tree generation
                secondaryWatch.Reset();
                secondaryWatch.Start();
                var parseTree = SpiceHelper.GetParseTree(tokens);
                secondaryWatch.Stop();
                RunOnGUIThread(() =>
                {
                    AppendStats($"Parse tree generation time: { secondaryWatch.ElapsedMilliseconds} ms");
                });

                // Netlist object model generation
                secondaryWatch.Reset();
                secondaryWatch.Start();
                var netlist = SpiceHelper.GetNetlist(parseTree);
                secondaryWatch.Stop();
                RunOnGUIThread(() =>
                {
                    AppendStats($"Netlist object model generation time: { secondaryWatch.ElapsedMilliseconds} ms");
                });

                // Translating Netlist object model to Spice# 
                secondaryWatch.Reset();
                secondaryWatch.Start();
                var sNetlist = SpiceHelper.GetSpiceSharpNetlist(netlist);
                secondaryWatch.Stop();

                RunOnGUIThread(() =>
                {
                    AppendStats($"Translating Netlist object model to Spice# time: { secondaryWatch.ElapsedMilliseconds} ms");
                });

                // Simulations
                RunOnGUIThread(() =>
                {
                    AppendStats($"---");
                    AppendStats($"Simulations found: {sNetlist.Simulations.Count}");
                });

                // Simulation run
                foreach (BaseSimulation simulation in sNetlist.Simulations)
                {
                    RunSimulation(secondaryWatch, sNetlist, simulation);                    
                }


                // Plots 
                RunOnGUIThread(() =>
                {
                    AppendStats($"Plots found: {sNetlist.Plots.Count}");
                });

                if (sNetlist.Plots.Count > 0)
                {
                    foreach (var plot in sNetlist.Plots)
                    {
                        bool positive = SpiceHelper.IsPlotPositive(plot);

                        RunOnGUIThread(() =>
                        {
                            this.PlotsTab.IsEnabled = true;
                            PlotControl control = new PlotControl(plot, positive);

                            var item = new TabItem() { Header = plot.Name };
                            item.Content = control;
                            this.PlotsTabs.Items.Add(item);
                        });
                    }
                }

                // Warnings
                foreach (var warning in sNetlist.Warnings)
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
        }

        private void RunSimulation(Stopwatch secondaryWatch, SpiceNetlist.SpiceSharpConnector.Netlist sNetlist, Simulations.BaseSimulation simulation)
        {
            secondaryWatch.Reset();
            secondaryWatch.Start();

            simulation.FinalizeSimulationExport += (arg,e) => {
                // Circuit object tab
                RunOnGUIThread(() =>
                {
                    TreeViewItem simulationItem = new TreeViewItem() { Header = simulation.Name };
                    TreeViewItem objects = new TreeViewItem() { Header = "Objects" };
                    simulationItem.Items.Add(objects);

                    var enumerator = sNetlist.Circuit.Objects.GetEnumerator();
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
                        TreeViewItem item = new TreeViewItem { Header = string.Format("{0}     -     ({1})", variable.Name, variable.UnknownType)};
                        variables.Items.Add(item);
                    }

                    this.CircuitElementsTreeView.Items.Add(simulationItem);
                });
            };
            simulation.Run(sNetlist.Circuit);
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
    }
}
