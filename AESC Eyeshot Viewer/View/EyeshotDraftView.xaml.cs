﻿using AESC_Eyeshot_Viewer.Events;
using AESC_Eyeshot_Viewer.Interfaces;
using AESC_Eyeshot_Viewer.ViewModel;
using devDept.CustomControls;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Entities;
using devDept.Eyeshot.Translators;
using devDept.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            DraftDesign.ActionMode = actionType.SelectByPick;
            DraftDesign.AssemblySelectionMode = Workspace.assemblySelectionType.Leaf;
        }

        private void DraftDesign_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && !DraftDesign.IsBusy)
            {
                var location = RenderContextUtility.ConvertPoint(DraftDesign.GetMousePosition(e));
                var selectedItem = DraftDesign.GetItemUnderMouseCursor(location);

                if (selectedItem != null && selectedItem.Item is Entity)
                    DesignViewEvents
                        .InvokeEntityWasSelectedEvent(this, new Events.EntityWasSelectedEventArgs
                        {
                            Entity = selectedItem.Item as Entity,
                        });
                
            }
        }

        private void DraftDesign_SelectionChanged(object sender, devDept.Eyeshot.Control.SelectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Selection changed!");
            foreach (var removedItem in e.RemovedItems)
                GetDataContext().SelectedDesignItems.Remove(removedItem);

            foreach (var addedItem in e.AddedItems)
                GetDataContext().SelectedDesignItems.Add(addedItem);

            System.Diagnostics.Debug.WriteLine($"Selected entities: {GetDataContext().SelectedDesignItems.Count}");
            if (GetDataContext().SelectedDesignItems.Count == 1)
                DesignViewEvents
                    .InvokeEntityWasSelectedEvent(this, new Events.EntityWasSelectedEventArgs 
                        { Entity = GetDataContext().SelectedDesignItems.First().Item as Entity });
        }

        private void DraftDesign_WorkCompleted(object sender, devDept.WorkCompletedEventArgs e)
        {
            if (DraftDesign.IsBusy) return;

            var context = DataContext as EyeshotDesignViewModel;

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
                DraftDesign.Layers.TryAdd(new Layer("Dimensions", System.Drawing.Color.ForestGreen));
                DraftDesign.Layers.TryAdd(new Layer("Reference geometry", System.Drawing.Color.Red));


                if (!_skipZoomFit)
                    DraftDesign.SetView(viewType.Top, true, false);

                EyeshotDesignLoadComplete?.Invoke(this, EventArgs.Empty);
            }

            else if (e.WorkUnit is Regeneration)
            {
                System.Diagnostics.Debug.WriteLine("Regenerating design view");
                DraftDesign.UpdateBoundingBox();
            }
            /*else if (e.WorkUnit is MinimumDistance minimumDistance)
            {
                Design.PointA = minimumDistance.PtA;
                Design.PointB = minimumDistance.PtB;
                Design.Distance = minimumDistance.Distance;

                Design.ZoomToPoints();
                Design.ResetSelection();
                Design.Entities.ClearSelection();

                Design.ActionMode = actionType.SelectVisibleByPickDynamic;

                IsMeasureVisible = true;
                return;
            }*/
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
                MessageBox.Show("You can only load 1 file at a time by dragging and dropping here. To load more, go to the tab: Load Files");
        }

        private void DraftDesign_Loaded(object sender, RoutedEventArgs e)
            => LoadDXFFileIntoDesignView((DataContext as EyeshotDesignViewModel).LoadedFilePath);

        private void LoadDXFFileIntoDesignView(string filePath)
        {
            var context = DataContext as EyeshotDesignViewModel;
            if (filePath != string.Empty && File.Exists(filePath))
            {
                try
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        var importResult = context.ImportFile(filePath, DraftDesign);

                        if (importResult == string.Empty)
                            MessageBox.Show("Could not open this file in the viewer, try again later", "Open failure", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
                catch (InvalidDataException exception)
                {
                    MessageBox.Show(exception.Message, "File format error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public EyeshotDesignViewModel GetDataContext() => DataContext as EyeshotDesignViewModel;
    }
}
