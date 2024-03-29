﻿using AESC_Eyeshot_Viewer.Events;
using AESC_Eyeshot_Viewer.Interfaces;
using AESC_Eyeshot_Viewer.ViewModel;
using devDept.CustomControls;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Geometry;
using devDept.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static AESC_Eyeshot_Viewer.View.EyeshotDesignView;

namespace AESC_Eyeshot_Viewer.View
{
    /// <summary>
    /// Interaction logic for EyeshotDraftView.xaml
    /// </summary>
    public partial class EyeshotDraftView : UserControl, IEyeshotDesignView
    {
        public event EyeshotDesignLoadCompleted EyeshotDesignLoadComplete;
        public EyeshotDraftView()
        {
            InitializeComponent();
            InitializeEyeshotControls();

            DraftDesign.ActionMode = actionType.SelectByPick;
            DraftDesign.AssemblySelectionMode = Workspace.assemblySelectionType.Leaf;
        }

        private void InitializeEyeshotControls()
        {
            var panMouseButton = new devDept.Eyeshot.Control.MouseButton
            {
                Button = mouseButtonsZPR.Right
            };

            Viewport.Pan.MouseButton = panMouseButton;
        }

        private void DraftDesign_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !DraftDesign.IsBusy)
            {
                var location = RenderContextUtility.ConvertPoint(DraftDesign.GetMousePosition(e));
                var selectedItem = DraftDesign.GetItemUnderMouseCursor(location);

                if (selectedItem != null && selectedItem.Item is Entity entity)
                {
                    // Cloning is required to avoid mutating the underlying entity by the below transformations
                    entity = entity.Clone() as Entity;

                    if (selectedItem.HasParents())
                    {
                        var transformation = new Identity() as Transformation;
                        // Apply parent's transformation to entity
                        for (int i = 0; i < selectedItem.Parents.Count; i++)
                            transformation = selectedItem.Parents.ElementAt(i).GetFullTransformation(DraftDesign.Blocks) * transformation;

                        entity.TransformBy(transformation);
                    }

                    DesignViewEvents
                        .InvokeEntityWasSelectedEvent(this, new EntityWasSelectedEventArgs
                        {
                            Entity = entity,
                            Unit = DraftDesign.CurrentBlock.Units,
                        });

                    if (GetDataContext().IsMeasureModeActive)
                    {
                        if (entity is Line line)
                        {
                            entity = line.GetNurbsForm();
                            entity.LineWeightMethod = colorMethodType.byEntity;
                            entity.LineWeight = 5.0f;
                        }
                        else if (entity is Arc arc)
                        {
                            entity = arc.GetNurbsForm();
                            entity.LineWeightMethod = colorMethodType.byEntity;
                            entity.LineWeight = 5.0f;
                        }
                        else if (entity is Circle circle)
                        {
                            entity = circle.GetNurbsForm();
                            entity.LineWeightMethod = colorMethodType.byEntity;
                            entity.LineWeight = 5.0f;
                        }

                        DesignViewEvents
                        .InvokeEntityWasSelectedEvent(this, new EntityWasSelectedEventArgs
                        {
                            Entity = entity,
                            IsMeasuring = true,
                            Unit = DraftDesign.CurrentBlock.Units,
                        });

                        if (DraftDesign.Entity1 == null)
                        {
                            DraftDesign.ResetPoints();
                            DraftDesign.ResetSelection();
                            DraftDesign.Entity1 = entity;
                            GetDataContext().UserGuide = Properties.Resources.GuideMeasureStepTwo;
                        }
                        else
                        {
                            try
                            {
                                DraftDesign.Entity2 = entity;
                                var minimumDistance = new MinimumDistance(DraftDesign.Entity1, DraftDesign.Entity2);
                                DraftDesign.ActionMode = actionType.None;
                                DraftDesign.StartWork(minimumDistance);

                                DraftDesign.ResetSelection();

                                GetDataContext().UserGuide = Properties.Resources.GuideMeasureComplete;
                            } catch 
                            {
                                DraftDesign.ResetPoints();
                                DraftDesign.ResetSelection();
                                GetDataContext().UserGuide = Properties.Resources.GuideMeasureUnsupportedSelection;
                            }
                           
                        }
                    }
                }
                    
                else if (selectedItem is null)
                    DesignViewEvents
                        .InvokeEntityWasSelectedEvent(this, new EntityWasSelectedEventArgs());
            }
        }

        private void DraftDesign_WorkCompleted(object sender, devDept.WorkCompletedEventArgs e)
        {
            if (DraftDesign.IsBusy) return;

            var context = GetDataContext();

            if (e.WorkUnit is ReadFileAsync workUnit)
            {
                var _skipZoomFit = false;
                BlockReference _brJittering = null;
                var _yAxisUp = false;
                var _insertAsBlock = false;
                var _jittering = true;

                if (workUnit.Entities != null && _yAxisUp)
                    workUnit.RotateEverythingAroundX();

                if (_insertAsBlock)
                {
                    _brJittering = ImportExportHelper.InsertAsBlock(DraftDesign, workUnit, new RegenOptions() { Async = true });

                    _skipZoomFit = false;
                }

                else workUnit.AddTo(DraftDesign);

                if (_jittering && _insertAsBlock) DraftDesign.RemoveJittering(_brJittering);

                else if (_jittering)
                {
                    DraftDesign.Entities.SelectAll();
                    DraftDesign.RemoveJittering();
                }

                DraftDesign.Layers[0].LineWeight = 2;
                DraftDesign.Layers[0].Color = DraftDesign.DrawingColor;


                if (!_skipZoomFit)
                    DraftDesign.SetView(viewType.Top, true, false);

                EyeshotDesignLoadComplete?.Invoke(this, EventArgs.Empty);
            }

            else if (e.WorkUnit is Regeneration)
                DraftDesign.UpdateBoundingBox();
            else if (e.WorkUnit is MinimumDistance minimumDistance)
            {
                DraftDesign.InvalidateMeasure();
                DraftDesign.ResetPoints();
                DraftDesign.ResetSelection();
                DraftDesign.PointA = minimumDistance.PtA;
                DraftDesign.PointB = minimumDistance.PtB;
                DraftDesign.Distance = minimumDistance.Distance;

                DraftDesign.ActionMode = actionType.SelectVisibleByPickDynamic;

                context.IsMeasureVisible = true;
            }
        }

        private void DraftDesign_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;
        }

        private void DraftDesign_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length == 1)
                LoadDXFFileIntoDesignView(files.FirstOrDefault());
            else
                MessageBox.Show(Properties.Resources.MaxFileLoadLimitOnViewer);
        }

        private void DraftDesign_Loaded(object sender, RoutedEventArgs e)
            => LoadDXFFileIntoDesignView(GetDataContext().LoadedFilePath);

        private void LoadDXFFileIntoDesignView(string filePath)
        {
            var context = GetDataContext();
            if (filePath != string.Empty && File.Exists(filePath))
            {
                try
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        var importResult = context.ImportFile(filePath, DraftDesign);

                        if (importResult == string.Empty)
                            MessageBox.Show(Properties.Resources.FailedToOpenFileInViewer, Properties.Resources.GeneralOpenFileFailure, MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
                catch (InvalidDataException exception)
                {
                    MessageBox.Show(exception.Message, Properties.Resources.GeneralFileFormatError, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public EyeshotDesignViewModel GetDataContext() => DataContext as EyeshotDesignViewModel;

        private void DraftDesign_KeyDown(object sender, KeyEventArgs e)
        {
            var context = GetDataContext();
            if (e.Key == Key.M)
            {
                context.IsMeasureModeActive = !context.IsMeasureModeActive;

                if (context.IsMeasureVisible && !context.IsMeasureModeActive)
                {
                    DraftDesign.ResetPoints();
                    DraftDesign.ResetSelection();
                }
            }
        }

        private void MeasurementButton_Click(object sender, RoutedEventArgs e)
        {
            var context = GetDataContext();
            context.IsMeasureModeActive = !context.IsMeasureModeActive;

            if (context.IsMeasureModeActive)
                MeasurementButton.Background = new SolidColorBrush() { Color = Colors.LightGray, Opacity = 0.8 };
            else
            {
                if (context.IsMeasureVisible)
                {
                    DraftDesign.ResetPoints();
                    DraftDesign.ResetSelection();
                    DraftDesign.Invalidate();
                }

                MeasurementButton.Background = new SolidColorBrush() { Color = Colors.Transparent };
            }
                
        }
    }
}
