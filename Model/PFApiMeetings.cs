using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pfAPIDownloader.Model
{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618

    public class PFApiMeetings
    {
        public bool IsError { get; set; }
        public bool IsOverRequestLimit { get; set; }
        public string PageDescription { get; set; }
        public string PageTitle { get; set; }

        [JsonProperty(PropertyName = "Result")]
        public PFApiMeet[] Meetings { get; set; }
        public object ServerTime { get; set; }
    }

    public class PFApiMeet
    {
        [JsonProperty(PropertyName = "Date")]
        public DateTime MeetingDate { get; set; }
        public DateTime DateStamp { get; set; }
        public bool HasSectionals { get; set; }
        public bool IsBarrierTrial { get; set; }
        public DateTime LastUpdate { get; set; }
        public int MeetingId { get; set; }
        public int RaceCount { get; set; }
        public int[] RaceNumbers { get; set; }
        public bool Resulted { get; set; }
        public string State { get; set; }
        public bool TABMeeting { get; set; }
        public string Track { get; set; }
        public int TrackId { get; set; }
    }
}
