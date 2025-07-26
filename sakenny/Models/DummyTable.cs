using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sakenny.Models
{
    public class DummyTable
    {

        public int id { get; set; }
        [Required]
        public string description { get; set; }

        //create one to one relationship with the user table
        [Required]
        [ForeignKey("User")]
        public string userId { get; set; }
        public User user { get; set; }

    }
}
