using Newtonsoft.Json;
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
    public class PFApiGetForm
    {
        public string Abbrev { get; set; }
        public DateTime MeetingDate { get; set; }
        public int MeetingId { get; set; }
        public int NumberOfRaces { get; set; }

        [JsonProperty(PropertyName = "RaceDetails")]
        public FormRaceDetails RaceDetail { get; set; }
        public string RailPosition { get; set; }
        public string Track { get; set; }
    }
    public class Horse
    {
        public int Age { get; set; }
        public int Barrier { get; set; }
        public double Claim { get; set; }
        public string Country { get; set; }
        public string Dam { get; set; }
        public string FoalDate { get; set; }
        public int FormId { get; set; }
        public int HorseId { get; set; }
        public string Jockey { get; set; }
        public int JockeyId { get; set; }
        public string Last10 { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string PrizeMoney { get; set; }
        public string Record { get; set; }
        public string RecordDistance { get; set; }
        public string RecordFirm { get; set; }
        public string RecordFirstUp { get; set; }
        public string RecordGood { get; set; }
        public string RecordHeavy { get; set; }
        public string RecordJumps { get; set; }
        public string RecordSecondUp { get; set; }
        public string RecordSoft { get; set; }
        public string RecordSynthetic { get; set; }
        public string RecordTrack { get; set; }
        public string RecordTrackDistance { get; set; }
        public int RunNumber { get; set; }
        public List<Run> Runs { get; set; }
        public string Sex { get; set; }
        public string Silks { get; set; }
        public string SilksSmall { get; set; }
        public string Sire { get; set; }
        public string Trainer { get; set; }
        public int TrainerId { get; set; }
        public double Weight { get; set; }
    }

    public class FormRaceDetails
    {
        public string AgeRestictions { get; set; }
        public string ClassRestrictions { get; set; }
        public int Distance { get; set; }
        public string Grade { get; set; }

        [JsonProperty(PropertyName = "Horses")]
        public List<Horse> Field { get; set; }
        public bool IsJumps { get; set; }
        public string JockeyClaim { get; set; }
        public int PrizeMoney { get; set; }
        public string PrizeMoneyBreakdownFormatted { get; set; }
        public int RaceId { get; set; }
        public string RaceName { get; set; }
        public int RaceNumber { get; set; }
        public object SexRestictions { get; set; }
        public string StartTime { get; set; }
        public string WeightRestictions { get; set; }
        public string WeightType { get; set; }
    }


    public class Run
    {
        public string AgeRestrictions { get; set; }
        public int Barrier { get; set; }
        public string Class { get; set; }
        public string Distance { get; set; }
        public int FieldSize { get; set; }
        public int FormId { get; set; }
        public string Gears { get; set; }
        public string InRun { get; set; }
        public string Jockey { get; set; }
        public double JockeyClaim { get; set; }
        public int JockeyId { get; set; }
        public double LimitWeight { get; set; }
        public string Margin { get; set; }
        public string MeetingDate { get; set; }
        public int MeetingId { get; set; }
        public string Name { get; set; }
        public string OtherRunners { get; set; }
        public string OtherRunnersEx { get; set; }
        public int Position { get; set; }
        public double Price { get; set; }
        public string Prizemoney { get; set; }
        public string PrizemoneyWon { get; set; }
        public int RaceId { get; set; }
        public int RaceNumber { get; set; }
        public int RunsFromSpell { get; set; }
        public string Sectional { get; set; }
        public string SexRestrictions { get; set; }
        public string StewardsReport { get; set; }
        public int TabNumber { get; set; }
        public string Time { get; set; }
        public string Track { get; set; }
        public string TrackCode { get; set; }
        public string TrackCondition { get; set; }
        public int TrainerId { get; set; }
        public double Weight { get; set; }
    }
}
