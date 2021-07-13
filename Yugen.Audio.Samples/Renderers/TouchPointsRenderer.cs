using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI;
using Windows.UI.Input;

namespace Yugen.Audio.Samples.Renderers
{
    public class TouchPointsRenderer
    {
        private const int _maxPoints = 100;

        private Queue<Vector2> _points = new Queue<Vector2>();

        public void OnPointerPressed()
        {
            _points.Clear();
        }

        public void OnPointerMoved(IList<PointerPoint> intermediatePoints)
        {
            foreach (var point in intermediatePoints)
            {
                if (point.IsInContact)
                {
                    if (_points.Count > _maxPoints)
                    {
                        _points.Dequeue();
                    }

                    _points.Enqueue(new Vector2((float)point.Position.X, (float)point.Position.Y));
                }
            }
        }

        public void Draw(CanvasDrawingSession ds)
        {
            int pointerPointIndex = 0;
            Vector2 prev = new Vector2(0, 0);
            const float penRadius = 10;
            foreach (Vector2 p in _points)
            {
                if (pointerPointIndex != 0)
                {
                    ds.DrawLine(prev, p, Colors.DarkRed, penRadius * 2);
                }
                ds.FillEllipse(p, penRadius, penRadius, Colors.DarkRed);
                prev = p;
                pointerPointIndex++;
            }

            if (_points.Count > 0)
            {
                _points.Dequeue();
            }
        }
    }
}