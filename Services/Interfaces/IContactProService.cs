namespace contactPro2.Services.Interfaces
{
    public interface IContactProService
    {

        public Task AddCategoriesToContactAsync(IEnumerable<int> categoryIds, int contactId);
        public Task RemoveCategoriesFromContactAsync(int contactId);

    }
}   