using System;

namespace MTCScanner.Models
{
    public class Item
    {
        public string Name { get; set; }
        public string Stand { get; set; }
        public string Company { get; set; }
        public string Id { get; set; }

        public string Text { get; set; }
        public string Description { get; set; }
    }

    public class GraphUser
    {
        public string displayName { get; set; }
        public string mail { get; set; }
        public string StandID { get; set; }
        public string userPrincipalName { get; set; }
        public string Company { get; set; }




    }

    public class BARCODE_TBL
    {

        public int ID { get; set; }
        public string EMAIL { get; set; }
        public string DISPLAY_NAME { get; set; }
        public string BARCODE { get; set; }
        public string TIMESTAMP { get; set; }
        public string NOTES { get; set; }
        public string STANDID { get; set; }
        public string COMPANY { get; set; }



    }

}