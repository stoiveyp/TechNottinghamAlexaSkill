﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TechNottingham.Common
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
            Titles = new[] { "tech nottingham" }
        };

        public static TechNottsEvent WomenInTech { get; } = new TechNottsEvent("Women in Technology", "women-in-tech-nottingham")
        {
            LargeImage="womenintechlarge.png",SmallImage="womenintech.png",
            Titles = new[] { "women in tech" }
        };
        public static TechNottsEvent TechOnToast { get; } = new TechNottsEvent("Tech on Toast", "tech-nottingham", "Toast")
        {
            LargeImage="techontoast.png", SmallImage="techontoast.png",
            Titles = new[] {"tech on toast"}
        };
        public static TechNottsEvent NottTuesday { get; } = new TechNottsEvent("Nott Tuesday", "nott-tuesday")
        {
            LargeImage = "nottTuesday.png",SmallImage = "nottTuesday.png",
            Titles = new[] {"nott tuesday","notts tuesday"}
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
                eventList.Add(title.ToLower(),this);
            }
        }

        public static Dictionary<string, TechNottsEvent> EventList { get; set; }

        public TechNottsEvent(string name, string type, string titleFilter = null)
        {
            Name = name;
            EventType = type;
            TitleFilter = titleFilter;
        }
    }
}
