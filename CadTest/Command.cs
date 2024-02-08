using Autodesk.AutoCAD.Runtime;
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
