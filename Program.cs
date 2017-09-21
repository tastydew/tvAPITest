/* Teamviewer API Test by Fredy Teyssedou *
 * 
 * Simple call to Teamviewer REST API to add the current
 * device to the current provided token's account
 * 
 */

using System;
using System.Net;
using Microsoft.Win32;

namespace TVAPITest
{
    class Program
    {

        static void Main(string[] args)
        {
            TeamviewerRootObject _deviceList = null;

            ////TODO: Remove this
            //_deviceList = TeamviewerAPI.getDevices();
            //ShowDevices(_deviceList);
            //Console.ReadKey();

            Console.WriteLine("Gathering System Information, please wait...");

            //get the remote control id of the client's computer
            string _clientID = "r" + getClientRemoteID();

            System.Threading.Thread.Sleep(1500);
            Console.Clear();

            //user enters information as they would like it seen on their TV contact list.
            Console.WriteLine("Enter the name of the computer as you want it seen on your Teamviewer contact list");
            string _alias = Console.ReadLine();
            Console.WriteLine("(optional) enter the description of the computer as you want it seen on your contact list");
            string _desc = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Please Wait....");

            Tuple<HttpStatusCode,TeamviewerRootObject> addDeviceResult = TeamviewerAPI.addCurrentDevice(_alias, _desc, _clientID);
            //make the call to the api to add the device, return the status code and act accordingly.
            if (addDeviceResult.Item1 == HttpStatusCode.OK)
            {
                Console.WriteLine("Please wait while your devices are retrieved.");
                _deviceList = TeamviewerAPI.getDevices();
                Console.Clear();
                Console.WriteLine("-----TEAMVIEWER DEVICES ON ACCOUNT --------");
                ShowDevices(_deviceList);
            }
            else if (addDeviceResult.Item1 == HttpStatusCode.NotAcceptable) //this is returned if the device already exists.
            {
                Console.WriteLine("This device already exists as a contact on your list as : " + addDeviceResult.Item2.Devices[0].alias);
                Console.WriteLine("Would you like to replace this with the computer name you entered? (Y/N)");
                string response = Console.ReadLine();

                if (response.ToLower() == "y")
                {
                    Console.WriteLine("Please wait while your computer is renamed on your contact list...");
                    HttpStatusCode replacementStatus = TeamviewerAPI.replaceCurrentDevice(addDeviceResult.Item2, _alias, _desc);
                    if (replacementStatus == HttpStatusCode.OK)
                    {
                        Console.WriteLine("Done!");
                    }
                }
                else
                {
                    Console.WriteLine("Okay. The device has not been changed.");
                }
                Console.WriteLine("Please press any key to continue...");
            }
            else
            {
                Console.WriteLine("There was a problem trying to get your device added to your contact list!");
            }

            Console.ReadKey();
        }

        private static string getClientRemoteID()
        {
            //get the appropriate registry key based of x86 / x64 systems
            RegistryKey key = null;
            if (Environment.Is64BitOperatingSystem)
            {
                key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\TeamViewer");
            }
            else
            {
                key = Registry.LocalMachine.OpenSubKey("Software\\TeamViewer");
            }

            if (key != null)
            {
                Object obj = key.GetValue("ClientID");
                if (obj != null)
                {
                    return obj.ToString();
                }
                return null;
            }
            return null;
        }

        //when a list of devices is returned, iterate through the list and display it to the user.
        static void ShowDevices(TeamviewerRootObject deviceList)
        {
            for (int i = 0; i <= deviceList.Devices.Count - 1; i++)
            {

                Console.WriteLine("Device # " + (i + 1) + ":");
                Console.WriteLine("Name: " + deviceList.Devices[i].alias);
                Console.WriteLine("Group ID: " + deviceList.Devices[i].groupid);
                Console.WriteLine("Remote ID: " + deviceList.Devices[i].remoteID);
                Console.WriteLine("Device ID: " + deviceList.Devices[i].deviceID);
                Console.WriteLine("Status : " + deviceList.Devices[i].online_state);
                Console.WriteLine("\n\n");
            }
        }
    }
}
