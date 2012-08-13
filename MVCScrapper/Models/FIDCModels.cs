using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace MVCScrapper.Models
{
    public class FIDCContext : DbContext
    {
        public DbSet<FIDCItem> Items { get; set; }
        public DbSet<FIDCResult> Results { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class FIDCResult
    {
        [Key]
        public int Id { get; set; }

        public ICollection<FIDCFilter> Filters { get; set; }

        public ICollection<FIDCItem> Items { get; set; }

        public DateTime TimeStamp { get; set; }
    }

    public class FIDCFilter
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class FIDCItem
    {
        [Key]
        public string Key { get; set; }

        public string Label { get; set; }

        public string Value { get; set; }

        public FIDCItem Parent { get; set; }

        public int GetLevel()
        {
            if (this.Parent == null) return 0;
            else return this.Parent.GetLevel() + 1;
        }

        public FIDCItem GetParentFromLevel(int level)
        {
            if (this.GetLevel() == level) return this;
            else if(this.Parent != null) return this.Parent.GetParentFromLevel(level);

            return null;
        }
    }
}