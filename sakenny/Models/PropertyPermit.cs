namespace sakenny.Models
{
    public class PropertyPermit
    {
        public int id { get; set; }
        public string AdminID { get; set; }
        public int PropertyID { get; set; }
        public bool status { get; set; }

        //navigation property many between property permit and admin
        public Admin Admins { get; set; }

        //navigation property many between property permit and propertyID
        public Property Properties { get; set; }
    }
}
