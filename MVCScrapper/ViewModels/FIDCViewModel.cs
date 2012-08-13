using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCScrapper.Models;
using MVCScrapper.Services;

namespace MVCScrapper.ViewModels
{
    public class FIDCMensalViewModel
    {
        public List<IScrapper> Scrappers { get; set; }
        public IScrapper ScrapperSelected { get; set; }
    }
}