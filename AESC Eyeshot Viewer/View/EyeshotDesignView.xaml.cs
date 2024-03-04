using devDept.Eyeshot.Control;
using devDept.Eyeshot.Translators;
using devDept.Eyeshot;
using devDept;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using AESC_Eyeshot_Viewer.ViewModel;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using devDept.Graphics;
using System.Windows.Forms;
using System.Windows.Input;
using AESC_Eyeshot_Viewer.Interfaces;
using AESC_Eyeshot_Viewer.Events;

namespace AESC_Eyeshot_Viewer.View
{
    /// <summary>
    /// Interaction logic for EyeshotDesignView.xaml
    /// </summary>
    public partial class EyeshotDesignView : System.Windows.Controls.UserControl, IEyeshotDesignView
    {
        public event EyeshotDesignLoadCompleted EyeshotDesignLoadComplete;
        
        public delegate void EyeshotDesignLoadCompleted(object sender, EventArgs eventArgs);

        public EyeshotDesignView()
        {
            InitializeComponent();
            InitializeEyeshotControls();

            Design.ActionMode = actionType.SelectVisibleByPickDynamic;
            Design.SelectionFilterMode = selectionFilterType.Face | selectionFilterType.Edge | selectionFilterType.Vertex;
            Design.AssemblySelectionMode = Workspace.assemblySelectionType.Leaf;
        }

        private void InitializeEyeshotControls()
        {
            var panMouseButton = new devDept.Eyeshot.Control.MouseButton
            {
                Button = mouseButtonsZPR.Middle
            };
            var rotateMouseButton = new devDept.Eyeshot.Control.MouseButton
            {
                Button = mouseButtonsZPR.Left,
                ModifierKey = modifierKeys.Shift,
            };

            Viewport.Pan.MouseButton = panMouseButton;
            Viewport.Rotate.MouseButton = rotateMouseButton;            
        }

        public void Design_WorkCompleted(object _, WorkCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Work Completed");
            if (Design.IsBusy) return;

            System.Diagnostics.Debug.WriteLine("Design not busy");

            var context = GetDataContext();

            if (e.WorkUnit is ReadFileAsync workUnit)
            {
                workUnit.AddTo(Design, new RegenOptions() { Async = true });

                if (!context.IsLoaded)
                {
                    // Send loaded event
                    context.InvokeIsLoadedEvent();
                    context.IsLoaded = true;
                }
            }
            else if (e.WorkUnit is Regeneration)
            {
                Design.Invalidate();
                Design.UpdateBoundingBox();
                EyeshotDesignLoadComplete?.Invoke(this, EventArgs.Empty);
            }
            else if (e.WorkUnit is MinimumDistance minimumDistance)
            {
                Design.PointA = minimumDistance.PtA;
                Design.PointB = minimumDistance.PtB;
                Design.Distance = minimumDistance.Distance;

                Design.ResetSelection();
                Design.Entities.ClearSelection();

                Design.ActionMode = actionType.SelectVisibleByPickDynamic;

                GetDataContext().IsMeasureVisible = true;
                return;
            }

            Design.Viewports.FirstOrDefault()?.SetView(viewType.Front);
            Design.ZoomFit(90);
            Design.Invalidate();
        }

        private void Design_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!Design.IsBusy && e.LeftButton == MouseButtonState.Pressed)
            {
                var location = RenderContextUtility.ConvertPoint(Design.GetMousePosition(e));

                var selectedItem = Design.GetItemUnderMouseCursor(location);
                Design.PointA = Design.ScreenToWorld(location);
                bool selected = selectedItem != null;

                if (selected)
                {
                    if (!(selectedItem.Item is Brep brep))
                    {
                        System.Windows.Forms.MessageBox.Show($"Measurement works only on Brep entities\r\nSelected item is of type {selectedItem.Item.GetType().Name}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    brep = brep.Clone() as Brep;

                    brep.Rebuild(0, true);

                    Entity entity;
                    if (selectedItem is SelectedVertex)
                    {
                        var selVertex = selectedItem as SelectedVertex;
                        entity = new devDept.Eyeshot.Entities.Point(brep.Vertices[selVertex.Index], 5.0f);
                    }
                    else if (selectedItem is SelectedEdge)
                    {
                        var selEdge = selectedItem as SelectedEdge;
                        entity = brep.Edges[selEdge.Index].Curve.GetNurbsForm().Clone() as Curve;
                        entity.LineWeightMethod = colorMethodType.byEntity;
                        entity.LineWeight = 5.0f;
                    }
                    else if (selectedItem is SelectedFace)
                    {
                        var selFace = selectedItem as SelectedFace;
                        entity = brep.Faces[selFace.Index].ConvertToSurface()[0].Clone() as Surface;
                    }
                    else
                        entity = brep;

                    entity = entity.Clone() as Entity;

                    if (selectedItem.HasParents())
                    {
                        var transformation = new Identity() as Transformation;
                        // Apply parent's transformation to entity
                        for (int i = 0; i < selectedItem.Parents.Count; i++)
                            transformation = selectedItem.Parents.ElementAt(i).GetFullTransformation(Design.Blocks) * transformation;

                        entity.TransformBy(transformation);
                    }

                    DesignViewEvents.InvokeEntityWasSelectedEvent(this, new EntityWasSelectedEventArgs { 
                        Entity = entity, 
                        Unit = Design.CurrentBlock.Units,
                        IsMeasuring = GetDataContext().IsMeasureModeActive,
                    });

                    if (Design.Entity1 == null)
                    {
                        Design.ResetPoints();
                        Design.Entity1 = entity;
                    }
                    else if (GetDataContext().IsMeasureModeActive)
                    {
                        Design.Entity2 = entity;
                        var minimumDistance = new MinimumDistance(Design.Entity1, Design.Entity2);
                        Design.ActionMode = actionType.None;
                        Design.StartWork(minimumDistance);
                    }
                    else
                    {
                        Design.ResetPoints();
                        Design.ResetSelection();
                    }
                }
            }
        }

        private void Design_KeyDown(object _, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.M)
            {
                var context = GetDataContext();
                context.IsMeasureModeActive = !context.IsMeasureModeActive;

                if (context.IsMeasureVisible && !context.IsMeasureModeActive)
                {
                    Design.ResetPoints();
                    Design.ResetSelection();
                }
            }
                
        }

        private void Design_Loaded(object sender, RoutedEventArgs e)
            => LoadSTPFileIntoDesignView(GetDataContext().LoadedFilePath);
        

        private void DraftDesign_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                e.Effects = System.Windows.DragDropEffects.Copy;
            else
                e.Effects = System.Windows.DragDropEffects.None;
        }

        private void DraftDesign_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetData(System.Windows.DataFormats.FileDrop) is string[] files && files.Length == 1)
                LoadSTPFileIntoDesignView(files.FirstOrDefault());
            else
                System.Windows.MessageBox.Show("You can only load 1 file at a time by dragging and dropping here. To load more, go to the tab: Load Files");
        }

        private void LoadSTPFileIntoDesignView(string filePath)
        {
            if (filePath != string.Empty && File.Exists(filePath))
            {
                try
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        var importResult = GetDataContext().ImportFile(filePath, Design);

                        if (importResult == string.Empty)
                            System.Windows.MessageBox.Show("Could not open this file in the viewer, try again later", "Open failure", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
                catch (InvalidDataException exception)
                {
                    System.Windows.MessageBox.Show(exception.Message, "File format error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public EyeshotDesignViewModel GetDataContext() => DataContext as EyeshotDesignViewModel;
    }
}
