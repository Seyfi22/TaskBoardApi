using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Data;
using TaskBoardApi.Exceptions;
using TaskBoardApi.Services.Interfaces;

namespace TaskBoardApi.Services.Implementations
{
    public class GenericService<TDto, TEntity> : IGenericService<TDto>
        where TEntity : class
    {
        protected readonly TaskBoardDbContext _context;
        protected readonly IMapper _mapper;
        private readonly DbSet<TEntity> _dbSet;

        public GenericService(TaskBoardDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<List<TDto>> GetAllAsync()
        {
            return await _dbSet
                .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public virtual async Task<TDto> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if(entity == null)
            {
                throw new NotFoundException($"{typeof(TEntity).Name} with id {id} not found.");
            }

            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if(entity == null)
            {
                throw new NotFoundException($"{typeof(TEntity).Name} with id {id} not found.");
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
