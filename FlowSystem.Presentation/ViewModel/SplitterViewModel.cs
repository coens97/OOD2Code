namespace FlowSystem.Presentation.ViewModel
{
    public class SplitterViewModel : ViewModelBase
    {
        private int _distrubution;

        public int Distrubution
        {
            get { return _distrubution; }
            set { SetValue(ref _distrubution, value); }
        }
    }
}