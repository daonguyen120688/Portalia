using PagedList;

namespace Portalia.ViewModels
{
    public class PagingViewModel<T>
    {
        private int _currenPage;

        public int? CurrenPage
        {
            get { return _currenPage; }
            set
            {
                _currenPage = value ?? 1;
            }
        }
        public IPagedList<T> Models { get; set; }
    }
}