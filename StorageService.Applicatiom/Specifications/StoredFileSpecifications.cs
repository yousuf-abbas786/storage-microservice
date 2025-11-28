using StorageService.Domain.Entities;

namespace StorageService.Application.Specifications
{
    public class GetAllFilesSpecification : BaseSpecification<StoredFile>
    {
        public GetAllFilesSpecification(int skip, int take)
        {
            Criteria = f => f.DeletedAt == null;
            AddOrderByDescending(f => f.CreatedAt);
            ApplyPaging(skip, take);
        }
    }

    public class SearchFilesByNameSpecification : BaseSpecification<StoredFile>
    {
        public SearchFilesByNameSpecification(string fileName, int skip, int take)
        {
            Criteria = f => f.DeletedAt == null && f.FileName.Contains(fileName);
            AddOrderByDescending(f => f.CreatedAt);
            ApplyPaging(skip, take);
        }
    }

    public class GetFileByIdSpecification : BaseSpecification<StoredFile>
    {
        public GetFileByIdSpecification(Guid id)
        {
            Criteria = f => f.Id == id && f.DeletedAt == null;
        }
    }
}

