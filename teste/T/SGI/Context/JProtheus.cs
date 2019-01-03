using SGI.Models;
using SGI.Models.Custom;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SGI.Context
{
    public class JProtheus : DbContext
    {
        public JProtheus()
            : base("ProtheusEntities")
        {
            Database.SetInitializer<JProtheus>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove unused conventions
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }
}