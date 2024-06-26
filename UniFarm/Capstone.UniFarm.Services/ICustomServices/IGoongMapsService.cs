﻿using Capstone.UniFarm.Services.Commons;
using Capstone.UniFarm.Services.ViewModels.ModelResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Services.ICustomServices
{
    public interface IGoongMapsService
    {
        Task<string> GetAddressFromLatLong(double latitude, double longitude);
        Task<GoongMapResponse> GetDistanceAndDuration(string origins, string destinations);
    }
}
