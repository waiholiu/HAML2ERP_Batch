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

        static void Main(string[] args)
        {
            var client = new RestClient("https://haml2erb.org/api/convert");

            var path = args[0];
            string[] files = System.IO.Directory.GetFiles(path, "*.haml", SearchOption.AllDirectories);

            foreach(var hamlFile in files)
            {
                Console.WriteLine($"Sending {hamlFile} to haml2erb service" );
                //open the file up

                var haml = File.ReadAllText(hamlFile);

                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AlwaysMultipartFormData = true;
                request.AddParameter("converter", "herbalizer");
                request.AddParameter("haml", haml);

                IRestResponse response = client.Execute(request);

                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    JObject json = JObject.Parse(response.Content);
                    var erb = json["erb"].ToString();

                    var hamlPath = Path.GetDirectoryName(hamlFile);
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(hamlFile);
                    var erbFileName = $"{hamlPath}\\{fileNameWithoutExtension}.erb";

                    if(File.Exists(erbFileName))
                    {
                        Console.WriteLine($"{erbFileName} exists already. Skip over");
                    }
                    else
                    {
                        Console.WriteLine($"writing erb file to {erbFileName}");
                        File.WriteAllText(erbFileName, erb);

                        Console.WriteLine($"deleting old haml file to {hamlPath}");
                        File.Delete(hamlFile);

                    }


                }






            }

        }
    }
}
