using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MVCScrapper.Models;

namespace MVCScrapper.DAL
{
    public class FIDCInitializer : DropCreateDatabaseIfModelChanges<FIDCContext>
    {
        protected override void Seed(FIDCContext context)
        {

        }
    }
}