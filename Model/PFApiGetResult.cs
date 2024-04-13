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
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class PFApiGetResult
    {
        public DateTime MeetingDate { get; set; }
        public int MeetingId { get; set; }
        public int NumberOfRaces { get; set; }
        public string Penetrometer { get; set; }
        public List<ResultRaceDetails> RaceDetails { get; set; }
        public string RailPosition { get; set; }
        public string Track { get; set; }
        public int TrackId { get; set; }
        public string Weather { get; set; }
    }

    public class ResultRaceDetails
    {
        public string AgeRestrictions { get; set; }
        public string Class { get; set; }
        public int Distance { get; set; }
        public string Group { get; set; }
        public bool IsJumps { get; set; }
        public int RaceId { get; set; }
        public int RaceNumber { get; set; }
        public string RaceTime { get; set; }
        public string RaceTimeString { get; set; }
        [JsonProperty(PropertyName = "Runners")]
        public List<Runner> Runner { get; set; }
        public int SectionalDistance { get; set; }
        public string SectionalTime { get; set; }
        public string SectionalTimeString { get; set; }
        public object SexRestrictions { get; set; }
        public string TrackCondition { get; set; }
        public int TrackConditionNumber { get; set; }
        public object TrackType { get; set; }
        public string WeightRestrictions { get; set; }
        public int WindDirection { get; set; }
        public int WindSpeed { get; set; }
    }

    public class Runner
    {
        public int Barrier { get; set; }
        public string Country { get; set; }
        public string Flucs { get; set; }
        public int FormId { get; set; }
        public string Gears { get; set; }
        public int HorseId { get; set; }
        public string InRun { get; set; }
        public string Jockey { get; set; }
        public int JockeyId { get; set; }
        public double Margin { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public int Position { get; set; }
        public double Price_Betfair { get; set; }
        public double Price_NSWTab { get; set; }
        public double Price_SP { get; set; }
        public double Price_VICTab { get; set; }
        public string PrizemoneyWon { get; set; }
        public int RunNumber { get; set; }
        public string Stewards { get; set; }
        public string Trainer { get; set; }
        public int TrainerId { get; set; }
        public double Weight { get; set; }
    }


}
