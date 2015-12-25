namespace FlowSystem.Presentation.ViewModel
{
    public class PipeViewModel : ViewModelBase
    {
        private double _maximumFlow;
        private double _currentFlow;

        public double MaximumFlow
        {
            get { return _maximumFlow; }
            set { SetValue(ref _maximumFlow, value); }
        }

        public double CurrentFlow
        {
            get { return _currentFlow; }
            set { SetValue(ref _currentFlow, value); }
        }
    }
}