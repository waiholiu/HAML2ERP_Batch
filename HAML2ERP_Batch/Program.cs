using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace HAML2ERP_Batch
{
    class Program
    {

        private static readonly HttpClient client = new HttpClient();
        const string logfilepath = "log.txt";

        static void Main(string[] args)
        {
            var client = new RestClient("https://haml2erb.org/api/convert");

            var workingDirectory = args[0];
            log($"working directory is {workingDirectory}");

            string[] files = System.IO.Directory.GetFiles(workingDirectory, "*.haml", SearchOption.AllDirectories);

            foreach(var hamlFile in files)
            {
                log($"Sending {hamlFile} to haml2erb service" );

                var haml = File.ReadAllText(hamlFile);

                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                request.AddParameter("converter", "herbalizer");
                request.AddParameter("haml", haml);

                IRestResponse response = client.Execute(request);

                JObject json = JObject.Parse(response.Content);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    
                    var erb = json["erb"].ToString();

                    var hamlPath = Path.GetDirectoryName(hamlFile);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(hamlFile);
                    var erbFileName = $"{hamlPath}\\{fileNameWithoutExtension}.erb";

                    if(File.Exists(erbFileName))
                    {
                        log($"{erbFileName} exists already. Skip over");
                    }
                    else
                    {
                        log($"writing erb file to {erbFileName}");
                        File.WriteAllText(erbFileName, erb);

                        log($"deleting old haml file to {hamlPath}");
                        File.Delete(hamlFile);

                    }


                }
                else
                {
                    log("Problem with the HAML, skipping over");
                    var erb = json["error"].ToString();
                    log(erb);
                }

            }

        }
        private static void log(string text)
        {
            Console.WriteLine(text);
            File.AppendAllText(logfilepath, text + "\n");
           

        }

    }



}
