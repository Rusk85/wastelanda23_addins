using Arma2Net;
using MySql.Data.MySqlClient;
using WastelandA23.Persistence.Config;
using System;
using System.IO;

namespace WastelandA23.Persistence
{
    [Addin("MySqlClient")]
    class MySqlClient : Addin
    {
        private string conStr;
        private IConStr conStrObj;


        
        public MySqlClient() 
        {
            conStrObj = new ConStr();
            conStr = DBCONFIG.getConnectionString(conStrObj, DbSchema.simple);
            InvocationMethod = new FFAsyncAddinInvocationMethod(this);
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
            string[] data = raw_data.Split(new char[]{','}, 2);
            var command = data[0];

            if (command == Command.SAVE_LOADOUT.ToString())
            {
                saveLoadout(new string[] { data[1], data[2] });
                return null;
            }
            else if (command == Command.GET_LOADOUT.ToString())
            {
                return "getLoadout()";
            }
            return null;
        }


        private void saveLoadout(string[] loadout_data)
        {
            Func<string, string> qP = quoteParam;
            var player_uid = qP(loadout_data[0]);
            var loadout = qP(loadout_data[1]);

            var cmd = new MySqlConnection(conStr).CreateCommand();
            cmd.CommandText = String.Format(@"INSERT INTO player(uid,loadout) VALUES ({0},{1}) ON DUPLICATE KEY UPDATE loadout={1}", player_uid, loadout);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            cmd.Connection.Close();

        }

        private string quoteParam(string param)
        {
            return "'" + param + "'";
        }


        public MySqlCommand getCommand(string conStr)
        {
            return new MySqlConnection(conStr).CreateCommand();
        }


        public void log(string msg)
        {
            string path = @"E:\Games\ArmA3\a3master\logs\" + this.GetType().Assembly.ToString() + ".txt";
            var w = File.AppendText(path);
            w.WriteLine(DateTime.Now + ": " + msg);
            w.Flush();
            w.Close();
        }


    }
}
