namespace DataTransferObjects
{
    public class QuestDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int? VolunteerID { get; set; }
        public int? DonationID { get; set; }
        public int? OrphanageID { get; set; }
        public int? DonationItemID { get; set; }
        public int? CharityMakerID { get; set; }
    }
}
