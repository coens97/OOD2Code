namespace FlowSystem.Presentation.ViewModel
{
    public class PipeViewModel : ViewModelBase
    {
        private double _maximumFlow;

        public double MaximumFlow
        {
            get { return _maximumFlow; }
            set { SetValue(ref _maximumFlow, value); }
        }
    }
}