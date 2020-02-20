using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace ImageScalerLib
{
    public class KvpDb
    {
        private static string DBFILENAME = "kvpLite.sqlite";
        private SQLiteConnection _dbConnection = null;
        public KvpDb()
        {
            if (this._dbConnection == null)
            {
                this._dbConnection = new SQLiteConnection("Data Source=" + DBFILENAME + ";Version=3;");
                if (System.IO.File.Exists(DBFILENAME) == false)
                {
                    SQLiteConnection.CreateFile(DBFILENAME);
                    this._dbConnection.Open();
                    using (var cmd = new SQLiteCommand("CREATE TABLE Kvp (key char(8) primary key, value varchar(20000));", this._dbConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    this._dbConnection.Close();
                }
            }
        }

        public bool SetKvp(KeyValuePair<string, string> keyValuePair)
        {
            if (KvpExists(keyValuePair.Key))
            {
                return false;
            }
            this._dbConnection.Open();
            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Kvp (key, value) values (@key, @value)", this._dbConnection))
            {
                cmd.Parameters.AddWithValue("@key", keyValuePair.Key);
                cmd.Parameters.AddWithValue("@value", keyValuePair.Value);
                cmd.ExecuteNonQuery();
            }
            this._dbConnection.Close();
            return true;
        }
        public KeyValuePair<string, string> GetKvp(string key)
        {
            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>();
            if (KvpExists(key) == false)
            {
                return keyValuePair;
            }
            this._dbConnection.Open();
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Kvp WHERE key=@key", this._dbConnection))
            {
                cmd.Parameters.AddWithValue("@key", key);
                using (var dbresult = cmd.ExecuteReader())
                {
                    dbresult.Read();
                    keyValuePair = new KeyValuePair<string, string>(dbresult.GetString(0), dbresult.GetString(1));
                }
            }
            this._dbConnection.Close();
            return keyValuePair;
        }
        private bool KvpExists(string key)
        {
            this._dbConnection.Open();
            bool exists = false;
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT EXISTS(SELECT 1 FROM Kvp WHERE key=@key LIMIT 1);", this._dbConnection))
            {
                cmd.Parameters.AddWithValue("@key", key);
                exists = (long)cmd.ExecuteScalar() == 1;
            }
            this._dbConnection.Close();
            return exists;
        }
    }
}
