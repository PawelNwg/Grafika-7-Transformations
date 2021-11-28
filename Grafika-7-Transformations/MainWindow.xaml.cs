using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Grafika_7_Transformations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Enums.Modes.PaintingMode paintingMode;
        private Point _dragStartCoords;
        private Polygon _currentPolygon;
        private PointCollection _points = new PointCollection();
        private Point _transformationsPoint = new Point(0, 0);
        public MainWindow()
        {
            InitializeComponent();
        }
        private void RadioButton_Create(object sender, RoutedEventArgs e)
        {
            paintingMode = Enums.Modes.PaintingMode.Create;
        }
        private void RadioButton_Translate(object sender, RoutedEventArgs e)
        {
            paintingMode = Enums.Modes.PaintingMode.Tranlate;
        }
        private void RadioButton_Rotate(object sender, RoutedEventArgs e)
        {
            paintingMode = Enums.Modes.PaintingMode.Rotate;
        }
        private void RadioButton_Scale(object sender, RoutedEventArgs e)
        {
            paintingMode = Enums.Modes.PaintingMode.Scale;
        }

        private void CanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (paintingMode == Enums.Modes.PaintingMode.Tranlate) return;
            else if (paintingMode == Enums.Modes.PaintingMode.Create)
            {
                Point newPoint = e.GetPosition(Canvas);
                _points.Add(newPoint);
                CreatePolygon();
                DrawPolygon();
            }
            else
            {
                Point newPoint = e.GetPosition(Canvas);
                _transformationsPoint = newPoint;
            }
        }

        private void CreatePolygon()
        {
            Polygon polygon = new Polygon();
            polygon.MouseMove += Shape_MouseMove;
            polygon.MouseDown += Shape_MouseDown;
            _currentPolygon = polygon;
        }

        private void DrawPolygon()
        {
            Canvas.Children.Clear();
            if (_currentPolygon == null)
                CreatePolygon();

            _currentPolygon.Points = _points;
            _currentPolygon.Stroke = Brushes.Cyan;
            _currentPolygon.Fill = Brushes.White;
            _currentPolygon.StrokeThickness = 5;

            Canvas.Children.Add(_currentPolygon);
        }

        private void Shape_MouseMove(object sender, MouseEventArgs e)
        {
            if (paintingMode == Enums.Modes.PaintingMode.Tranlate && e.LeftButton == MouseButtonState.Pressed)
            {
                var newCoords = e.GetPosition(Canvas);

                _points = new PointCollection();
                Polygon polygon = _currentPolygon as Polygon;

                foreach (var point in polygon.Points)
                {
                    double x = point.X + newCoords.X - _dragStartCoords.X;
                    double y = point.Y + newCoords.Y - _dragStartCoords.Y;
                    _points.Add(new Point(x, y));
                }

                _currentPolygon.Points = _points;

                _dragStartCoords.X = newCoords.X;
                _dragStartCoords.Y = newCoords.Y;

                DrawPolygon();
            }
        }
        private void CanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {

            double angle = 20, scale = 0.05;
            if (e.Delta < 0)
            {
                angle = -angle;
                scale = 1 / scale;
            }

            if (paintingMode == Enums.Modes.PaintingMode.Rotate && e.LeftButton == MouseButtonState.Pressed)
            {

                var newCoords = e.GetPosition(Canvas);
                _points = new PointCollection();
                Polygon polygon = _currentPolygon as Polygon;
               

                foreach (var point in polygon.Points)
                {
                    _points.Add(RotatePoint(point, newCoords.X, newCoords.Y, angle));
                }

                _currentPolygon.Points = _points;
                DrawPolygon();
            }
            if (paintingMode == Enums.Modes.PaintingMode.Scale && e.LeftButton == MouseButtonState.Pressed)
            {

                var newCoords = e.GetPosition(Canvas);
                _points = new PointCollection();
                Polygon polygon = _currentPolygon as Polygon;


                foreach (var point in polygon.Points)
                {
                    _points.Add(ScalePoint(point, newCoords.X, newCoords.Y, scale));
                }

                _currentPolygon.Points = _points;
                DrawPolygon();
            }
        }

        private void Shape_MouseDown(object sender, MouseEventArgs e)
        {
            if (!(paintingMode == Enums.Modes.PaintingMode.Create))
            {
                _currentPolygon = sender as Polygon;
                _dragStartCoords = e.GetPosition(Canvas);
            }
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            var isXCorrect = Double.TryParse(Create_X1.Text, out double x);
            var isYCorrect = Double.TryParse(Create_Y1.Text, out double y);
            var isParamCorrect = Double.TryParse(Create_X2.Text, out double p);

            switch (paintingMode)
            {
                case Enums.Modes.PaintingMode.Create:
                    if (paintingMode == Enums.Modes.PaintingMode.Create)
                    {
                        if (isXCorrect && isYCorrect)
                        {
                            Point point = new Point(x, y);
                            _points.Add(point);
                            CreatePolygon();
                            DrawPolygon();
                        }
                    }
                    break;
                case Enums.Modes.PaintingMode.Tranlate:
                    if (paintingMode == Enums.Modes.PaintingMode.Tranlate)
                    {
                        if (isXCorrect && isYCorrect)
                        {
                            _points = TranslateShape(x, y);
                            DrawPolygon();
                        }
                    }
                    break;
                case Enums.Modes.PaintingMode.Rotate:
                    if (paintingMode == Enums.Modes.PaintingMode.Rotate)
                    {
                        if (isXCorrect && isYCorrect && isParamCorrect)
                        {
                            _points = RotateShape(x, y, p);
                            DrawPolygon();
                        }
                    }
                    break;
                case Enums.Modes.PaintingMode.Scale:
                    if (paintingMode == Enums.Modes.PaintingMode.Scale)
                    {
                        if (isXCorrect && isYCorrect && isParamCorrect)
                        {
                            _points = ScaleShape(x, y, p);
                            DrawPolygon();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void ClearCanvasClick(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear();
            _points = new();
        }

        private PointCollection RotateShape(double x, double y, double angle)
        {
            PointCollection translatedPoints = new PointCollection();
            angle *= Math.PI / 180;
            foreach (var point in _points)
            {
                double X = x + ((point.X - x) * Math.Cos(angle)) - ((point.Y - y) * Math.Sin(angle));
                double Y = y + ((point.X - x) * Math.Sin(angle)) + ((point.Y - y) * Math.Cos(angle));
                translatedPoints.Add(new Point(X, Y));
            }

            return translatedPoints;
        }

        private Point RotatePoint(Point point, double x, double y, double angle)
        {
            angle *= Math.PI / 180;
            double X = x + ((point.X - x) * Math.Cos(angle)) - ((point.Y - y) * Math.Sin(angle));
            double Y = y + ((point.X - x) * Math.Sin(angle)) + ((point.Y - y) * Math.Cos(angle));
            return new Point(X, Y);
        }

        private PointCollection ScaleShape(double x, double y, double k)
        {
            PointCollection translatedPoints = new PointCollection();
            foreach (var point in _points)
            {
                double X = (point.X * k) + ((1 - k) * x);
                double Y = (point.Y * k) + ((1 - k) * y);
                translatedPoints.Add(new Point(X, Y));
            }

            return translatedPoints;
        }
        private Point ScalePoint(Point point, double x, double y, double angle)
        {
            double X = (point.X * angle) + ((1 - angle) * x);
            double Y = (point.Y * angle) + ((1 - angle) * y); 
            return new Point(X, Y);
        }
        private PointCollection TranslateShape(double x, double y)
        {
            PointCollection points = new PointCollection();
            foreach (var p in _points)
            {
                double X = p.X + x;
                double Y = p.Y + y;
                points.Add(new Point(X, Y));
            }

            return points;
        }

    }
}
