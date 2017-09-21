/* Teamviewer API Test by Fredy Teyssedou *
 * 
 * A simple test to get, post and delete devices via 
 * the REST api for teamviewer.
 * 
 */

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Linq;

namespace TVAPITest
{
    public static class TeamviewerAPI
    {
        public enum CallType
        {
            GET,
            POST,
            DELETE
        };

        private const string accessToken = "XXXXXXXXXXXXXXXXXXXX";
        private const string apiAddress = "https://webapi.teamviewer.com/api/v1/devices";
        private const string _groupID = "XXXXXXXXX";

        public static TeamviewerRootObject getDevices(string parameters = null)
        {
            //create request with auth token
            HttpWebRequest request = WebRequest.Create(apiAddress + parameters) as HttpWebRequest;
            request.Headers.Set("Authorization", "Bearer " + accessToken);
            request.Method = CallType.GET.ToString();

            try
            {
                //create a request to get
                WebResponse response = request.GetResponse() as HttpWebResponse;
                string content;
                TeamviewerRootObject deviceList;
                using (var requestContent = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(requestContent))
                    {
                        content = reader.ReadToEnd();

                        return deviceList = JsonConvert.DeserializeObject<TeamviewerRootObject>(content);
                    }
                }

            }
            catch (WebException ex)
            {
                Console.WriteLine("Error getting response : " + ex.Message);
                return null;
            }

        }

        public static HttpStatusCode replaceCurrentDevice(TeamviewerRootObject oldDevice, string _newAlias, string _description)
        {
            HttpWebRequest request = WebRequest.Create(apiAddress + "/" + oldDevice.Devices[0].deviceID) as HttpWebRequest;
            request.Headers.Set("Authorization", "Bearer " + accessToken);
            request.ContentType = "application/json";
            request.Method = CallType.DELETE.ToString();

            HttpWebResponse response = null;
            try
            {
                //create a request to get
                response = (HttpWebResponse) request.GetResponse();
                
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    var addDeviceResponse = addCurrentDevice(_newAlias, _description, oldDevice.Devices[0].remoteID);
                    return addDeviceResponse.Item1;
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("Error getting response : " + ex.Message);
            }

            return response.StatusCode;
           
        }

        public static Tuple<HttpStatusCode,TeamviewerRootObject> addCurrentDevice(string _alias, string _description, string clientID)
        {
            //check to see if the device already exists on the account.
            TeamviewerRootObject deviceQuery = getDevices("?remotecontrol_id=" + clientID);
            if (deviceQuery.Devices.Exists(x => x.remoteID == clientID))
            {
                return Tuple.Create(HttpStatusCode.NotAcceptable, deviceQuery); //return a status code other than OK to handle later.
            }

            HttpWebRequest request = WebRequest.Create(apiAddress) as HttpWebRequest;
            request.Headers.Set("Authorization", "Bearer " + accessToken);
            request.ContentType = "application/json";
            var postData = new TeamviewerDevice
            {
                remoteID = clientID,
                groupid = _groupID, // add your group id here.
                description = _description,
                alias = _alias
            };

            string postToJson = JsonConvert.SerializeObject(postData);
            request.Method = CallType.POST.ToString();
            try
            {
                using (var postStream = new StreamWriter(request.GetRequestStream()))
                {
                    postStream.Write(postToJson);
                    postStream.Flush();
                    postStream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return Tuple.Create(response.StatusCode, deviceQuery);
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return Tuple.Create(HttpStatusCode.Conflict, deviceQuery);
            }
        }
    }
}
