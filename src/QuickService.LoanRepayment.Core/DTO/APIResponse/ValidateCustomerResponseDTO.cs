namespace QuickService.LoanRepayment.Core.DTO.APIResponse
{
    public class ValidateCustomerResponseDTO
    {
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string CustomerId { get; set; }
        public string AccountScheme { get; set; }
        public string AccountSchemeCode { get; set; }
        public string AccountSegment { get; set; }
    }
}