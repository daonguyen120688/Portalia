using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;

namespace Portalia.Service
{
    public abstract class BaseService<T> where T : class, IObjectState
    {
        protected readonly IRepositoryAsync<T> _Repository;
        protected readonly IUnitOfWorkAsync _unitOfWork;
        protected BaseService(IRepositoryAsync<T> repository, IUnitOfWorkAsync unitOfWork)
        {
            _Repository = repository;
            _unitOfWork = unitOfWork;
        }
    }
}
