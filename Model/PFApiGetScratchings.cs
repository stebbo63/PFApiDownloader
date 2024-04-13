using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618
#pragma warning disable CS8600

namespace pfAPIDownloader.Model
{

    public class PFApiScrMeet
    {
        public bool Abandoned { get; set; }
        public DateTime LastUpdate { get; set; }
        public string MeetingDate { get; set; }
        public string[] Scratchings { get; set; }
        public string Source { get; set; }
        public int TrackCondition { get; set; }
        public int TrackConditionNumber { get; set; }
        public int TrackConditionOld { get; set; }
        public string TrackName { get; set; }
        public string Weather { get; set; }
    }

}
