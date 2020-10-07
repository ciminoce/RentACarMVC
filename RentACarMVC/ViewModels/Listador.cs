using System.Collections.Generic;

namespace RentACarMVC.ViewModels
{
    public class Listador<T>:PaginadorGenerico where T:class
    {
        public IEnumerable<T> Registros { get; set; }
    }
}