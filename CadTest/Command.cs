using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using CadTest.Jig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadTest
{
    public class Command
    {
        [CommandMethod("MyTest", CommandFlags.Session)]
        public void MyTest()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            JigTest(db, ed);
        }

        private static void JigTest(Autodesk.AutoCAD.DatabaseServices.Database db, Editor ed)
        {
            using (var trans = db.TransactionManager.StartTransaction())
            using (var arrowPolyline = new ArrowPolyline())
            {
                arrowPolyline.CreatePolyline(db, trans, default);
                var jig = new PickPointJig(arrowPolyline, "Pick Point");
                var jigResult = ed.Drag(jig);
                if (jigResult.Status != PromptStatus.OK)
                {
                    return;
                }
                else
                {
                    Point3d point = jig.GetPoint();
                }
            }
        }

        private static void TestCadSystemVariable()
        {
            using (new CadSystemVariable(("FileDia", 0), ("CmdDia", 0)))
            {
                //do something
            }
        }
    }
}
