namespace Menu.API.Persistence.Entities
{
    public class MenuImage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(2000)]
        public string Url { get; set; } = string.Empty;

        public Guid MenuId { get; set; }
        public Menu Menu { get; set; } = default!;
        public bool IsThumbnail { get; set; }
    }
}