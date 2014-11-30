using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TiS.Core.TisCommon.Storage
{
    public interface IStorageCommon
    {
        /// <summary>
        /// Writes dataset object as XML
        /// </summary>
        /// <param name="data">Dataset</param>
        /// <param name="fileName"></param>
        /// <remarks>Used locally</remarks>
        void WriteDataSet(DataSet data, string fileName, bool includeSchema = false);
    }
}
