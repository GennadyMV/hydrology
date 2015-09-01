using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HydrologyWeb.Models
{
    public class ViewExpense
    {
        public ViewExpense()
        {
            theList = new List<ViewExpenseEntity>();
        }
        public List<ViewExpenseEntity> theList;
        public void Add(string fileName)
        {
            ViewExpenseEntity item = new ViewExpenseEntity()
            item.Name = fileName;
            this.theList.Add(item);
        }
    }
    public class ViewExpenseEntity
    {
        public string Name;
    }
}