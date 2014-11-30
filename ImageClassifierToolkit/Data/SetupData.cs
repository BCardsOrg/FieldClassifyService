using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using TiS.Recognition.FieldClassifyService.Service;
using TiS.Recognition.FieldClassifyService.Data;
using TiS.Recognition.FieldClassifyService.InterfaceForReflection.Models;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data
{


     [Serializable]
    public class SetupFieldData
    {
         public  SetupFieldData(string value_in)
         {
             Name = value_in;
             IsSelected = true;
         }
         public bool IsSelected { get; set; }
         public string Name { get; set; }
         public int NumApear = 0;
    }

     [Serializable]
    public class SetupFieldViewModel
    {
         public SetupFieldViewModel(SetupFieldData SetupFieldDataIn)
         {
             _SetupFieldData = SetupFieldDataIn;
         }

         public SetupFieldViewModel(string name_in)
         {
             _SetupFieldData = new SetupFieldData(name_in);
         }

         public bool IsSelected { get { return _SetupFieldData.IsSelected; } set { _SetupFieldData.IsSelected = value; } }
         public int NumApear { get { return _SetupFieldData.NumApear; } }
         public string Name { get { return _SetupFieldData.Name; } }
         public Color rectColor;
         SetupFieldData _SetupFieldData;
         public SetupFieldData SetupFieldDataProp {
             get { return _SetupFieldData; } 
             }
    }


    [Serializable]
    public class SetupData
    {
        List<PageData> m_pages = new List<PageData>();
        List<SetupFieldData> m_fields = new List<SetupFieldData>();

        public SetupData()
        {
            ClassifierConfigurations = new List<ConfigurationFieldClassifier>();
        }

        public void Clear()
        {
            m_fields.Clear();
            m_pages.Clear();
        }

        public IEnumerable<PageData> Pages
        {
            get
            {
                return m_pages;
            }
        }

        public IEnumerable<SetupFieldData> Fields
        {
            get
            {
                return m_fields;
            }
        }

        public IEnumerable<SetupFieldData> FilteredFields
        {
            get
            {
                if (_filter.Length == 0)
                    return m_fields;
                else
                    return m_fields.Where(a => _filter.Contains(a.Name)).ToList();
            }
            set
            {
                m_fields = value.ToList();
            }
        }

        public ConfigurationFieldClassifier BaseClassifierConfiguration { get; set; }

        public IEnumerable<ConfigurationFieldClassifier> ClassifierConfigurations { get; private set; }

        public void AddClassifierConfiguration(ConfigurationFieldClassifier configuration)
        {
            (ClassifierConfigurations as IList).Add(configuration);
        }

        public void ClearConfigurationsList()
        {
            (ClassifierConfigurations as IList).Clear();
        }

        public static void Save(SetupData setupDate, string fileName)
        {
            BinaryFormatter f = new BinaryFormatter();
            using (var file = File.OpenWrite(fileName))
            {
                f.Serialize(file, setupDate);
            }
        }

        public static SetupData Load(string fileName)
        {
            if (File.Exists(fileName) == false)
            {
                return null;
            }

            BinaryFormatter f = new BinaryFormatter();
            using (var file = File.OpenRead(fileName))
            {
                return f.Deserialize(file) as SetupData;
            }
        }


        internal void AddPage(PageData pageData)
        {
            m_pages.Add(pageData);
            foreach (var field in pageData.DocData.Fields)
            {
                SetupFieldData setupFieldData = m_fields.FirstOrDefault(x => x.Name == field.Name);
                if (setupFieldData == null)
                {
                    setupFieldData = new SetupFieldData(field.Name);
                    m_fields.Add( setupFieldData );
                }

                if ( (string.IsNullOrWhiteSpace(field.Contents) == false) && (field.Rectangle.IsEmpty == false) )
                {
                    setupFieldData.NumApear += 1;
                }
            }
        }

        internal void RemoveBadFields(string [] filter)
        {
            int minNumApear = (m_pages.Count * 50) / 100;
            m_fields.RemoveAll(x => x.NumApear < minNumApear);
            _filter = filter;
        }

        string[] _filter = new string[0];

    }
}
