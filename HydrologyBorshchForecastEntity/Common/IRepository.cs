using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydrologyBorshchForecastEntity.Common
{
    public interface IRepository<T>
    {
        void Save(T entity);
        void Update(T entity);
        void Delete(T entiy);
        T GetById(int id);
        IList<T> GetAll();
    }
}