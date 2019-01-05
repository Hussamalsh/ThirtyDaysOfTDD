using System;

namespace TDDlearn2.Store
{
    public interface ICustomerService
    {
         Customer GetCustomer(Guid customerId);
    }
}
