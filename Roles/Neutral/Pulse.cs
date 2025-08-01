﻿namespace EHR.Neutral;

public class Pulse : RoleBase
{
    private const int Id = 11950;

    public static OptionItem OddKillCooldown;
    public static OptionItem EvenKillCooldown;
    public static OptionItem AfterMeetingKillCooldown;
    public static OptionItem CanVent;
    public static OptionItem HasImpostorVision;

    public override bool IsEnable => false;

    public override void SetupCustomOption()
    {
        Options.SetupRoleOptions(Id, TabGroup.NeutralRoles, CustomRoles.Pulse);

        OddKillCooldown = new FloatOptionItem(Id + 10, "OddKillCooldown", new(0f, 120f, 0.5f), 27.5f, TabGroup.NeutralRoles)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.Pulse])
            .SetValueFormat(OptionFormat.Seconds);

        EvenKillCooldown = new FloatOptionItem(Id + 11, "EvenKillCooldown", new(0f, 120f, 0.5f), 15f, TabGroup.NeutralRoles)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.Pulse])
            .SetValueFormat(OptionFormat.Seconds);

        AfterMeetingKillCooldown = new FloatOptionItem(Id + 12, "AfterMeetingKillCooldown", new(0f, 120f, 0.5f), 22.5f, TabGroup.NeutralRoles)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.Pulse])
            .SetValueFormat(OptionFormat.Seconds);

        CanVent = new BooleanOptionItem(Id + 13, "CanVent", true, TabGroup.NeutralRoles)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.Pulse]);

        HasImpostorVision = new BooleanOptionItem(Id + 14, "ImpostorVision", true, TabGroup.NeutralRoles)
            .SetParent(Options.CustomRoleSpawnChances[CustomRoles.Pulse]);
    }

    public override void Init() { }

    public override void Add(byte playerId) { }
}