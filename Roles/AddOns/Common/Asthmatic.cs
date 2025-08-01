﻿using System.Collections.Generic;
using System.Linq;
using EHR.Modules;
using Hazel;
using UnityEngine;
using static EHR.Options;

namespace EHR.AddOns.Common;

internal class Asthmatic : IAddon
{
    private static readonly Dictionary<byte, Counter> Timers = [];
    private static readonly Dictionary<byte, string> LastSuffix = [];
    private static readonly Dictionary<byte, Vector2> LastPosition = [];
    private static int MinRedTime;
    private static int MaxRedTime;
    private static int MinGreenTime;
    private static int MaxGreenTime;

    public static bool RunChecks = true;
    public AddonTypes Type => AddonTypes.Harmful;

    public void SetupCustomOption()
    {
        SetupAdtRoleOptions(15419, CustomRoles.Asthmatic, canSetNum: true, teamSpawnOptions: true);

        AsthmaticMinRedTime = new IntegerOptionItem(15430, "AsthmaticMinRedTime", new(1, 90, 1), 5, TabGroup.Addons)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Asthmatic])
            .SetValueFormat(OptionFormat.Seconds);

        AsthmaticMaxRedTime = new IntegerOptionItem(15427, "AsthmaticMaxRedTime", new(1, 90, 1), 15, TabGroup.Addons)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Asthmatic])
            .SetValueFormat(OptionFormat.Seconds);

        AsthmaticMinGreenTime = new IntegerOptionItem(15428, "AsthmaticMinGreenTime", new(1, 90, 1), 5, TabGroup.Addons)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Asthmatic])
            .SetValueFormat(OptionFormat.Seconds);

        AsthmaticMaxGreenTime = new IntegerOptionItem(15429, "AsthmaticMaxGreenTime", new(1, 90, 1), 30, TabGroup.Addons)
            .SetParent(CustomRoleSpawnChances[CustomRoles.Asthmatic])
            .SetValueFormat(OptionFormat.Seconds);
    }

    private static int RandomRedTime(char _)
    {
        return IRandom.Instance.Next(MinRedTime, MaxRedTime);
    }

    private static int RandomGreenTime(char _)
    {
        return IRandom.Instance.Next(MinGreenTime, MaxGreenTime);
    }

    public static void Init()
    {
        Timers.Clear();
        LastSuffix.Clear();
        LastPosition.Clear();

        MinRedTime = AsthmaticMinRedTime.GetInt();
        MaxRedTime = AsthmaticMaxRedTime.GetInt();
        MinGreenTime = AsthmaticMinGreenTime.GetInt();
        MaxGreenTime = AsthmaticMaxGreenTime.GetInt();
    }

    public static void Add()
    {
        LateTask.New(() =>
        {
            var r = IRandom.Instance;
            long now = Utils.TimeStamp;

            foreach (PlayerControl pc in Main.AllAlivePlayerControls)
            {
                if (pc.Is(CustomRoles.Asthmatic))
                    Timers[pc.PlayerId] = new(30, r.Next(MinRedTime, MaxRedTime), now, '●', false, RandomRedTime, RandomGreenTime);
            }
        }, 8f, log: false);
    }

    public static void OnFixedUpdate()
    {
        foreach (KeyValuePair<byte, Counter> kvp in Timers.ToArray())
        {
            PlayerState state = Main.PlayerStates[kvp.Key];

            if (state.IsDead || !state.SubRoles.Contains(CustomRoles.Asthmatic))
            {
                state.RemoveSubRole(CustomRoles.Asthmatic);
                Timers.Remove(kvp.Key);
                LastSuffix.Remove(kvp.Key);
                LastPosition.Remove(kvp.Key);
                continue;
            }

            kvp.Value.Update();
        }
    }

    public static void OnCheckPlayerPosition(PlayerControl pc)
    {
        if (!Main.IntroDestroyed || !pc.Is(CustomRoles.Asthmatic) || ExileController.Instance || !RunChecks || !Timers.TryGetValue(pc.PlayerId, out Counter counter)) return;

        Vector2 currentPosition = pc.Pos();

        if (CheckInvalidMovementPatch.ExemptedPlayers.Remove(pc.PlayerId) && CheckInvalidMovementPatch.LastPosition.TryGetValue(pc.PlayerId, out Vector2 position))
            LastPosition[pc.PlayerId] = position;

        if (!LastPosition.TryGetValue(pc.PlayerId, out Vector2 previousPosition))
        {
            LastPosition[pc.PlayerId] = currentPosition;
            return;
        }

        currentPosition.x += previousPosition.x * Time.deltaTime;
        currentPosition.y += previousPosition.y * Time.deltaTime;

        Vector2 direction = currentPosition - previousPosition;

        direction.Normalize();

        float distanceX = currentPosition.x - previousPosition.x;
        float distanceY = currentPosition.y - previousPosition.y;

        const float limit = 2f;

        if (direction.y is > 0 or < 0 || direction.x is > 0 or < 0)
        {
            switch (counter.IsRed)
            {
                case true when distanceY > limit || distanceY < -limit || distanceX > limit || distanceX < -limit:
                    pc.Suicide(PlayerState.DeathReason.Asthma);
                    Main.PlayerStates[pc.PlayerId].RemoveSubRole(CustomRoles.Asthmatic);
                    Timers.Remove(pc.PlayerId);
                    LastSuffix.Remove(pc.PlayerId);
                    LastPosition.Remove(pc.PlayerId);
                    return;
                case false:
                    LastPosition[pc.PlayerId] = currentPosition;
                    break;
            }
        }

        string suffix = GetSuffixText(pc.PlayerId);

        if (!pc.IsModdedClient() && (!LastSuffix.TryGetValue(pc.PlayerId, out string beforeSuffix) || beforeSuffix != suffix))
        {
            Utils.NotifyRoles(SpecifySeer: pc, SpecifyTarget: pc);
            Utils.SendRPC(CustomRPC.SyncAsthmatic, pc.PlayerId, suffix);
        }

        LastSuffix[pc.PlayerId] = suffix;
    }

    public static void ReceiveRPC(MessageReader reader)
    {
        LastSuffix[reader.ReadByte()] = reader.ReadString();
    }

    public static string GetSuffixText(byte id)
    {
        if (Main.PlayerStates.TryGetValue(id, out var state) && !state.IsDead)
        {
            if (Timers.TryGetValue(id, out Counter counter))
                return $"{counter.ColoredArrow} <font=\"DIGITAL-7 SDF\" material=\"DIGITAL-7 Black Outline\">{counter.ColoredTimerString}</font>";

            if (id.IsPlayerModdedClient() && !id.IsHost() && LastSuffix.TryGetValue(id, out string lastSuffix))
                return lastSuffix;
        }

        return string.Empty;
    }
}