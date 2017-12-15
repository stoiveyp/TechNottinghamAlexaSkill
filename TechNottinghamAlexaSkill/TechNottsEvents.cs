using System.Collections.Generic;
using System.Linq;
using Alexa.NET.Request;

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
        public string[] Titles { get; set; }

        public static TechNottsEvent TechNottingham { get; } = new TechNottsEvent("Tech Nottingham", "tech-nottingham")
        {
            LargeImage = "techNottinghamLarge.png", SmallImage = "techNottingham.png",
            Titles = new[] { "tech nottingham", "tech notts","tech nott","tek nott","tek notts" }
        };

        public static TechNottsEvent WomenInTech { get; } = new TechNottsEvent("Women in Technology", "women-in-tech-nottingham")
        {
            LargeImage="womenintechlarge.png",SmallImage="womenintech.png",
            Titles = new[] { "women in technology", "women in tech", "women", "women in" }
        };
        public static TechNottsEvent TechOnToast { get; } = new TechNottsEvent("Tech on Toast", "tech-nottingham", "Toast")
        {
            LargeImage="techontoast.png", SmallImage="techontoast.png",
            Titles = new[] {"tech on toast","tekrom toast", "teckrom toast", "techrom toast"}
        };
        public static TechNottsEvent NottTuesday { get; } = new TechNottsEvent("Nott Tuesday", "nott-tuesday")
        {
            LargeImage = "nottTuesday.png",SmallImage = "nottTuesday.png",
            Titles = new[] {"nott tuesday", "notts tuesday"}
        };
        public static TechNottsEvent Empty { get; } = new TechNottsEvent(string.Empty, string.Empty);

        static TechNottsEvent()
        {
            var eventList = new Dictionary<string, TechNottsEvent>();
            TechNottingham.RegisterWith(eventList);
            WomenInTech.RegisterWith(eventList);
            NottTuesday.RegisterWith(eventList);
            TechOnToast.RegisterWith(eventList);
            EventList = eventList;
        }

        private void RegisterWith(Dictionary<string, TechNottsEvent> eventList)
        {
            foreach (var title in Titles ?? new string[] { })
            {
                eventList.Add(title,this);
            }
        }

        public static Dictionary<string, TechNottsEvent> EventList { get; set; }

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
