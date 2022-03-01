﻿using System;

namespace TrialByFire.Tresearch.Models
{
    public class TopSearch
    {
        public DateTime topSearchDate { get; set; }

        public string searchString { get; set; }

        public int searchCount { get; set; }

        public TopSearch(DateTime topSearchDate, string searchString, int searchCount)
        {
            this.topSearchDate = topSearchDate;
            this.searchString = searchString;
            this.searchCount = searchCount;
        }
    }
}