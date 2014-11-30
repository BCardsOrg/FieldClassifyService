using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace TiS.Core.TisCommon
{
    public struct MyFormType
    {
        public string FormTypeName;

        public int FormTypeCount;

        public MyFormType(string name, int count)
        {
            FormTypeName = name;
            FormTypeCount = count;
        }
    }

    /// <summary>
    /// FormTypesPackage
    /// </summary>
    public class FormTypesPackage
    {
        public List<MyFormType> FormTypeCounter { get; set; }

        public FormTypesPackage()
        {
            FormTypeCounter = new List<MyFormType>();
        }

        public override string ToString() 
        {
            var formsString = FormTypeCounter.Select(f => string.Format("{0}:{1}", f.FormTypeName, f.FormTypeCount));

            return string.Join(",", formsString);
        }

        public static FormTypesPackage FromString(string source)
        {
            FormTypesPackage package = new FormTypesPackage();
            foreach (var current in source.Split(new char[] { ',' }))
            {
                var formTypeString = current.Split(new char[] { ':' });
                package.AddFormType(formTypeString[0], int.Parse(formTypeString[1]));
            }

            return package;
        }

        public void AddFormType(string formType, int count)
        {
            FormTypeCounter.Add(new MyFormType(formType, count));
        }
    }
}
