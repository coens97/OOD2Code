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
using System.Linq;
using System.Windows.Shapes;
using FlowSystem.Common.Interfaces;

namespace FlowSystem.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Brush ButtonColor = Brushes.LightGray;
        private static readonly Brush ButtonActiveColor = Brushes.White;
        private static readonly int ComponentHeight = 32;
        private static readonly int ComponentWidth = 32;

        private IFlowModel _flowModel;

        private bool _changes = false;
        private Mode _mode = Mode.Mouse;
        private ComponentControl _selectedComponent;

        private IFlowOutput _pathStart;
        private int _startIndex;
        private List<PointEntity> _pathPoints;
        private Path _currentPath;
        private bool _ignoreClick = false;

        private Dictionary<PipeEntity, Path> _pipePaths = new Dictionary<PipeEntity, Path>();  

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
            BtnMouse.Background = ButtonActiveColor;
        }

        private void ResetButtons()
        {
            var buttons = new [] {BtnMouse, BtnDraw, BtnMerger, BtnPump, BtnSink, BtnSplitter};
            foreach (var button in buttons)
            {
                button.Background = ButtonColor;
            }

            _pathStart = null;
            _pathPoints = null;
            if (_currentPath != null)
                CanvasFlow.Children.Remove(_currentPath);
            _currentPath = null;
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
            BtnMouse.Background = ButtonActiveColor;
        }

        private void BtnDraw_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Draw;
            BtnDraw.Background = ButtonActiveColor;

            _pathStart = null;
            _pathPoints = new List<PointEntity>();
        }

        private void BtnMerger_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Merger;
            BtnMerger.Background = ButtonActiveColor;
        }

        private void BtnPump_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Pump;
            BtnPump.Background = ButtonActiveColor;
        }

        private void BtnSink_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Sink;
            BtnSink.Background = ButtonActiveColor;
        }

        private void BtnSplitter_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Splitter;
            BtnSplitter.Background = ButtonActiveColor;
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

            if (component == null)
                return;

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
                    Distrubution = splitter.Distrubution
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

        private int GetMouseInputOutputIndex(PointEntity mouse, int nrOfIndex)
        {
            if (nrOfIndex == 1)
                return 0;
            return (int)(mouse.Y / (ComponentHeight/nrOfIndex));
        }
#region CanvasMousdownEvents
        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_ignoreClick) // If mousedown is triggered on component this event doesn't need to be triggered
            {
                _ignoreClick = false;
                return;
            }

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
                        if (_pathStart != null)
                        {
                            _pathPoints.Add(point);

                            if (_currentPath == null)
                            {
                                _currentPath = new Path
                                {
                                    Stroke = Brushes.Black,
                                    StrokeThickness = 3,
                                    Data = GetGeometryOfDrawingPath()
                                };
                                CanvasFlow.Children.Add(_currentPath);
                            }
                            else
                            {
                                _currentPath.Data = GetGeometryOfDrawingPath();
                            }
                            return; // Skip the ResetMode();
                        }
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
            var component = sender as ComponentControl;
            if (component == null)
                throw new Exception("Something went wrong, please try again.");

            var p = e.GetPosition(component);
            var mouse = new PointEntity
            {
                X = p.X,
                Y = p.Y
            };

            try
            {
                // Components are only selectable when in mouse mode
                switch (_mode)
                {
                    case Mode.Mouse:
                        SetSelectedComponent(component);
                        break;
                    case Mode.Draw:
                        _ignoreClick = true;
                        if (_pathStart == null)
                        {
                            var start = component.Component as IFlowOutput;
                            if (start == null)
                            {
                                MessageBox.Show("Please select a component with an output");
                            }
                            else
                            {
                                SetSelectedComponent(component);
                                _pathStart = start;
                                _startIndex = GetMouseInputOutputIndex(mouse, start.FlowOutput.Length);
                            }
                        }
                        else
                        {
                            var end = component.Component as IFlowInput;
                            if (end == null)
                            {
                                MessageBox.Show("Please select a component with input");
                            }
                            else
                            {
                                var point = new PointEntity
                                {
                                    X = end.Position.X,
                                    Y = end.Position.Y + 16
                                };

                                _pathPoints.Add(point);
                                _currentPath.Data = GetGeometryOfDrawingPath();

                                var endIndex = GetMouseInputOutputIndex(mouse, end.FlowInput.Length);
                                var pipe =_flowModel.AddPipe(_pathStart, end, _pathPoints, _startIndex, endIndex);
                                _pipePaths[pipe] = _currentPath;

                                _pathStart = null;
                                _pathPoints = null;
                                // CanvasFlow.Children.Remove(_currentPath);
                                _currentPath = null;
                                ResetMode();
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private Geometry GetGeometryOfDrawingPath()
        {
            var geometryString = $"M{_pathStart.Position.X},{_pathStart.Position.Y} ";
            var p = _pathPoints.Select(x => $"L{x.X},{x.Y}");
            geometryString += string.Join(" ", p);
            return Geometry.Parse(geometryString);
        }
#region ViewModelsChanged
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
                _changes = true;
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
            // The model of the data can be changed here, but since business logic isn't allowed here it is put in the model
            var viewmodel = sender as SplitterViewModel;
            var splitter = _selectedComponent.Component as SplitterEntity;

            try
            {
                _flowModel.SplitterPropertyChanged(
                    splitter, e,
                    new SplitterEntity
                    {
                        Distrubution = viewmodel.Distrubution
                    });
                _changes = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                viewmodel.Distrubution = splitter.Distrubution;
            }
        }
#endregion
    }
}