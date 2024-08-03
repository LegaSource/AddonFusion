using System;

namespace AddonFusion.Behaviours
{
    internal class ProtectiveCord : AddonProp
    {
        protected override Type AddonType => typeof(Shovel);
        protected override string ToolTip => "Parry : [Q]"; // En dur car non géré dans le jeu de base donc je ne me casse pas la tête
    }
}
