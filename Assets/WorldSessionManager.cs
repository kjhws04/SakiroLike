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
            // ����Ʈ�� �÷��̾ ������ �߰�
            if (!players.Contains(player))
            {
                players.Add(player);
            }

            // �÷��̾ null�� ���, ����Ʈ���� ����
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
            // ����Ʈ�� �÷��̾ ������ ����
            if (players.Contains(player))
            {
                players.Remove(player);
            }

            // �÷��̾ null�� ���, ����Ʈ���� ����
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