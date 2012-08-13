using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCScrapper.Models;

namespace MVCScrapper.Services
{
    public interface IScrapper
    {
        int Id { get; set; }
        string URLSeed {get; set;}
        string Label {get; set;}
        List<ScrapperFilter> Filters { get; }
        FIDCResult Result { get; }
    }

    public class ScrapperFilter
    {
        public string ID { get; set; }
        public string Label { get; set; }
        public string ExternalID { get; set; }
        public string Value { get; set; }
        public List<ScrapperFilterItem> AvailableValues { get; set; }

        public string ToURLParameter()
        {
            return this.ExternalID + "=" + this.Value;
        }

        public FIDCFilter ToDataFilter()
        {
            return new FIDCFilter()
            {
                Name = this.ID,
                Value = this.Value,
            };
        }
    }

    public class ScrapperFilterItem
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}