using AESC_Eyeshot_Viewer.Models;
using devDept.Eyeshot;
using devDept.Eyeshot.Control;
using devDept.Eyeshot.Entities;
using devDept.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AESC_Eyeshot_Viewer.ViewModel
{
    public class MeasureSupportingEyeshotDesign : Design
    {
        private const float _scaleFactor = 1.1f;
        private const double ZoomOnPointFactor = 1.5;

        private readonly Font _font;

        public Entity Entity1 = null;
        public Entity Entity2 = null;

        private Point3D _pointA = null;

        public Point3D PointA
        {
            get
            {
                return _pointA;
            }

            set
            {
                _pointA = value;
            }
        }

        private Point3D _pointB = null;
        public Point3D PointB
        {
            get
            {
                return _pointB;
            }

            set
            {
                _pointB = value;
                Blocks[Blocks.RootBlockName].CustomData = new DistancePoints(_pointA, _pointB);
            }
        }

        private bool _componentSelection;

        private double _distance;
        private double _roundedDistance;

        public double Distance
        {
            get
            {
                return _distance;
            }

            set
            {
                _distance = value;
                _roundedDistance = Math.Round(_distance, 3);
            }
        }

        private readonly Graphics graphics;

        public MeasureSupportingEyeshotDesign() : base()
        {
            Selection.LineWeightScaleFactor = 4;
            _font = new Font("FontFamily", 12.0f, System.Drawing.FontStyle.Bold);
            graphics = Graphics.FromImage(new Bitmap(1, 1));
            _componentSelection = true;
        }

        public void ResetSelection()
        {
            Entity1 = null;
            Entity2 = null;
        }

        public void ResetPoints()
        {
            _pointA = null;
            _pointB = null;

            Blocks[Blocks.RootBlockName].CustomData = null;
        }

        public void ZoomToPoints()
        {
            if (PointA != null && PointB != null)
            {
                var entityA = Mesh.CreateSphere(1, 5, 5);
                var entityB = Mesh.CreateSphere(1, 5, 5);

                var scaling = new Scaling(new Line(PointA, PointB).MidPoint, ZoomOnPointFactor);
                entityA.TransformBy(scaling * new Translation(PointA.AsVector));
                entityB.TransformBy(scaling * new Translation(PointB.AsVector));

                IList<Entity> entities = new List<Entity>()
                {
                    entityA,
                    entityB,
                };

                ZoomFit(entities, false);
            }
            else
                ZoomFit();
        }

        public void SetWholeEntitySelection(bool componentSelection)
        {
            _componentSelection = componentSelection;
            if (!_componentSelection)
                SelectionFilterMode = selectionFilterType.Entity;
            else
                SelectionFilterMode = selectionFilterType.Face | selectionFilterType.Edge | selectionFilterType.Vertex;
        }

        public void ManageSavedDistance(object customData)
        {
            if (customData is DistancePoints distancePoints)
            {
                _pointA = distancePoints.PointA;
                _pointB = distancePoints.PointB;
            }
        }

        protected override void DrawOverlay(DrawSceneParams data)
        {
            if (_pointA != null && _pointB != null)
            {
                var lineColour = ActiveViewport.Background.GetContrastColor();
                var textColour = ActiveViewport.Background.GetContrastColorInverted();

                var prevLineSize = data.RenderContext.CurrentLineWidth;
                var prevPointSize = data.RenderContext.CurrentPointSize;
                var prevWireColor = data.RenderContext.CurrentWireColor;

                var midPoint = WorldToScreen(new Line(PointA, PointB).MidPoint);

                string text = $"{_roundedDistance} mm";
                var textSize = graphics.MeasureString(text, _font);
                textSize.Width *= _scaleFactor;
                textSize.Height *= _scaleFactor;

                data.RenderContext.DrawQuad(new RectangleF((float)midPoint.X - textSize.Width / 2, (float)midPoint.Y - textSize.Height / 2, textSize.Width, textSize.Height));

                data.RenderContext.SetLineSize(7);
                data.RenderContext.SetPointSize(10);
                data.RenderContext.SetColorWireframe(lineColour);

                var i = WorldToScreen(PointA);
                var e = WorldToScreen(PointB);
                ActiveViewport.Camera.GetFrame(out Point3D origin, out Vector3D camX, out Vector3D camY, out _);

                var plane = new Plane(origin, camX, camY);

                if (plane.DistanceTo(PointA) < 0 && plane.DistanceTo(PointB) < 0)
                    data.RenderContext.DrawLinesOnTheFly(new Point3D[] { i, e });

                DrawText((int)midPoint.X, (int)midPoint.Y, text, _font, textColour, ContentAlignment.MiddleCenter);

                data.RenderContext.SetLineSize(prevLineSize);
                data.RenderContext.SetPointSize(prevPointSize);
                data.RenderContext.SetColorWireframe(prevWireColor);
            }
        }
    }
}
