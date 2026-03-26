using UnityEngine;

namespace Prototype.Signals
{
    public readonly struct PlayerSpawnedSignal
    {
        public readonly GameObject Player;

        public PlayerSpawnedSignal(GameObject player)
        {
            Player = player;
        }
    }
}
