﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Users;

//https://github.com/eneifert/ZendeskApi_v2
//https://developers.pipedrive.com/v1
namespace ImportPipedriveToZendesk {
    class Program {

        static string PipedriveApiUrl = "http://api.pipedrive.com/v1";
        static string PipedriveApiToken = "";
        static ZendeskApi zendeskApi = new ZendeskApi("https://.zendesk.com/api/v2", "@.com", "", "");

        static void Main(string[] args) {

            PipedrivePerson person;
            bool updateUser;

            // contador de usuarios atualizados
            int usersUpdatedCount = 0;

            // contador de usuários sem telefone
            int usersWithoutPhoneCount = 0;

            // pega todos os usuarios do zendesk paginados
            GroupUserResponse usersResponse = zendeskApi.Users.GetAllUsers();

            // varre todas as paginas de usuarios
            while (usersResponse.NextPage != null)
            {
                // varre todos os usuarios da pagina
                foreach (var user in usersResponse.Users)
                {
                    Console.Out.Write(String.Format("{0} - {1} - ", user.Id, user.Email));

                    try
                    {
                        // procura pessoa no pipedrive
                        person = GetPipedrivePersonByEmail(user.Email);

                        Console.Out.Write("FOUND!!! ");

                        updateUser = false;

                        if (person.phones.Count() > 0) {
                            if (user.Phone == null) {
                                user.Phone = person.phones.First<PipedriveContact>().value;
                                updateUser = true;
                            }                        
                        }   

                        if (person.phones.Count() > 1) {
                            if (user.CustomFields["celular"] == null) {
                                user.CustomFields["celular"] = person.phones[1].value;
                                updateUser = true;
                            }   
                        }

                        if (updateUser) {
                            zendeskApi.Users.UpdateUser(user);
                            usersUpdatedCount++;
                            Console.Out.Write("PHONE UPDATED!!! " + usersUpdatedCount);
                        }
                    }
                    catch (Newtonsoft.Json.JsonSerializationException ex)
                    {
                        Console.Out.Write("Not...");
                    }
                    catch (Exception ex)
                    {
                        Console.Out.Write(ex.Message);
                    }

                    Console.Out.WriteLine();
                }

                // pega a proxima pagina
                usersResponse = zendeskApi.Users.GetByPageUrl<GroupUserResponse>(usersResponse.NextPage);
            }

            Console.Out.WriteLine();
            Console.Out.WriteLine("Users updated count: " + usersUpdatedCount);
            Console.Out.WriteLine("Users without phone count: " + usersWithoutPhoneCount);

            Console.ReadLine();
        }

        private static PipedrivePerson GetPipedrivePersonByEmail(string email, int start = 0) {

            string searchByEmailUrl = String.Format("{0}/persons/find?api_token={1}&start={2}&search_by_email=1&term={3}", 
                PipedriveApiUrl, PipedriveApiToken, start, email
            );

            PipedriveResponse searchResponse = SendPipedriveRequest(searchByEmailUrl);            

            if (searchResponse.data == null)
            {
                return new PipedrivePerson();
            }

            PipedrivePerson person = searchResponse.data.FirstOrDefault(p => !String.IsNullOrEmpty(p.phone));

            string searchByIdUrl = String.Format("{0}/persons/{2}?api_token={1}",
                PipedriveApiUrl, PipedriveApiToken, person.id
            );
            searchResponse = SendPipedriveRequest(searchByIdUrl);

            return searchResponse.person;
        }

        private static PipedriveResponse SendPipedriveRequest(string url)
        {
            WebRequest wrGETURL = WebRequest.Create(url);
            Stream objStream = wrGETURL.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            string sLine = objReader.ReadLine();
            sLine = sLine.Replace("\"phone\":[", "\"phones\":[");
            sLine = sLine.Replace("\"data\":{", "\"person\":{");
            PipedriveResponse pipedriveResponse = JsonConvert.DeserializeObject<PipedriveResponse>(sLine);           

            return pipedriveResponse;
        }
    }
}
