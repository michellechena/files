using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TelerikMvcWebMail
{
    public static class Common
    {
        public static string CallWebApi(string APIUrl, RestSharp.Method Method,dynamic Paramiter=null)
        {
            var client = new RestClient(SessionMangment.Users_.APIHostUrl);           

            var request = new RestRequest(APIUrl, Method);      
            if(Paramiter!=null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddBody(Paramiter); // uses JsonSerializer
            }     
            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as stringret 
            return content;

        }
    }
}