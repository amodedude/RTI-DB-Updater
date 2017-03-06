using RTI.DataBase.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RTI.DataBase.Interfaces.Repositories
{
    public interface ICustomerWaterRepository
    {
        IEnumerable<customer_water> GetAllCustomerWatersWithSouce(source source);
        void Remove(customer_water customerWater);
    }
}
