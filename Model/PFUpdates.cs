﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pfAPIDownloader.Model
{
    public class PFUpdates
    {
        public int NumApiCalls        = 0;
        public DateTime LastCalled    = DateTime.MinValue;
        public DateTime DailyCheckTS  = DateTime.MinValue;
        public DateTime HourlyCheckTS = DateTime.MinValue;

        public Dictionary<int, DateTime> MeetingUpdated    = new Dictionary<int, DateTime>();
        public Dictionary<int, string> RaceHashes          = new Dictionary<int, string>();
        public Dictionary<int, string> ResultHashes        = new Dictionary<int, string>();
        public Dictionary<string, string> RatingsHashes    = new Dictionary<string, string>();
        public Dictionary<string, string> ScratchUpdates   = new Dictionary<string, string>();

        // keep track of when meetings and races are added so we can clear old data from memory
        public Dictionary<int, DateTime> MeetingAdded = new Dictionary<int, DateTime>();
        public Dictionary<int, DateTime> RaceAdded    = new Dictionary<int, DateTime>();

        public void ClearHashes()
        {
            RatingsHashes.Clear();
            ScratchUpdates.Clear();
        }
    }
}
