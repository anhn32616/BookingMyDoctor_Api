namespace booking_my_doctor.DTOs
{
    public class ApiResponse
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}
