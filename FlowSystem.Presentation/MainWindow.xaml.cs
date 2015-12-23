using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FlowSystem.Business.Interfaces;
using FlowSystem.Common;
using Microsoft.Win32;

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

        private bool _changes = false;

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

        private bool CheckChangesAndAsk()
        {
            return !_changes ||
                   MessageBox.Show(
                       "You have unsaved changes, do you want to continue?",
                       "Flow system",
                       MessageBoxButton.YesNo) == MessageBoxResult.Yes;
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
            ResetMode();
            if (!CheckChangesAndAsk())
                return;

            var openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() != true)
                return;
            
            _flowModel.OpenFile(openFileDialog.FileName);

            _changes = false;
        }

        private void Save(bool overwrite)
        {
            ResetMode();

            var saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() != true)
                return;

            if (_flowModel.FileAlreadyExist(saveFileDialog.FileName)
                && !overwrite
                && MessageBox.Show("The file already excist, do you want to overwrite?", "Flow system", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            _flowModel.SaveFile(saveFileDialog.FileName);

            _changes = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Save(false);
        }

        private void BtnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            Save(true);
        }
        #endregion

        private void Frame_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this);
            var point = new PointEntity {X = (int)p.X, Y = (int)p.Y};

            try
            {
                switch (_mode)
                {
                    case Mode.Mouse:

                        break;
                    case Mode.Merger:
                        _flowModel.AddMerger(point);
                        _changes = true;
                        break;
                    case Mode.Pump:
                        _flowModel.AddPump(point);
                        _changes = true;
                        break;
                    case Mode.Draw:

                        _changes = true;
                        break;
                    case Mode.Sink:
                        _flowModel.AddSink(point);
                        _changes = true;
                        break;
                    case Mode.Splitter:
                        _flowModel.AddSplitter(point);
                        _changes = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                ResetMode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
