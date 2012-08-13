using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using MVCScrapper.Models;

namespace MVCScrapper.Services
{
    public class FIDCScrapperMensal : IScrapper
    {
        public int Id { get; set; }
        public string URLSeed { get; set; }
        public string Label { get; set; }

        private FIDCResult _result = null;
        public FIDCResult Result
        {
            get {
                if (_result != null) return _result;
                return Load();
            }
        }

        public FIDCScrapperMensal()
        {
            URLSeed = @"http://cvmweb.cvm.gov.br/SWB/Sistemas/SCW/CPublica/InfoMensFIDC/CPublicaInfoMensFIDC.aspx?PK_PARTIC=60945";
            Label = "Mensal (até Dezembro de 2011)";
        }

        private List<ScrapperFilter> _filters = null;
        public List<ScrapperFilter> Filters
        {
            get {
                if (_filters != null) return _filters;
                return GetFilters();
            }
        }

        private List<ScrapperFilter> GetFilters()
        {
            if (_filters == null)
            {
                _filters = new List<ScrapperFilter>();

                // Month
                ScrapperFilter month = new ScrapperFilter() { ID = "competencia", Label = "Competência", ExternalID = "COMPTC", AvailableValues = new List<ScrapperFilterItem>(), Value = "12/2010" };

                HtmlDocument doc = Scrapper.GetDocument(URLSeed + "&" + month.ExternalID + "=" + month.Value);

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
                                if (year <= 2011)
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

        private FIDCResult Load()
        {
            string finalURL = URLSeed;

            foreach (ScrapperFilter filter in this.Filters)
            {
                finalURL += "&" + filter.ToURLParameter();
            }

            HtmlDocument doc = Scrapper.GetDocument(finalURL);

            FIDCResult result = new FIDCResult()
            {
                Id = 0,
                TimeStamp = DateTime.Now,
                Items = new List<FIDCItem>()
            };

            if (doc.DocumentNode != null)
            {
                // Cabeçalho
                HtmlNodeCollection tCabecalho = doc.DocumentNode.SelectNodes("//table[@id='tbCabecalhoInfo']/tr");
                if (tCabecalho != null)
                {
                    foreach (HtmlNode row in tCabecalho)
                    {
                        HtmlNode labelNode = row.SelectSingleNode("td[1]/span");
                        HtmlNode valueTextNode = row.SelectSingleNode("td[2]/span");
                        HtmlNode valueSelectNode = row.SelectSingleNode("td[2]/select/option[@selected]");

                        if (labelNode != null && (valueTextNode != null || valueSelectNode != null))
                        {
                            // Key
                            string key = "";
                            HtmlAttribute keyAttr = labelNode.Attributes.FirstOrDefault(x => x.Name.ToLower() == "id");
                            if (keyAttr != null) key = keyAttr.Value;

                            // Label
                            string label = labelNode.InnerText;

                            // Value
                            string value = "";
                            if (valueTextNode != null) value = valueTextNode.InnerText;
                            else if (valueSelectNode != null) value = valueSelectNode.Attributes.FirstOrDefault(x => x.Name.ToLower() == "value").Value;

                            result.Items.Add(new FIDCItem() { Key = key.CleanString(), Label = label.CleanString(), Value = value.CleanString() });
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Row Not Processed : " + row.InnerHtml);
                        }
                    }
                }

                // Corpo
                HtmlNodeCollection tCorpo = doc.DocumentNode.SelectNodes("//table[@id='tbCorpoInfo']/tr");
                if (tCorpo != null)
                {
                    FIDCItem previous = null;
                    foreach (HtmlNode row in tCorpo)
                    {
                        HtmlNode labelNode = row.SelectSingleNode("td[1]/span");
                        HtmlNode valueTextNode = row.SelectSingleNode("td[2]/span");

                        if (labelNode != null)
                        {
                            // Key
                            string key = "";
                            HtmlAttribute keyAttr = labelNode.Attributes.FirstOrDefault(x => x.Name.ToLower() == "id");
                            if (keyAttr != null) key = keyAttr.Value.CleanString();

                            // Label
                            string label = labelNode.InnerText.CleanString();


                            // Value
                            string value = (valueTextNode != null) ? valueTextNode.InnerText.CleanString() : null;

                            // Parent
                            int level = labelNode.InnerText.CleanString(false).StartsWithCounter("&nbsp;&nbsp;");

                            FIDCItem parent = (previous != null && level > 0) ? previous.GetParentFromLevel(level - 1) : null;

                            System.Diagnostics.Debug.WriteLine(level + " ::: " + label + " ::: " + labelNode.InnerText);
                            result.Items.Add(new FIDCItem() { Parent = parent, Key = key, Label = label, Value = value });
                            previous = result.Items.Last();
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Row Not Processed : " + row.InnerHtml);
                        }
                    }
                }

                // CotaSenior
                HtmlNodeCollection tHorizontais = doc.DocumentNode.SelectNodes("//table[@id='dgInfoCotaSenior'] | //table[@id='dgInfoCotaSubrd']");
                if (tHorizontais != null && tHorizontais.Count() == 2)
                {
                    foreach (HtmlNode tCotaSenior in tHorizontais)
                    {
                        HtmlNode parentNode = tCotaSenior.SelectSingleNode("preceding-sibling::*[1]/tr[last()]");
                        FIDCItem parent = new FIDCItem() { Key = "Anexo I - " + parentNode.InnerText.ToKey(), Label = "Anexo I - " + parentNode.InnerText.CleanString() };
                        result.Items.Add(parent);

                        if (tCotaSenior != null)
                        {
                            HtmlNodeCollection labelCol = tCotaSenior.SelectNodes("tr[1]/td");
                            HtmlNodeCollection valuesCol = tCotaSenior.SelectNodes("tr[2]/td");

                            if (labelCol != null && valuesCol != null)
                            {
                                int itemNumber = System.Math.Min(labelCol.Count(), valuesCol.Count());

                                string key = "";
                                string label = "";
                                string value = "";
                                for (int i = 0; i < itemNumber; i++)
                                {
                                    label = labelCol[i].InnerText;
                                    value = valuesCol[i].InnerText;

                                    result.Items.Add(new FIDCItem() { Parent = parent, Key = key.CleanString(), Label = label.CleanString(), Value = value.CleanString() });
                                }
                            }
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("TABELAS HORIZONTAIS: ERROR!!!");
                }
            }


            return result;
        }

    }
}