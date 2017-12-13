using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using Alexa.NET.Request;
using Amazon.S3.Model;

namespace TechNottinghamAlexaSkill
{
    public class TechNottsEvent
    {
        public string Name { get; }
        public string EventType { get; }
        public string TitleFilter { get; }
        public string InSentence => " " + Name;
        public string SmallImage { get; set; }
        public string LargeImage { get; set; }

        public static TechNottsEvent TechNottingham { get; } = new TechNottsEvent("Tech Nottingham", "tech-nottingham") { LargeImage = "techNottinghamLarge.png", SmallImage = "techNottingham.png" };
        public static TechNottsEvent WomenInTech { get; } = new TechNottsEvent("Women in Technology", "women-in-tech-nottingham"){LargeImage="womenintechlarge.png",SmallImage="womenintech.png"};
        public static TechNottsEvent TechOnToast { get; } = new TechNottsEvent("Tech on Toast", "tech-nottingham", "Toast"){LargeImage="techontoast.png", SmallImage="techontoast.png" };
        public static TechNottsEvent NottTuesday { get; } = new TechNottsEvent("Nott Tuesday", "nott-tuesday"){LargeImage = "nottTuesday.png",SmallImage = "nottTuesday.png"};
        public static TechNottsEvent Empty { get; } = new TechNottsEvent(string.Empty, string.Empty);

        public static Dictionary<string, TechNottsEvent> EventList = new Dictionary<string, TechNottsEvent>
        {
            {"tech nottingham", TechNottingham},
            {"tech notts", TechNottingham},
            {"women in tech", WomenInTech},
            {"women in technology", WomenInTech},
            {"nott tuesday",NottTuesday},
            {"notts tuesday",NottTuesday},
            {"tech on toast",TechOnToast }
        };

        public TechNottsEvent(string name, string type, string titleFilter = null)
        {
            Name = name;
            EventType = type;
            TitleFilter = titleFilter;
        }

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
