using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace jaslab5
{
    public class NHibernateHelper
    {
        public static ISession OpenSession()
        {
            Func<string,string> env = Environment.GetEnvironmentVariable;
            var sessionFactory = Fluently.Configure()
                    .Database(PostgreSQLConfiguration
                        .PostgreSQL82.ConnectionString(c => c
                            .Host(env("db_host"))
                            .Port(int.Parse(env("db_port")))
                            .Database(env("db_name"))
                            .Username(env("db_user"))
                            .Password(env("db_password"))
                        )
                    )
                    .Mappings(m => m.FluentMappings.Add<CabinMap>().Add<PassengerMap>())
                    .ExposeConfiguration(c =>
                    {
                        c.Properties.Add("hbm2ddl.keywords", "none");
                        new SchemaExport(c).Create(false, false);
                    })
                    .BuildSessionFactory();

            return sessionFactory.OpenSession();
        }
        
    }
}