using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using EHR.Modules;
using Hazel;
using static EHR.Options;

namespace EHR.Neutral;

public class HeadHunter : RoleBase
{
    private const int Id = 12870;
    public static List<byte> PlayerIdList = [];

    private static OptionItem KillCooldown;
    public static OptionItem CanVent;
    private static OptionItem HasImpostorVision;
    private static OptionItem SuccessKillCooldown;
    private static OptionItem FailureKillCooldown;
    private static OptionItem NumOfTargets;
    private static OptionItem MinKCD;
    private static OptionItem MaxKCD;
    private byte HeadHunterId;
    public float KCD = AdjustedDefaultKillCooldown;

    public List<byte> Targets = [];

    public override bool IsEnable => PlayerIdList.Count > 0;

    public override void SetupCustomOption()
    {
        SetupRoleOptions(Id, TabGroup.NeutralRoles, CustomRoles.HeadHunter);

        KillCooldown = new FloatOptionItem(Id + 10, "KillCooldown", new(0f, 180f, 0.5f), 27.5f, TabGroup.NeutralRoles).SetParent(CustomRoleSpawnChances[CustomRoles.HeadHunter])
            .SetValueFormat(OptionFormat.Seconds);

        SuccessKillCooldown = new FloatOptionItem(Id + 11, "HHSuccessKCDDecrease", new(0f, 180f, 0.5f), 3f, TabGroup.NeutralRoles).SetParent(CustomRoleSpawnChances[CustomRoles.HeadHunter])
            .SetValueFormat(OptionFormat.Seconds);

        FailureKillCooldown = new FloatOptionItem(Id + 12, "HHFailureKCDIncrease", new(0f, 180f, 0.5f), 10f, TabGroup.NeutralRoles).SetParent(CustomRoleSpawnChances[CustomRoles.HeadHunter])
            .SetValueFormat(OptionFormat.Seconds);

        CanVent = new BooleanOptionItem(Id + 13, "CanVent", true, TabGroup.NeutralRoles).SetParent(CustomRoleSpawnChances[CustomRoles.HeadHunter]);
        HasImpostorVision = new BooleanOptionItem(Id + 14, "ImpostorVision", true, TabGroup.NeutralRoles).SetParent(CustomRoleSpawnChances[CustomRoles.HeadHunter]);

        NumOfTargets = new IntegerOptionItem(Id + 15, "HHNumOfTargets", new(0, 10, 1), 3, TabGroup.NeutralRoles)
            .SetParent(CustomRoleSpawnChances[CustomRoles.HeadHunter])
            .SetValueFormat(OptionFormat.Times);

        MaxKCD = new FloatOptionItem(Id + 16, "HHMaxKCD", new(0f, 180f, 0.5f), 40f, TabGroup.NeutralRoles).SetParent(CustomRoleSpawnChances[CustomRoles.HeadHunter])
            .SetValueFormat(OptionFormat.Seconds);

        MinKCD = new FloatOptionItem(Id + 17, "HHMinKCD", new(0f, 180f, 0.5f), 10f, TabGroup.NeutralRoles).SetParent(CustomRoleSpawnChances[CustomRoles.HeadHunter])
            .SetValueFormat(OptionFormat.Seconds);
    }

    public override void Init()
    {
        PlayerIdList = [];
        Targets = [];
        HeadHunterId = byte.MaxValue;
    }

    public override void Add(byte playerId)
    {
        PlayerIdList.Add(playerId);
        HeadHunterId = playerId;
        Targets = [];
        LateTask.New(ResetTargets, 8f, log: false);
        KCD = KillCooldown.GetFloat();
    }

    public override void Remove(byte playerId)
    {
        PlayerIdList.Remove(playerId);
    }

    private void SendRPC()
    {
        MessageWriter writer = Utils.CreateRPC(CustomRPC.SyncHeadHunter);
        writer.Write(HeadHunterId);
        writer.Write(Targets.Count);
        foreach (byte target in Targets.ToArray()) writer.Write(target);

        Utils.EndRPC(writer);
    }

    public static void ReceiveRPC(MessageReader reader)
    {
        byte playerId = reader.ReadByte();
        if (Main.PlayerStates[playerId].Role is not HeadHunter hh) return;

        hh.Targets.Clear();
        int count = reader.ReadInt32();
        for (var i = 0; i < count; i++) hh.Targets.Add(reader.ReadByte());
    }

    public override void ApplyGameOptions(IGameOptions opt, byte id)
    {
        opt.SetVision(HasImpostorVision.GetBool());
    }

    public override bool CanUseImpostorVentButton(PlayerControl pc)
    {
        return CanVent.GetBool();
    }

    public override void SetKillCooldown(byte id)
    {
        Main.AllPlayerKillCooldown[id] = KCD;
    }

    public override void OnReportDeadBody()
    {
        ResetTargets();
    }

    public override bool OnCheckMurder(PlayerControl killer, PlayerControl target)
    {
        float tempkcd = KCD;

        if (Targets.Contains(target.PlayerId))
            Math.Clamp(KCD -= SuccessKillCooldown.GetFloat(), MinKCD.GetFloat(), MaxKCD.GetFloat());
        else
            Math.Clamp(KCD += FailureKillCooldown.GetFloat(), MinKCD.GetFloat(), MaxKCD.GetFloat());

        if (Math.Abs(KCD - tempkcd) > 0.1f)
        {
            killer.ResetKillCooldown();
            killer.SyncSettings();
        }

        return true;
    }

    public override void OnMurder(PlayerControl killer, PlayerControl target)
    {
        killer.SetKillCooldown(KCD);
    }

    public override string GetSuffix(PlayerControl seer, PlayerControl target, bool hud = false, bool meeting = false)
    {
        if (!hud) return string.Empty;

        byte targetId = seer.PlayerId;
        var output = string.Empty;
        if (Main.PlayerStates[targetId].Role is not HeadHunter hh) return output;

        for (var i = 0; i < hh.Targets.Count; i++)
        {
            byte playerId = hh.Targets[i];
            if (i != 0) output += ", ";

            output += playerId.ColoredPlayerName();
        }

        return targetId != 0xff ? $"<color=#00ffa5>Targets:</color> <b>{output}</b>" : string.Empty;
    }

    private void ResetTargets()
    {
        if (!AmongUsClient.Instance.AmHost) return;

        Targets.Clear();

        for (var i = 0; i < NumOfTargets.GetInt(); i++)
        {
            try
            {
                var cTargets = new List<PlayerControl>(Main.AllAlivePlayerControls.Where(pc => !Targets.Contains(pc.PlayerId) && pc.GetCustomRole() != CustomRoles.HeadHunter));
                if (cTargets.Count == 0) break;

                PlayerControl target = cTargets.RandomElement();
                Targets.Add(target.PlayerId);
            }
            catch (Exception ex)
            {
                Logger.Warn($"Not enough targets for Head Hunter could be assigned. This may be due to a low player count or the following error:\n\n{ex}", "HeadHunterAssignTargets");
                SendRPC();
                Utils.NotifyRoles(SpecifySeer: Utils.GetPlayerById(HeadHunterId));
                break;
            }
        }

        SendRPC();
        Utils.NotifyRoles(SpecifySeer: Utils.GetPlayerById(HeadHunterId));
    }
}