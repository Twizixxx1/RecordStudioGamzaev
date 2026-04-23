namespace RecordStudioGamzaev.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int StudioId { get; set; }
        public Studio? Studio { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Price { get; set; }

    }
}
