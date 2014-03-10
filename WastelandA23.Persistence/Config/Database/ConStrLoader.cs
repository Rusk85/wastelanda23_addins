using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace WastelandA23.Database
{
    public class ConStrLoader
    {

        public Dictionary<string, string> conStrDic = new Dictionary<string, string>();
        private const string confName = "db.conf";
        private static FileInfo confFile = ConStrLoader.setConfPath();


        public ConStrLoader()
        {
            loadConnectionStrings(confFile);
        }


        private void loadConnectionStrings(FileInfo fileInfo)
        {
            foreach (string s in File.ReadAllLines(fileInfo.FullName))
            {
                var l = s.Split(':');
                if (!String.IsNullOrEmpty(s) && l.Length == 2) { conStrDic.Add(l[0], l[1]); }
            }
        }


        private static FileInfo setConfPath()
        {
            var path = Assembly.GetExecutingAssembly().Location;
            path = new DirectoryInfo(path).Parent.FullName;
            return new FileInfo(Path.Combine(path, confName));
        }


    }
}
