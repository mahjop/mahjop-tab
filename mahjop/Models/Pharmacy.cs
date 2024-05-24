namespace mahjop.Models
{
    public class Pharmacy
    {
        public int PharmacyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; } // التقييم
        public byte[] PharmacyPicture { get; set; }
        public bool IsActive { get; set; }
        public string UserId { get; set; }
    }
}
