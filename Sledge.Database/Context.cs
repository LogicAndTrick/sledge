using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Sledge.Database.Models;
using System.Linq;

namespace Sledge.Database
{
    public class Context
    {
        public static Context DBContext { get; set; }

        public static Context Create()
        {
            var dbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var connection = new SQLiteConnection("Data Source=" + Path.Combine(dbPath, "Sledge.db3"));
            return new Context(connection);
        }

        static Context()
        {
            DBContext = Create();
        }

        private IDbConnection _conn;

        public Context(IDbConnection conn)
        {
            _conn = conn;
        }

        #region Get All
        public List<Setting> GetAllSettings()
        {
            var list = new List<Setting>();
            try
            {
                _conn.Open();
                using (var comm = _conn.CreateCommand())
                {
                    comm.CommandText = "SELECT ID, Key, Value FROM Settings";
                    using (var rdr = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            list.Add(new Setting
                            {
                                Key = rdr.GetString(1),
                                Value = rdr.GetString(2),
                            });
                        }
                    }
                }
            }
            finally
            {
                _conn.Close();
            }
            return list;
        }

        public List<Game> GetAllGames()
        {
            var list = new List<Game>();
            try
            {
                _conn.Open();
                using (var comm = _conn.CreateCommand())
                {
                    comm.CommandText = "SELECT BuildID, EngineID, GameDir, ID, ModDir, Name FROM Games";
                    using (var rdr = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            list.Add(new Game
                            {
                                BuildID = rdr.GetInt32(0), 
                                EngineID = rdr.GetInt32(1), 
                                GameDir = rdr.GetString(2), 
                                ID = rdr.GetInt32(3), 
                                ModDir = rdr.GetString(4),
                                Name = rdr.GetString(5) 
                            });
                        }
                    }
                }
            }
            finally
            {
                _conn.Close();
            }
            return list;
        }

        public List<Engine> GetAllEngines()
        {
            var list = new List<Engine>();
            try
            {
                _conn.Open();
                using (var comm = _conn.CreateCommand())
                {
                    comm.CommandText = "SELECT ID, Name FROM Engines";
                    using (var rdr = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            list.Add(new Engine
                            {
                                ID = rdr.GetInt32(0),
                                Name = rdr.GetString(1),
                            });
                        }
                    }
                }
            }
            finally
            {
                _conn.Close();
            }
            return list;
        }

        public List<Fgd> GetAllFgds()
        {
            var list = new List<Fgd>();
            try
            {
                _conn.Open();
                using (var comm = _conn.CreateCommand())
                {
                    comm.CommandText = "SELECT ID, GameID, Path FROM Fgds";
                    using (var rdr = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            list.Add(new Fgd
                            {
                                ID = rdr.GetInt32(0),
                                GameID = rdr.GetInt32(1),
                                Path = rdr.GetString(2)
                            });
                        }
                    }
                }
            }
            finally
            {
                _conn.Close();
            }
            return list;
        }

        public List<Wad> GetAllWads()
        {
            var list = new List<Wad>();
            try
            {
                _conn.Open();
                using (var comm = _conn.CreateCommand())
                {
                    comm.CommandText = "SELECT ID, GameID, Path FROM Wads";
                    using (var rdr = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (rdr.Read())
                        {
                            list.Add(new Wad
                            {
                                ID = rdr.GetInt32(0),
                                GameID = rdr.GetInt32(1),
                                Path = rdr.GetString(2)
                            });
                        }
                    }
                }
            }
            finally
            {
                _conn.Close();
            }
            return list;
        }
        #endregion

        public void SaveAllSettings(IEnumerable<Setting> settings)
        {
            try
            {
                _conn.Open();
                using (var comm = _conn.CreateCommand())
                {
                    comm.CommandText = "DELETE FROM Settings";
                    comm.ExecuteNonQuery();
                }
                using (var comm = _conn.CreateCommand())
                {
                    comm.CommandText = "INSERT INTO Settings " + String.Join(" UNION ",
                        settings.Select(x => String.Format("SELECT '{0}', '{1}'", x.Key.Replace("'", "''"), x.Value.Replace("'", "''"))));
                    comm.ExecuteNonQuery();
                }
            }
            finally
            {
                _conn.Close();
            }
        }
    }
}
