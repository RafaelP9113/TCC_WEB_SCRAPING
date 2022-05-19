using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;

namespace TCC_WEB_SCRAPING.Controllers
{
    public class HomeController : Controller
    {

        public class TituloValor
        {
            public string Titulo { get; set; }
            public string valor { get; set; }
        }
        public ActionResult Index()
        {
            string url = "https://en.wikipedia.org/wiki/List_of_programmers";

            //var path = @"C:/Users/Ronieri/Documents/GitHub/TCC_WEB_SCRAPING/HTML_FILES/FurnaceTable.html"; // PC
            //var path = @"C:/Users/rafaelpinheiro/Desktop/Rafael/GIT/TCC_WEB_SCRAPING/HTML_FILES/1ZoneDataCenterCRAC_wApproachTempTable.html";  // Note trampo
            var path = @"C:\Users\Rafael\Desktop\Rafael\GIT\TCC_WEB_SCRAPING\HTML_FILES\1ZoneDataCenterCRAC_wApproachTempTable.html"; // Note facul

            var doc = new HtmlDocument();
            doc.Load(path);
            string StringDoc = doc.Text;
            //int index = StringDoc.IndexOf("Total Site Energy");
            //string TotalSiteEnergy = "";
            //TotalSiteEnergy = StringDoc.Substring((index + 47), 11);
            //TotalSiteEnergy = TotalSiteEnergy.TrimStart();

            //index = StringDoc.IndexOf("Net Site Energy");
            //string NetSiteEnergy = "";
            //NetSiteEnergy = StringDoc.Substring((index + 47), 9);
            //NetSiteEnergy = NetSiteEnergy.TrimStart();

            //index = StringDoc.IndexOf("Total Source Energy");
            //string TotalSourceEnergy = "";
            //TotalSourceEnergy = StringDoc.Substring((index + 48), 12);
            //TotalSourceEnergy = TotalSourceEnergy.TrimStart();

            //index = StringDoc.IndexOf("Net Source Energy");
            //string NetSourceEnergy = "";
            //NetSourceEnergy = StringDoc.Substring((index + 47), 11);
            //NetSourceEnergy = NetSourceEnergy.TrimStart();

            //var node = doc.DocumentNode.SelectSingleNode("//body");

            //var response = CallUrl(url).Result;
            var tituloValors = ParseHtml(StringDoc);
            //WriteToCsv(linkList);

            
            return View();
        }

        private static  Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return  response;
        }

        private Dictionary<string,string> ParseHtml(string html)
        {

            string[] Titulo = null;
            Titulo[0] = "Total Site Energy";
            Titulo[1] = "Net Site Energy";
            Titulo[2] = "Total Source Energy";
            Titulo[3] = "Net Source Energy";
            Titulo[4] = "Heating";
            Titulo[5] = "Cooling";
            Titulo[6] = "Interior Lighting";
            Titulo[7] = "Exterior Lighting";
            Titulo[8] = "Interior Equipment";
            Titulo[9] = "Exterior Equipment";
            Titulo[10] = "Fans";
            Titulo[11] = "Pumps";
            Titulo[12] = "Heat Rejection";
            Titulo[13] = "Humidification";
            Titulo[14] = "Heat Recovery";
            Titulo[15] = "Water Systems";
            Titulo[16] = "Refrigeration";
            Titulo[17] = "Generators";

            int[] index = null;
            index[0] = 1;
            index[1] = 1;
            index[2] = 1;
            index[3] = 1;
            index[4] = 13;
            index[5] = 13;
            index[6] = 13;
            index[7] = 13;
            index[8] = 13;
            index[9] = 13;
            index[10] = 13;
            index[11] = 13;
            index[12] = 13;
            index[13] = 13;
            index[14] = 13;
            index[15] = 13;
            index[16] = 13;
            index[17] = 13;

            string[] Categoria = null;
            Categoria[0] = "Electricity [GJ]";
            Categoria[1] = "Natural Gas [GJ]";
            Categoria[2] = "Gasoline [GJ]";
            Categoria[3] = "Diesel [GJ]";
            Categoria[4] = "Coal [GJ]";
            Categoria[5] = "Fuel Oil No 1 [GJ]";
            Categoria[6] = "Fuel Oil No 2 [GJ]";
            Categoria[7] = "Propane [GJ]";
            Categoria[8] = "Other Fuel 1 [GJ]";
            Categoria[9] = "Other Fuel 2 [GJ]";
            Categoria[10] = "District Cooling [GJ]";
            Categoria[11] = "District Heating [GJ]";
            Categoria[12] = "Water [m3]";







            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var htmlNodes = htmlDoc.DocumentNode.Descendants("td")
                    .ToList();

            List<TituloValor> tituloValors = new List<TituloValor>();
            Dictionary<string, string> Dict = new Dictionary<string, string>();

            TituloValor tituloValor = new TituloValor();

            Dict.Add(htmlNodes[4].InnerText, tituloValor.valor = htmlNodes[5].InnerText.TrimStart());
            Dict.Add(htmlNodes[8].InnerText, tituloValor.valor = htmlNodes[9].InnerText.TrimStart());

            for (int i = 0; i < htmlNodes.Count; i++)
            {
                for(int j = 0; j < Titulo.Length; j++)
                {
                    if (Titulo[j] == htmlNodes[i].InnerText)
                    {
                        int count = index[i];
                        if (count < 10)
                        {
                            Dict.Add(htmlNodes[i].InnerText, htmlNodes[i + count].InnerText);
                        }
                        else
                        {
                            for(int k = 0; k < count; k++)
                            {
                                Dict.Add((Titulo[j] + Categoria[k]), htmlNodes[i + k + 1].InnerText);
                            }
                        }
                    }
                }

            }




            

            return Dict;

        }

        private void WriteToCsv(List<string> links)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.AppendLine(link);
            }

            System.IO.File.WriteAllText("links.csv", sb.ToString());
        }


        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}