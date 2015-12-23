using System;
using System.Windows;
using System.Windows.Media;
using FlowSystem.Business.Interfaces;

namespace FlowSystem.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Brush _buttonColor = Brushes.LightGray;
        private static readonly Brush _buttonActiveColor = Brushes.White;

        private IFlowModel _flowModel;

        private Mode _mode = Mode.Mouse;
        public MainWindow(IFlowModel flowModel)
        {
            _flowModel = flowModel;
            InitializeComponent();
            ResetMode();
        }

        private void ResetMode()
        {
            ResetButtons();
            _mode = Mode.Mouse;
            BtnMouse.Background = _buttonActiveColor;
        }

        private void ResetButtons()
        {
            var buttons = new [] {BtnMouse, BtnDraw, BtnMerger, BtnPump, BtnSink, BtnSplitter};
            foreach (var button in buttons)
            {
                button.Background = _buttonColor;
            }
        }

#region ButtonEvents
        private void BtnMouse_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Mouse;
            BtnMouse.Background = _buttonActiveColor;
        }

        private void BtnDraw_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Draw;
            BtnDraw.Background = _buttonActiveColor;
        }

        private void BtnMerger_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Merger;
            BtnMerger.Background = _buttonActiveColor;
        }

        private void BtnPump_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Pump;
            BtnPump.Background = _buttonActiveColor;
        }

        private void BtnSink_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Sink;
            BtnSink.Background = _buttonActiveColor;
        }

        private void BtnSplitter_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Splitter;
            BtnSplitter.Background = _buttonActiveColor;
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSaveAs_Click(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (_mode)
            {
                case Mode.Mouse:

                break;
                case Mode.Merger:

                break;
                case Mode.Pump:

                break;
                case Mode.Draw:

                break;
                case Mode.Sink:

                break;
                case Mode.Splitter:

                break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
