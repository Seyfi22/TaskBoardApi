using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Data;
using TaskBoardApi.Services.Interfaces;

namespace TaskBoardApi.Services.Implementations
{
    public class GenericService<TDto, TEntity> : IGenericService<TDto>
        where TEntity : class
    {
        private readonly TaskBoardDbContext _context;
        private readonly IMapper _mapper;
        private readonly DbSet<TEntity> _dbSet;

        public GenericService(TaskBoardDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _dbSet = context.Set<TEntity>();
        }


        public async Task<List<TDto>> GetAllAsync()
        {
            return await _dbSet
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TDto> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if(entity == null)
            {
                return default;
            }

            return _mapper.Map<TDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if(entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
