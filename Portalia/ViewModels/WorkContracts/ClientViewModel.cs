using System.Collections.Generic;

namespace Portalia.ViewModels.WorkContract
{
    public class ClientViewModel
    {
        public int EntityRelationshipId { get; set; }
        public string Name { get; set; }
        public List<int> AddressIds { get; set; }
        public List<ClientAddressViewModel> Offices { get; set; }
    }

    public class ClientAddressViewModel
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}