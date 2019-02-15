﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using ReviewNotifier.Config;
using System.Threading.Tasks;
using System.Timers;
using ReviewNotifier.Helpers;
using ReviewNotifier.Observer;
using ReviewNotifier.Models;

namespace ReviewNotifier
{
    class Program
    {
        private static Settings _settings;
        private static LastIdSettings _lastIdSettings;
        private static TeamsNotifier _teams;
        private static LoginBuilder _loginBuilder;
        private static TfsDataConnector _tfs;
        private static Timer timer;
        private static int _lastId;
        private static string _webHookUrl;

        static void Main(string[] args)
        {
            _settings = new Configuration().GetSettings();
            _lastIdSettings = new LastIdSettings();
            _lastId = _lastIdSettings.Get();
            _teams = new TeamsNotifier(_webHookUrl);
            _loginBuilder = new LoginBuilder(_settings);
            _tfs = new TfsDataConnector(_settings, _loginBuilder, _lastId);

            timer = new Timer
            {
                AutoReset = true,
                Interval = 60000
            };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            //Timer_Elapsed(null, null);
            Console.ReadKey();
        }

        
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var reviews = _tfs.GetReviewData();

            foreach (var review in reviews)
            {
                _teams.Send(review);
            }

            _lastId = reviews.Any() ? reviews.Max(x => x.Id) : 1;
            _lastIdSettings.Save(_lastId);

        }
    }
}