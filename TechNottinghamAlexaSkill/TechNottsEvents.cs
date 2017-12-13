using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Alexa.NET.Request;

namespace TechNottinghamAlexaSkill
{
    public class TechNottsEvent
    {
        public string Name { get; }
        public string EventType { get; }
        public string TitleFilter { get; }
        public string InSentence => " " + Name;

        static TechNottsEvent()
        {
            var womenInTech = new TechNottsEvent("Women in Technology","women-in-tech-nottingham");
            EventList.Add("women in tech",womenInTech);
            EventList.Add("women in technology",womenInTech);

            var techNottingham = new TechNottsEvent("Tech Nottingham","tech-nottingham");
            EventList.Add("tech nottingham", techNottingham);
            EventList.Add("tech notts",techNottingham);

            var techOnToast = new TechNottsEvent("Tech on Toast","tech-nottingham","Toast");
            EventList.Add("tech on toast",techOnToast);

            var nottTuesday = new TechNottsEvent("Nott Tuesday","nott-tuesday");
            EventList.Add("nott tuesday",nottTuesday);
            EventList.Add("notts tuesday",nottTuesday);
        }

        public TechNottsEvent(string name, string type, string titleFilter = null)
        {
            Name = name;
            EventType = type;
            TitleFilter = titleFilter;
        }

        public static readonly TechNottsEvent Empty = new TechNottsEvent(string.Empty,string.Empty);
        public static readonly IDictionary<string,TechNottsEvent> EventList = new Dictionary<string, TechNottsEvent>();

        public static TechNottsEvent Parse(Intent intent)
        {
            if ((intent?.Slots?.Count ?? 0) == 0 || string.IsNullOrWhiteSpace(intent.Slots["event"].Value) || !EventList.Keys.Contains(intent.Slots["event"].Value.ToLower()))
            {
                return Empty;
            }

            return EventList[intent.Slots["event"].Value.ToLower()];
        }
    }
}
