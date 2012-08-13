using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCScrapper.Models;
using MVCScrapper.Services;

namespace MVCScrapper.Controllers
{
    public class FIDCController : Controller
    {
        //
        // GET: /FIDC/

        public ActionResult Mensal(int? serviceId, string competencia)
        {
            ViewBag.Title = "FIDC";
            ViewBag.Message = "Fundo de Investimento em Direitos Creditórios";

            ViewModels.FIDCMensalViewModel model = new ViewModels.FIDCMensalViewModel();

            model.Scrappers = FIDCService.GetScrappers();

            if (serviceId == null)
            {
                model.ScrapperSelected = model.Scrappers[0];
            }
            else
            {
                model.ScrapperSelected = model.Scrappers.FirstOrDefault(x => x.Id == serviceId);
            }

            if (model.ScrapperSelected != null)
            {
                ScrapperFilter filterMonth = model.ScrapperSelected.Filters.FirstOrDefault(x => x.ID.ToLower() == "competencia");
                if (filterMonth != null && competencia != null)
                {
                    filterMonth.Value = competencia.Replace(" ","/");
                }
            }

            return View(model);
        }

    }
}
