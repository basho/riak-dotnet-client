using CorrugatedIron.Messages;

namespace CorrugatedIron.Models.RiakDt
{
    public class SetOperation :IDtOp
    {
        public DtOp ToDtOp()
        {
            return new DtOp
            {
                set_op = new SetOp()
            };
        }
    }

    public class MapOperation :IDtOp
    {
        public DtOp ToDtOp()
        {
            return new DtOp
            {
                map_op = new MapOp()
            };
        }
    }
}
