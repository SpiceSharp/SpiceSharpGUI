namespace SpiceSharp.Runner.Windows.ViewModels
{
    public class SummarySimulationStatistics
    {
        public int Iterations { get; set; }
        public double SolveTime { get; set; }
        public double LoadTime { get; set; }
        public double ReorderTime { get; set; }
        public double BehaviorCreationTime { get; set; }
        public int Timepoints { get; set; }
        public int TransientIterations { get; set; }
        public double TransientTime { get; set; }
        public int AcceptedTimepoints { get; set; }
        public int RejectedTimepoints { get; set; }
        public double TotalSimulationsTime { get; set; }
    }
}
