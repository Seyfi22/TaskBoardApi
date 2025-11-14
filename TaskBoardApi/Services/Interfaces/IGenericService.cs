namespace TaskBoardApi.Services.Interfaces
{
    public interface IGenericService<TDto>
    {
        Task<List<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);

        // As our post and put methods for User and Task are completely different we don't create Generic Post or Generic Put -- because of One-To-Many relation
    }
}
