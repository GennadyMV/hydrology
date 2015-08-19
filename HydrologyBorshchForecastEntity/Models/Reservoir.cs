using GeospaceEntity.Repositories;
using HydrologyBorshchForecastEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrologyBorshchForecastEntity.Models
{
    public class Reservoir
    {
        public virtual int ID { get; set; }
        public virtual DateTime created_at { get; set; }
        public virtual DateTime updated_at { get; set; }
        public virtual DateTime forecasted_at { get; set; }        
        public virtual string MeteoModel { get; set; }
        public virtual double Inflow_obs_WB { get; set; }
        public virtual double Inflow_obs_HM { get; set; }
        public virtual double Inflow_for1 { get; set; }
        public virtual double Inflow_for2 { get; set; }
        public virtual double Inflow_for3 { get; set; }
        public virtual double Inflow_for4 { get; set; }
        public virtual double Inflow_for5 { get; set; }

        public virtual void Save()
        {
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            IRepository<Reservoir> repo = new ReservoirRepository();
            repo.Save(this);
        }

        public virtual void Update()
        {
            this.updated_at = DateTime.Now;
            IRepository<Reservoir> repo = new ReservoirRepository();
            repo.Update(this);
        }
        public static Reservoir GetByDate(DateTime forecasted_at, string MeteoModel)
        {
            ReservoirRepository repo = new ReservoirRepository();
            return repo.GetByDate(forecasted_at, MeteoModel);
        }



    }
}
