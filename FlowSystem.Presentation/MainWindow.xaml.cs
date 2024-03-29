﻿using System;
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
using System.Globalization;
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
        private static readonly Brush ComponentColor = Brushes.Black;
        private static readonly Brush WarningColor = Brushes.Red;
        private static readonly Brush SelectedColor = Brushes.DeepSkyBlue;

        private static readonly int ComponentHeight = 32;
        private static readonly int ComponentWidth = 32;

        private IFlowModel _flowModel;

        private bool _changes = false;
        private Mode _mode = Mode.Mouse;
        private ComponentControl _selectedComponent;
        private Path _selectedPath;

        private IFlowOutput _pathStart;
        private int _startIndex;
        private List<PointEntity> _pathPoints;
        private Tuple<Path, TextBlock> _currentPath;
        private bool _ignoreClick = false;
        private string _path = string.Empty;
        private IComponentEntity _clone;

        private Dictionary<Tuple<Path, TextBlock>, PipeEntity> _pipePaths = new Dictionary<Tuple<Path, TextBlock>, PipeEntity>(); // These could be done nicer
        private Dictionary<Tuple<Path, TextBlock>, PipeEntity> _overloadedPipes = new Dictionary<Tuple<Path, TextBlock>, PipeEntity>();
        public MainWindow(IFlowModel flowModel)
        {
            Icon = ImageAwesome.CreateImageSource(FontAwesomeIcon.Paw, Brushes.Black);
            _flowModel = flowModel;
            _flowModel.FlowNetworkUpdated += OnFlowNetworkUpdated;
            InitializeComponent();
            ResetMode();
        }

        private void ResetMode()
        {
            ResetButtons();
            _mode = Mode.Mouse;
            BtnMouse.Background = ButtonActiveColor;
            _clone = null;
        }

        private void ResetButtons()
        {
            var buttons = new[] { BtnMouse, BtnDraw, BtnMerger, BtnPump, BtnSink, BtnSplitter, BtnClone, BtnDelete };
            foreach (var button in buttons)
            {
                button.Background = ButtonColor;
            }

            _pathStart = null;
            _pathPoints = null;
            if (_currentPath != null)
            {
                CanvasFlow.Children.Remove(_currentPath.Item1);
                CanvasFlow.Children.Remove(_currentPath.Item2);
            }
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
            if (!CheckChangesAndAsk())
                return;
            ResetAll();

            var openFileDialog = new OpenFileDialog { Filter = "Flow file (*.flow)|*.flow" };
            if (openFileDialog.ShowDialog() != true)
                return;

            _path = openFileDialog.FileName;
            _flowModel.OpenFile(openFileDialog.FileName);

            ShowNetwork();
            _changes = false;
        }

        private void ShowNetwork()
        {
            foreach (var componentEntity in _flowModel.FlowNetwork.Components)
            {
                AddComponentToScreen(componentEntity);
            }

            foreach (var pipeEntity in _flowModel.FlowNetwork.Pipes)
            {
                _pipePaths[CreatePath(pipeEntity.Path)] = pipeEntity;
            }
            OnFlowNetworkUpdated();
        }

        private void ResetAll()
        {
            _flowModel.FlowNetwork = new FlowNetworkEntity
            {
                Components = new List<IComponentEntity>(),
                Pipes = new List<PipeEntity>()
            };
            ResetMode();
            _pipePaths = new Dictionary<Tuple<Path, TextBlock>, PipeEntity >();
            _overloadedPipes = new Dictionary<Tuple<Path, TextBlock>, PipeEntity>();
            _changes = false;
            _path = string.Empty;
            CanvasFlow.Children.Clear();
        }
        private void BtnNewFile_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckChangesAndAsk())
                return;
            ResetAll();
        }
        private void BtnClone_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Clone;
            BtnClone.Background = ButtonActiveColor;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            _mode = Mode.Delete;
            BtnDelete.Background = ButtonActiveColor;
        }

        private void Save(bool saveAs)
        {
            ResetMode();

            string path;
            if (!saveAs && _path != string.Empty)
            {
                path = _path;
            }
            else
            {
                var saveFileDialog = new SaveFileDialog { Filter = "Flow file (*.flow)|*.flow" };
                if (saveFileDialog.ShowDialog() != true)
                    return;
                path = saveFileDialog.FileName;
            }
            _flowModel.SaveFile(path);
            _path = path;

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
            ResetSelected();
        
            component.Foreground = SelectedColor;

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
            return (int)(mouse.Y / (ComponentHeight / nrOfIndex));
        }

        private PointEntity GetInputOuputPosition(IComponentEntity component, bool input, int nrOfIndex)
        {
            double x, y;
            if (input)
            {
                var inp = component as IFlowInput;
                if (inp == null)
                    throw new Exception("Component is not input");
                x = component.Position.X;
                y = component.Position.Y + (ComponentHeight / (inp.FlowInput.Length + 1)) * (nrOfIndex + 1);

            }
            else
            {
                var outp = component as IFlowOutput;
                if (outp == null)
                    throw new Exception("Component is not output");
                x = component.Position.X + ComponentWidth;
                y = component.Position.Y + (ComponentHeight / (outp.FlowOutput.Length + 1)) * (nrOfIndex + 1);
            }
            return new PointEntity
            {
                X = x,
                Y = y
            };
        }

        private void CreatePathIfDoesntExist()
        {
            if (_currentPath == null)
            {
                _currentPath = CreatePath(_pathPoints);
            }
            else
            {
                _currentPath.Item1.Data = GetGeometryOfDrawingPath(_pathPoints);
                MoveTextBox(_pathPoints, _currentPath.Item2);
            }
        }

        private void MoveTextBox(List<PointEntity> points, TextBlock textBlock)
        {
            // Calculate the textblock position
            // Would be nice to describe in Design document, but code is written really quickly
            // But hope you can get the gist of it since LINQ is very readable
            var middleX = (points.First().X + points.Last().X) / 2;
            //  Get the line at the middle of the path
            var left = points.Where(x => x.X <= middleX).Aggregate((i1, i2) => i1.X >= i2.X ? i1 : i2);
            var right = points.Where(x => x.X > middleX).Aggregate((i1, i2) => i1.X < i2.X ? i1 : i2);

            var angle = Math.Atan2(right.Y - left.Y, right.X - left.X);
            var yPos = left.Y + Math.Tan(angle) * (middleX - left.X);

            yPos += (angle > 0) ? -30 : 4;

            Canvas.SetLeft(textBlock, middleX);
            Canvas.SetTop(textBlock, yPos);
        }

        private Tuple<Path, TextBlock> CreatePath(List<PointEntity> points)
        {
            var path = new Path
            {
                Stroke = ComponentColor,
                StrokeThickness = 6,
                Data = GetGeometryOfDrawingPath(points)
            };

            var textBlock = new TextBlock
            {
                Text = "0",
                Foreground = ComponentColor,
                FontSize = 20
            };

            MoveTextBox(points, textBlock);

            path.MouseDown += Pipe_MouseDown;

            CanvasFlow.Children.Add(path);
            CanvasFlow.Children.Add(textBlock);

            return new Tuple<Path, TextBlock>(path, textBlock);
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
            var point = new PointEntity { X = p.X, Y = p.Y };
            try
            {
                switch (_mode)
                {
                    case Mode.Mouse:
                    case Mode.Delete:
                        break; // The code for selecting and deleting components is in the Component_MouseDown method.
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

                            CreatePathIfDoesntExist();
                            return; // Skip the ResetMode();
                        }
                        break;
                    case Mode.Sink:
                        AddComponentToScreen(_flowModel.AddSink(point));
                        break;
                    case Mode.Splitter:
                        AddComponentToScreen(_flowModel.AddSplitter(point));
                        break;
                    case Mode.Clone:
                        if (_clone != null)
                        {
                            var duplicate =_flowModel.DuplicateComponent(_clone, point);
                            AddComponentToScreen(duplicate);
                            _clone = null;
                        }
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

        private void ResetSelected()
        {
            if (_selectedComponent != null)
            {
                _selectedComponent.Foreground = ComponentColor;
                _selectedComponent = null;
            }

            if (_selectedPath != null)
            {
                _selectedPath.Stroke = _overloadedPipes.Any(x => Equals(x.Key.Item1, _selectedPath)) ? WarningColor : ComponentColor;
                _selectedPath = null;
            }
            PropertiesSidebar.Content = null;
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
                                _startIndex = GetMouseInputOutputIndex(mouse, start.FlowOutput.Length);
                                var point = GetInputOuputPosition(start, false, _startIndex);

                                _pathPoints.Add(point);

                                SetSelectedComponent(component);
                                _pathStart = start;
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
                                var endIndex = GetMouseInputOutputIndex(mouse, end.FlowInput.Length);
                                var point = GetInputOuputPosition(end, true, endIndex);
                                
                                _pathPoints.Add(point);

                                CreatePathIfDoesntExist();
                                var pipe = _flowModel.AddPipe(_pathStart, end, _pathPoints, _startIndex, endIndex);
                                _pipePaths[_currentPath] = pipe;
                                OnFlowNetworkUpdated();

                                _pathStart = null;
                                _pathPoints = null;
                                _currentPath = null;
                                ResetMode();
                                SetSelectedComponent(component);
                            }
                        }
                        break;
                    case Mode.Delete:
                        if (MessageBox.Show(
                            "By deleting this component the connected pipes will also be deleted." +
                            "Are you sure you want to delete this component?",
                            "Flow system",
                            MessageBoxButton.YesNo) == MessageBoxResult.No) return;
                        var deletedPipes = _flowModel.DeleteComponent(component.Component);
                        CanvasFlow.Children.Remove(component);
                        deletedPipes.ForEach(x =>
                        {
                            var path = _pipePaths.First(z => z.Value.Equals(x)).Key;
                            CanvasFlow.Children.Remove(path.Item1);
                            CanvasFlow.Children.Remove(path.Item2);
                            _pipePaths.Remove(path);
                        });
                        ResetMode();
                        break;
                    case Mode.Clone:  
                        if (_clone == null)
                        {
                            _ignoreClick = true;
                            SetSelectedComponent(component);
                            _clone = component.Component;
                        }
                        break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Pipe_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var path = sender as Path;
            if (path == null)
                throw new Exception("Event is added to wrong component");
            if (_currentPath != null && Equals(path, _currentPath.Item1))
                return;
            var pipe = _pipePaths.First(x => Equals(x.Key.Item1, path)).Value;

            switch (_mode)
            {
                case Mode.Mouse:
                {
                    ResetSelected();

                    path.Stroke = SelectedColor;
                    _selectedPath = path;

                    var pipeViewModel = new PipeViewModel
                    {
                        MaximumFlow = pipe.MaximumFlow,
                        CurrentFlow = pipe.CurrentFlow
                    };

                    pipeViewModel.PropertyChanged += PipeViewModelChanged;
                    PropertiesSidebar.Content = pipeViewModel;
                }
                    break;
                case Mode.Delete:
                    _flowModel.DeletePipe(pipe);
                    var tuple = _pipePaths.First(x => Equals(x.Key.Item1, path)).Key;
                    CanvasFlow.Children.Remove(tuple.Item1);
                    CanvasFlow.Children.Remove(tuple.Item2);
                    _pipePaths.Remove(tuple);
                    break;
            }
        }

        #endregion

        private Geometry GetGeometryOfDrawingPath(List<PointEntity> pipe)
        {
            var first = true;
            var p = pipe.Select(x =>
            {
                if (!first) return $"L{x.X},{x.Y}";
                first = false;
                return $"M{x.X},{x.Y}";
            });
            return Geometry.Parse(string.Join(" ", p));
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

        private void PipeViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            // The model of the data can be changed here, but since business logic isn't allowed here it is put in the model
            var viewmodel = sender as PipeViewModel;

            try
            {
                _flowModel.PipePropertyChanged(
                    _pipePaths.First(x => Equals(x.Key.Item1, _selectedPath)).Value, e,
                    new PipeEntity
                    {
                        MaximumFlow = viewmodel.MaximumFlow
                    });
                _changes = true;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        public void OnFlowNetworkUpdated()
        {
            // Reset all pipe colors
            foreach (var keyValuePair in _pipePaths)
            {
                keyValuePair.Key.Item1.Stroke = ComponentColor;
                keyValuePair.Key.Item2.Foreground = ComponentColor;
                keyValuePair.Key.Item2.Text = keyValuePair.Value.CurrentFlow.ToString(CultureInfo.InvariantCulture);
            }

            _overloadedPipes = _pipePaths.Where(x =>
                x.Value.CurrentFlow > x.Value.MaximumFlow)
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var keyValuePair in _overloadedPipes)
            {
                keyValuePair.Key.Item1.Stroke = WarningColor;
                keyValuePair.Key.Item2.Foreground = WarningColor;
            }
        }
    }
}