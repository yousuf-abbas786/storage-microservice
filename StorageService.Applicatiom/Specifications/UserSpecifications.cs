using StorageService.Domain.Entities;

namespace StorageService.Application.Specifications
{
    public class GetUserByUsernameAndTenantSpecification : BaseSpecification<User>
    {
        public GetUserByUsernameAndTenantSpecification(string username, string? tenantId)
        {
            Criteria = u => u.Username == username && u.TenantId == tenantId;
        }
    }

    public class GetUserByEmailSpecification : BaseSpecification<User>
    {
        public GetUserByEmailSpecification(string email)
        {
            Criteria = u => u.Email == email;
        }
    }
}

