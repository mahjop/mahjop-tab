namespace mahjop.Models
{
    public class Lab
    {
        public int LabId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; } // التقييم
        public byte[] LabPicture { get; set; }
        public bool IsActive { get; set; }
        public string UserId { get; set; }
    }
}
