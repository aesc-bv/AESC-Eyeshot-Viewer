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

namespace AESC_Eyeshot_Viewer.View
{
    /// <summary>
    /// Interaction logic for EyeshotDesignView.xaml
    /// </summary>
    public partial class EyeshotDesignView : System.Windows.Controls.UserControl
    {
        public bool IsMeasureModeActive { get; set; } = false;
        public bool IsMeasureVisible { get; set; } = false;
        public event EyeshotDesignLoadCompleted EyeshotDesignLoadComplete;
        public event EntitySelected EntityWasSelected;
        
        public delegate void EntitySelected(object sender, EntityWasSelectedEventArgs e);
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
            if (Design.IsBusy) return;

            var context = DataContext as EyeshotDesignViewModel;

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
                Design.UpdateBoundingBox();
                EyeshotDesignLoadComplete?.Invoke(this, EventArgs.Empty);
            }
            else if (e.WorkUnit is MinimumDistance minimumDistance)
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
            }

            Design.Viewports.FirstOrDefault()?.SetView(viewType.Trimetric);
            Design.ZoomFit(90);
            Design.Invalidate();
        }

        private void Viewport_MouseWheel(object _, MouseWheelEventArgs e) => Viewport.ZoomCamera(e.Delta);

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
                        entity = brep.Edges[selEdge.Index].Curve.GetNurbsForm();
                        entity.LineWeightMethod = colorMethodType.byEntity;
                        entity.LineWeight = 5.0f;
                    }
                    else if (selectedItem is SelectedFace)
                    {
                        var selFace = selectedItem as SelectedFace;
                        entity = brep.Faces[selFace.Index].ConvertToSurface()[0];
                    }
                    else
                        entity = brep;

                    if (selectedItem.HasParents())
                    {
                        var transformation = new Identity() as Transformation;
                        // Apply parent's transformation to entity
                        for (int i = 0; i < selectedItem.Parents.Count; i++)
                            transformation = selectedItem.Parents.ElementAt(i).GetFullTransformation(Design.Blocks) * transformation;

                        entity.TransformBy(transformation);
                    }

                    EntityWasSelected?.Invoke(this, new EntityWasSelectedEventArgs { Entity = entity });

                    if (Design.Entity1 == null)
                    {
                        Design.ResetPoints();
                        Design.Entity1 = entity;
                    }
                    else if (IsMeasureModeActive)
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
                IsMeasureModeActive = !IsMeasureModeActive;

                if (IsMeasureVisible && !IsMeasureModeActive)
                {
                    Design.ResetPoints();
                    Design.ResetSelection();
                }
            }
                
        }

        private void Design_Loaded(object sender, RoutedEventArgs e)
        {
            var context = DataContext as EyeshotDesignViewModel;
            if (context.LoadedFileName != string.Empty && File.Exists(context.LoadedFilePath))
                context.ImportFileSTP(context.LoadedFilePath, Design);
        }
    }

    public class EntityWasSelectedEventArgs
    {
        public Entity Entity { get; set; }
    }
}
