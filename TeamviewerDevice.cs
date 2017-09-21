/* Teamviewer API Test by Fredy Teyssedou *
 * 
 * A simple test to get, post and delete devices via 
 * the REST api for teamviewer.
 * 
 */

using Newtonsoft.Json;

namespace TVAPITest
{

    public class TeamviewerDevice
    {
        [JsonProperty("device_id")]
        public string deviceID { get; set; }
        [JsonProperty("remotecontrol_id")]
        public string remoteID { get; set; }
        [JsonProperty("groupid")]
        public string groupid { get; set; }
        [JsonProperty("alias")]
        public string alias { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("online_state")]
        public string online_state { get; set; }
        [JsonProperty("supported_features")]
        public string features { get; set; }
        [JsonProperty("assigned_to")]
        public string assignedTo { get; set; }
        [JsonProperty("policy_id")]
        public string policy_id { get; set; }
    }
}
