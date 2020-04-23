using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HouseRent
{
    /// <summary>
    /// Data structure for storing country's Information
    /// </summary>
    public struct Country
    {
        int countryId;
        string countryName;

        public int CountryId
        {
            get { return countryId; }
            set { countryId = value; }
        }

        public string CountryName
        {
            get { return countryName; }
            set { countryName = value; }
        }

        public Country(int cID, string cName){
            countryId = cID;
            countryName = cName;
        }
    }

    public class CountriesDAL:DALGen 
    {       
        /// <summary>
        /// Gets the list of all countries
        /// </summary>
        /// <returns></returns>
        /// 
        public List<Country> GetCountryList(ref string error)
        {
            string query = "SELECT * FROM Orszagok";
            SqlDataReader dataReader = ExecuteReader(query, ref error);

            List<Country> countryList = new List<Country>();

            if (error == "OK")
            {
                Country item = new Country();
                while (dataReader.Read())
                {                   
                    try
                    {
                        item.CountryId = Convert.ToInt32(dataReader[0]);
                        item.CountryName = dataReader[1].ToString();
                        countryList.Add(item);
                    }
                    catch (Exception ex)
                    {
                        error = "Invalid data " + ex.Message;
                    }
                }
            }
            CloseDataReader(dataReader);

            return countryList;
        }
    }
}
