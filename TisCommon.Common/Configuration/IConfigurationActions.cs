using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiS.Core.TisCommon.Configuration
{
    public interface IConfigurationActions
    {
        void Load();
        void Save();
    }
}
