using System.Collections;

namespace demo_netcore_mvc.Models
{
    public class CrudModel<T>
    {
        public string Action { get; set; }
        public string Table { get; set; }
        public string KeyColumn { get; set; }
        public object Key { get; set; }
        public T Value { get; set; }
        public List<T> Added { get; set; }
        public List<T> Changed { get; set; }
        public List<T> Deleted { get; set; }
        public IDictionary Params { get; set; }
    }
}
