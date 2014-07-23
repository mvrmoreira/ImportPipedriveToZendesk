using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ZendeskApi_v2;

//https://github.com/eneifert/ZendeskApi_v2
//https://developers.pipedrive.com/v1
namespace ImportPipedriveToZendesk {
    class Program {
        static void Main(string[] args) {

            GetPipedrivePersonByEmail("teste");

            ZendeskApi api = new ZendeskApi("https://mundipagg.zendesk.com/api/v2", "xxxxxxxxxx@xxxxxxxxxxx.com", "", "xxxxxxxxxxxxxxxxxxx");
            var usersResponse = api.Users.GetAllUsers();
            foreach (var user in usersResponse.Users) {
                
            }

            Console.ReadLine();

        }

        private static PipedriveResponse GetPipedrivePersonByEmail(string email) {            

            string sURL = "http://api.pipedrive.com/v1/persons/find?term=email@email.com&start=0&search_by_email=1&api_token=xxxxxxxxxxxxxxxxxxx";
            WebRequest wrGETURL = WebRequest.Create(sURL);
            Stream objStream = wrGETURL.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            string sLine = objReader.ReadLine();
            PipedriveResponse pipedriveResponse = JsonConvert.DeserializeObject<PipedriveResponse>(sLine);

            return pipedriveResponse;
        }
    }
}
