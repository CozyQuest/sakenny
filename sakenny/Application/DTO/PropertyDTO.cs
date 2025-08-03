using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sakenny.DAL.Models;

namespace sakenny.Application.DTO
{
    public class PropertyDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PropertyTypeId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public string District { get; set; }
        public int BuildingNo { get; set; }
        public int Level { get; set; }
        public int FlatNo { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public int RoomCount { get; set; }
        public int BathroomCount { get; set; }
        public double Space { get; set; }
        public decimal Price { get; set; }
        public int PeopleCapacity { get; set; }
        public string MainImageUrl { get; set; }
        public List<string> Images { get; set; }
        public HashSet<GetAllServiceDTO> Services { get; set; }
        public UserOwnerDTO User { get; set; }
    }
}