using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WastelandA23.Persistence.Config;

namespace WastelandA23
{




    public static class DBCONFIG
    {

        public static string getConnectionString(IConStr conStr, DbSchema dbSchema)
        {
            return conStr.getConnectionString(dbSchema);
        }


    }
}
