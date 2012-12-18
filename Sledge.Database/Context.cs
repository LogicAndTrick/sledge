using IQToolkit;
using IQToolkit.Data.SQLite;
using Sledge.Database.Models;
using IQToolkit.Data;
using IQToolkit.Data.Common;
using IQToolkit.Data.Mapping;
using System.Reflection;
using System.Text;
using System.IO;

namespace Sledge.Database
{
    public class Context
    {
        private static QueryPolicy GetPolicy()
        {
            var ep = new EntityPolicy();

            ep.IncludeWith<Game>(e => e.Fgds, true);
            ep.IncludeWith<Game>(e => e.Wads, true);

            ep.IncludeWith<Build>(e => e.Games, true);

            ep.IncludeWith<Engine>(e => e.Games, true);
            ep.IncludeWith<Engine>(e => e.Builds, true);

            return ep;
        }

        public static Context Create()
        {
            var xmlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Context), "Mapping.xml");
            var dbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xml = (new StreamReader(xmlStream)).ReadToEnd();
            var map = XmlMapping.FromXml(xml);
            
            var provider = DbEntityProvider.From(
                typeof (SQLiteQueryProvider),
                Path.Combine(dbPath, "Sledge.db3"),
                map,
                GetPolicy());
            return new Context(provider);
        }

        public static Context DBContext { get; set; }

        static Context()
        {
            DBContext = Create();
        }

        public IEntityProvider Provider { get; private set; }

        public Context(IEntityProvider provider)
        {
            Provider = provider;
        }

        public virtual IEntityTable<Game> Games { get { return Provider.GetTable<Game>("Games"); } }
        public virtual IEntityTable<Fgd> Fgds { get { return Provider.GetTable<Fgd>("Fgds"); } }
        public virtual IEntityTable<Wad> Wads { get { return Provider.GetTable<Wad>("Wads"); } }
        public virtual IEntityTable<Build> Builds { get { return Provider.GetTable<Build>("Builds"); } }
        public virtual IEntityTable<Engine> Engines { get { return Provider.GetTable<Engine>("Engines"); } }
        public virtual IEntityTable<Setting> Settings { get { return Provider.GetTable<Setting>("Settings"); } }
    }
}
