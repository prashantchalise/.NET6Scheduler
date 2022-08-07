using PROJBP.Model;
using Microsoft.EntityFrameworkCore;

namespace PROJBP.Service
{
	public interface IService
	{
	}

	public interface IEntityService<T> : IService
	where T : BaseEntity
	{
		int Create(T entity);
		int Delete(T entity);
		Task<IList<T>> GetAllAsync();
		int Update(T entity);
	}

	public abstract class EntityService<T> : IEntityService<T> where T : BaseEntity
	{
		protected IContext _context;
		protected DbSet<T> _dbset;

		public EntityService(IContext context)
		{
			_context = context;
			_dbset = _context.Set<T>();  

		}


		public virtual int Create(T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			_dbset.Add(entity);

			return _context.SaveChanges();
		}


		public virtual int Update(T entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");
			_context.Entry(entity).State = EntityState.Modified;
			return _context.SaveChanges();
		}

		public virtual int Delete(T entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");
			_dbset.Remove(entity);
			return _context.SaveChanges();
		}

        public virtual async Task<IList<T>> GetAllAsync()
        {
			return await _dbset.ToListAsync<T>(); 
        }
    }
}