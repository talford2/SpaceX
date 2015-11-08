using UnityEngine;
using System.Collections;

public class Fighter1Idle : NpcState<Fighter1>
{
    public Fighter1Idle(Fighter1 npc) : base(npc)
    {
        Name = "Idle";
    }
}
