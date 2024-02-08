using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadTest.Jig
{
    public class PickPointJig : EntityJig
    {
        private Point3d startPoint;
        private Point3d endPoint;
        private readonly string message;
        private readonly ArrowPolyline arrowPolyline;

        public PickPointJig(ArrowPolyline arrowPolyline, string message) : base(arrowPolyline.Polyline)
        {
            this.arrowPolyline = arrowPolyline;
            this.startPoint = arrowPolyline.StartPoint;
            this.message = message;
        }
        public Point3d GetPoint()
        {
            return endPoint;
        }
        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            var options = new JigPromptPointOptions(message);
            var result = prompts.AcquirePoint(options);
            if (result.Status == PromptStatus.OK)
            {
                if (endPoint == result.Value)
                {
                    return SamplerStatus.NoChange;
                }
                else
                {
                    endPoint = result.Value;
                    return SamplerStatus.OK;
                }
            }
            return SamplerStatus.Cancel;
        }
        protected override bool Update()
        {
            arrowPolyline.UpdatePointsOfArrowPolyline(arrowPolyline.StartPoint, endPoint);
            return true;
        }
    }
}
