using HydrologyBorshchForecastEntity.Repositories;
using HydrologyBorshchForecastEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrologyBorshchForecastEntity.Models
{
    public class River
    {
        public River()
        {
            this.Gauge = "";
            ID = -1;
        }
        public virtual int ID { get; set; }
        public virtual DateTime created_at { get; set; }
        public virtual DateTime updated_at { get; set; }
        public virtual DateTime forecasted_at { get; set; }
        public virtual int HydroPostCode { get; set; }
        public virtual string Gauge { get; set; }
        public virtual int FloodPlaneMark { get; set; }
        public virtual int AdverseFact { get; set; }
        public virtual int DangerFact { get; set; }
        public virtual int Level_obs { get; set; }
        public virtual int Level_for1 { get; set; }
        public virtual int Level_for2 { get; set; }
        public virtual int Level_for3 { get; set; }
        public virtual int Level_for4 { get; set; }
        public virtual int Level_for5 { get; set; }
        public virtual int Level_for6 { get; set; }

        public virtual double Height { get; set; }

        public virtual void Save()
        {
            this.created_at = DateTime.Now;
            this.updated_at = DateTime.Now;
            IRepository<River> repo = new RiverRepository();
            repo.Save(this);
        }

        public virtual void Update()
        {
            this.updated_at = DateTime.Now;
            IRepository<River> repo = new RiverRepository();
            repo.Update(this);
        }
        public static River GetByDate(DateTime forecasted_at, int HydroPostCode)
        {
            RiverRepository repo = new RiverRepository();
            return repo.GetByDate(forecasted_at, HydroPostCode);
        }

    }
}
