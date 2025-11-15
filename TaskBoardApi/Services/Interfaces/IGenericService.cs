namespace TaskBoardApi.Services.Interfaces
{
    public interface IGenericService<TDto>
    {
        Task<List<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
