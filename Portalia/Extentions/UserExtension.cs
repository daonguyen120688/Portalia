using System;
using System.Collections.Generic;
using System.Linq;
using Portalia.Core.Dtos.User;
using Portalia.ViewModels.Users;

namespace Portalia.Extentions
{
    public static class UserExtension
    {
        public static PagingUserItemViewModel ToPagingUserItemViewModel(this PagingUserItemDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new PagingUserItemViewModel
            {
                IsEmployee = dto.IsEmployee,
                Email = dto.Email,
                Id = dto.Id,
                IsActive = dto.IsActive,
                FullName = dto.FullName,
                PercentOfInfo = dto.PercentOfInfo,
                CreatedDate = dto.Created.ToStringWithSpecificFormat(),
                IsNew = dto.IsNew,
                PortaliaUserProfileId = dto.PortaliaUserProfileId,
                WorkContractStatusId = dto.WorkContractStatusId,
                WorkContractId = dto.WorkContractId,
                DocumentId = dto.DocumentId ?? 0
            };
        }

        public static IList<PagingUserItemViewModel> ToPagingUserItemViewModels(this IList<PagingUserItemDto> dtos)
        {
            return dtos?.Select(s => s.ToPagingUserItemViewModel()).ToList() ?? new List<PagingUserItemViewModel>();
        }

        public static PagingUserViewModel ToPagingUserViewModel(this PagingUserDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new PagingUserViewModel
            {
                TotalPages = dto.TotalPages,
                CurrentPage = dto.CurrentPage,
                Users = dto.Users.ToPagingUserItemViewModels()
            };
        }

        public static FilterUserDto ToFilterUserDto(this FilterUserViewModel viewModel)
        {
            if (viewModel == null)
            {
                return null;
            }

            return new FilterUserDto
            {
                PageNumber = viewModel.PageNumber,
                PageSize = viewModel.PageSize,
                IsEmployee = viewModel.IsEmployee,
                SearchUserNameQuery = !String.IsNullOrEmpty(viewModel.SearchUserNameQuery)
                    ? viewModel.SearchUserNameQuery.Trim().ToLower()
                    : String.Empty,
                Status = viewModel.Status
            };
        }
    }
}