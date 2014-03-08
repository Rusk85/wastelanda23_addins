using System;
using System.IO;
using System.Collections.Generic;
using Arma2Net;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;

namespace WastelandA23.Persistence
{
    [Addin("MySqlClient")]
    class MySqlClient : Addin
    {
        public MySqlClient() { }


        private string conStr = "";


        public override string Invoke(string args, int maxResultSize)
        {
            try
            {
                //InvocationMethod = new FFAsyncAddinInvocationMethod(this);
                log(args);
                return null;
                //var split = (args ?? "").Split(',');
                //if (split[0] == "get")
                //{
                //    return getScalar("");
                //}
                //else if (split[0] == "set")
                //{

                //}
                //return getScalar("");
                //return String.Empty;
            }
            catch (Exception e)
            {
                log(e.Message);
            }
            return null;
        }


        public MySqlCommand getCommand(string conStr)
        {
            return new MySqlConnection(conStr).CreateCommand();
        }


        public void insertRow(string guid, string loadout)
        {
            try
            {
                var cmd = this.getCommand(conStr);
                cmd.CommandText = String.Format("INSERT INTO {0} VALUES({1},{2})", "player", guid, loadout);
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception e)
            {
                log(e.Message);
            }
        }


        public string getScalar(string query)
        {
            var cmd = this.getCommand(conStr);
            cmd.CommandText = @"SELECT someValue FROM player WHERE idPlayer='0' LIMIT 1";
            var da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return (string)dt.Rows[0]["someValue"];
        }


        public void setScalar(string value)
        {
            try
            {
                var cmd = this.getCommand(conStr);
                cmd.CommandText = @"UPDATE Player SET someValue='" + DateTime.Now.ToString() + "' WHERE idPlayer='0'";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log(e.Message);
            }
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
