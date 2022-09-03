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
using System.Diagnostics;
using TCC_WEB_SCRAPING.Mode;

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
            return View();
        }

        public ActionResult Submit()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Submit(Response response)
        {
            try
            {
                Dosubmit(response);
            }
            catch 
            {
                return View("Failed");
            }
            
            return View("Sucess");

        }


#region Submit

        private void Dosubmit(Response response)
        {

            var path = response.Htmlpath;

            var doc = new HtmlDocument();
            doc.Load(path);
            string StringDoc = doc.Text;


            int index = StringDoc.IndexOf("Values gathered over");
            string TotalHours = "";
            TotalHours = StringDoc.Substring((index + 20), 13);
            TotalHours = TotalHours.TrimStart();


            Dictionary<string, string> Dict = new Dictionary<string, string>();
            Dict.Add("Total Hours", TotalHours);


            var tituloValors = ParseHtml(StringDoc);
            WriteToCsv(tituloValors, Dict, response);
        }


        private Dictionary<string, string> ParseHtml(string html)
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

        private void WriteToCsv(Dictionary<string, string> Dict, Dictionary<string, string> Hours, Response response)
        {
            StringBuilder sb = new StringBuilder();

            DateTime thisDay = DateTime.Today;
            string Dataatual = thisDay.ToString("d");

            Dictionary<string, string> Desc = new Dictionary<string, string>();
            Dict.Add("Description", response.Desc);
            Dict.Add("Data", Dataatual);


            foreach (var entry in Hours)
            {
                sb.AppendLine($"{entry.Key};{entry.Value}");
            }

            foreach (var entry in Dict)
            {
                sb.AppendLine($"{entry.Key};{entry.Value}");
            }

            foreach (var entry in Desc)
            {
                sb.AppendLine($"{entry.Key};{entry.Value}");
            }

            System.IO.File.WriteAllText(response.Knimepath, sb.ToString());

            DoKnime(response);


        }

        private void DoKnime(Response response)
        {
            string command = "/C cd D:/Program Files/KNIME && knime --launcher.suppressErrors -reset -nosplash -application org.knime.product.KNIME_BATCH_APPLICATION -workflowDir=";
            var knimeWork = response.Knimepath.Split('/');
            var knimework = "";
            var knimelenth = knimeWork.Length - 2;
            for (int i = 0; i <= knimelenth; i++)
            {
                knimework += knimeWork[i] + '/';
            }
            knimework += "TCC_WEB_SCRAPING\"";


            command += '\"' + knimework ;

            ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
            info.UseShellExecute = true;
            info.WorkingDirectory = "D:/Program Files/KNIME";
            info.Arguments = command;
            Process.Start(info);
        }


        #endregion Submit

    }
}