using Newtonsoft.Json;
using pfAPIDownloader.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618
#pragma warning disable CS8600

namespace pfAPIDownloader.DataAccess
{
    public class PFApiRepository
    {
        #region FIELDS
        string m_apibase        = "https://www.puntingform.com.au/api";
        string m_GetFormText    = "{0}/formdataservice/GetFormText/{1}/{2}/{3}{4}";
        string m_GetFormJSON    = "{0}/formdataservice/GetForm/{1}/{2}/{3}{4}";
        string m_GetMeetingList = "{0}/formdataservice/GetMeetingListExt/{1}{2}";
        string m_GetResults     = "{0}/formdataservice/GetResults/{1}/{2}{3}";
        string m_GetGearChanges = "{0}/formdataservice/GetGearChanges{1}";
        string m_ExportMeetings = "{0}/formdataservice/ExportMeetings/{1}/true{2}";   // true includes barrier trials. send false for no barrier trials
        string m_ExportRaces    = "{0}/formdataservice/ExportRaces/{1}/true{2}";      // true includes barrier trials. send false for no barrier trials
        string m_ExportFields   = "{0}/formdataservice/ExportFields/{1}/true{2}";     // true includes barrier trials. send false for no barrier trials
        string m_GetScratchings = "{0}/ScratchingsService/GetAllScratchings{1}";
        string m_GetRatings     = "{0}/RatingsService/GetRatingsText/{1}/{2}{3}";
        string m_GetSouthCoast  = "{0}/ratingsservice/GetSouthcoastData/premium/false/all{1}";

        string m_apikey         = string.Empty;
        string m_DumpDir        = string.Empty;
        DateTime m_LastSend     = DateTime.MinValue;
        #endregion

        public int n_ApiCalls = 0;

        #region Constructor
        public PFApiRepository(string apikey, string dumpdir)
        {
            m_apikey  = "?apikey=" + apikey;
            m_DumpDir = dumpdir;

            if (!Directory.Exists(dumpdir))
                Directory.CreateDirectory(dumpdir);
        }
        #endregion

        #region Public Methods
        public PFApiMeetings? GetMeets(DateTime meetDate)
        {
            try
            {
                string url = String.Format(m_GetMeetingList, m_apibase, meetDate.ToString("dd-MMM-yyyy"), m_apikey);
                string json = CallApi(url);

                // Dump the returned data to file
                if (m_DumpDir != null && Directory.Exists(m_DumpDir))
                {
                    File.WriteAllText($"{m_DumpDir}getapimeets_{meetDate.ToString("yyyy-MM-dd")}.txt", json);
                }

                if (!string.IsNullOrEmpty(json))
                {
                    PFApiMeetings result = JsonConvert.DeserializeObject<PFApiMeetings>(json);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
            }
            return null;
        }
        public PFApiGetForm? GetForm(string track, DateTime meetDate, int raceno)
        {
            try
            {
                string url = String.Format(m_GetFormJSON, m_apibase, track, raceno, meetDate.ToString("dd-MMM-yyyy"), m_apikey);
                string json = CallApi(url);

                if (m_DumpDir != null && Directory.Exists(m_DumpDir))
                {
                    File.WriteAllText($"{m_DumpDir}getapiform_{meetDate.ToString("yyyy-MM-dd")}_{track}_{raceno.ToString("00")}.txt", json);
                }

                if (!string.IsNullOrEmpty(json))
                {
                    PFApiGetForm result = JsonConvert.DeserializeObject<PFApiGetForm>(json);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
        public PFApiGetResult? GetResults(string track, DateTime meetDate)
        {
            try
            {
                string url = String.Format(m_GetResults, m_apibase, track, meetDate.ToString("dd-MMM-yyyy"), m_apikey);
                string json = CallApi(url);

                if (m_DumpDir != null && Directory.Exists(m_DumpDir))
                {
                    File.WriteAllText($"{m_DumpDir}getapiresults_{meetDate.ToString("yyyy-MM-dd")}_{track}.txt", json);
                }

                if (!string.IsNullOrEmpty(json))
                {
                    PFApiGetResult result = JsonConvert.DeserializeObject<PFApiGetResult>(json);

                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
        public List<string>? GetRatings(string track, DateTime meetDate)
        {
            try
            {
                string url = String.Format(m_GetRatings, m_apibase, track, meetDate.ToString("dd-MMM-yyyy"), m_apikey);
                string ratings = CallApi(url);

                if (m_DumpDir != null && Directory.Exists(m_DumpDir))
                {
                    File.WriteAllText($"{m_DumpDir}getapiratings_{meetDate.ToString("yyyy-MM-dd")}_{track}.txt", ratings);
                }

                if (!string.IsNullOrEmpty(ratings))
                {
                    return ratings.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
        public List<string>? GetSouthCoastData()
        {
            try
            {
                string url     = String.Format(m_GetSouthCoast, m_apibase, m_apikey);
                string ratings = CallApi(url);

                if (m_DumpDir != null && Directory.Exists(m_DumpDir))
                {
                    File.WriteAllText($"{m_DumpDir}getsouthcoast.txt", ratings);
                }

                if (!string.IsNullOrEmpty(ratings))
                {
                    return ratings.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
        public List<string>? GetGearChanges()
        {
            try
            {
                string url   = String.Format(m_GetGearChanges, m_apibase, m_apikey);
                string gears = CallApi(url);

                if (m_DumpDir != null && Directory.Exists(m_DumpDir))
                {
                    File.WriteAllText($"{m_DumpDir}getgearchanges.txt", gears);
                }

                if (!string.IsNullOrEmpty(gears))
                {
                    return gears.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
        public List<string>? ExportMeetings(DateTime meetDate)
        {
            try
            {
                string url = String.Format(m_ExportMeetings, m_apibase, meetDate.ToString("dd-MMM-yyyy"), m_apikey);
                string ratings = CallApi(url);

                if (m_DumpDir != null && Directory.Exists(m_DumpDir))
                {
                    File.WriteAllText($"{m_DumpDir}getapiexportmeetings_{meetDate.ToString("yyyy-MM-dd")}.txt", ratings);
                }

                if (!string.IsNullOrEmpty(ratings))
                {
                    return ratings.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
        public List<string>? ExportRaces(DateTime meetDate)
        {
            try
            {
                string url = String.Format(m_ExportRaces, m_apibase, meetDate.ToString("dd-MMM-yyyy"), m_apikey);
                string ratings = CallApi(url);

                if (m_DumpDir != null && Directory.Exists(m_DumpDir))
                {
                    File.WriteAllText($"{m_DumpDir}getapiexportraces_{meetDate.ToString("yyyy-MM-dd")}.txt", ratings);
                }

                if (!string.IsNullOrEmpty(ratings))
                {
                    return ratings.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
        public List<string>? ExportFields(DateTime meetDate)
        {
            try
            {
                string url = String.Format(m_ExportFields, m_apibase, meetDate.ToString("dd-MMM-yyyy"), m_apikey);
                string ratings = CallApi(url);

                if (m_DumpDir != null && Directory.Exists(m_DumpDir))
                {
                    File.WriteAllText($"{m_DumpDir}getapiexportfields_{meetDate.ToString("yyyy-MM-dd")}.txt", ratings);
                }

                if (!string.IsNullOrEmpty(ratings))
                {
                    return ratings.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }
        public List<PFApiScrMeet>? GetScratchings()
        {
            try
            {
                string url = String.Format(m_GetScratchings, m_apibase, m_apikey);
                string json = CallApi(url);

                if (m_DumpDir != null && Directory.Exists(m_DumpDir))
                {
                    File.WriteAllText($"{m_DumpDir}getapiscratchings.txt", json);
                }

                if (!string.IsNullOrEmpty(json))
                {
                    List<PFApiScrMeet> result = JsonConvert.DeserializeObject<List<PFApiScrMeet>>(json);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return new List<PFApiScrMeet>();
        }

        #endregion

        #region Private Methods
        private string CallApi(string url)
        {
            /* ensure there is a minimum 1 second delay between each call
             * while many of the loop's have a 1 second delay, this ensures
             * that there will always be at least one second between calls
             */
            while (m_LastSend.AddMilliseconds(1100) > DateTime.Now)
            {
                Thread.Sleep(100);
            }

            // Call the PF API and download the data
            var client   = new RestClient();
            var request  = new RestRequest(url);
            var response = client.Get(request);

            // track usage.
            m_LastSend = DateTime.Now;
            n_ApiCalls++;

            if (response != null && response.IsSuccessStatusCode && !string.IsNullOrEmpty(response.Content))
            {
                return response.Content;
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
