namespace SpiceSharpGUI.Windows.ViewModels
{
    public class SimulationStatistics
    {
        public int SimulationNo { get; set; }
        public string SimulationName { get; set; }
        public double BehaviorCreationTime { get; set; }
        public long ExecutionTime { get; internal set; }
        public long FinishTime { get; internal set; }
        public long SetupTime { get; internal set; }
        public long ValidationTime { get; internal set; }
    }
}
