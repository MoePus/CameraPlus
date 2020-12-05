using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using LogLevel = IPA.Logging.Logger.Level;

namespace CameraPlus
{
    public static class MultiplayerSession
    {
        public static MultiplayerSessionManager SessionManager { get; private set; }
        public static List<IConnectedPlayer> connectedPlayers;
        public static bool ConnectedMultiplay;
        public static MultiplayerPlayersManager playersManager = null;
        public static MultiplayerLobbyController LobbyContoroller = null;
        public static List<Transform> LobbyAvatarPlace;

        public static void Init()
        {
            connectedPlayers = new List<IConnectedPlayer>();
            LobbyAvatarPlace = new List<Transform>();
            ConnectedMultiplay = false;
            SessionManager = Resources.FindObjectsOfTypeAll<MultiplayerSessionManager>().FirstOrDefault();
            if (SessionManager == null)
                Logger.Log($"Unable to get MultiplayerSessionManager", LogLevel.Error);
            else
            {
                Logger.Log($"Success Find SessionManager", LogLevel.Info);

                SessionManager.connectedEvent += OnSessionConnected;
                SessionManager.disconnectedEvent += OnSessionDisconnected;
                SessionManager.playerConnectedEvent += OnSessionPlayerConnected;
                SessionManager.playerDisconnectedEvent += OnSessionPlayerDisconnected;
            }
        }

        public static void Close()
        {
            ConnectedMultiplay = false;
            if (SessionManager != null)
            {
                SessionManager.connectedEvent -= OnSessionConnected;
                SessionManager.disconnectedEvent -= OnSessionDisconnected;
                SessionManager.playerConnectedEvent -= OnSessionPlayerConnected;
                SessionManager.playerDisconnectedEvent -= OnSessionPlayerDisconnected;
            }
        }

        private static void OnSessionConnected()
        {
            ConnectedMultiplay = true;
            connectedPlayers.Clear();
            playersManager = Resources.FindObjectsOfTypeAll<MultiplayerPlayersManager>().FirstOrDefault();
            Logger.Log($"SessionManager Connected", LogLevel.Info);

            LobbyContoroller = Resources.FindObjectsOfTypeAll<MultiplayerLobbyController>().FirstOrDefault();
            if (LobbyContoroller == null)
                Logger.Log($"Unable to get LobbyContoroller", LogLevel.Error);
            connectedPlayers.Add(SessionManager.localPlayer);
            Logger.Log($"ConnectedPlayer---------------", LogLevel.Info);
            for (int i = 0; i < connectedPlayers.Count; i++)
                Logger.Log($"ConnectedPlayer {connectedPlayers[i].userName},{connectedPlayers[i].sortIndex}", LogLevel.Info);
        }
        private static void OnSessionDisconnected(DisconnectedReason reason)
        {
            ConnectedMultiplay = false;
            connectedPlayers.Clear();
            LobbyAvatarPlace.Clear();
            Logger.Log($"SessionManager Disconnected {reason}", LogLevel.Info);
        }
        private static void OnSessionPlayerConnected(IConnectedPlayer player)
        {
            if (playersManager==null)
                playersManager = Resources.FindObjectsOfTypeAll<MultiplayerPlayersManager>().FirstOrDefault();

            connectedPlayers.Add(player);
            connectedPlayers = connectedPlayers.OrderBy(pl => pl.sortIndex)
                    .ToList();

            Logger.Log($"ConnectedPlayer---------------", LogLevel.Info);
            for (int i = 0; i < connectedPlayers.Count; i++)
                Logger.Log($"ConnectedPlayer {connectedPlayers[i].userName},{connectedPlayers[i].sortIndex}", LogLevel.Info);
        }
        private static void OnSessionPlayerDisconnected(IConnectedPlayer player)
        {
            foreach (IConnectedPlayer p in connectedPlayers.ToArray())
            {
                if (p.userId == player.userId)
                {
                    connectedPlayers.Remove(p);
                    break;
                }
            }
            Logger.Log($"SessionManager PlayerDisconnected {player.userName},{player.sortIndex}", LogLevel.Info);
        }

        public static void LoadLobbyAvatarPlace()
        {
            try
            {
                Transform LobbyOffset;
                LobbyAvatarPlace.Clear();
                foreach (MultiplayerLobbyAvatarPlace multiLobbyAvatarPlace in Resources.FindObjectsOfTypeAll<MultiplayerLobbyAvatarPlace>())
                {
                    LobbyOffset = multiLobbyAvatarPlace.transform;
                    LobbyAvatarPlace.Add(LobbyOffset);
                }
                LobbyAvatarPlace.Select(tr => new { tr.position }).Distinct();
                LobbyAvatarPlace = LobbyAvatarPlace.OrderByDescending(tr => tr.position.x).ToList();
                for (int i = 0; i < LobbyAvatarPlace.Count ; i++)
                {
                    if (LobbyAvatarPlace[i].position == Vector3.zero)
                    {
                        Transform tr = LobbyAvatarPlace[i];
                        LobbyAvatarPlace.RemoveAt(i);
                        LobbyAvatarPlace.Insert(0, tr);
                        break;
                    }
                }
                List<Transform> Tr= ShiftLobbyPositionList(LocalPlayerSortIndex());
                if (Tr != null) LobbyAvatarPlace = Tr;
                else
                    Logger.Log($"LobbyAvatarPlace SortError", LogLevel.Info);
                
            }
            catch {
                Logger.Log($"Unable to LoadLobbyAvatarPlace", LogLevel.Error);
            }
        }
        public static int LocalPlayerSortIndex()
        {
            int result = 0;
            foreach (IConnectedPlayer player in connectedPlayers)
            {
                if (player.isMe)
                {
                    result = player.sortIndex;
                    break;
                }
            }
            return result;
        }
        public static List<Transform> ShiftLobbyPositionList(int shiftValue)
        {
            if (shiftValue < 0 || shiftValue >= LobbyAvatarPlace.Count) return null;

            List<Transform> result = LobbyAvatarPlace;
            for (int i=0; i < shiftValue ; i++)
            {
                result.Insert(0,result[result.Count-1]);
                result.RemoveAt(result.Count-1);
            }
            return result;
        }
    }
}
