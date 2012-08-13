using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using MVCScrapper.Models;

namespace MVCScrapper.Services
{
    public static class FIDCService
    {
        public static List<IScrapper> GetScrappers()
        {
            List<IScrapper> list = new List<IScrapper>();

            list.Add(new FIDCScrapperMensal() { Id = 1 });
            list.Add(new FIDCScrapperMensal489() { Id = 2 });

            return list;
        }

        private static string startURL = @"http://cvmweb.cvm.gov.br/SWB/Sistemas/SCW/CPublica/InfoMensFIDC/CPublicaInfoMensFIDC.aspx?PK_PARTIC=60945&COMPTC=";

        public static List<string> GetCompetencias()
        {
            List<string> result = new List<string>();

            HtmlDocument doc = Scrapper.GetDocument(startURL + "12/2010");

            if (doc.DocumentNode != null)
            {
                // Cabeçalho
                HtmlNodeCollection tMeses = doc.DocumentNode.SelectNodes("//select/option/@value");
                if (tMeses != null && tMeses.Count > 0)
                {
                    foreach (HtmlNode item in tMeses)
                    {
                        HtmlAttribute valueAttr = item.Attributes.FirstOrDefault(x => x.Name.ToLower() == "value");
                        if (valueAttr != null)
                        {
                            result.Add(valueAttr.Value);
                        }
                    }
                }
            }

            return result;
        }
    }
}