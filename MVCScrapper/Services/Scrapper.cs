using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using HtmlAgilityPack;

namespace MVCScrapper.Services
{
    public class Scrapper
    {
        public static HtmlDocument GetDocument(string url)
        {
            System.Diagnostics.Debug.WriteLine("MVCScrapper.Services.Scrapper.GetDocument( url:" + url + " )");

            HtmlWeb webGet = new HtmlWeb();
            webGet.OverrideEncoding = Encoding.GetEncoding("ISO-8859-1");
            return webGet.Load(url);
        }

        public static HtmlDocument GetDocumentFromFile(string file)
        {
            System.Diagnostics.Debug.WriteLine("MVCScrapper.Services.Scrapper.GetDocumentFromFile( file:" + file + " )");
            
            StreamReader streamReader = new StreamReader(file);
            string html = streamReader.ReadToEnd();
            streamReader.Close();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
    }
}