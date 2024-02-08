using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadTest
{
    internal static class XDataHelper
    {
        public static void SetXData(this DBObject dBObject, Database database, Transaction trans, string appName, string value)
        {
            using (var regAppTable = trans.GetObject(database.RegAppTableId, OpenMode.ForRead) as RegAppTable)
            {
                //查看当前名称是否已存在
                if (!regAppTable.Has(appName))
                {
                    //名称不存在的话需要先注册
                    regAppTable.UpgradeOpen();
                    RegAppTableRecord raTabRcd = new RegAppTableRecord();
                    raTabRcd.Name = appName;
                    regAppTable.Add(raTabRcd);
                    regAppTable.DowngradeOpen();
                    trans.AddNewlyCreatedDBObject(raTabRcd, true);
                }
                //编辑要存储的数据
                ResultBuffer buffer = new ResultBuffer();
                buffer.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, appName));
                buffer.Add(new TypedValue((int)DxfCode.ExtendedDataAsciiString, value));
                dBObject.XData = buffer;
            }
        }
        public static string GetXData(this DBObject dBObject, string appName)
        {
            if (dBObject.XData != null)
            {
                var buffer = dBObject.GetXDataForApplication(appName);
                if (buffer != null)
                {
                    var enumerator = buffer.GetEnumerator();
                    if (enumerator.MoveNext() && enumerator.MoveNext())
                    {
                        return enumerator.Current.Value.ToString();
                    }
                }
            }
            return null;
        }
    }
}
