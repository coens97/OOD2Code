using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlowSystem.Presentation.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (property != null)
            {
                if (property.Equals(value)) return;
            }

            OnPropertyChanged(propertyName);
            property = value;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}