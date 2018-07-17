namespace SpiceSharp.Runner.Windows.ViewModels
{
    public class SimulationStatistics
    {
        public int SimulationNo { get; set; }
        public string SimulationName { get; set; }
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
    }
}
