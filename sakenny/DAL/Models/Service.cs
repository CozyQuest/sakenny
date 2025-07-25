﻿using sakenny.DAL.Interfaces;

namespace sakenny.DAL.Models
{
    public class Service : ISoftDeletable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<Property> Properties { get; set; }
    }
}
