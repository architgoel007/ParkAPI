using ParkAPI.Models;
using System.Collections.Generic;

namespace ParkAPI.Repository.IRepository
{
    public interface INationalParkRepository
    {
        //FOR ALL DATA
        ICollection<NationalPark> GetNationalParks();
        //FOR GET ONE NATIONAL PARK
        NationalPark GetNationalPark(int natonalParkId);
        //IS NATIONAL PARK EXISTS BASED ON NAME?
        bool NationalParkExists(string name);
        //IS NATIONAL PARK EXISTS BASED ON ID?
        bool NationalParkExists(int id);
        //FOR CREATING NATIONAL PARK
        bool CreateNationalPark(NationalPark nationalPark);
        //FOR UPDATING NATIONAL PARK
        bool UpdateNationalPark(NationalPark nationalPark);
        //FOR DELETING NATIONAL PARK
        bool DeleteNationalPark(NationalPark nationalPark);
        //FOR SAVE CHANGES
        bool Save();
    }
}
