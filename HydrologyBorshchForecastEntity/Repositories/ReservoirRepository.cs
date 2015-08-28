using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using HydrologyBorshchForecastEntity.Models;
using HydrologyBorshchForecastEntity.Common;

namespace HydrologyBorshchForecastEntity.Repositories
{
    public class ReservoirRepository : IRepository<Reservoir>
    {
        #region IRepository<Measurement> Members

        void IRepository<Reservoir>.Save(Reservoir entity)
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

        void IRepository<Reservoir>.Update(Reservoir entity)
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

        void IRepository<Reservoir>.Delete(Reservoir entity)
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

        Reservoir IRepository<Reservoir>.GetById(int id)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.CreateCriteria<Reservoir>().Add(Restrictions.Eq("ID", id)).UniqueResult<Reservoir>();
        }

        public Reservoir GetByDate(DateTime forecasted_at, string MeteoModel)
        {
            using (ISession session = NHibernateHelper.OpenSession())
                return session.CreateCriteria<Reservoir>().Add(Restrictions.Eq("forecasted_at", forecasted_at.Date)).Add(Restrictions.Eq("MeteoModel", MeteoModel)).UniqueResult<Reservoir>();
        }

        IList<Reservoir> IRepository<Reservoir>.GetAll()
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(Reservoir));
                criteria.AddOrder(Order.Desc("ID"));
                return criteria.List<Reservoir>();
            }
        }

        #endregion
    }

}
