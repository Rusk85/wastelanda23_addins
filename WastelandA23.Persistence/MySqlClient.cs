using Arma2Net;
using MySql.Data.MySqlClient;
using WastelandA23.Database;
using System;
using System.IO;
using System.Reflection;

namespace WastelandA23.Persistence
{
    [Addin("MySqlClient")]
    class MySqlClient : Addin
    {
        private string conStr;
        Func<string[], string> tS;
        Func<string, string> rQ;

        
        public MySqlClient() 
        {
            conStr = DBCONFIG.getConnectionString(DbSchema.SIMPLE);
            tS = toString;
            rQ = removeQuotes;
            InvocationMethod = new AsyncAddinInvocationMethod(this);
        }


        public override string Invoke(string args, int maxResultSize)
        {
            try
            {
                log(args);
                return proccessSQFCall(args);
            }
            catch (Exception e)
            {
                log(e.Message); 
            }
            return null;
        }


        private string proccessSQFCall(string raw_data)
        {
            
            string[] data = rQ(raw_data).Split(new char[]{','}, 3);
            log(tS(data));
            var command = data[0];
            log(command);

            if (command == Command.SAVE_LOADOUT.ToString())
            {
                log("IS SAVE_LOADOUT");
                saveLoadout(new string[] { data[1], data[2] });
                return null;
            }
            else if (command == Command.GET_LOADOUT.ToString())
            {
                return "getLoadout()";
            }
            log("IS_NO_COMMAND");
            return null;
        }


        private void saveLoadout(string[] loadout_data)
        {
            Func<string, string> qP = quoteParam;
            var player_uid = qP(loadout_data[0]);
            var loadout = qP(loadout_data[1]);
            log(tS(loadout_data));

            var cmd = new MySqlConnection(conStr).CreateCommand();
            cmd.CommandText = String.Format(@"INSERT INTO player(uid,loadout) VALUES ({0},{1}) ON DUPLICATE KEY UPDATE loadout={1}", player_uid, loadout);
            log(cmd.CommandText);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

        }

        private string quoteParam(string param)
        {
            return "'" + param + "'";
        }

        private string removeQuotes(string str)
        {
            return str.Replace("\"", "");
        }


        public MySqlCommand getCommand(string conStr)
        {
            return new MySqlConnection(conStr).CreateCommand();
        }


        public void log(string msg)
        {
            string ext = ".log";
            var path = Path.Combine(new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.FullName);
            var w = File.AppendText(new FileInfo(
                Path.Combine(path, MethodBase.GetCurrentMethod().DeclaringType.Name + ext)).FullName);
            w.WriteLine(DateTime.Now + ": " + msg);
            w.Flush();
            w.Close();
        }

        public string toString(string[] array)
        {
            var ret = "";
            foreach (string s in array)
            {
                ret += s + ";";
            }
            return ret;
        }
    }
}
