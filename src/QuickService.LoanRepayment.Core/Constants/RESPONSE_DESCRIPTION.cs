namespace QuickService.LoanRepayment.Core.Constants
{
    public sealed class RESPONSE_DESCRIPTION
    {
        private RESPONSE_DESCRIPTION()
        { }

        public const string GENERAL_FAILURE = "Your request could not be processed at the moment, please try again later.";
        public const string INVALID_CUSTOMER = "Invalid customer details.";
        public const string DOC_BASE_64_PARSE_ERROR = "Issue document has a wrong format.";
        public const string INVALID_TRANSACTION_TYPE = "Invalid transaction type";
        public const string REQUEST_PROCESSED_SUCCESSFULLY = "Request processed successfully.";
        public const string REQUEST_LOGGED_SUCCESSFULLY = "Request logged successfully.";
        public const string OTP_VALIDATION_FAILED = "Otp validation failed";
        public const string OTP_VALIDATION_SUCCESSFUL = "Otp validation successful";
        public const string REQUEST_CREATION_SUCCESS = "Your request has been initiated successfully";
        public const string FINACLE_SUBMISSION_FAILURE = "Your Loan Repayment request was not completed, please try again later.";

        public const string INVALID_TRANSACTION_DATE = "Transaction date cannot be in the future.";
        public const string INVALID_AMOUNT = "Amount should be greater than 0.";
        public const string INVALID_REQUEST_DATE = "Invalid request date";
        public const string INVALID_TO_DATE_FROM_DATE = "From date must be earlier than To date.";
        public const string DATE_PARSING_FAILURE = "Request date could not be detected";
        public const string EMPTY_REJECTION_MESSAGE = "Rejection reason is missing";

        public const string INVALID_REQUEST = "Invalid request";
        public const string INVALID_DETAILS = "Invalid details";
        public const string UNAUTHORIZED_OPERATION = "You are not authorized to perform this operation";
        public const string INVALID_REQUEST_STATUS = "Invalid request status";
        public const string EMPTY_SAPID = "SapId is missing";
        public const string DOCUMENT_NOT_FOUND = "Document could not be found at the moment.";
        public const string ASSIGN_ERROR = "This request is already being processed or has been treated";

        public const string INVALID_BVN = "The BVN you entered did not match";
        public const string INVALID_DOB = "The Date of birth you entered did not match";
        public const string INVALID_BVN_AND_DOB = "The BVN and date of birth you entered did not match";
        public const string PHONE_VALIDATION_FAILED = "The Phone Number you entered did not match that on the Account";
    }
}