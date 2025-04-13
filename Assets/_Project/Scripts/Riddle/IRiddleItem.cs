using System.Collections.Generic;

namespace AE.Riddle
{
    public interface IRiddleItem
    {
        string Condition { get; }
        string Key { get; }
        string Result { get; }

        IList<RiggleTrigger> Triggers { get; }
    }
}