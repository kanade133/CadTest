using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadTest
{
    internal static class TransformationHelper
    {
        /// <summary>
        /// Gets the transformation matrix from World Coordinate System (WCS) to the view Display Coordinate System (DCS).
        /// </summary>
        /// <param name="view">Instance to which the method applies.</param>
        /// <returns>The WCS to DCS transformation matrix.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="view"/> is null.</exception>
        public static Matrix3d WorldToEye(this Viewport view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            return
                Matrix3d.WorldToPlane(view.ViewDirection) *
                Matrix3d.Displacement(view.ViewTarget.GetAsVector().Negate()) *
                Matrix3d.Rotation(view.TwistAngle, view.ViewDirection, view.ViewTarget);
        }
        public static Matrix3d EyeToWorld(this Viewport view)
        {
            return WorldToEye(view).Inverse();
        }
        /// <summary>
        /// Gets the transformation matrix from World Coordinate System (WCS) to the view Display Coordinate System (DCS).
        /// </summary>
        /// <param name="view">Instance to which the method applies.</param>
        /// <returns>The WCS to DCS transformation matrix.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name ="view"/> is null.</exception>
        public static Matrix3d WorldToEye(this AbstractViewTableRecord view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            return
                Matrix3d.WorldToPlane(view.ViewDirection) *
                Matrix3d.Displacement(view.Target.GetAsVector().Negate()) *
                Matrix3d.Rotation(view.ViewTwist, view.ViewDirection, view.Target);
        }
        public static Matrix3d EyeToWorld(this AbstractViewTableRecord view)
        {
            return WorldToEye(view).Inverse();
        }
        public static void ZoomView()
        {
            dynamic acadApp = Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
            acadApp.ZoomExtents();
        }
        public static void ZoomObjects(Document doc, Transaction tr, ObjectIdCollection idCol)
        {
            Editor ed = doc.Editor;
            using (ViewTableRecord view = ed.GetCurrentView())
            {
                Matrix3d WCS2DCS = Matrix3d.PlaneToWorld(view.ViewDirection);
                WCS2DCS = Matrix3d.Displacement(view.Target - Point3d.Origin) * WCS2DCS;
                WCS2DCS = Matrix3d.Rotation(-view.ViewTwist, view.ViewDirection, view.Target) * WCS2DCS;
                WCS2DCS = WCS2DCS.Inverse();
                var ent = (Autodesk.AutoCAD.DatabaseServices.Entity)tr.GetObject(idCol[0], OpenMode.ForRead);
                Extents3d ext = ent.GeometricExtents;
                ext.TransformBy(WCS2DCS);
                for (int i = 1; i < idCol.Count; i++)
                {
                    ent = (Autodesk.AutoCAD.DatabaseServices.Entity)tr.GetObject(idCol[i], OpenMode.ForRead);
                    Extents3d tmp = ent.GeometricExtents;
                    tmp.TransformBy(WCS2DCS);
                    ext.AddExtents(tmp);
                }
                double ratio = view.Width / view.Height;
                double width = ext.MaxPoint.X - ext.MinPoint.X;
                double height = ext.MaxPoint.Y - ext.MinPoint.Y;
                if (width > (height * ratio))
                    height = width / ratio;
                Point2d center = new Point2d((ext.MaxPoint.X + ext.MinPoint.X) / 2.0, (ext.MaxPoint.Y + ext.MinPoint.Y) / 2.0);
                view.Height = height;
                view.Width = width;
                view.CenterPoint = center;
                ed.SetCurrentView(view);
            }
        }
        public static void ZoomExtents(Document doc, Point3d minPoint, Point3d maxPoint)
        {
            Editor ed = doc.Editor;
            using (ViewTableRecord view = ed.GetCurrentView())
            {
                Matrix3d WCS2DCS = Matrix3d.PlaneToWorld(view.ViewDirection);
                WCS2DCS = Matrix3d.Displacement(view.Target - Point3d.Origin) * WCS2DCS;
                WCS2DCS = Matrix3d.Rotation(-view.ViewTwist, view.ViewDirection, view.Target) * WCS2DCS;
                WCS2DCS = WCS2DCS.Inverse();
                Extents3d ext = new Extents3d(minPoint, maxPoint);
                ext.TransformBy(WCS2DCS);
                double ratio = view.Width / view.Height;
                double width = ext.MaxPoint.X - ext.MinPoint.X;
                double height = ext.MaxPoint.Y - ext.MinPoint.Y;
                if (width > (height * ratio))
                    height = width / ratio;
                Point2d center = new Point2d((ext.MaxPoint.X + ext.MinPoint.X) / 2.0, (ext.MaxPoint.Y + ext.MinPoint.Y) / 2.0);
                view.Height = height;
                view.Width = width;
                view.CenterPoint = center;
                ed.SetCurrentView(view);
            }
        }
    }
}
