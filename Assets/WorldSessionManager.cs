using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SA
{
    public class WorldSessionManager : MonoBehaviour
    {
        public static WorldSessionManager instance;

        [Header("Active Players In Session")]
        public List<PlayerManager> players = new List<PlayerManager>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddPlayerToActivePlayersList(PlayerManager player)
        {
            // 리스트에 플레이어가 없으면 추가
            if (!players.Contains(player))
            {
                players.Add(player);
            }

            // 플레이어가 null인 경우, 리스트에서 제거
            for (int i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }

        public void RemovePlayerFromActivePlayersList(PlayerManager player)
        {
            // 리스트에 플레이어가 있으면 제거
            if (players.Contains(player))
            {
                players.Remove(player);
            }

            // 플레이어가 null인 경우, 리스트에서 제거
            for (int i = players.Count; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }
    }
}