namespace Portalia.Core.Dtos.Message
{
    public class MessageDto
    {
        public bool HasError { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public static MessageDto GetErrorMessage(string message = "")
        {
            return new MessageDto
            {
                Message = string.IsNullOrEmpty(message) ? "Invalid data" : message,
                HasError = true,
                Data = null
            };
        }

        public static MessageDto GetSuccessMessage(string message = "", object data = null)
        {
            return new MessageDto
            {
                Message = string.IsNullOrEmpty(message) ? "Success" : message,
                HasError = false,
                Data = data
            };
        }
    }
}
