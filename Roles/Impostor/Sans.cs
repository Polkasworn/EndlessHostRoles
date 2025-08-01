﻿using System;
using System.Collections.Generic;
using AmongUs.GameOptions;
using EHR.Modules;
using EHR.Neutral;
using Hazel;
using static EHR.Options;

namespace EHR.Impostor;

public class Sans : RoleBase
{
    private const int Id = 600;
    public static List<byte> PlayerIdList = [];

    private static OptionItem DefaultKillCooldown;
    private static OptionItem ReduceKillCooldown;
    private static OptionItem MinKillCooldown;
    private static OptionItem ShowProgressText;
    public static OptionItem BardChance;

    private bool CanVent;
    private float DefaultKCD;
    private bool HasImpostorVision;
    private bool ShowProgressTxt;
    private float MinKCD;

    private float NowCooldown;
    private float ReduceKCD;
    private bool ResetKCDOnMeeting;
    private byte SansID;

    private CustomRoles UsedRole;

    public override bool IsEnable => PlayerIdList.Count > 0;

    public override void SetupCustomOption()
    {
        SetupRoleOptions(Id, TabGroup.ImpostorRoles, CustomRoles.Sans);

        DefaultKillCooldown = new FloatOptionItem(Id + 10, "SansDefaultKillCooldown", new(0f, 180f, 0.5f), 30f, TabGroup.ImpostorRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Sans])
            .SetValueFormat(OptionFormat.Seconds);

        ReduceKillCooldown = new FloatOptionItem(Id + 11, "SansReduceKillCooldown", new(0f, 30f, 0.5f), 3.5f, TabGroup.ImpostorRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Sans])
            .SetValueFormat(OptionFormat.Seconds);

        MinKillCooldown = new FloatOptionItem(Id + 12, "SansMinKillCooldown", new(0f, 30f, 0.5f), 10f, TabGroup.ImpostorRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Sans])
            .SetValueFormat(OptionFormat.Seconds);

        ShowProgressText = new BooleanOptionItem(Id + 13, "SansShowProgressText", true, TabGroup.ImpostorRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Sans]);
            
        BardChance = new IntegerOptionItem(Id + 14, "BardChance", new(0, 100, 5), 0, TabGroup.ImpostorRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Sans])
            .SetValueFormat(OptionFormat.Percent);
    }

    public override void Init()
    {
        PlayerIdList = [];
    }

    public override void Add(byte playerId)
    {
        PlayerIdList.Add(playerId);

        SansID = playerId;
        UsedRole = Main.PlayerStates[playerId].MainRole;

        switch (UsedRole)
        {
            case CustomRoles.Sans:
                DefaultKCD = DefaultKillCooldown.GetFloat();
                ReduceKCD = ReduceKillCooldown.GetFloat();
                MinKCD = MinKillCooldown.GetFloat();
                ShowProgressTxt = ShowProgressText.GetBool();
                ResetKCDOnMeeting = false;
                HasImpostorVision = true;
                CanVent = true;
                break;
            case CustomRoles.Juggernaut:
                DefaultKCD = Juggernaut.DefaultKillCooldown.GetFloat();
                ReduceKCD = Juggernaut.ReduceKillCooldown.GetFloat();
                MinKCD = Juggernaut.MinKillCooldown.GetFloat();
                ShowProgressTxt = Juggernaut.ShowProgressText.GetBool();
                ResetKCDOnMeeting = false;
                HasImpostorVision = Juggernaut.HasImpostorVision.GetBool();
                CanVent = Juggernaut.CanVent.GetBool();
                break;
            case CustomRoles.Reckless:
                DefaultKCD = Reckless.DefaultKillCooldown.GetFloat();
                ReduceKCD = Reckless.ReduceKillCooldown.GetFloat();
                MinKCD = Reckless.MinKillCooldown.GetFloat();
                ShowProgressTxt = Reckless.ShowProgressText.GetBool();
                ResetKCDOnMeeting = true;
                HasImpostorVision = Reckless.HasImpostorVision.GetBool();
                CanVent = Reckless.CanVent.GetBool();
                break;
        }

        NowCooldown = DefaultKCD;
    }

    public override void Remove(byte playerId)
    {
        PlayerIdList.Remove(playerId);
    }

    public override void SetKillCooldown(byte id)
    {
        Main.AllPlayerKillCooldown[id] = NowCooldown;
    }

    public override void ApplyGameOptions(IGameOptions opt, byte playerId)
    {
        opt.SetVision(HasImpostorVision);
    }

    public override bool CanUseImpostorVentButton(PlayerControl pc)
    {
        return CanVent;
    }

    public override void OnMurder(PlayerControl killer, PlayerControl target)
    {
        NowCooldown = Math.Clamp(NowCooldown - ReduceKCD, MinKCD, DefaultKCD);
        Utils.SendRPC(CustomRPC.SyncRoleData, SansID, NowCooldown);
        killer?.ResetKillCooldown();
        killer?.SyncSettings();
    }

    public override void OnReportDeadBody()
    {
        if (!ResetKCDOnMeeting) return;

        NowCooldown = DefaultKCD;
        Utils.SendRPC(CustomRPC.SyncRoleData, SansID, NowCooldown);
    }

    public override string GetProgressText(byte playerId, bool comms)
    {
        if (!ShowProgressTxt) return base.GetProgressText(playerId, comms);
        
        double reduction = Math.Round(DefaultKCD - NowCooldown, 1);
        string nowKCD = string.Format(Translator.GetString("KCD"), Math.Round(NowCooldown, 1));
        return $"{base.GetProgressText(playerId, comms)} <#ffffff>-</color> {nowKCD} <#8B0000>(-{reduction}s)</color>";
    }

    public void ReceiveRPC(MessageReader reader)
    {
        NowCooldown = reader.ReadSingle();
    }
}
