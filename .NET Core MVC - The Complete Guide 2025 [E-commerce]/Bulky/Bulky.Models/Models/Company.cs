using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama perusahaan tidak boleh kosong.")]
        [Display(Name = "Nama Perusahaan")]
        public string Name { get; set; }

        [Display(Name = "Alamat Jalan")]
        public string? StreetAddress { get; set; }

        [Display(Name = "Kota")]
        public string? City { get; set; }

        [Display(Name = "Provinsi")]
        public string? State { get; set; }

        [Display(Name = "Kode Pos")]
        public string? PostalCode { get; set; }

        [Display(Name = "Nomor Telepon")]
        public string? PhoneNumber { get; set; }
    }
}