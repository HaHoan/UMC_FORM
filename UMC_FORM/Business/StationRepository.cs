using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UMC_FORM.Models;

namespace UMC_FORM.Business
{
    public static class StationRepository
    {
        public static List<Form_Stations> GetStations(string stationNo)
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Stations.Where(r => r.STATION_NO == stationNo).ToList();
            }
        }
        public static List<Form_Stations> GetStations()
        {
            using (DataContext context = new DataContext())
            {
                return context.Form_Stations.ToList();
            }
        }
    }
}
