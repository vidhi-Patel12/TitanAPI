namespace Internal_Portal.Models
{
    public class Career
    {
        public int CareerId { get; set; }
        public string Employementtype { get; set; }
        public string Location { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
