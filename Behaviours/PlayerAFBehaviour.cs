using GameNetcodeStuff;
using UnityEngine;

namespace AddonFusion.Behaviours
{
    internal class PlayerAFBehaviour : MonoBehaviour
    {
        // PROTECTIVE TANK
        public bool isParrying = false;
        public bool isParryOnCooldown = false;
        public EnemyAI parriedEnemy;
        public PlayerControllerB parriedPlayer;
        // SENZU
        public bool isRevivable = false;
    }
}
