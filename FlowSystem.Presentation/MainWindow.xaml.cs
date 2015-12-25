using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FlowSystem.Business.Interfaces;
using FlowSystem.Common;
using FlowSystem.Common.Components;
using FlowSystem.Presentation.Controls;
using FlowSystem.Presentation.ViewModel;
using FontAwesome.WPF;
using Microsoft.Win32;
using System.ComponentModel;
using FlowSystem.Common.Interfaces;

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
        private ComponentControl _selectedComponent;

        public MainWindow(IFlowModel flowModel)
        {
            Icon = ImageAwesome.CreateImageSource(FontAwesomeIcon.Paw, Brushes.Black);
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

        private void SetSelectedComponent(ComponentControl component)
        {
            if (_selectedComponent != null)
                _selectedComponent.Style = null;

            var style = new Style
            {
                TargetType = typeof(ComponentControl)
            };

            style.Setters.Add(new Setter(OpacityProperty, 0.4)); // So lazy
            component.Style = style;

            _selectedComponent = component;

            var pump = component.Component as PumpEntity;
            var splitter = component.Component as SplitterEntity;

            if (pump != null)
            {
                var pumpViewModel = new PumpViewModel
                {
                    CurrentFlow = pump.CurrentFlow,
                    MaximumFlow = pump.MaximumFlow
                };
                pumpViewModel.PropertyChanged += PumpViewModelChanged;
                PropertiesSidebar.Content = pumpViewModel;
            }
            else if (splitter != null)
            {
                var splitterViewModel = new SplitterViewModel
                {
                    Devision = splitter.Distrubution
                };
                splitterViewModel.PropertyChanged += SplitterViewModelChanged;
                PropertiesSidebar.Content = splitterViewModel;
            }
            else
            {
                PropertiesSidebar.Content = null;
            }
        }

        private void AddComponentToScreen(IComponentEntity component)
        {
            var mouseDownHandler = new MouseButtonEventHandler(Component_MouseDown);
            var componentControl = new ComponentControl(component, mouseDownHandler);
            CanvasFlow.Children.Add(componentControl);
            SetSelectedComponent(componentControl);
            _changes = true;
        }
#region CanvasMousdownEvents
        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var p = e.GetPosition(CanvasFlow);
            var point = new PointEntity {X = p.X, Y = p.Y};
            try
            {
                switch (_mode)
                {
                    case Mode.Mouse:

                        break;
                    case Mode.Merger:
                        AddComponentToScreen(_flowModel.AddMerger(point));
                        break;
                    case Mode.Pump:
                        AddComponentToScreen(_flowModel.AddPump(point));
                        break;
                    case Mode.Draw:

                        _changes = true;
                        break;
                    case Mode.Sink:
                        AddComponentToScreen(_flowModel.AddSink(point));
                        break;
                    case Mode.Splitter:
                        AddComponentToScreen(_flowModel.AddSplitter(point));
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

        private void Component_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Components are only selectable when in mouse mode
            if (_mode != Mode.Mouse)
                return;

            var component = sender as ComponentControl;
            if (component == null)
                throw new Exception("Something went wrong, pleasee try again.");

            SetSelectedComponent(component);            
        }
        #endregion

        private void PumpViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            // The model of the data can be changed here, but since business logic isn't allowed here it is put in the model
            var viewmodel = sender as PumpViewModel;
            var pump = _selectedComponent.Component as PumpEntity;
            
            try
            {
                _flowModel.PumpPropertyChanged(
                    pump, e,
                    new PumpEntity
                    {
                        MaximumFlow = viewmodel.MaximumFlow,
                        CurrentFlow = viewmodel.CurrentFlow
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                viewmodel.MaximumFlow = pump.MaximumFlow;
                viewmodel.CurrentFlow = pump.CurrentFlow;
            }
        }

        private void SplitterViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            //_flowModel.ComponentPropertyChanged(_selectedComponent.Component, e);
        }
    }
}