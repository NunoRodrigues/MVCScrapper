using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using MVCScrapper.Models;

namespace MVCScrapper.Services
{
    public class FIDCScrapperMensal489 : IScrapper
    {
        public int Id { get; set; }
        public string URLSeed { get; set; }
        public string Label { get; set; }

        private FIDCResult _result = null;
        public FIDCResult Result
        {
            get
            {
                if (_result != null) return _result;
                return Load();
            }
        }

        private List<ScrapperFilter> _filters = null;
        public List<ScrapperFilter> Filters
        {
            get
            {
                if (_filters != null) return _filters;
                return GetFilters();
            }
        }

        public FIDCScrapperMensal489()
        {
            URLSeed = @"http://cvmweb.cvm.gov.br/SWB/Sistemas/SCW/CPublica/InfoMensFIDC/CPublicaInfoMensFIDC489.aspx?PK_PARTIC=60945&PTB=FALSE";
            Label = "Mensal (apartir de Janeiro de 2012)";
        }

        private List<ScrapperFilter> GetFilters()
        {
            if (_filters == null)
            {
                _filters = new List<ScrapperFilter>();

                // Month
                ScrapperFilter month = new ScrapperFilter() { ID = "competencia", Label = "Competência", ExternalID = "COMPTC", AvailableValues = new List<ScrapperFilterItem>(), Value = "05/2012" };

                HtmlDocument doc = Scrapper.GetDocument(URLSeed + "&" + month.ExternalID + "=" + month.Value);
                //HtmlDocument doc = Scrapper.GetDocumentFromFile(AppDomain.CurrentDomain.BaseDirectory + "Services\\TestData\\Mensal489.html");

                if (doc.DocumentNode != null)
                {
                    // Cabeçalho
                    HtmlNodeCollection tMeses = doc.DocumentNode.SelectNodes("//select/option/@value");
                    if (tMeses != null && tMeses.Count > 0)
                    {
                        foreach (HtmlNode item in tMeses)
                        {
                            string value = item.GetAttributeValue("value", "");
                            if (value != null && value.IndexOf('/') > 0)
                            {
                                int year = int.Parse(value.Split('/')[1]);
                                if (year >= 2012)
                                {
                                    month.AvailableValues.Add(new ScrapperFilterItem() { Label = value, Value = value });
                                }
                            }
                        }
                    }
                }

                _filters.Add(month);
            }
 
            return _filters;
        }

        private enum TableStyle
        {
            NotDefined,
            Horizontal,
            Vertical,
        }

        private FIDCResult Load()
        {
            FIDCResult result = new FIDCResult()
            {
                Id = 0,
                TimeStamp = DateTime.Now,
                Items = new List<FIDCItem>()
            };

            string finalURL = URLSeed;

            foreach (ScrapperFilter filter in this.Filters)
            {
                finalURL += "&" + filter.ToURLParameter();
            }

            HtmlDocument doc = Scrapper.GetDocument(finalURL);
            //HtmlDocument doc = Scrapper.GetDocumentFromFile(AppDomain.CurrentDomain.BaseDirectory + "Services\\TestData\\Mensal489.html");

            if (doc.DocumentNode != null)
            {
                FIDCItem previous = null;
                foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
                {
                    TableStyle style = TableStyle.NotDefined;

                    // Try to find Table Style
                    if (style == TableStyle.NotDefined)
                    {
                        int columns = 0;
                        foreach (HtmlNode row in table.SelectNodes("tr"))
                        {
                            HtmlNodeCollection cols = row.SelectNodes("td");
                            if (cols != null && cols.Count > columns)
                            {
                                columns = cols.Count;
                            }
                        }

                        if (columns == 2) style = TableStyle.Vertical;
                        if (columns > 3) style = TableStyle.Horizontal;
                    }

                    if (style == TableStyle.Vertical)
                    {
                        foreach (HtmlNode row in table.SelectNodes("tr"))
                        {
                            HtmlNodeCollection labelNodes = row.SelectNodes("td[1]/span");
                            HtmlNodeCollection valueTextNodes = row.SelectNodes("td[2]/span");

                            if (labelNodes != null && valueTextNodes != null)
                            {
                                if (labelNodes.Count == 1 && valueTextNodes.Count == 1)
                                {
                                    prepareItem(result, labelNodes[0], valueTextNodes[0], previous);
                                    previous = result.Items.Last();
                                }

                                if (labelNodes.Count == 2)
                                {
                                    prepareItem(result, labelNodes[0], labelNodes[1], previous);
                                    previous = result.Items.Last();

                                    prepareItem(result, valueTextNodes[0], valueTextNodes[1], previous);
                                    previous = result.Items.Last();
                                }
                            }
                        }
                    }
                    else if (style == TableStyle.Horizontal)
                    {

                    }
                }
            }


            return result;
        }


        private static bool prepareItem(FIDCResult result, HtmlNode labelNode, HtmlNode valueTextNode, FIDCItem previousItem)
        {
            if (labelNode != null && valueTextNode != null)
            {
                // Key
                string key = "";
                HtmlAttribute keyAttr = labelNode.Attributes.FirstOrDefault(x => x.Name.ToLower() == "id");
                if (keyAttr != null) key = keyAttr.Value;

                // Label
                string label = labelNode.InnerText;

                // Value
                string value = "";
                value = valueTextNode.InnerText;

                // Parent
                int level = labelNode.InnerText.CleanString(false).StartsWithCounter("&nbsp;&nbsp;&nbsp;&nbsp;");

                FIDCItem parent = (previousItem != null && level > 0) ? previousItem.GetParentFromLevel(level - 1) : null;

                result.Items.Add(new FIDCItem() { Key = key.CleanString(), Label = label.CleanString(), Value = value.CleanString(), Parent = parent });

                return true;
            }

            return false;
        }
    }
}