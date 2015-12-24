namespace FlowSystem.Presentation.ViewModel
{
    public class PumpViewModel : ViewModelBase
    {
        private double _currentFlow;
        private double _maximumFlow;

        public double CurrentFlow
        {
            get { return _currentFlow; }
            set { SetValue(ref _currentFlow, value); }
        }

        public double MaximumFlow
        {
            get { return _maximumFlow; }
            set { SetValue(ref _maximumFlow, value); }
        }
    }
}