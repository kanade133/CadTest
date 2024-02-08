using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadTest.Jig
{
    public class ArrowPolyline : IDisposable
    {
        public Polyline Polyline { get; private set; }
        public Point3d StartPoint { get; private set; }
        public Point3d EndPoint { get; private set; }
        private const double _lineWidth = 0;
        private const double _arrowLength = 2;
        private const string _polylineType = "PHANTOM";
        private const string _lineTypeFileName = "acad.lin";

        public ArrowPolyline() { }
        public ArrowPolyline(Polyline polyline)
        {
            this.Polyline = polyline;
            this.StartPoint = polyline.StartPoint;
            this.EndPoint = polyline.EndPoint;
        }

        public static ArrowPolylineCollection GetArrowPolylines(Transaction trans, Database db, bool isReadonly)
        {
            var list = new ArrowPolylineCollection();
            using (var btr = trans.GetObject(db.CurrentSpaceId, OpenMode.ForRead) as BlockTableRecord)
            {
                foreach (var objectId in btr)
                {
                    var polyline = trans.GetObject(objectId, OpenMode.ForRead) as Polyline;
                    if (polyline != null)
                    {
                        if (!isReadonly)
                        {
                            polyline.UpgradeOpen();
                        }
                        list.Add(new ArrowPolyline(polyline));
                    }
                }
            }
            return list;
        }

        public void CreatePolyline(Database db, Transaction trans, Point3d startPoint)
        {
            using (var btr = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord)
            {
                CreatePolyline(db, trans, btr, startPoint);
            }
        }
        public void CreatePolyline(Database db, Transaction trans, Point3d startPoint, Point3d endPoint)
        {
            using (var btr = trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord)
            {
                CreatePolyline(db, trans, btr, startPoint, endPoint);
            }
        }
        public void CreatePolyline(Database db, Transaction trans, BlockTableRecord layoutBlockTableRecord, Point3d startPoint, Point3d endPoint)
        {
            CreatePolyline(db, trans, layoutBlockTableRecord, startPoint);
            UpdatePointsOfArrowPolyline(startPoint, endPoint);
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }
        private void CreatePolyline(Database db, Transaction trans, BlockTableRecord layoutBlockTableRecord, Point3d startPoint)
        {
            LoadLineType(db, trans);
            const int vertices = 7;
            this.Polyline = new Polyline(7) { Linetype = _polylineType };
            //ArrowPolyline consists of seven points with seven vertices added in advance
            //   2
            //    \
            //0----1(3,5)--6
            //    /
            //   4
            for (int i = 0; i < vertices; i++)
            {
                this.Polyline.AddVertexAt(i, new Point2d(startPoint.X, startPoint.Y), 0, _lineWidth, _lineWidth);
            }
            layoutBlockTableRecord.AppendEntity(this.Polyline);
            trans.AddNewlyCreatedDBObject(this.Polyline, true);
            this.StartPoint = startPoint;
            this.EndPoint = startPoint;
        }
        public void UpdatePointsOfArrowPolyline(Point3d startPoint, Point3d endPoint)
        {
            var midpoint = new Point2d((startPoint.X + endPoint.X) / 2, (startPoint.Y + endPoint.Y) / 2);// at 1/2 of the line
            var vectorMidToArrowLeft = new Vector2d(startPoint.X - midpoint.X, startPoint.Y - midpoint.Y).RotateBy(ConvertDegree2Radian(30)).GetNormal().MultiplyBy(_arrowLength);// Arrow is 30° to the left
            var arrowLeftPoint = new Point2d(vectorMidToArrowLeft.X + midpoint.X, vectorMidToArrowLeft.Y + midpoint.Y);
            var vectorMidToArrowRight = new Vector2d(startPoint.X - midpoint.X, startPoint.Y - midpoint.Y).RotateBy(ConvertDegree2Radian(-30)).GetNormal().MultiplyBy(_arrowLength);
            var arrowRightPoint = new Point2d(vectorMidToArrowRight.X + midpoint.X, vectorMidToArrowRight.Y + midpoint.Y);
            this.Polyline.SetPointAt(0, new Point2d(startPoint.X, startPoint.Y));
            this.Polyline.SetPointAt(1, new Point2d(midpoint.X, midpoint.Y));
            this.Polyline.SetPointAt(2, new Point2d(arrowLeftPoint.X, arrowLeftPoint.Y));
            this.Polyline.SetPointAt(3, new Point2d(midpoint.X, midpoint.Y));
            this.Polyline.SetPointAt(4, new Point2d(arrowRightPoint.X, arrowRightPoint.Y));
            this.Polyline.SetPointAt(5, new Point2d(midpoint.X, midpoint.Y));
            this.Polyline.SetPointAt(6, new Point2d(endPoint.X, endPoint.Y));
            this.StartPoint = startPoint;
            this.EndPoint = endPoint;
        }
        public void Erase()
        {
            Polyline.Erase();
        }
        private void LoadLineType(Database db, Transaction trans)
        {
            var lineTypeTable = trans.GetObject(db.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
            if (!lineTypeTable.Has(_polylineType))
            {
                db.LoadLineTypeFile(_polylineType, _lineTypeFileName);
            }
        }
        private static double ConvertDegree2Radian(double degs) => degs * Math.PI / 180;
        private static double ConvertRadian2Degree(double rad) => rad / Math.PI * 180;

        public void Dispose()
        {
            Polyline.Dispose();
        }
    }
}
