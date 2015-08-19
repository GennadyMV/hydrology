using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using HydrologyBorshchForecastEntity.Models;
using HydrologyBorshchForecastEntity.Common;

namespace GeospaceEntity.Repositories
{
    public class RiverRepository : IRepository<River>
    {
        #region IRepository<Measurement> Members

        void IRepository<River>.Save(River entity)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(entity);
                    transaction.Commit();
                }
            }
        }

        void IRepository<River>.Update(River entity)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(entity);
                    transaction.Commit();
                }
            }
        }

        void IRepository<River>.Delete(River entity)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(entity);
                    transaction.Commit();
                }
            }
        }

        River IRepository<River>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.CreateCriteria<River>().Add(Restrictions.Eq("ID", id)).UniqueResult<River>();
        }

        public River GetByDate(DateTime forecasted_at, int HydroPostCode)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.CreateCriteria<River>().Add(Restrictions.Eq("forecasted_at", forecasted_at.Date)).Add(Restrictions.Eq("HydroPostCode", HydroPostCode)).UniqueResult<River>();
        }

        IList<River> IRepository<River>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(River));
                criteria.AddOrder(Order.Desc("ID"));
                return criteria.List<River>();
            }
        }
        
        #endregion
    }

}
