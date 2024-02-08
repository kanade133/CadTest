using Autodesk.AutoCAD.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadTest
{
    /// <summary>
    /// Automates saving/changing/restoring system variables
    /// </summary>
    public class CadSystemVariable : IDisposable
    {
        private Dictionary<string, object> _dic = new Dictionary<string, object>();

        public CadSystemVariable(params (string name, object value)[] variables)
        {
            foreach (var (name, value) in variables)
            {
                _dic[name] = Application.GetSystemVariable(name);
                Application.SetSystemVariable(name, value);
            }
        }
        public void Dispose()
        {
            foreach (var kv in _dic)
            {
                Application.SetSystemVariable(kv.Key, kv.Value);
            }
            _dic.Clear();
            _dic = null;
        }
    }
}
