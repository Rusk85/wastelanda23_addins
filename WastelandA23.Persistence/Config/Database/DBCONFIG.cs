using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WastelandA23.Database;

namespace WastelandA23
{




    public static class DBCONFIG
    {


        private static ConStrLoader conStrLoader = new ConStrLoader();


        public static string getConnectionString(DbSchema dbSchema)
        {
            var dic = conStrLoader.conStrDic;
            string ret = String.Empty;
            dic.TryGetValue(dbSchema.ToString(), out ret);
            return ret;
        }


    }
}
