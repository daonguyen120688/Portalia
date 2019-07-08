using System.Collections.Generic;

namespace Portalia.Core.Dtos.User
{
    public class PasswordValidationMessageDto
    {
        public bool HasError { get; set; }
        public List<string> Messages { get; set; }
    }
}
