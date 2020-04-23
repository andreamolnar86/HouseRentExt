using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace HouseRent
{
    /// <summary>
    ///  Data structure for storing owner's information
    /// </summary>
    public struct Owner
    {
        int ownerId;
        string ownerName;
        string ownerEmail;

        public int OwnerId
        {
            get { return ownerId; }
            set { ownerId = value; }
        }

        public string OwnerName
        {
            get { return ownerName; }
            set { ownerName = value; }
        }

        public string OwnerEmail
        {
            get { return ownerEmail; }
            set { ownerEmail = value; }
        }

        public Owner(int oId, string oName, string oEmail)
        {
            ownerId = oId;
            ownerName = oName;
            ownerEmail = oEmail;
        }
    } 
}
