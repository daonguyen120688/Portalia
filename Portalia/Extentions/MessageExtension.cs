using Portalia.Core.Dtos.Message;
using Portalia.ViewModels;

namespace Portalia.Extentions
{
    public static class MessageExtension
    {
        public static MessageViewModel ToMessageViewModel(this MessageDto dto)
        {
            if (dto == null)
            {
                return new MessageViewModel
                {
                    HasError = true,
                    Message = "Invalid model",
                    Data = null
                };
            }

            return new MessageViewModel
            {
                HasError = dto.HasError,
                Message = dto.Message,
                Data = dto.Data
            };
        }
    }
}