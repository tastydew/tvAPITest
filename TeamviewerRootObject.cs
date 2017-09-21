/* Teamviewer API Test by Fredy Teyssedou *
 * 
 * A simple test to get, post and delete devices via 
 * the REST api for teamviewer.
 * 
 */

using System.Collections.Generic;
using Newtonsoft.Json;

namespace TVAPITest
{
    public class TeamviewerRootObject
    {
        [JsonProperty("devices")]
        public List<TeamviewerDevice> Devices { get; set; }
    }
}
