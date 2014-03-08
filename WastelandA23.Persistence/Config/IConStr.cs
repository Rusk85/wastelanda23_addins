using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WastelandA23.Persistence.Config
{
    interface IConStr
    {
        string getConnectionString(DbSchema dbSchema);
    }
}
