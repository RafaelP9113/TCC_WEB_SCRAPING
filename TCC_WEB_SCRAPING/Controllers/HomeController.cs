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
            var path = @"C:/Users/rafaelpinheiro/Desktop/Rafael/GIT/TCC_WEB_SCRAPING/HTML_FILES/1ZoneDataCenterCRAC_wApproachTempTable.html";  // Note trampo
            //var path = @"C:\Users\Rafael\Desktop\Rafael\GIT\TCC_WEB_SCRAPING\HTML_FILES\1ZoneDataCenterCRAC_wApproachTempTable.html"; // Note facul

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

            int index = StringDoc.IndexOf("Values gathered over");
            string TotalHours = "";
            TotalHours = StringDoc.Substring((index + 20), 13);
            TotalHours = TotalHours.TrimStart();

            //var node = doc.DocumentNode.SelectSingleNode("//body");
            Dictionary<string, string> Dict = new Dictionary<string, string>();
            Dict.Add("Total Hours", TotalHours);

            //var response = CallUrl(url).Result;
            var tituloValors = ParseHtml(StringDoc);
            WriteToCsv(tituloValors, Dict);

            
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

            string[] Titulo = new string[18];
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

            int[] index = new int[18];
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

            string[] Categoria = new string[13];
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

            Dictionary<string, string> Dict = new Dictionary<string, string>();


            for (int i = 0; i < htmlNodes.Count; i++)
            {
                    
                for (int j = 0; j < Titulo.Length; j++)
                {
                    if (Titulo[j] == htmlNodes[i].InnerText)
                    {
                        int count = index[j];
                        if (count < 10)
                        {
                            Dict.Add(htmlNodes[i].InnerText, htmlNodes[i + count].InnerText);
                        }
                        else
                        {
                            for (int k = 0; k < count; k++)
                            {
                                if (Dict.Count < 186)
                                {
                                    Dict.Add(("Site " + Titulo[j] + " " + Categoria[k]), htmlNodes[i + k + 1].InnerText);
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < htmlNodes.Count; i++)
            {

                if (htmlNodes[i].InnerText == "Source District Heating [GJ]")
                {
                    int cont2 = 1;
                    for (int s = 0; s < Titulo.Length; s++)
                    {

                        for (int j = 0; j < Titulo.Length; j++)
                        {

                            if (Titulo[j] == htmlNodes[i + cont2].InnerText)
                            {
                                int count = index[j] - 1;

                                for (int k = 0; k < count; k++)
                                {
                                    if (Dict.Count < 354)
                                    {
                                        Dict.Add(("Source " + Titulo[j] + " " + Categoria[k]), htmlNodes[cont2 + i + k + 1].InnerText);
                                    }
                                }
                            }
                        }
                        cont2 += 13;
                    }
                }
            } 

            
            return Dict;

        }

        private void WriteToCsv(Dictionary<string,string> Dict, Dictionary<string, string> Hours)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var entry in Hours)
            {
                sb.AppendLine($"{entry.Key};{entry.Value}");
            }

            foreach (var entry in Dict)
            {
                sb.AppendLine($"{entry.Key};{entry.Value}") ;
            }

            System.IO.File.WriteAllText("C:/Users/rafaelpinheiro/Desktop/Rafael/Pasta.csv", sb.ToString());
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