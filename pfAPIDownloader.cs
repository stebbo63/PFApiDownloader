using Newtonsoft.Json;
using pfAPIDownloader.DataAccess;
using pfAPIDownloader.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618
#pragma warning disable CS8600
#pragma warning disable CS8602

namespace pfAPIDownloader
{

    public class pfAPIDownloader
    {
        string apikey                 = "12345";
        string savelocation           = "d:\\temp\\pfapi\\";
        int pastDays                  = -2;
        int nextDays                  = 4;
        bool DoLast7Export            = false;
        bool ChangesThisPass          = false;
        DateTime lastCheck  = DateTime.MinValue;
        PFApiRepository pfRepo;
        PFUpdates pfUpdateData;

        public void Start()
        {
            Console.WriteLine("PF API Downloader example program");
            Console.WriteLine("Press x to exit the program");
            Console.WriteLine("===========================");

            // read apikey from file rather than source
            string apikeyfile = $"{savelocation}apikey.key";
            if (File.Exists(apikeyfile))
                apikey = File.ReadAllText(apikeyfile);

            // Create PFApiRepository object and read "cached" data from file
            pfRepo = new PFApiRepository(apikey, savelocation);
            pfUpdateData = ReadUpdateData();

            // Check date of last called to reset number of apicalls if required
            if (pfUpdateData.LastCalled.Date < DateTime.Now.Date)
            {
                pfRepo.n_ApiCalls       = 0;
                pfUpdateData.LastCalled = DateTime.Now;
            }
            else
                pfRepo.n_ApiCalls = pfUpdateData.NumApiCalls;

            // Run until we get a signal to close.
            while (true)
            {
                // Check Daily Updates.  This should happen once per day on initial load.
                if (pfUpdateData.DailyCheckTS.Date < DateTime.Now.Date)
                {
                    DoDailyUpdates();
                    pfUpdateData.DailyCheckTS = DateTime.Now;
                }

                // Check Hourly Updates.
                if (pfUpdateData.HourlyCheckTS.AddMinutes(60) < DateTime.Now)
                {
                    DoHourlyUpdates();
                    pfUpdateData.HourlyCheckTS = DateTime.Now;
                }

                // Do meeting Checks every 10 mins
                lastCheck       = DateTime.Now;
                for (int i = pastDays; i <= nextDays; i++)
                {
                    // Get list of meetings for each date
                    DateTime mDate = DateTime.Now.Date.AddDays(i);
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm")} Checking meetings for date {mDate.ToString("yyyy-MM-dd")}");
                    PFApiMeetings pfMeets = pfRepo.GetMeets(mDate);
                    Thread.Sleep(1000);

                    if (pfMeets != null)
                    {
                        // Check each meeting to see if it's new, or has been updated since it's last check
                        // GetMeetings may include barrier trials for past dates, so ignore those.
                        foreach (var pfMeet in pfMeets.Meetings.Where(t => t.IsBarrierTrial == false))
                        {
                            if (MeetingIsNewOrUpdated(pfMeet))
                            {
                                // Update the meeting.
                                Console.WriteLine($"Meeting at {pfMeet.Track} on {pfMeet.MeetingDate.ToString("dd-MMM")} is new or updated. Last updated in API at {pfMeet.LastUpdate.ToString("HH:mm on dd-MMM")}");

                                if (pfMeet.Resulted)
                                {
                                    DownloadMeetingResults(pfMeet);
                                }
                                else if (pfMeet.MeetingDate.Date < DateTime.Now.AddDays(-1).Date)
                                {
                                    // ignore this.  The api only has "form" available for 1 day after the meeting. 
                                }
                                else
                                {
                                    DownloadMeetingForm(pfMeet);
                                    DownloadMeetingRatings(pfMeet);
                                }

                                ChangesThisPass = true;
                            }
                            
                            CheckExit();
                        }
                    }                    
                }

                if (ChangesThisPass)
                {
                    DownloadGearChanges();
                    ChangesThisPass = false;
                }                

                // Check Scratchings
                CheckScratchings();

                Console.WriteLine($"      Number of api calls made today is {pfRepo.n_ApiCalls}");

                // Save Update Data each 10 mins
                SaveUpdateData();

                // Wait 10 minutes for next pass.
                TwiddleThumbsTillNextPass();
            }
        }
        private void CheckScratchings()
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm")} Checking Scratchings");
            List<PFApiScrMeet> pfScratchings = pfRepo.GetScratchings();
            foreach (var pfScr in pfScratchings)
            {
                if (ScratchingsAreNewOrUpdated(pfScr))
                {
                    // Scratchings for this meeting have changed.  
                    Console.WriteLine($"         Scratchings at {pfScr.TrackName} on {pfScr.MeetingDate} have changed.");

                    // You will likely want to do something here

                    ChangesThisPass = true;
                }
            }
        }
        private bool DownloadMeetingForm(PFApiMeet pfMeet)
        {
            Console.WriteLine($"   Downloading form for meeting at {pfMeet.Track} on {pfMeet.MeetingDate.ToString("dd-MMM")}");
            foreach (var r in pfMeet.RaceNumbers)
            {
                PFApiGetForm pfRace = pfRepo.GetForm(pfMeet.Track, pfMeet.MeetingDate, r);

                if (pfRace != null && RaceIsNewOrUpdated(pfRace))
                {
                    Console.WriteLine($"     Race {pfRace.RaceDetail.RaceNumber} at {pfMeet.Track} on {pfMeet.MeetingDate.ToString("dd-MMM")} has changed");

                    // You will likely want to action something here.
                }

                Thread.Sleep(1000);
            }

            return true;
        }
        private bool DownloadMeetingResults(PFApiMeet pfMeet)
        {
            Console.WriteLine($"   Downloading results for meeting at {pfMeet.Track} on {pfMeet.MeetingDate.ToString("dd-MMM")}");
            PFApiGetResult pfResults = pfRepo.GetResults(pfMeet.Track, pfMeet.MeetingDate);

            if (pfResults != null && RaceResultsAreNewOrUpdated(pfResults))
            {
                Console.WriteLine($"     Results at {pfMeet.Track} on {pfMeet.MeetingDate.ToString("dd-MMM")} have changed");

                // You will likely want to action something here.
            }

            Thread.Sleep(1000);
            return true;
        }
        private bool DownloadMeetingRatings(PFApiMeet pfMeet)
        {
            Console.WriteLine($"   Downloading ratings for meeting at {pfMeet.Track} on {pfMeet.MeetingDate.ToString("dd-MMM")}");
            List<string> pfRatings = pfRepo.GetRatings(pfMeet.Track, pfMeet.MeetingDate);

            if (pfRatings != null && RatingsAreNewOrUpdated(pfRatings, pfMeet.Track, pfMeet.MeetingDate))
            {
                Console.WriteLine($"     Ratings at {pfMeet.Track} on {pfMeet.MeetingDate.ToString("dd-MMM")} have changed");

                // You will likely want to action something here.
            }

            Thread.Sleep(1000);
            return true;
        }
        private bool DownloadRatingsViaSouthCoastCall()
        {
            Console.WriteLine($"   Downloading ratings vai GetSouthCoastRatings call");
            List<string> pfRatings = pfRepo.GetSouthCoastData();

            if (pfRatings != null && RatingsAreNewOrUpdated(pfRatings, "all", DateTime.Now.Date))
            {
                Console.WriteLine($"     Ratings have changed");

                // You will likely want to action something here.
            }

            Thread.Sleep(1000);
            return true;
        }
        private bool DownloadGearChanges()
        {
            Console.WriteLine($"   Downloading GearChanages");
            List<string> pfGears = pfRepo.GetGearChanges();

            if (pfGears != null && RatingsAreNewOrUpdated(pfGears, "all", DateTime.Now.Date))
            {
                Console.WriteLine($"     Gear changes has changed");

                // You will likely want to action something here.
            }

            Thread.Sleep(1000);
            return true;
        }
        private bool DownloadPastSevenDaysResults()
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm")} Downloading last 7 days worth of fields / results");
            DateTime yesterday      = DateTime.Now.AddDays(-1).Date;
            List<string> pfMeetings = pfRepo.ExportMeetings(yesterday);
            List<string> pfRaces    = pfRepo.ExportRaces(yesterday);
            List<string> pfFields   = pfRepo.ExportFields(yesterday);

            // You will likely want to do something with this data here

            return true;
        }
        private void DoDailyUpdates()
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm")} Performing Daily Updates");

            // Download meetings and results data for the past 7 days using the ExportXXX API calls.
            // This is useful as sometimes there are barrier trials that come in late and also occasional results updates.
            // Some clients use this to ensure their databases are kept as up to date as possible
            if (DoLast7Export)
                DownloadPastSevenDaysResults();

        }
        private void DoHourlyUpdates()
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm")} Performing Hourly Updates");
        }
        private bool MeetingIsNewOrUpdated(PFApiMeet pfMeet)
        {
            if (pfUpdateData.MeetingUpdated.ContainsKey(pfMeet.MeetingId))
            {
                if (pfMeet.LastUpdate > pfUpdateData.MeetingUpdated[pfMeet.MeetingId])
                {
                    // meeting has been updated, store new LastUpdate and return true
                    pfUpdateData.MeetingUpdated[pfMeet.MeetingId] = pfMeet.LastUpdate;
                    return true;
                }
            }
            else
            {
                // meeting is new
                pfUpdateData.MeetingUpdated.Add(pfMeet.MeetingId, pfMeet.LastUpdate);
                return true;
            }

            // If neither of the above, then the meeting is not updated.
            return false;
        }
        private bool ScratchingsAreNewOrUpdated(PFApiScrMeet pfScr)
        {
            /* The PF API Scratchings "Last Updated" doesn't discern which meetings
             * have actually been updated, so this routine creates it's own hash of
             * track condition and number of scratchings to determine if there has
             * been any changes at the specific venue 
             */

            string key  = $"{pfScr.MeetingDate}_{pfScr.TrackName}";
            string hash = $"{pfScr.TrackConditionNumber},{pfScr.Scratchings.Count()}";
            if (pfUpdateData.ScratchUpdates.ContainsKey(key))
            {
                if (!pfUpdateData.ScratchUpdates[key].Equals(hash))
                {
                    // meeting has been updated, store new LastUpdate and return true
                    pfUpdateData.ScratchUpdates[key] = hash;
                    return true;
                }
            }
            else
            {
                // meeting is new
                pfUpdateData.ScratchUpdates.Add(key, hash);
                return true;
            }

            // If neither of the above, then the meeting is not updated.
            return false;
        }
        private bool RaceIsNewOrUpdated(PFApiGetForm pfRace)
        {
            string hash = GetHash(JsonConvert.SerializeObject(pfRace));
            if (pfUpdateData.RaceHashes.ContainsKey(pfRace.RaceDetail.RaceId))
            {
                if (!pfUpdateData.RaceHashes[pfRace.RaceDetail.RaceId].Equals(hash))
                {
                    // Race is different, so store new hash and return true.
                    pfUpdateData.RaceHashes[pfRace.RaceDetail.RaceId] = hash;
                    return true;
                }
            }
            else
            {
                // race is new
                pfUpdateData.RaceHashes.Add(pfRace.RaceDetail.RaceId, hash);
                return true;
            }
            // If neither of the above, then race is not changed.
            return false;
        }
        private bool RaceResultsAreNewOrUpdated(PFApiGetResult pfResults)
        {
            string hash = GetHash(JsonConvert.SerializeObject(pfResults));
            if (pfUpdateData.ResultHashes.ContainsKey(pfResults.MeetingId))
            {
                if (!pfUpdateData.ResultHashes[pfResults.MeetingId].Equals(hash))
                {
                    // meeting results are different, store new hash and return true
                    pfUpdateData.ResultHashes[pfResults.MeetingId] = hash;
                    return true;
                }
            }
            else
            {
                pfUpdateData.ResultHashes.Add(pfResults.MeetingId, hash);
                return true;
            }
            // If neither of the above, then race is not changed.
            return false;
        }
        private bool RatingsAreNewOrUpdated(List<string> pfRatings, string track, DateTime meetDate)
        {
            string key  = $"{meetDate.ToString("yyyyMMdd")}_{track.Replace(" ","_")}";
            string hash = GetHash(JsonConvert.SerializeObject(pfRatings)) ;
            if (pfUpdateData.RatingsHashes.ContainsKey(key))
            {
                if (!pfUpdateData.RatingsHashes[key].Equals(hash))
                {
                    // ratings are changed, store new hash and return true
                    pfUpdateData.RatingsHashes[key] = hash;
                    return true;
                }
            }
            else
            {
                pfUpdateData.RatingsHashes.Add(key, hash);
                return true;
            }
            // If neither of the above, then race is not changed.
            return false;
        }
        private void TwiddleThumbsTillNextPass()
        {
            while (lastCheck.AddMinutes(10) > DateTime.Now)
            {
                Thread.Sleep(1000);
                CheckExit();
            }
        }
        private void CheckExit()
        {
            if (Console.KeyAvailable == true && Console.ReadKey().KeyChar == 'x')
            {
                SaveUpdateData();
                Environment.Exit(0);
            }

            // Close the program after 11:30pm each nite.
            if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 30)
            {
                SaveUpdateData();
                Environment.Exit(0);
            }
        }
        private PFUpdates ReadUpdateData()
        {
            PFUpdates result = new PFUpdates();
            try
            {
                string json = File.ReadAllText($"{savelocation}apiupdatedata.txt");
                if (!string.IsNullOrEmpty( json ))
                {
                    result = JsonConvert.DeserializeObject<PFUpdates>( json );
                    return result;
                }
            }
            catch
            {
                // don't do anything, return default result
            }

            return result;
        }
        private void SaveUpdateData()
        {
            pfUpdateData.NumApiCalls = pfRepo.n_ApiCalls;
            pfUpdateData.LastCalled  = DateTime.Now;

            string json = JsonConvert.SerializeObject(pfUpdateData);
            File.WriteAllText($"{savelocation}apiupdatedata.txt", json);
        }
        private string GetHash(string obj)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(obj);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
