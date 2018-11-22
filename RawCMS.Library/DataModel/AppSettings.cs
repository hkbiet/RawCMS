using System;
using System.Collections.Generic;
using System.Text;

namespace RawCMS.Library.DataModel
{
    public class AppSettings
    {
        public MongoSettings DatabaseConnection { get; set; }

        public List<string> PluginFolders { get; set; }
    }
}
