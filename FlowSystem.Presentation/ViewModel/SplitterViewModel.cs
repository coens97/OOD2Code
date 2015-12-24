namespace FlowSystem.Presentation.ViewModel
{
    public class SplitterViewModel : ViewModelBase
    {
        private int _devision;

        public int Devision
        {
            get { return _devision; }
            set { SetValue(ref _devision, value); }
        }
    }
}