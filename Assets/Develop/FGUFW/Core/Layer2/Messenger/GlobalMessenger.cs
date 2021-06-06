
namespace FGUFW.Core
{
    static public class GlobalMessenger
    {
        static public IOrderedMessenger<object> M = new OrderedMessenger2<object>();
    }
}