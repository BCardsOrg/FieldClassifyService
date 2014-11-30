
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Data;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services;
using TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Modules;
using TiS.Recognition.FieldClassifyService.Data;
using TiS.Recognition.FieldClassifyService.Common.Data;

namespace TiS.Recognition.FieldClassifyService.FieldClassifyToolkit.Services
{
    public class ModelsService
    {
        static ModelsService m_service = new ModelsService();

        public event EventHandler Changed;

        static public ModelsService Service
        {
            get
            {
                return m_service;
            }
        }
        string m_modelsFolder;
        public string ModelsFolder
        {
            get
            {
                return m_modelsFolder;
            }
            set
            {
                if (m_modelsFolder != value)
                {
                    Directory.CreateDirectory(value);
                    m_modelsFolder = value;
                    UpdateModelsInfo();
                }
            }
        }
        public void Save(ModelData modelData)
        {
            string modelFileName = Path.Combine(ModelsFolder, modelData.Name + "." + Const.ModelFileExt);

            Repository.UpdateTrainingDataPath(modelFileName);

            BinaryFormatter f = new BinaryFormatter();
            using (var stream = File.OpenWrite(modelFileName))
            {
                f.Serialize(stream, modelData);
            }
            m_modelsInfo.Add(GetModelInfo(modelData));

            OnChanged();
        }
        public void Save(ModelData modelData, Stream stream)
        {
            BinaryFormatter f = new BinaryFormatter();
            f.Serialize(stream, modelData);
        }

        public ModelData Load(string modelName)
        {
            string modelFileName = Path.Combine(ModelsFolder, modelName + "." + Const.ModelFileExt);

            BinaryFormatter f = new BinaryFormatter();
            using (var stream = File.OpenRead(modelFileName))
            {
                return f.Deserialize(stream) as ModelData;
            }
        }

        List<ModelDataInfo> m_modelsInfo = new List<ModelDataInfo>();
        public IEnumerable<ModelDataInfo> ModelsInfo
        {
            get
            {
                return m_modelsInfo;
            }
        }

        private ModelDataInfo GetModelInfo(ModelData modelData)
        {
            return new ModelDataInfo()
                        {
                            Name = modelData.Name,
                            CreateDate = modelData.CreateDate,
                            Match = modelData.Match,
                            Reject = modelData.Reject,
                            FP = modelData.FP,
                        };
        }

        public void UpdateModelsInfo()
        {
            m_modelsInfo.Clear();
            foreach (var modelName in Directory.GetFiles(ModelsFolder, "*.icmdl")
                                                .Select( x => Path.GetFileNameWithoutExtension(x) ))
            {
                var modelData = Load(modelName);
                m_modelsInfo.Add(GetModelInfo(modelData));
            }
            OnChanged();
        }
        protected void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }

    }
}
