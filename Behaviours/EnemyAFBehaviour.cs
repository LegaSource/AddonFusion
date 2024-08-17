using GameNetcodeStuff;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AddonFusion.Behaviours
{
    internal class EnemyAFBehaviour : MonoBehaviour
    {
        public PlayerControllerB playerHitBy;
        // WEED KILLER
        public int pyrethrinTankBehaviourIndex = -1;
        public bool isPyrethrinTankActive = false;
        // SPRAY TANK
        public bool isSaltImmuned = false;
        public int chanceToSwitchHauntingPlayer;

        public void PyrethrinTankBehaviour(EnemyAI enemyAI)
        {
            Transform transform = enemyAI.ChooseFarthestNodeFromPosition(playerHitBy.transform.position, false, 0, false, 50, false);
            if (transform != null)
            {
                isPyrethrinTankActive = true;
                enemyAI.ChangeOwnershipOfEnemy(playerHitBy.actualClientId);
                StopAggressiveAI(enemyAI);
                enemyAI.SetDestinationToPosition(transform.position, true);
                StartCoroutine(EndBehaviourCoroutine(enemyAI, AddonFusion.pyrethrinTankValues.FirstOrDefault(e => e.EntityName.Equals(enemyAI.enemyType.enemyName)).FleeDuration));
            }
        }

        public void StopAggressiveAI(EnemyAI enemyAI)
        {
            enemyAI.targetPlayer = null;
            if (enemyAI is RedLocustBees redLocustBees)
            {
                redLocustBees.wasInChase = false;
                redLocustBees.StopSearch(redLocustBees.searchForHive);
            }
            else if (enemyAI is HoarderBugAI hoarderBugAI)
            {
                hoarderBugAI.isAngry = true;
                hoarderBugAI.StopSearch(hoarderBugAI.searchForPlayer);
            }
            else if (enemyAI is ButlerBeesEnemyAI butlerBeesEnemyAI)
            {
                butlerBeesEnemyAI.StopSearch(butlerBeesEnemyAI.searchForPlayers);
            }
            enemyAI.moveTowardsDestination = true;
            enemyAI.movingTowardsTargetPlayer = false;
        }

        private IEnumerator EndBehaviourCoroutine(EnemyAI enemyAI, float duration)
        {
            yield return new WaitForSeconds(duration);
            isPyrethrinTankActive = false;
            enemyAI.ChangeOwnershipOfEnemy(playerHitBy.actualClientId);
            enemyAI.SetMovingTowardsTargetPlayer(playerHitBy);
            playerHitBy = null;
            if (enemyAI is SandSpiderAI sandSpiderAI)
            {
                sandSpiderAI.ChasePlayer(enemyAI.targetPlayer);
            }
            else if (enemyAI is CentipedeAI centipedeAI)
            {
                centipedeAI.chaseTimer = 0f;
                enemyAI.SwitchToBehaviourState(2);
            }
            else if (enemyAI is RedLocustBees redLocustBees)
            {
                redLocustBees.wasInChase = true;
                redLocustBees.lastSeenPlayerPos = enemyAI.targetPlayer.transform.position;
                redLocustBees.lostLOSTimer = 0f;
                redLocustBees.StartSearch(redLocustBees.lastSeenPlayerPos, redLocustBees.searchForHive);
                redLocustBees.SwitchToBehaviourState(2);
            }
            else if (enemyAI is HoarderBugAI hoarderBugAI)
            {
                hoarderBugAI.angryTimer = 4f;
                hoarderBugAI.angryAtPlayer = enemyAI.targetPlayer;
                hoarderBugAI.SwitchToBehaviourState(2);
            }
            else if (enemyAI is ButlerBeesEnemyAI butlerBeesEnemyAI)
            {
                butlerBeesEnemyAI.chasePlayerTimer = 0f;
                butlerBeesEnemyAI.StartSearch(enemyAI.targetPlayer.transform.position, butlerBeesEnemyAI.searchForPlayers);
                butlerBeesEnemyAI.SwitchToBehaviourState(-1);
            }
            else
            {
                enemyAI.SwitchToBehaviourState(0);
            }
        }
    }
}
