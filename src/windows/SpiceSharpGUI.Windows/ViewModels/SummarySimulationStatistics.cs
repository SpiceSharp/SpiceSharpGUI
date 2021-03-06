﻿namespace SpiceSharpGUI.Windows.ViewModels
{
    public class SummarySimulationStatistics
    {
        public double BehaviorCreationTime { get; set; }
        public long ExecutionTime { get; internal set; }
        public long FinishTime { get; internal set; }
        public long SetupTime { get; internal set; }
        public long ValidationTime { get; internal set; }
        public long TotalSimulationsTime { get; internal set; }
    }
}
