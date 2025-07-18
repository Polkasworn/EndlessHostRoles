﻿using System.Collections.Generic;
using AmongUs.GameOptions;
using EHR.Modules;
using Hazel;
using UnityEngine;
using static EHR.Options;

namespace EHR.Neutral;

public class Bandit : RoleBase
{
    private const int Id = 18420;
    public static bool On;

    private static OptionItem KillCooldown;
    private static OptionItem MaxSteals;
    private static OptionItem StealMode;
    private static OptionItem CanStealBetrayalAddon;
    private static OptionItem CanStealImpOnlyAddon;
    private static OptionItem CanVent;
    private static OptionItem CanSabotage;
    private static OptionItem HasImpostorVision;

    private static Dictionary<byte, int> TotalSteals = [];
    private static Dictionary<byte, Dictionary<byte, CustomRoles>> Targets = [];

    private static readonly string[] BanditStealModeOpt =
    [
        "BanditStealMode.OnMeeting",
        "BanditStealMode.Instantly"
    ];

    public override bool IsEnable => On;

    public override void SetupCustomOption()
    {
        SetupRoleOptions(Id, TabGroup.NeutralRoles, CustomRoles.Bandit);

        MaxSteals = new IntegerOptionItem(Id + 10, "BanditMaxSteals", new(1, 20, 1), 3, TabGroup.NeutralRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Bandit]);

        KillCooldown = new FloatOptionItem(Id + 11, "KillCooldown", new(0f, 180f, 0.5f), 22.5f, TabGroup.NeutralRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Bandit])
            .SetValueFormat(OptionFormat.Seconds);

        StealMode = new StringOptionItem(Id + 12, "BanditStealMode", BanditStealModeOpt, 0, TabGroup.NeutralRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Bandit]);

        CanStealBetrayalAddon = new BooleanOptionItem(Id + 13, "BanditCanStealBetrayalAddon", true, TabGroup.NeutralRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Bandit]);

        CanStealImpOnlyAddon = new BooleanOptionItem(Id + 14, "BanditCanStealImpOnlyAddon", true, TabGroup.NeutralRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Bandit]);

        CanVent = new BooleanOptionItem(Id + 16, "CanVent", true, TabGroup.NeutralRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Bandit]);

        CanSabotage = new BooleanOptionItem(Id + 15, "CanUseSabotage", true, TabGroup.NeutralRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Bandit]);

        HasImpostorVision = new BooleanOptionItem(Id + 17, "ImpostorVision", true, TabGroup.NeutralRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Bandit]);
    }

    public override void Init()
    {
        Targets = [];
        TotalSteals = [];
        On = false;
    }

    public override void Add(byte playerId)
    {
        On = true;
        TotalSteals[playerId] = 0;
        Targets[playerId] = [];
    }

    public override void ApplyGameOptions(IGameOptions opt, byte id)
    {
        opt.SetVision(HasImpostorVision.GetBool());
    }

    public override bool CanUseImpostorVentButton(PlayerControl pc)
    {
        return CanVent.GetBool();
    }

    public override bool CanUseSabotage(PlayerControl pc)
    {
        return base.CanUseSabotage(pc) || (CanSabotage.GetBool() && pc.IsAlive());
    }

    private static void SendRPC(byte playerId /*, bool isTargetList = false*/)
    {
        if (!On || !Utils.DoRPC) return;

        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBanditStealLimit, SendOption.Reliable);
        writer.Write(playerId);
        writer.Write(TotalSteals[playerId]);
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public static void ReceiveRPC(MessageReader reader)
    {
        byte PlayerId = reader.ReadByte();
        int Limit = reader.ReadInt32();
        if (!TotalSteals.TryAdd(PlayerId, 0)) TotalSteals[PlayerId] = Limit;
    }

    public override void SetKillCooldown(byte id)
    {
        Main.AllPlayerKillCooldown[id] = KillCooldown.GetFloat();
    }

    private static CustomRoles? SelectRandomAddon(PlayerControl Target)
    {
        if (!AmongUsClient.Instance.AmHost) return null;

        List<CustomRoles> AllSubRoles = Main.PlayerStates[Target.PlayerId].SubRoles;

        for (var i = 0; i < AllSubRoles.Count; i++)
        {
            CustomRoles role = AllSubRoles[i];

            if (IsBlacklistedAddon(role))
            {
                Logger.Info($"Removed {role} from stealable addons", "Bandit");
                AllSubRoles.Remove(role);
            }
        }

        if (AllSubRoles.Count == 0)
        {
            Logger.Info("No stealable addons found on the target.", "Bandit");
            return null;
        }

        CustomRoles addon = AllSubRoles.RandomElement();
        return addon;
    }

    private static bool IsBlacklistedAddon(CustomRoles role)
    {
        return (role.IsImpOnlyAddon() && !CanStealImpOnlyAddon.GetBool()) || (role.IsBetrayalAddon() && !CanStealBetrayalAddon.GetBool()) || StartGameHostPatch.BasisChangingAddons.ContainsKey(role) || role.IsNotAssignableMidGame();
    }

    public override bool OnCheckMurder(PlayerControl killer, PlayerControl target)
    {
        if (!On) return true;

        if (!target.HasSubRole()) return true;

        if (TotalSteals[killer.PlayerId] >= MaxSteals.GetInt())
        {
            Logger.Info("Max steals reached killing the player", "Bandit");
            TotalSteals[killer.PlayerId] = MaxSteals.GetInt();
            return true;
        }

        CustomRoles? SelectedAddOn = SelectRandomAddon(target);
        if (SelectedAddOn == null) return true; // no stealable addons found on the target.

        killer.ResetKillCooldown();

        return killer.CheckDoubleTrigger(target, () =>
        {
            if (StealMode.GetValue() == 1)
            {
                Main.PlayerStates[target.PlayerId].RemoveSubRole((CustomRoles)SelectedAddOn);
                Logger.Info($"Successfully removed {SelectedAddOn} addon from {target.GetNameWithRole().RemoveHtmlTags()}", "Bandit");
                killer.RpcSetCustomRole((CustomRoles)SelectedAddOn);
                Logger.Info($"Successfully Added {SelectedAddOn} addon to {killer.GetNameWithRole().RemoveHtmlTags()}", "Bandit");
            }
            else
            {
                Targets[killer.PlayerId][target.PlayerId] = (CustomRoles)SelectedAddOn;
                Logger.Info($"{killer.GetNameWithRole().RemoveHtmlTags()} will steal {SelectedAddOn} addon from {target.GetNameWithRole().RemoveHtmlTags()} after meeting starts", "Bandit");
            }

            TotalSteals[killer.PlayerId]++;
            SendRPC(killer.PlayerId);
            Utils.NotifyRoles(SpecifySeer: killer, SpecifyTarget: target, ForceLoop: true);
            Utils.NotifyRoles(SpecifySeer: target, SpecifyTarget: killer, ForceLoop: true);
            if (!DisableShieldAnimations.GetBool()) killer.RpcGuardAndKill(target);

            killer.ResetKillCooldown();
            killer.SetKillCooldown();
        });
    }

    public override void OnReportDeadBody()
    {
        if (!On) return;

        if (StealMode.GetValue() == 1) return;

        foreach (KeyValuePair<byte, Dictionary<byte, CustomRoles>> kvp1 in Targets)
        {
            byte banditId = kvp1.Key;
            PlayerControl banditpc = Utils.GetPlayerById(banditId);
            if (banditpc == null || !banditpc.IsAlive()) continue;

            Dictionary<byte, CustomRoles> innerDictionary = kvp1.Value;
            Utils.NotifyRoles(SpecifySeer: banditpc);

            foreach (KeyValuePair<byte, CustomRoles> kvp2 in innerDictionary)
            {
                byte targetId = kvp2.Key;
                PlayerControl target = Utils.GetPlayerById(banditId);
                if (target == null) continue;

                CustomRoles role = kvp2.Value;
                Main.PlayerStates[targetId].RemoveSubRole(role);
                Logger.Info($"Successfully removed {role} addon from {target.GetNameWithRole().RemoveHtmlTags()}", "Bandit");
                banditpc.RpcSetCustomRole(role);
                Logger.Info($"Successfully Added {role} addon to {banditpc.GetNameWithRole().RemoveHtmlTags()}", "Bandit");
                Utils.NotifyRoles(SpecifySeer: target, SpecifyTarget: banditpc);
            }

            Targets[banditId].Clear();
        }
    }

    public override string GetProgressText(byte playerId, bool comms)
    {
        return Utils.ColorString(TotalSteals[playerId] < MaxSteals.GetInt() ? Utils.GetRoleColor(CustomRoles.Bandit).ShadeColor(0.25f) : Color.gray, TotalSteals.TryGetValue(playerId, out int stealLimit) ? $"({MaxSteals.GetInt() - stealLimit})" : "Invalid");
    }
}