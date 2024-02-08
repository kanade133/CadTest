using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadTest.Jig
{
    public class MovePointJig : DrawJig
    {
        private Point3d newLocation;
        private readonly Entity point;
        private readonly ArrowPolyline outletPolyline;
        private readonly ArrowPolylineCollection inletPolylines;
        private readonly string message;

        public MovePointJig(Entity point, Point3d pointlocation, ArrowPolyline outletPolyline, ArrowPolylineCollection inletPolylines, string message)
        {
            this.point = point;
            this.newLocation = pointlocation;
            this.outletPolyline = outletPolyline;
            this.inletPolylines = inletPolylines;
            this.message = message;
        }
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            var result = prompts.AcquirePoint(new JigPromptPointOptions(message));
            if (result.Status == PromptStatus.OK)
            {
                var newPoint = new Point3d(result.Value.X, result.Value.Y, newLocation.Z);
                if (newLocation == newPoint)
                {
                    return SamplerStatus.NoChange;
                }
                else
                {
                    newLocation = newPoint;
                    return SamplerStatus.OK;
                }
            }
            return SamplerStatus.Cancel;
        }
        protected override bool WorldDraw(Autodesk.AutoCAD.GraphicsInterface.WorldDraw draw)
        {
            if (outletPolyline != null)
            {
                outletPolyline.UpdatePointsOfArrowPolyline(newLocation, outletPolyline.EndPoint);
            }
            foreach (var inletPolyline in inletPolylines)
            {
                inletPolyline.UpdatePointsOfArrowPolyline(inletPolyline.StartPoint, newLocation);
            }
            UpdateCogoPointLocation(point, newLocation);
            //变换完后再Draw
            draw.Geometry.Draw(point);
            if (outletPolyline != null)
            {
                draw.Geometry.Draw(outletPolyline.Polyline);
            }
            foreach (var inletPolyline in inletPolylines)
            {
                draw.Geometry.Draw(inletPolyline.Polyline);
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="point"></param>
        private void UpdateCogoPointLocation(Entity entity, Point3d point)
        {
            //entity.TransformBy(Matrix3d.Displacement(point - entity.Position));
        }
    }
}
