using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;

namespace MoreCompany
{
    [HarmonyPatch(typeof(HUDManager), "AddTextToChatOnServer")]
    public static class SendChatToServerPatch
    {
        public static bool Prefix(string chatMessage, int playerId = -1)
        {
            if (StartOfRound.Instance.IsHost)
            {
                // DEBUG COMMANDS
                if (chatMessage.StartsWith("/mc") && DebugCommandRegistry.commandEnabled)
                {
                    String command = chatMessage.Replace("/mc ", "");
                    DebugCommandRegistry.HandleCommand(command.Split(' '));
                    return false;
                }
            }

            return true;
        }
    }

    [HarmonyPatch]
    public static class ClientReceiveMessagePatch
    {
        [HarmonyPatch(typeof(HUDManager), "AddTextMessageClientRpc")]
        [HarmonyPrefix]
        public static bool AddTextMessageClientRpc_Prefix(HUDManager __instance, string chatMessage)
        {
            if (chatMessage.StartsWith("[morecompanycosmetics]"))
            {
                return false;
            }
            return true;
        }
    }


    [HarmonyPatch]
    public static class PreventOldVersionChatSpamPatch
    {
        [HarmonyPatch(typeof(HUDManager), "AddChatMessage")]
        [HarmonyPrefix]
        public static bool AddChatMessage_Prefix(string chatMessage, string nameOfUserWhoTyped = "")
        {
            if (chatMessage.StartsWith("[replacewithdata]") || chatMessage.StartsWith("[morecompanycosmetics]"))
            {
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(HUDManager), "AddPlayerChatMessageClientRpc")]
        [HarmonyPrefix]
        public static bool AddPlayerChatMessageClientRpc_Prefix(string chatMessage, int playerId)
        {
            if (chatMessage.StartsWith("[replacewithdata]") || chatMessage.StartsWith("[morecompanycosmetics]"))
            {
                return false;
            }

            return true;
        }
    }
}
