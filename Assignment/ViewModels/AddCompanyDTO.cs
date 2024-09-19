using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Assignment.ViewModels
{
    public class AddCompanyDTO
    {
        [Required]
        public string Name { get; set; } = "";
        [Required]
        [JsonRequired]
        public int TotalEmployee { get; set; }
        [Required]
        [JsonRequired]
        public DateTime EstablishedOn { get; set; }
    }
}
