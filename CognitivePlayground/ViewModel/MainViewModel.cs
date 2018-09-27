﻿using GalaSoft.MvvmLight;
using log4net;

namespace CognitivePlayground.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MainViewModel));
        public string Title { get; set; } = "CognitivePlayground";
    }
}