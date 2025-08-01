using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using AmongUs.Data;
using AmongUs.GameOptions;
using AmongUs.InnerNet.GameDataMessages;
using EHR.AddOns.Common;
using EHR.AddOns.Crewmate;
using EHR.AddOns.GhostRoles;
using EHR.AddOns.Impostor;
using EHR.Coven;
using EHR.Crewmate;
using EHR.Impostor;
using EHR.Modules;
using EHR.Neutral;
using EHR.Patches;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using InnerNet;
using Newtonsoft.Json;
using UnityEngine;
using static EHR.Translator;

namespace EHR;
/*
List of symbols that work in game

1-stars: ★ ☆ ⁂ ⁑ ✽
2-arrows: ☝ ☞ ☟ ☜ ↑ ↓ → ← ↔ ↕  ⬆ ↗ ➡ ↘ ⬇ ↙ ⬅ ↖ ↕ ↔  ⤴ ⤵ ￩ ￪ ￫ ￬ ⇦ ⇧ ⇨ ⇩ ⇵ ⇄ ⇅ ⇆  ↹
3- shapes • ○ ◦ ⦿ ▲ ▼ ♠ ♥ ♣ ♦ ♤ ♡ ♧ ♢ ■ □ ▢ ▣ ▤ ▥ ▦ ▧ ▨ ▩ ▪ ▫  ◌ ● ◐ ◑ ◒ ◓ ◯ ⦿ ◆ ◇ ◈ ❖  ▩  ▱ ▶ ◀
4- symbols: ✓ ∞ † ✚ ♫ ♪ ♲ ♳ ♴ ♵ ♶ ♷ ♸ ♹ ♺ ♻ ♼ ♽☎ ☏ ✂ ♀♂ ⚠
5- emojis:  😂☹️😆☺️😎😉😅😊😋😀😁😂😃😄😅😍😎😋😊😉😆☺️☁️☂️☀️
6- random: ‰ § ¶ © ™ ¥ $ ¢ € ƒ  £ Æ

other:  ∟ ⌠ ⌡ ╬ ╨ ▓ ▒ ░ « » █ ▄ ▌▀▐│ ┤ ╡ ╢ ╖ ╕ ╣ ║ ╗ ╝ ╜ ╛ ┐ └ ┴ ┬ ─ ┼ ╞ ╟ ╚ ╔ ╩ ╦ ╠ ═ ╬ ╧ ╨ ╤ ╥ ╙ ╘ ╒ ╓ ╫ ╪ ┘ ┌ Θ ∩ ¿

Copyright: © ® ™ ℡ № ℀ ℅ ☎ ☏ ‰ § ¶ ☜
Currency: ¢ $ € £ ¥ ₩ ₫ ￥ ¤ ƒ
Bracket: 〈 〉 《 》 「 」 『 』 【 】 〔 〕 ︵ ︶ ︷ ︸ ︹ ︺ ︻ ︼ ︽ ︾ ︿ ﹀ ﹁ ﹂ ﹃ ﹄ ﹙ ﹚ ﹛ ﹜ ﹝ ﹞ ﹤ ﹥ （ ） ＜ ＞ ｛ ｝ 〖 〗 〘 〙 〚 〛 « » ‹ › 〈 〉 〱
Card symbol: ♤ ♠ ♧ ♣ ♡ ♥ ♢ ♦
Musical: ♩ ♪ ♫ ♬ ♭ ♮ ♯ ° ø ≠
Degree: ° ℃ ℉ ☀ ☁ ☂ ☃ ☉ ♁ ♨ ㎎ ㎏ ㎜ ㎝ ㎞ ㎡ ㏄ ㏎ ㏑ ㏒ ㏕
Arrow: ↕ ↖ ↗ ↘ ↙ ↸ ↹ ⇦ ⇧ ⇨ ⇩ ⌅ ⌆ ⏎ ▶ ➔ ⤴ ⤵ ↓ ↔ ← → ↑ ⇵ ⬅ ⬆ ⬇
Astrological: ☯ ✚ †  ‡ ♁ ❖ 卍 卐   〷
Heart: ♥ ♡
Check: ✓ ∨ √ 〤 〥
Gender: ♀ ♂ ☹ ☺  〠 ヅ ツ ㋡ 웃 유 ü Ü シ ッ ㋛ ☃ 〲 〴
Punct: · ‑ ‒ – — ― ‘ ’ ‚ “ ” „  •  ‥ … ‧ ′ ″ ‵ ʻ ˇ ˉ ˊ ˋ ˙ ～ ¿ ﹐ ﹒ ﹔ ﹕ ！ ＃ ＄ ％ ＆ ＊ ， ． ： ； ？ ＠ 、 。 〃 〝 〞 ︰
Math: π ∞ Σ √  ∫ ∬ ∭ ∀  ∂ ∃  ∅ ∆ ∇ ∈ ∉ ∊ ∋ ∏  ∑ − ∓  ∕ ∝ ∟ ∠  ∣ ∥ ∦ ∧ ∨ ∩ ∪ ∴ ∵ ∶ ∷ ∽ ≃ ≅ ≈ ≌ ≒ ≠ ≡ ≢ ≤ ≥ ≦ ≧ ≪ ≫ ≮ ≯ ≲ ≳ ≶ ≷ ⊂ ⊃ ⊄ ⊅ ⊆ ⊇ ⊊ ⊋ ⊕ ⊖ ⊗ ⊘ ⊙ ⊠ ⊥ ⊿ ⋚ ⋛ ⋯ ﹢ ﹣ ＋ － ／ ＝ ÷ ±
Number: Ⅰ Ⅱ Ⅲ Ⅳ Ⅴ Ⅵ Ⅶ Ⅷ Ⅸ Ⅹ Ⅺ Ⅻ ⅰ ⅱ ⅲ ⅳ ⅴ ⅵ ⅶ ⅷ ⅸ ⅹ ⅺ ⅻ ➀ ➁ ➂ ➃ ➄ ➅ ➆ ➇ ➈ ➉ ➊ ➋ ➌ ➍ ➎ ➏ ➐ ➑ ➒ ➓ ⓵ ⓶ ⓷ ⓸ ⓹ ⓺ ⓻ ⓼ ⓽ ⓾ ⓿ ❶ ❷ ❸ ❹ ❺ ❻ ❼ ❽ ❾ ❿  ¹ ² ³ ⁴ ⓪ ① ② ③ ④ ⑤ ⑥ ⑦ ⑧ ⑨ ⑩ ⑪ ⑫ ⑬ ⑭ ⑮ ⑯ ⑰ ⑱ ⑲ ⑳ ⑴ ⑵ ⑶ ⑷ ⑸ ⑹ ⑺ ⑻ ⑼ ⑽ ⑾ ⑿ ⒀ ⒁ ⒂ ⒃ ⒄ ⒅ ⒆ ⒇ ⒈ ⒉ ⒊ ⒋ ⒌ ⒍ ⒎ ⒏ ⒐ ⒑ ⒒ ⒓ ⒔ ⒕ ⒖ ⒗ ⒘ ⒙ ⒚ ⒛ ㈠ ㈡ ㈢ ㈣ ㈤ ㈥ ㈦ ㈧ ㈨ ㈩ ㊀ ㊁ ㊂ ㊃ ㊄ ㊅ ㊆ ㊇ ㊈ ㊉ ０ １ ２ ３ ４ ５ ６ ７ ８ ９
fract: ⅓ ¾ ¼ % ℅ ‰
technic: ⌅ ⌆ ⌇ ⌒  ⌘ ﹘ ﹝ ﹞ ﹟ ﹡ 〶 ␣
square: ▀ ▁ ▂ ▃ ▄ ▅ ▆ ▇ ▉ ▊ ▋ █ ▌ ▐ ▍ ▎ ▏ ▕ ░ ▒ ▓ ▔ ▢ ▣ ▤ ▥ ▦ ▧ ▨ ▩ ▪ ▫ ▱ ■  ⊠ 〓 ◊ ◈ ◇ ◆ ☖ ☗
triangle: ▲ ▼ ◀ ◣ ◥ ◤ ◢ ▶ ◁ △ ▽ ▷ ∆ ∇ ⊿
Line: │ ┃ ╽ ╿ ╏ ║ ╎ ┇ ︱ ┊ ︳ ┋ ┆ ╵ 〡 〢 ╹ ╻ ╷ 〣 ≡ ︴ ﹏ ﹌ ﹋ ╳ ╲ ╱ ︶ ︵ 〵 〴 〳 〆 ` ‐
Corner: ﹄ ﹃ ﹂ ﹁ ┕ ┓ └ ┐ ┖ ┒ ┗ ┑ ┍ ┙ ┏ ┛ ┎ ┚ ┌ ┘ 「 」 『 』 ├ ┝ ┞ ┟ ┠ ┡ ┢ ┣ ┤ ┥ ┦ ┧ ┨ ┩ ┪ ┫ ┬ ┭ ┮ ┯ ┰ ┱ ┲ ┳ ┴ ┵ ┶ ┷ ┸ ┹ ┺ ┻ ┼ ┽ ┾ ┿ ╀ ╁ ╂ ╃ ╄ ╅ ╆ ╇ ╈ ╉ ╊ ╋ ╒ ╕ ╓ ╖ ╔ ╗ ╘ ╛ ╙ ╜ ╚ ╝ ╞ ╡ ╟ ╢ ╠ ╣ ╥ ╨ ╧ ╤ ╦ ╩ ╪ ╫ ╬ 〒 ⊥ ╭ ╮ ╯ ╰  〦 〧 〨 ∟
Circle: ◉ ○ ◌ ◍ ◎ ● ◐ ◑ ◒ ◓ ⊗ ⊙ ◯ 〇 〶 ◦ ∅ ⊕ ⊖ ⊘ ⦿ ⚽ ⚾〄
phonenetic: θ ð
Latin: ĩ Ň Ⓐ Ⓑ Ⓒ Ⓓ Ⓔ Ⓕ Ⓖ Ⓗ Ⓘ Ⓙ Ⓚ Ⓛ Ⓜ Ⓝ Ⓞ Ⓟ Ⓠ Ⓡ Ⓢ Ⓣ Ⓤ Ⓥ Ⓦ Ⓧ Ⓨ Ⓩ ⓐ ⓑ ⓒ ⓓ ⓔ ⓕ ⓖ ⓗ ⓘ ⓙ ⓚ ⓛ ⓜ ⓝ ⓞ ⓟ ⓠ ⓡ ⓢ ⓣ ⓤ ⓥ ⓦ ⓧ ⓨ ⓩ
Symbols Emoji: ™ 〰 🆗 🆕 🆙 🆒 🆓 🆖 🅿 Ⓜ 🆑 🆘 🆚 ⚠ 🅰 🅱 🆎 🅾 ♻ 🆔
*/

public static class Utils
{
    public const string EmptyMessage = "<size=0>.</size>";

    private static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime StartTime = DateTime.UtcNow;
    private static readonly long EpochStartSeconds = (long)(StartTime - Epoch).TotalSeconds;
    private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();

    public static long TimeStamp => EpochStartSeconds + (long)Stopwatch.Elapsed.TotalSeconds;

    private static readonly StringBuilder SelfSuffix = new();
    private static readonly StringBuilder SelfMark = new(20);
    private static readonly StringBuilder TargetSuffix = new();
    private static readonly StringBuilder TargetMark = new(20);

    private static readonly Dictionary<string, Sprite> CachedSprites = [];

    private static long LastNotifyRolesErrorTS = TimeStamp;

    public static long GameStartTimeStamp;

    private static readonly Dictionary<byte, (string Text, int Duration, bool Long)> LongRoleDescriptions = [];

    public static bool DoRPC => AmongUsClient.Instance.AmHost && Main.AllPlayerControls.Any(x => x.IsModdedClient() && !x.IsHost());
    public static int TotalTaskCount => Main.RealOptionsData.GetInt(Int32OptionNames.NumCommonTasks) + Main.RealOptionsData.GetInt(Int32OptionNames.NumLongTasks) + Main.RealOptionsData.GetInt(Int32OptionNames.NumShortTasks);
    private static int AllPlayersCount => Main.PlayerStates.Values.Count(state => state.countTypes != CountTypes.OutOfGame);
    public static int AllAlivePlayersCount => Main.AllAlivePlayerControls.Count(pc => !pc.Is(CountTypes.OutOfGame));
    public static bool IsAllAlive => Main.PlayerStates.Values.All(state => state.countTypes == CountTypes.OutOfGame || !state.IsDead);

    public static long GetTimeStamp(DateTime? dateTime = null)
    {
        return (long)((dateTime ?? DateTime.Now).ToUniversalTime() - Epoch).TotalSeconds;
    }

    public static void ErrorEnd(string text)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            Logger.Fatal($"{text} error, triggering anti-black screen measures", "Anti-Blackout");
            Main.OverrideWelcomeMsg = GetString("AntiBlackOutNotifyInLobby");
            LateTask.New(() => { Logger.SendInGame(GetString("AntiBlackOutLoggerSendInGame") /*, true*/, Color.red); }, 3f, "Anti-Black Msg SendInGame");

            LateTask.New(() =>
            {
                CustomWinnerHolder.ResetAndSetWinner(CustomWinner.Error);
                GameManager.Instance.LogicFlow.CheckEndCriteria();
                RPC.ForceEndGame(CustomWinner.Error);
            }, 5.5f, "Anti-Black End Game");
        }
        else
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AntiBlackout, SendOption.Reliable, AmongUsClient.Instance.HostId);
            writer.Write(text);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            if (Options.EndWhenPlayerBug.GetBool())
                LateTask.New(() => Logger.SendInGame(GetString("AntiBlackOutRequestHostToForceEnd") /*, true*/, Color.red), 3f, "Anti-Black Msg SendInGame");
            else
            {
                LateTask.New(() => Logger.SendInGame(GetString("AntiBlackOutHostRejectForceEnd") /*, true*/, Color.red), 3f, "Anti-Black Msg SendInGame");

                LateTask.New(() =>
                {
                    AmongUsClient.Instance.ExitGame(DisconnectReasons.Custom);
                    Logger.Fatal($"{text} error, disconnected from game", "Anti-black");
                }, 8f, "Anti-Black Exit Game");
            }
        }
    }

    public static void CheckAndSetVentInteractions()
    {
        if (Main.AllPlayerControls.Any(VentilationSystemDeterioratePatch.BlockVentInteraction))
            SetAllVentInteractions();
    }

    public static void TPAll(Vector2 location, bool log = true)
    {
        foreach (PlayerControl pc in Main.AllAlivePlayerControls) TP(pc.NetTransform, location, log);
    }

    public static int NumSnapToCallsThisRound;

    public static bool TP(CustomNetworkTransform nt, Vector2 location, bool noCheckState = false, bool log = true)
    {
        PlayerControl pc = nt.myPlayer;
        var sendOption = SendOption.Reliable;
        bool submerged = SubmergedCompatibility.IsSubmerged();

        if (!noCheckState)
        {
            if (pc.Is(CustomRoles.AntiTP)) return false;

            if (pc.inVent || pc.inMovingPlat || pc.onLadder || !pc.IsAlive() || pc.MyPhysics.Animations.IsPlayingAnyLadderAnimation() || pc.MyPhysics.Animations.IsPlayingEnterVentAnimation() || (submerged && pc.IsLocalPlayer() && SubmergedCompatibility.GetInTransition()))
            {
                if (log) Logger.Warn($"Target ({pc.GetNameWithRole().RemoveHtmlTags()}) is in an un-teleportable state - Teleporting canceled", "TP");
                return false;
            }

            switch (Vector2.Distance(pc.Pos(), location))
            {
                case < 0.3f:
                {
                    if (log) Logger.Warn($"Target ({pc.GetNameWithRole().RemoveHtmlTags()}) is too close to the destination - Teleporting canceled", "TP");
                    return false;
                }
                case < 1.5f when !GameStates.IsLobby:
                {
                    if (log) Logger.Msg($"Target ({pc.GetNameWithRole().RemoveHtmlTags()}) is too close to the destination - Changed to SendOption.None", "TP");
                    sendOption = SendOption.None;
                    break;
                }
            }
        }

        switch (AmongUsClient.Instance.AmHost)
        {
            case true:
                nt.SnapTo(location, (ushort)(nt.lastSequenceId + 328));
                nt.SetDirtyBit(uint.MaxValue);
                break;
            case false when !nt.AmOwner:
                return false;
        }

        if (NumSnapToCallsThisRound > 80)
        {
            if (log) Logger.Warn($"Too many SnapTo calls this round ({NumSnapToCallsThisRound}) - Changed to SendOption.None", "TP");
            sendOption = SendOption.None;
        }

        if (GameStates.CurrentServerType != GameStates.ServerType.Vanilla)
            sendOption = SendOption.Reliable;

        var newSid = (ushort)(nt.lastSequenceId + 8);
        MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(nt.NetId, (byte)RpcCalls.SnapTo, sendOption);
        NetHelpers.WriteVector2(location, messageWriter);
        messageWriter.Write(newSid);
        AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

        if (log) Logger.Info($"{pc.GetNameWithRole().RemoveHtmlTags()} => {location}", "TP");

        CheckInvalidMovementPatch.LastPosition[pc.PlayerId] = location;
        CheckInvalidMovementPatch.ExemptedPlayers.Add(pc.PlayerId);
        AFKDetector.TempIgnoredPlayers.Add(pc.PlayerId);
        LateTask.New(() => AFKDetector.TempIgnoredPlayers.Remove(pc.PlayerId), 0.2f + CalculatePingDelay(), log: false);

        if (submerged)
        {
            SubmergedCompatibility.ChangeFloor(pc.PlayerId, pc.transform.position.y > -7);
            SubmergedCompatibility.CheckOutOfBoundsElevator(pc);
        }

        if (sendOption == SendOption.Reliable) NumSnapToCallsThisRound++;
        return true;
    }

    public static bool TPToRandomVent(CustomNetworkTransform nt, bool log = true)
    {
        Il2CppReferenceArray<Vent> vents = ShipStatus.Instance.AllVents;
        Vent vent = vents.RandomElement();

        Logger.Info($"{nt.myPlayer.GetNameWithRole().RemoveHtmlTags()} => {vent.transform.position} (vent)", "TP");

        return TP(nt, new(vent.transform.position.x, vent.transform.position.y + 0.3636f), log);
    }

    public static ClientData GetClientById(int id)
    {
        try { return AmongUsClient.Instance.allClients.ToArray().FirstOrDefault(cd => cd.Id == id); }
        catch { return null; }
    }

    public static bool IsAnySabotageActive()
    {
        return CustomSabotage.Instances.Count > 0 || new[] { SystemTypes.Electrical, SystemTypes.Reactor, SystemTypes.Laboratory, SystemTypes.LifeSupp, SystemTypes.Comms, SystemTypes.HeliSabotage, SystemTypes.MushroomMixupSabotage }.Any(IsActive);
    }

    public static bool IsActive(SystemTypes type)
    {
        try
        {
            if (GameStates.IsLobby || !ShipStatus.Instance || !ShipStatus.Instance.Systems.TryGetValue(type, out ISystemType systemType)) return false;

            int mapId = Main.NormalOptions.MapId;

            switch (type)
            {
                case SystemTypes.Electrical:
                {
                    if (mapId == 5) return false;

                    var switchSystem = systemType.CastFast<SwitchSystem>();
                    return switchSystem is { IsActive: true };
                }
                case SystemTypes.Reactor:
                {
                    switch (mapId)
                    {
                        case 2:
                            return false;
                        case 4:
                            var heliSabotageSystem = systemType.CastFast<HeliSabotageSystem>();
                            return heliSabotageSystem != null && heliSabotageSystem.IsActive;
                        default:
                            var reactorSystemType = systemType.CastFast<ReactorSystemType>();
                            return reactorSystemType is { IsActive: true };
                    }
                }
                case SystemTypes.Laboratory:
                {
                    if (mapId != 2) return false;

                    var reactorSystemType = systemType.CastFast<ReactorSystemType>();
                    return reactorSystemType is { IsActive: true };
                }
                case SystemTypes.LifeSupp:
                {
                    if (mapId is 2 or 4 or 5) return false;

                    var lifeSuppSystemType = systemType.CastFast<LifeSuppSystemType>();
                    return lifeSuppSystemType is { IsActive: true };
                }
                case SystemTypes.Comms:
                {
                    if (mapId is 1 or 5)
                    {
                        var hqHudSystemType = systemType.CastFast<HqHudSystemType>();
                        return hqHudSystemType is { IsActive: true };
                    }

                    var hudOverrideSystemType = systemType.CastFast<HudOverrideSystemType>();
                    return hudOverrideSystemType is { IsActive: true };
                }
                case SystemTypes.HeliSabotage:
                {
                    if (mapId != 4) return false;

                    var heliSabotageSystem = systemType.CastFast<HeliSabotageSystem>();
                    return heliSabotageSystem != null && heliSabotageSystem.IsActive;
                }
                case SystemTypes.MushroomMixupSabotage:
                {
                    if (mapId != 5) return false;

                    var mushroomMixupSabotageSystem = systemType.CastFast<MushroomMixupSabotageSystem>();
                    return mushroomMixupSabotageSystem != null && mushroomMixupSabotageSystem.IsActive;
                }
                default:
                    return false;
            }
        }
        catch (Exception e)
        {
            ThrowException(e);
            return false;
        }
    }

    public static void SetVision(this IGameOptions opt, bool hasImpVision)
    {
        if (hasImpVision)
        {
            opt.SetFloat(FloatOptionNames.CrewLightMod, opt.GetFloat(FloatOptionNames.ImpostorLightMod));
            if (IsActive(SystemTypes.Electrical)) opt.SetFloat(FloatOptionNames.CrewLightMod, opt.GetFloat(FloatOptionNames.CrewLightMod) * 5);
            return;
        }

        opt.SetFloat(FloatOptionNames.ImpostorLightMod, opt.GetFloat(FloatOptionNames.CrewLightMod));
        if (IsActive(SystemTypes.Electrical)) opt.SetFloat(FloatOptionNames.ImpostorLightMod, opt.GetFloat(FloatOptionNames.ImpostorLightMod) / 5);
    }

    private static void TargetDies(PlayerControl killer, PlayerControl target)
    {
        if (target.IsAlive() || GameStates.IsMeeting) return;

        CustomRoles targetRole = target.GetCustomRole();

        foreach (PlayerControl seer in Main.AllPlayerControls)
        {
            if (KillFlashCheck(killer, target, seer))
            {
                seer.KillFlash();
                continue;
            }

            if (targetRole == CustomRoles.SuperStar && seer.IsAlive())
            {
                if (!Options.ImpKnowSuperStarDead.GetBool() && seer.IsImpostor()) continue;
                if (!Options.NeutralKnowSuperStarDead.GetBool() && (seer.GetCustomRole().IsNeutral() || seer.Is(CustomRoles.Bloodlust))) continue;
                if (!Options.CovenKnowSuperStarDead.GetBool() && seer.Is(CustomRoleTypes.Coven)) continue;

                seer.KillFlash();
                seer.Notify(ColorString(GetRoleColor(CustomRoles.SuperStar), GetString("OnSuperStarDead")));
            }
        }

        switch (targetRole)
        {
            case CustomRoles.SuperStar when !Main.SuperStarDead.Contains(target.PlayerId):
                Main.SuperStarDead.Add(target.PlayerId);
                break;
            case CustomRoles.Demolitionist:
                Demolitionist.OnDeath(killer, target);
                break;
        }
    }

    private static bool KillFlashCheck(PlayerControl killer, PlayerControl target, PlayerControl seer)
    {
        if (seer.Is(CustomRoles.GM) || seer.Is(CustomRoles.Seer)) return true;

        if (!seer.IsAlive() || killer == seer || target == seer) return false;

        return seer.Is(CustomRoles.EvilTracker) && EvilTracker.KillFlashCheck(killer, target);
    }

    public static void BlackOut(this IGameOptions opt, bool blackOut)
    {
        opt.SetFloat(FloatOptionNames.ImpostorLightMod, blackOut ? 0 : Main.DefaultImpostorVision);
        opt.SetFloat(FloatOptionNames.CrewLightMod, blackOut ? 0 : Main.DefaultCrewmateVision);
    }

    public static void SaveComboInfo()
    {
        SaveFile($"{Main.DataPath}/EHR_DATA/AlwaysCombos.json");
        SaveFile($"{Main.DataPath}/EHR_DATA/NeverCombos.json");
        return;

        void SaveFile(string path)
        {
            try
            {
                Il2CppSystem.Collections.Generic.Dictionary<int, Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<string>>> data = new();
                Dictionary<int, Dictionary<CustomRoles, List<CustomRoles>>> dict = path.Contains("Always") ? Main.AlwaysSpawnTogetherCombos : Main.NeverSpawnTogetherCombos;

                dict.Do(kvp =>
                {
                    data[kvp.Key] = new();

                    kvp.Value.Do(pair =>
                    {
                        var key = pair.Key.ToString();
                        data[kvp.Key][key] = new();
                        pair.Value.Do(x => data[kvp.Key][key].Add(x.ToString()));
                    });
                });

                File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
            }
            catch (Exception e)
            {
                Logger.Error("Failed to save combo info", "SaveComboInfo");
                ThrowException(e);
            }
        }
    }

    public static void LoadComboInfo()
    {
        LoadFile($"{Main.DataPath}/EHR_DATA/AlwaysCombos.json");
        LoadFile($"{Main.DataPath}/EHR_DATA/NeverCombos.json");
        return;

        void LoadFile(string path)
        {
            try
            {
                if (!File.Exists(path)) return;

                var data = JsonConvert.DeserializeObject<Il2CppSystem.Collections.Generic.Dictionary<int, Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<string>>>>(File.ReadAllText(path));
                Dictionary<int, Dictionary<CustomRoles, List<CustomRoles>>> dict = path.Contains("Always") ? Main.AlwaysSpawnTogetherCombos : Main.NeverSpawnTogetherCombos;
                dict.Clear();

                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<int, Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Collections.Generic.List<string>>> kvp in data)
                {
                    dict[kvp.Key] = [];

                    foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, Il2CppSystem.Collections.Generic.List<string>> pair in kvp.Value)
                    {
                        var key = Enum.Parse<CustomRoles>(pair.Key);
                        dict[kvp.Key][key] = [];
                        foreach (string n in pair.Value) dict[kvp.Key][key].Add(Enum.Parse<CustomRoles>(n));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to load combo info", "LoadComboInfo");
                ThrowException(e);
            }
        }
    }

    public static void RemovePlayerFromPreviousRoleData(PlayerControl target)
    {
        if (!Main.PlayerStates.TryGetValue(target.PlayerId, out PlayerState state)) return;
        state.Role.Remove(target.PlayerId);
    }

    public static string GetDisplayRoleName(byte playerId, bool pure = false, bool seeTargetBetrayalAddons = false)
    {
        (string, Color) textData = GetRoleText(playerId, playerId, pure, seeTargetBetrayalAddons);
        return ColorString(textData.Item2, textData.Item1);
    }

    public static string GetRoleName(CustomRoles role, bool forUser = true)
    {
        return GetRoleString(role.ToString(), forUser);
    }

    public static string GetRoleMode(CustomRoles role, bool parentheses = true)
    {
        if (Options.HideGameSettings.GetBool() && Main.AllPlayerControls.Length > 1) return string.Empty;

        string mode;

        try
        {
            mode = !role.IsAdditionRole()
                ? GetString($"Rate{role.GetMode()}")
                : GetString($"Rate{Options.CustomAdtRoleSpawnRate[role].GetInt()}");
        }
        catch (KeyNotFoundException) { mode = GetString("Rate0"); }

        mode = mode.Replace("color=", string.Empty);
        return parentheses ? $"({mode})" : mode;
    }

    public static Color GetRoleColor(CustomRoles role)
    {
        string hexColor = Main.RoleColors.GetValueOrDefault(role, "#ffffff");
        _ = ColorUtility.TryParseHtmlString(hexColor, out Color c);
        return c;
    }

    public static string GetRoleColorCode(CustomRoles role)
    {
        return Main.RoleColors.GetValueOrDefault(role, "#ffffff");
    }

    public static (string, Color) GetRoleText(byte seerId, byte targetId, bool pure = false, bool seeTargetBetrayalAddons = false)
    {
        PlayerState seerState = Main.PlayerStates[seerId];
        PlayerState targetState = Main.PlayerStates[targetId];

        CustomRoles seerMainRole = seerState.MainRole;
        List<CustomRoles> seerSubRoles = seerState.SubRoles;

        CustomRoles targetMainRole = targetState.MainRole;
        List<CustomRoles> targetSubRoles = targetState.SubRoles;

        bool self = seerId == targetId || seerState.IsDead;

        if (!self && Main.DiedThisRound.Contains(seerId)) return (string.Empty, Color.white);

        if (Options.CurrentGameMode == CustomGameMode.HideAndSeek && targetMainRole == CustomRoles.Agent && CustomHnS.PlayerRoles[seerId].Interface.Team != Team.Impostor)
            targetMainRole = CustomRoles.Hider;

        if ((ExileController.Instance || targetState.IsDead || (GameStates.IsMeeting && MeetingHud.Instance.state is MeetingHud.VoteStates.Results or MeetingHud.VoteStates.Proceeding or MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted)) && !GameStates.IsEnded && Forger.Forges.TryGetValue(targetId, out var forgedRole))
            targetMainRole = forgedRole;

        if (!self && seerMainRole.IsImpostor() && targetMainRole == CustomRoles.DoubleAgent && DoubleAgent.ShownRoles.TryGetValue(targetId, out CustomRoles shownRole))
            targetMainRole = shownRole;

        var loversShowDifferentRole = false;

        if (!GameStates.IsEnded && targetMainRole == CustomRoles.LovingImpostor && !self && seerMainRole != CustomRoles.LovingCrewmate && !seerSubRoles.Contains(CustomRoles.Lovers))
        {
            targetMainRole = Lovers.LovingImpostorRoleForOtherImps.GetValue() switch
            {
                0 => CustomRoles.ImpostorEHR,
                1 => Lovers.LovingImpostorRole,
                _ => CustomRoles.LovingImpostor
            };

            loversShowDifferentRole = true;
        }

        string roleText = GetRoleName(targetMainRole);
        Color roleColor = GetRoleColor(loversShowDifferentRole ? CustomRoles.Impostor : targetMainRole);

        if (LastImpostor.CurrentId == targetId) roleText = GetRoleString("Last-") + roleText;

        if (Options.NameDisplayAddons.GetBool() && !pure && self)
        {
            foreach (CustomRoles subRole in targetSubRoles)
            {
                if (subRole is not CustomRoles.LastImpostor and not CustomRoles.Madmate and not CustomRoles.Charmed and not CustomRoles.Recruit and not CustomRoles.Lovers and not CustomRoles.Contagious and not CustomRoles.Bloodlust and not CustomRoles.Entranced and not CustomRoles.Egoist)
                {
                    string str = GetString("Prefix." + subRole);
                    if (!subRole.IsAdditionRole()) str = GetString(subRole.ToString());

                    roleText = ColorString(GetRoleColor(subRole), (Options.AddBracketsToAddons.GetBool() ? "<#ffffff>(</color>" : string.Empty) + str + (Options.AddBracketsToAddons.GetBool() ? "<#ffffff>)</color>" : string.Empty) + " ") + roleText;
                }
            }
        }

        if (seerMainRole == CustomRoles.LovingImpostor && self)
            roleColor = GetRoleColor(CustomRoles.LovingImpostor);

        if (targetSubRoles.Contains(CustomRoles.Madmate))
        {
            roleColor = GetRoleColor(CustomRoles.Madmate);
            roleText = GetRoleString("Mad-") + roleText;
        }

        if (targetSubRoles.Contains(CustomRoles.Recruit))
        {
            roleColor = GetRoleColor(CustomRoles.Recruit);
            roleText = GetRoleString("Recruit-") + roleText;
        }

        if (targetSubRoles.Contains(CustomRoles.Charmed) && (self || pure || seeTargetBetrayalAddons || seerMainRole == CustomRoles.Succubus || (Succubus.TargetKnowOtherTarget.GetBool() && seerSubRoles.Contains(CustomRoles.Charmed))))
        {
            roleColor = GetRoleColor(CustomRoles.Charmed);
            roleText = GetRoleString("Charmed-") + roleText;
        }

        if (targetSubRoles.Contains(CustomRoles.Entranced) && (self || pure || seeTargetBetrayalAddons || seerMainRole == CustomRoles.Siren || (Siren.CovenKnowEntranced.GetValue() == 1 && seerMainRole.IsCoven()) || (Siren.EntrancedKnowEntranced.GetBool() && seerSubRoles.Contains(CustomRoles.Entranced))))
        {
            roleColor = GetRoleColor(CustomRoles.Entranced);
            roleText = GetRoleString("Entranced-") + roleText;
        }

        if (targetSubRoles.Contains(CustomRoles.Contagious) && (self || pure || seeTargetBetrayalAddons || seerMainRole == CustomRoles.Virus || (Virus.TargetKnowOtherTarget.GetBool() && seerSubRoles.Contains(CustomRoles.Contagious))))
        {
            roleColor = GetRoleColor(CustomRoles.Contagious);
            roleText = GetRoleString("Contagious-") + roleText;
        }

        if (targetSubRoles.Contains(CustomRoles.Bloodlust) && (self || pure || seeTargetBetrayalAddons))
        {
            roleColor = GetRoleColor(CustomRoles.Bloodlust);
            roleText = $"{GetString("Prefix.Bloodlust")} {roleText}";
        }

        if (targetSubRoles.Contains(CustomRoles.Egoist) && (self || pure || (seeTargetBetrayalAddons && Options.ImpEgoistVisibalToAllies.GetBool())))
        {
            roleColor = GetRoleColor(CustomRoles.Egoist);
            roleText = $"{GetString("Prefix.Egoist")} {roleText}";
        }

        return (roleText, roleColor);
    }

    private static string GetKillCountText(byte playerId, bool ffa = false)
    {
        if (Main.PlayerStates.All(x => x.Value.GetRealKiller() != playerId) && !ffa) return string.Empty;

        return ' ' + ColorString(new(255, 69, 0, byte.MaxValue), string.Format(GetString("KillCount"), Main.PlayerStates.Count(x => x.Value.GetRealKiller() == playerId)));
    }

    public static string GetVitalText(byte playerId, bool realKillerColor = false)
    {
        PlayerState state = Main.PlayerStates[playerId];
        string deathReason = state.IsDead ? GetString("DeathReason." + state.deathReason) : GetString("Alive");

        if (realKillerColor)
        {
            byte killerId = state.GetRealKiller();
            Color color = killerId != byte.MaxValue ? Main.PlayerColors[killerId] : GetRoleColor(CustomRoles.Doctor);
            if (state.deathReason == PlayerState.DeathReason.Disconnected) color = new(255, 255, 255, 50);

            deathReason = ColorString(color, deathReason);
        }

        return deathReason;
    }

    public static MessageWriter CreateRPC(CustomRPC rpc)
    {
        return AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)rpc, SendOption.Reliable);
    }

    public static void EndRPC(MessageWriter writer)
    {
        AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public static void SendRPC(CustomRPC rpc, params object[] data)
    {
        if (!DoRPC) return;

        MessageWriter w;

        try { w = CreateRPC(rpc); }
        catch { return; }

        try
        {
            foreach (object o in data)
            {
                switch (o)
                {
                    case byte b:
                        w.Write(b);
                        break;
                    case int i:
                        w.WritePacked(i);
                        break;
                    case float f:
                        w.Write(f);
                        break;
                    case string s:
                        w.Write(s);
                        break;
                    case bool b:
                        w.Write(b);
                        break;
                    case long l:
                        w.Write(l.ToString());
                        break;
                    case char c:
                        w.Write(c.ToString());
                        break;
                    case Vector2 v:
                        w.Write(v);
                        break;
                    case Vector3 v:
                        w.Write(v);
                        break;
                    case InnerNetObject ino:
                        w.WriteNetObject(ino);
                        break;
                    case Color color:
                        w.Write(color);
                        break;
                    case Color32 color32:
                        w.Write(color32);
                        break;
                    default:
                        try
                        {
                            if (o != null && Enum.TryParse(o.GetType(), o.ToString(), out object e) && e != null)
                                w.WritePacked((int)e);
                        }
                        catch (InvalidCastException e) { ThrowException(e); }

                        break;
                }
            }
        }
        finally { EndRPC(w); }
    }

    public static void IncreaseAbilityUseLimitOnKill(PlayerControl killer)
    {
        if (Main.PlayerStates[killer.PlayerId].Role is Mafioso { IsEnable: true } mo) mo.OnMurder(killer, null);

        float add = GetSettingNameAndValueForRole(killer.GetCustomRole(), "AbilityUseGainWithEachKill");
        killer.RpcIncreaseAbilityUseLimitBy(add);
    }

    public static void ThrowException(Exception ex, [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerMemberName = "")
    {
        try
        {
            StackTrace st = new(1, true);
            StackFrame[] stFrames = st.GetFrames();

            StackFrame firstFrame = stFrames.FirstOrDefault();

            StringBuilder sb = new();
            sb.Append($" {ex.GetType().Name}: {ex.Message}\n      thrown by {ex.Source}\n      at {ex.TargetSite}\n      in {fileName.Split('\\')[^1]}\n      at line {lineNumber}\n      in method \"{callerMemberName}\"\n------ Method Stack Trace ------");

            var skip = true;

            foreach (StackFrame sf in stFrames)
            {
                if (skip)
                {
                    skip = false;
                    continue;
                }

                MethodBase callerMethod = sf.GetMethod();

                string callerMethodName = callerMethod?.Name;
                string callerClassName = callerMethod?.DeclaringType?.FullName;

                sb.Append($"\n      at {callerClassName}.{callerMethodName}");
            }

            sb.Append("\n------ End of Method Stack Trace ------");
            sb.Append("\n------ Exception ------\n   ");

            sb.Append(ex.StackTrace?.Replace("\r\n", "\n").Replace("\\n", "\n").Replace("\n", "\n   "));

            sb.Append("\n------ End of Exception ------");
            sb.Append("\n------ Exception Stack Trace ------\n");

            StackTrace stEx = new(ex, true);
            StackFrame[] stFramesEx = stEx.GetFrames();

            foreach (StackFrame sf in stFramesEx)
            {
                MethodBase callerMethod = sf.GetMethod();

                string callerMethodName = callerMethod?.Name;
                string callerClassName = callerMethod?.DeclaringType?.FullName;

                sb.Append($"\n      at {callerClassName}.{callerMethodName} in {sf.GetFileName()}, line {sf.GetFileLineNumber()}");
            }

            sb.Append("\n------ End of Exception Stack Trace ------");

            Logger.Error(sb.ToString(), firstFrame?.GetMethod()?.ToString(), multiLine: true);
        }
        catch { }
    }

    public static void SetAllVentInteractions()
    {
        var ventilationSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].CastFast<VentilationSystem>();
        if (ventilationSystem != null) VentilationSystemDeterioratePatch.SerializeV2(ventilationSystem);
    }

    public static bool IsRevivingRoleAlive()
    {
        return Main.AllAlivePlayerControls.Any(x => x.GetCustomRole() is CustomRoles.Altruist or CustomRoles.Occultist or CustomRoles.TimeMaster);
    }

    public static bool HasTasks(NetworkedPlayerInfo p, bool forRecompute = true)
    {
        if (GameStates.IsLobby) return false;
        if (p.Tasks == null) return false;
        if (p.Role == null) return false;

        var hasTasks = true;
        PlayerState state = Main.PlayerStates[p.PlayerId];
        if (p.Disconnected) return false;
        if (p.Role.IsImpostor) hasTasks = false;

        switch (Options.CurrentGameMode)
        {
            case CustomGameMode.SoloKombat:
            case CustomGameMode.FFA:
            case CustomGameMode.HotPotato:
            case CustomGameMode.CaptureTheFlag:
            case CustomGameMode.NaturalDisasters:
            case CustomGameMode.RoomRush:
            case CustomGameMode.KingOfTheZones:
            case CustomGameMode.TheMindGame:
            case CustomGameMode.Quiz:
            case CustomGameMode.BedWars:
                return false;
            case CustomGameMode.HideAndSeek:
                return CustomHnS.HasTasks(p);
            case CustomGameMode.MoveAndStop:
            case CustomGameMode.Speedrun:
                return !p.IsDead;
        }

        CustomRoles role = state.MainRole;

        switch (role)
        {
            case CustomRoles.GM:
            case CustomRoles.Sheriff when !Options.UsePets.GetBool() || !Sheriff.UsePet.GetBool():
            case CustomRoles.Curser:
            case CustomRoles.Arsonist:
            case CustomRoles.Jackal:
            case CustomRoles.Sidekick:
            case CustomRoles.Poisoner:
            case CustomRoles.Eclipse:
            case CustomRoles.Pyromaniac:
            case CustomRoles.NSerialKiller:
            case CustomRoles.Slenderman:
            case CustomRoles.Amogus:
            case CustomRoles.Weatherman:
            case CustomRoles.NoteKiller:
            case CustomRoles.Vortex:
            case CustomRoles.Beehive:
            case CustomRoles.RouleteGrandeur:
            case CustomRoles.Nonplus:
            case CustomRoles.Tremor:
            case CustomRoles.Evolver:
            case CustomRoles.Rogue:
            case CustomRoles.Patroller:
            case CustomRoles.Simon:
            case CustomRoles.Chemist:
            case CustomRoles.Samurai:
            case CustomRoles.QuizMaster:
            case CustomRoles.Bargainer:
            case CustomRoles.Tiger:
            case CustomRoles.SoulHunter:
            case CustomRoles.Enderman:
            case CustomRoles.Mycologist:
            case CustomRoles.Bubble:
            case CustomRoles.Hookshot:
            case CustomRoles.Sprayer:
            case CustomRoles.Doppelganger:
            case CustomRoles.NecroGuesser:
            case CustomRoles.PlagueDoctor:
            case CustomRoles.Postman:
            case CustomRoles.Dealer:
            case CustomRoles.Auditor:
            case CustomRoles.Magistrate:
            case CustomRoles.Seamstress:
            case CustomRoles.Spirit:
            case CustomRoles.Starspawn:
            case CustomRoles.RoomRusher:
            case CustomRoles.SchrodingersCat:
            case CustomRoles.Shifter:
            case CustomRoles.Technician:
            case CustomRoles.Tank:
            case CustomRoles.Investor:
            case CustomRoles.Gaslighter:
            case CustomRoles.Impartial:
            case CustomRoles.Backstabber:
            case CustomRoles.Predator:
            case CustomRoles.Reckless:
            case CustomRoles.WeaponMaster:
            case CustomRoles.Magician:
            case CustomRoles.Vengeance:
            case CustomRoles.HeadHunter:
            case CustomRoles.Pulse:
            case CustomRoles.Werewolf:
            case CustomRoles.Bandit:
            case CustomRoles.Jailor when !Options.UsePets.GetBool() || !Jailor.UsePet.GetBool():
            case CustomRoles.Traitor:
            case CustomRoles.Glitch:
            case CustomRoles.Pickpocket:
            case CustomRoles.Maverick:
            case CustomRoles.Jinx:
            case CustomRoles.Parasite:
            case CustomRoles.Agitater:
            case CustomRoles.Crusader when !Options.UsePets.GetBool() || !Crusader.UsePet.GetBool():
            case CustomRoles.Refugee:
            case CustomRoles.Jester:
            case CustomRoles.Mario:
            case CustomRoles.Vulture:
            case CustomRoles.God:
            case CustomRoles.Innocent:
            case CustomRoles.Pelican:
            case CustomRoles.Medusa:
            case CustomRoles.Revolutionist:
            case CustomRoles.FFF:
            case CustomRoles.Gamer:
            case CustomRoles.HexMaster:
            case CustomRoles.Wraith:
            case CustomRoles.Juggernaut:
            case CustomRoles.Ritualist:
            case CustomRoles.DarkHide:
            case CustomRoles.Collector:
            case CustomRoles.ImperiusCurse:
            case CustomRoles.Provocateur:
            case CustomRoles.BloodKnight:
            case CustomRoles.Camouflager:
            case CustomRoles.Totocalcio:
            case CustomRoles.Romantic:
            case CustomRoles.VengefulRomantic:
            case CustomRoles.RuthlessRomantic:
            case CustomRoles.Succubus:
            case CustomRoles.Necromancer:
            case CustomRoles.Deathknight:
            case CustomRoles.Amnesiac:
            case CustomRoles.Monarch when !Options.UsePets.GetBool() || !Monarch.UsePet.GetBool():
            case CustomRoles.Deputy when !Options.UsePets.GetBool() || !Deputy.UsePet.GetBool():
            case CustomRoles.Virus:
            case CustomRoles.Farseer when !Options.UsePets.GetBool() || !Farseer.UsePet.GetBool():
            case CustomRoles.Aid when !Options.UsePets.GetBool() || !Aid.UsePet.GetBool():
            case CustomRoles.Socialite when !Options.UsePets.GetBool() || !Socialite.UsePet.GetBool():
            case CustomRoles.Escort when !Options.UsePets.GetBool() || !Escort.UsePet.GetBool():
            case CustomRoles.DonutDelivery when !Options.UsePets.GetBool() || !DonutDelivery.UsePet.GetBool():
            case CustomRoles.Gaulois when !Options.UsePets.GetBool() || !Gaulois.UsePet.GetBool():
            case CustomRoles.Analyst when !Options.UsePets.GetBool() || !Analyst.UsePet.GetBool():
            case CustomRoles.Witness when !Options.UsePets.GetBool() || !Options.WitnessUsePet.GetBool():
            case CustomRoles.Goose:
            case CustomRoles.Pursuer:
            case CustomRoles.Spiritcaller:
            case CustomRoles.PlagueBearer:
            case CustomRoles.Pestilence:
            case CustomRoles.Doomsayer:
                hasTasks = false;
                break;
            case CustomRoles.Dad when ((Dad)state.Role).DoneTasks:
            case CustomRoles.Workaholic:
            case CustomRoles.Terrorist:
            case CustomRoles.Sunnyboy:
            case CustomRoles.Convict:
            case CustomRoles.Opportunist:
            case CustomRoles.Executioner:
            case CustomRoles.Lawyer:
            case CustomRoles.Phantasm:
                if (forRecompute) hasTasks = false;
                break;
            case CustomRoles.Pawn:
            case CustomRoles.Cherokious:
            case CustomRoles.Crewpostor:
            case CustomRoles.Hypocrite:
                if (forRecompute && !p.IsDead) hasTasks = false;
                if (p.IsDead) hasTasks = false;
                break;
            case CustomRoles.Wizard:
                hasTasks = true;
                break;
            default:
                if (role.IsImpostor() || role.IsCoven())
                    hasTasks = false;

                break;
        }

        hasTasks &= !(CopyCat.Instances.Any(x => x.CopyCatPC.PlayerId == p.PlayerId) && forRecompute && (!Options.UsePets.GetBool() || CopyCat.UsePet.GetBool()));
        hasTasks |= p.Object.UsesPetInsteadOfKill() && role is not (CustomRoles.Refugee or CustomRoles.Necromancer or CustomRoles.Deathknight or CustomRoles.Sidekick);

        foreach (CustomRoles subRole in state.SubRoles)
        {
            switch (subRole)
            {
                case CustomRoles.Entranced:
                case CustomRoles.Madmate:
                case CustomRoles.Charmed:
                case CustomRoles.Recruit:
                case CustomRoles.Egoist:
                case CustomRoles.Contagious:
                case CustomRoles.Rascal:
                case CustomRoles.EvilSpirit:
                    hasTasks &= !forRecompute;
                    break;
                case CustomRoles.Bloodlust:
                    hasTasks = false;
                    break;
                case CustomRoles.Specter:
                case CustomRoles.Haunter:
                    hasTasks = !forRecompute;
                    break;
            }
        }

        return hasTasks;
    }

    public static bool CanBeMadmate(this PlayerControl pc)
    {
        return pc != null && pc.IsCrewmate() && !pc.Is(CustomRoles.Madmate)
               && !(
                   (pc.Is(CustomRoles.Sheriff) && !Options.SheriffCanBeMadmate.GetBool()) ||
                   (pc.Is(CustomRoles.Mayor) && !Options.MayorCanBeMadmate.GetBool()) ||
                   (pc.Is(CustomRoles.NiceGuesser) && !Options.NGuesserCanBeMadmate.GetBool()) ||
                   (pc.Is(CustomRoles.Snitch) && !Options.SnitchCanBeMadmate.GetBool()) ||
                   (pc.Is(CustomRoles.Judge) && !Options.JudgeCanBeMadmate.GetBool()) ||
                   (pc.Is(CustomRoles.Marshall) && !Options.MarshallCanBeMadmate.GetBool()) ||
                   (pc.Is(CustomRoles.Farseer) && !Options.FarseerCanBeMadmate.GetBool()) ||
                   (pc.Is(CustomRoles.President) && !Options.PresidentCanBeMadmate.GetBool()) ||
                   pc.Is(CustomRoles.NiceSwapper) ||
                   pc.Is(CustomRoles.Speedrunner) ||
                   pc.Is(CustomRoles.Needy) ||
                   pc.Is(CustomRoles.Loyal) ||
                   pc.Is(CustomRoles.SuperStar) ||
                   pc.Is(CustomRoles.Egoist) ||
                   pc.Is(CustomRoles.DualPersonality)
               );
    }

    public static bool IsRoleTextEnabled(PlayerControl __instance)
    {
        switch (Options.CurrentGameMode)
        {
            case CustomGameMode.CaptureTheFlag or CustomGameMode.NaturalDisasters or CustomGameMode.RoomRush or CustomGameMode.KingOfTheZones or CustomGameMode.Quiz or CustomGameMode.TheMindGame or CustomGameMode.BedWars:
            case CustomGameMode.Standard when IsRevivingRoleAlive() && Main.DiedThisRound.Contains(PlayerControl.LocalPlayer.PlayerId):
                return PlayerControl.LocalPlayer.Is(CustomRoles.GM);
            case CustomGameMode.FFA or CustomGameMode.SoloKombat or CustomGameMode.MoveAndStop or CustomGameMode.HotPotato or CustomGameMode.Speedrun:
            case CustomGameMode.HideAndSeek when CustomHnS.IsRoleTextEnabled(PlayerControl.LocalPlayer, __instance):
                return true;
        }

        if ((Main.VisibleTasksCount && !PlayerControl.LocalPlayer.IsAlive() && Options.GhostCanSeeOtherRoles.GetBool()) || (PlayerControl.LocalPlayer.Is(CustomRoles.Mimic) && Main.VisibleTasksCount && __instance.Data.IsDead && Options.MimicCanSeeDeadRoles.GetBool())) return true;

        if (__instance.IsLocalPlayer()) return true;

        switch (__instance.GetCustomRole())
        {
            case CustomRoles.Crewpostor when PlayerControl.LocalPlayer.Is(CustomRoleTypes.Impostor) && Options.CrewpostorKnowsAllies.GetBool():
            case CustomRoles.Hypocrite when PlayerControl.LocalPlayer.Is(CustomRoleTypes.Impostor) && Hypocrite.KnowsAllies.GetBool():
            case CustomRoles.Jackal when PlayerControl.LocalPlayer.Is(CustomRoles.Jackal):
            case CustomRoles.Jackal when PlayerControl.LocalPlayer.Is(CustomRoles.Sidekick):
            case CustomRoles.Jackal when PlayerControl.LocalPlayer.Is(CustomRoles.Recruit):
            case CustomRoles.Recruit when PlayerControl.LocalPlayer.Is(CustomRoles.Jackal):
            case CustomRoles.Recruit when PlayerControl.LocalPlayer.Is(CustomRoles.Sidekick):
            case CustomRoles.Recruit when PlayerControl.LocalPlayer.Is(CustomRoles.Recruit):
            case CustomRoles.Sidekick when PlayerControl.LocalPlayer.Is(CustomRoles.Jackal):
            case CustomRoles.Sidekick when PlayerControl.LocalPlayer.Is(CustomRoles.Sidekick):
            case CustomRoles.Sidekick when PlayerControl.LocalPlayer.Is(CustomRoles.Recruit):
            case CustomRoles.Workaholic when Workaholic.WorkaholicVisibleToEveryone.GetBool():
            case CustomRoles.Doctor when !__instance.HasEvilAddon() && Options.DoctorVisibleToEveryone.GetBool():
            case CustomRoles.Mayor when Mayor.MayorRevealWhenDoneTasks.GetBool() && __instance.GetTaskState().IsTaskFinished:
            case CustomRoles.Marshall when Marshall.CanSeeMarshall(PlayerControl.LocalPlayer) && __instance.GetTaskState().IsTaskFinished:
                return true;
        }

        return (__instance.IsMadmate() && PlayerControl.LocalPlayer.IsMadmate() && Options.MadmateKnowWhosMadmate.GetBool()) ||
               (__instance.IsMadmate() && PlayerControl.LocalPlayer.Is(CustomRoleTypes.Impostor) && Options.ImpKnowWhosMadmate.GetBool()) ||
               (__instance.Is(CustomRoleTypes.Impostor) && PlayerControl.LocalPlayer.IsMadmate() && Options.MadmateKnowWhosImp.GetBool()) ||
               (__instance.Is(CustomRoles.Mimic) && Main.VisibleTasksCount && !__instance.IsAlive()) ||
               (__instance.Is(CustomRoleTypes.Impostor) && PlayerControl.LocalPlayer.Is(CustomRoles.Crewpostor) && Options.AlliesKnowCrewpostor.GetBool()) ||
               (__instance.Is(CustomRoleTypes.Impostor) && PlayerControl.LocalPlayer.Is(CustomRoles.Hypocrite) && Hypocrite.AlliesKnowHypocrite.GetBool()) ||
               __instance.Is(CustomRoleTypes.Impostor) && PlayerControl.LocalPlayer.Is(CustomRoleTypes.Impostor) && Options.ImpKnowAlliesRole.GetBool() && CustomTeamManager.ArentInCustomTeam(PlayerControl.LocalPlayer.PlayerId, __instance.PlayerId) ||
               (__instance.Is(CustomRoleTypes.Coven) && PlayerControl.LocalPlayer.Is(CustomRoleTypes.Coven)) ||
               (Main.LoversPlayers.TrueForAll(x => x.PlayerId == __instance.PlayerId || x.IsLocalPlayer()) && Main.LoversPlayers.Count == 2 && Lovers.LoverKnowRoles.GetBool()) ||
               (CustomTeamManager.AreInSameCustomTeam(__instance.PlayerId, PlayerControl.LocalPlayer.PlayerId) && CustomTeamManager.IsSettingEnabledForPlayerTeam(__instance.PlayerId, CTAOption.KnowRoles)) ||
               Main.PlayerStates.Values.Any(x => x.Role.KnowRole(PlayerControl.LocalPlayer, __instance)) ||
               PlayerControl.LocalPlayer.IsRevealedPlayer(__instance) ||
               (PlayerControl.LocalPlayer.Is(CustomRoles.God) && God.KnowInfo.GetValue() == 2) ||
               PlayerControl.LocalPlayer.Is(CustomRoles.GM) ||
               Markseeker.PlayerIdList.Any(x => Main.PlayerStates[x].Role is Markseeker { IsEnable: true, TargetRevealed: true } ms && ms.MarkedId == __instance.PlayerId) ||
               Main.GodMode.Value;
    }

    public static string GetFormattedRoomName(string roomName)
    {
        return roomName == "Outside" ? "<#00ffa5>Outside</color>" : $"<#ffffff>In</color> <#00ffa5>{roomName}</color>";
    }

    public static string GetFormattedVectorText(Vector2 pos)
    {
        return $"<#777777>(at {pos.ToString().Replace("(", string.Empty).Replace(")", string.Empty)})</color>";
    }

    public static string GetProgressText(PlayerControl pc)
    {
        TaskState taskState = pc.GetTaskState();
        var comms = false;

        if (taskState.HasTasks)
        {
            if (IsActive(SystemTypes.Comms)) comms = true;
            if (Camouflager.IsActive) comms = true;
        }

        return GetProgressText(pc.PlayerId, comms);
    }

    public static string GetProgressText(byte playerId, bool comms = false)
    {
        switch (Options.CurrentGameMode)
        {
            case CustomGameMode.MoveAndStop: return GetTaskCount(playerId, comms, AmongUsClient.Instance.AmHost);
            case CustomGameMode.Speedrun:
            case CustomGameMode.Standard when Forger.Forges.ContainsKey(playerId) && Main.PlayerStates.TryGetValue(playerId, out var state) && state.IsDead:
                return string.Empty;
        }

        StringBuilder progressText = new();
        PlayerControl pc = GetPlayerById(playerId);

        try { progressText.Append(Main.PlayerStates[playerId].Role.GetProgressText(playerId, comms)); }
        catch (Exception ex) { Logger.Error($"For {pc.GetNameWithRole().RemoveHtmlTags()}, failed to get progress text:  " + ex, "Utils.GetProgressText"); }

        if (pc.Is(CustomRoles.Damocles)) progressText.Append($" {Damocles.GetProgressText(playerId)}");
        if (pc.Is(CustomRoles.Stressed)) progressText.Append($" {Stressed.GetProgressText(playerId)}");
        if (pc.Is(CustomRoles.Circumvent)) progressText.Append($" {Circumvent.GetProgressText(playerId)}");

        if (pc.Is(CustomRoles.Taskcounter))
        {
            string totalCompleted = comms ? "?" : $"{GameData.Instance.CompletedTasks}";
            progressText.Append($" <#00ffa5>{totalCompleted}</color><#ffffff>/{GameData.Instance.TotalTasks}</color>");
        }

        if (progressText.Length != 0 && !progressText.ToString().RemoveHtmlTags().StartsWith(' '))
            progressText.Insert(0, ' ');

        return progressText.ToString();
    }

    public static string GetAbilityUseLimitDisplay(byte playerId, bool usingAbility = false)
    {
        try
        {
            float limit = playerId.GetAbilityUseLimit();
            if (float.IsNaN(limit) /* || limit is > 100 or < 0*/) return string.Empty;

            Color textColor;

            if (limit < 1)
                textColor = Color.red;
            else if (usingAbility)
                textColor = Color.green;
            else
                textColor = GetRoleColor(Main.PlayerStates[playerId].MainRole).ShadeColor(0.25f);

            return ColorString(textColor, $" ({Math.Round(limit, 1)})");
        }
        catch { return string.Empty; }
    }

    public static string GetTaskCount(byte playerId, bool comms, bool moveAndStop = false)
    {
        try
        {
            if ((playerId == 0 && Main.GM.Value) || ChatCommands.Spectators.Contains(playerId) || !Main.PlayerStates.TryGetValue(playerId, out PlayerState state)) return string.Empty;

            switch (state.IsDead)
            {
                case false when !Options.ShowTaskCountWhenAlive.GetBool():
                case true when !Options.ShowTaskCountWhenDead.GetBool():
                    return string.Empty;
            }

            TaskState taskState = state.TaskState;
            if (!taskState.HasTasks) return string.Empty;

            NetworkedPlayerInfo info = GetPlayerInfoById(playerId);
            Color taskCompleteColor = HasTasks(info) ? Color.green : GetRoleColor(state.MainRole).ShadeColor(0.5f);
            Color nonCompleteColor = HasTasks(info) ? Color.yellow : Color.white;

            if (Workhorse.IsThisRole(playerId)) nonCompleteColor = Workhorse.RoleColor;

            Color normalColor = taskState.IsTaskFinished ? taskCompleteColor : nonCompleteColor;

            if (Main.PlayerStates.TryGetValue(playerId, out PlayerState ps))
            {
                normalColor = ps.MainRole switch
                {
                    CustomRoles.Hypocrite => Color.red,
                    CustomRoles.Crewpostor => Color.red,
                    CustomRoles.Cherokious => GetRoleColor(CustomRoles.Cherokious),
                    CustomRoles.Pawn => GetRoleColor(CustomRoles.Pawn),
                    _ => normalColor
                };
            }

            Color textColor = comms ? Color.gray : normalColor;
            string completed = comms ? "?" : $"{taskState.CompletedTasksCount}";
            return ColorString(textColor, $" {(moveAndStop ? "<size=2>" : string.Empty)}{completed}/{taskState.AllTasksCount}{(moveAndStop ? $" <#ffffff>({MoveAndStop.GetLivesRemaining(playerId)} \u2665)</color></size>" : string.Empty)}");
        }
        catch { return string.Empty; }
    }

    public static void ShowActiveSettingsHelp(byte playerId = byte.MaxValue)
    {
        List<Message> messages = [new(GetString("CurrentActiveSettingsHelp") + ":", playerId)];
        if (Options.DisableDevices.GetBool()) messages.Add(new(GetString("DisableDevicesInfo"), playerId));
        if (Options.SyncButtonMode.GetBool()) messages.Add(new(GetString("SyncButtonModeInfo"), playerId));
        if (Options.SabotageTimeControl.GetBool()) messages.Add(new(GetString("SabotageTimeControlInfo"), playerId));
        if (Options.RandomMapsMode.GetBool()) messages.Add(new(GetString("RandomMapsModeInfo"), playerId));
        if (Main.GM.Value) messages.Add(new(GetRoleName(CustomRoles.GM) + GetString("GMInfoLong"), playerId));
        messages.AddRange(from role in Enum.GetValues<CustomRoles>() where role.IsEnable() && !role.IsVanilla() select new Message(GetRoleName(role) + GetRoleMode(role) + GetString($"{role}InfoLong"), playerId));
        if (Options.NoGameEnd.GetBool()) messages.Add(new(GetString("NoGameEndInfo"), playerId));
        messages.SendMultipleMessages();
    }

    /// <summary>
    ///     Gets all players within a specified radius from the specified location
    /// </summary>
    /// <param name="radius">The radius</param>
    /// <param name="from">The location which the radius is counted from</param>
    /// <returns>A list containing all PlayerControls within the specified range from the specified location</returns>
    public static IEnumerable<PlayerControl> GetPlayersInRadius(float radius, Vector2 from)
    {
        return from tg in Main.AllAlivePlayerControls let dis = Vector2.Distance(@from, tg.Pos()) where !Pelican.IsEaten(tg.PlayerId) && !tg.inVent where dis <= radius select tg;
    }

    public static void ShowActiveSettings(byte playerId = byte.MaxValue)
    {
        if (Options.HideGameSettings.GetBool() && playerId != byte.MaxValue)
        {
            SendMessage(GetString("Message.HideGameSettings"), playerId, sendOption: SendOption.None);
            return;
        }

        if (Options.DIYGameSettings.GetBool())
        {
            SendMessage(GetString("Message.NowOverrideText"), playerId, sendOption: SendOption.None);
            return;
        }

        StringBuilder sb = new();
        sb.Append($" \u2605 {GetString("TabGroup.SystemSettings")}");
        Options.GroupedOptions[TabGroup.SystemSettings].Do(CheckAndAppendOptionString);
        sb.Append($"\n\n \u2605 {GetString("TabGroup.GameSettings")}");
        Options.GroupedOptions[TabGroup.GameSettings].Do(CheckAndAppendOptionString);

        SendMessage(sb.ToString().RemoveHtmlTags(), playerId);
        return;

        void CheckAndAppendOptionString(OptionItem item)
        {
            if (item.GetBool() && item.Parent == null && !item.IsCurrentlyHidden())
                sb.Append($"\n{item.GetName(true)}: {item.GetString()}");
        }
    }

    public static void ShowAllActiveSettings(byte playerId = byte.MaxValue)
    {
        if (Options.HideGameSettings.GetBool() && playerId != byte.MaxValue)
        {
            SendMessage(GetString("Message.HideGameSettings"), playerId, sendOption: SendOption.None);
            return;
        }

        if (Options.DIYGameSettings.GetBool())
        {
            SendMessage(GetString("Message.NowOverrideText"), playerId, sendOption: SendOption.None);
            return;
        }

        StringBuilder sb = new();

        sb.Append(GetString("Settings")).Append(':');

        foreach (KeyValuePair<CustomRoles, OptionItem> role in Options.CustomRoleCounts)
        {
            if (!role.Key.IsEnable()) continue;

            string mode;

            try
            {
                mode = !role.Key.IsAdditionRole()
                    ? GetString($"Rate{role.Key.GetMode()}")
                    : GetString($"Rate{Options.CustomAdtRoleSpawnRate[role.Key].GetInt()}");
            }
            catch (KeyNotFoundException) { continue; }

            mode = mode.Replace("color=", string.Empty);

            sb.Append($"\n【{GetRoleName(role.Key)}:{mode} ×{role.Key.GetCount()}】\n");
            ShowChildrenSettings(Options.CustomRoleSpawnChances[role.Key], ref sb);
        }

        foreach (OptionItem opt in OptionItem.AllOptions)
        {
            if (opt.GetBool() && opt.Parent == null && opt.Id is >= 80000 and < 640000 && !opt.IsCurrentlyHidden())
            {
                if (opt.Name is "KillFlashDuration" or "RoleAssigningAlgorithm")
                    sb.Append($"\n【{opt.GetName(true)}: {opt.GetString()}】\n");
                else
                    sb.Append($"\n【{opt.GetName(true)}】\n");

                ShowChildrenSettings(opt, ref sb);
            }
        }

        SendMessage(sb.ToString().RemoveHtmlTags(), playerId);
    }

    public static void CopyCurrentSettings()
    {
        StringBuilder sb = new();

        if (Options.HideGameSettings.GetBool() && !AmongUsClient.Instance.AmHost)
        {
            ClipboardHelper.PutClipboardString(GetString("Message.HideGameSettings"));
            return;
        }

        sb.Append($"━━━━━━━━━━━━【{GetString("Roles")}】━━━━━━━━━━━━");

        foreach (KeyValuePair<CustomRoles, OptionItem> role in Options.CustomRoleCounts)
        {
            if (!role.Key.IsEnable()) continue;

            string mode;

            try
            {
                mode = !role.Key.IsAdditionRole()
                    ? GetString($"Rate{role.Key.GetMode()}")
                    : GetString($"Rate{Options.CustomAdtRoleSpawnRate[role.Key].GetInt()}");
            }
            catch (KeyNotFoundException) { continue; }

            mode = mode.Replace("color=", string.Empty);

            sb.Append($"\n【{GetRoleName(role.Key)}:{mode} ×{role.Key.GetCount()}】\n");
            ShowChildrenSettings(Options.CustomRoleSpawnChances[role.Key], ref sb);
        }

        sb.Append($"━━━━━━━━━━━━【{GetString("Settings")}】━━━━━━━━━━━━");

        foreach (OptionItem opt in OptionItem.AllOptions.Where(x => x.GetBool() && x.Parent == null && x.Id is >= 80000 and < 640000 && !x.IsCurrentlyHidden()))
        {
            if (opt.Name == "KillFlashDuration")
                sb.Append($"\n【{opt.GetName(true)}: {opt.GetString()}】\n");
            else
                sb.Append($"\n【{opt.GetName(true)}】\n");

            ShowChildrenSettings(opt, ref sb);
        }

        sb.Append("\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501\u2501");
        ClipboardHelper.PutClipboardString(sb.ToString().RemoveHtmlTags());
    }

    public static void ShowActiveRoles(byte playerId = byte.MaxValue)
    {
        if (Options.HideGameSettings.GetBool() && playerId != byte.MaxValue)
        {
            SendMessage(GetString("Message.HideGameSettings"), playerId, sendOption: SendOption.None);
            return;
        }

        StringBuilder sb = new();
        sb.Append($"\n{GetRoleName(CustomRoles.GM)}: {(Main.GM.Value ? GetString("RoleRate") : GetString("RoleOff"))}");

        StringBuilder impsb = new();
        StringBuilder neutralsb = new();
        StringBuilder crewsb = new();
        StringBuilder covensb = new();
        StringBuilder addonsb = new();
        StringBuilder ghostsb = new();

        foreach (CustomRoles role in Options.CurrentGameMode == CustomGameMode.HideAndSeek ? CustomHnS.AllHnSRoles : Enum.GetValues<CustomRoles>().Except(CustomHnS.AllHnSRoles))
        {
            string mode;

            try
            {
                mode = !role.IsAdditionRole() || role.IsGhostRole()
                    ? GetString($"Rate{role.GetMode()}")
                    : GetString($"Rate{Options.CustomAdtRoleSpawnRate[role].GetInt()}");
            }
            catch (KeyNotFoundException) { continue; }

            mode = mode.Replace("color=", string.Empty);

            if (role.IsEnable())
            {
                var roleDisplay = $"\n{ColorString(GetRoleColor(role).ShadeColor(0.25f), GetString(role.ToString()))}: {mode} x{role.GetCount()}";

                if (role.IsGhostRole()) ghostsb.Append(roleDisplay);
                else if (role.IsAdditionRole()) addonsb.Append(roleDisplay);
                else if (role.IsCrewmate()) crewsb.Append(roleDisplay);
                else if (role.IsImpostor() || role.IsMadmate()) impsb.Append(roleDisplay);
                else if (role.IsNeutral()) neutralsb.Append(roleDisplay);
                else if (role.IsCoven()) covensb.Append(roleDisplay);
            }
        }

        new List<Message>
        {
            new($"<size=80%>{sb.Append("\n.").ToString().RemoveHtmlTags()}</size>", playerId, GetString("GMRoles")),
            new($"<size=80%>{impsb.Append("\n.").ToString().RemoveHtmlTags()}</size>", playerId, ColorString(GetRoleColor(CustomRoles.Impostor), GetString("ImpostorRoles"))),
            new($"<size=80%>{crewsb.Append("\n.").ToString().RemoveHtmlTags()}</size>", playerId, ColorString(GetRoleColor(CustomRoles.Crewmate), GetString("CrewmateRoles"))),
            new($"<size=80%>{neutralsb.Append("\n.").ToString().RemoveHtmlTags()}</size>", playerId, GetString("NeutralRoles")),
            new($"<size=80%>{covensb.Append("\n.").ToString().RemoveHtmlTags()}</size>", playerId, GetString("CovenRoles")),
            new($"<size=80%>{ghostsb.Append("\n.").ToString().RemoveHtmlTags()}</size>", playerId, GetString("GhostRoles")),
            new($"<size=80%>{addonsb.Append("\n.").ToString().RemoveHtmlTags()}</size>", playerId, GetString("AddonRoles"))
        }.SendMultipleMessages();
    }

    public static void ShowChildrenSettings(OptionItem option, ref StringBuilder sb, int deep = 0, bool f1 = false, bool disableColor = true)
    {
        foreach (var opt in option.Children.Select((v, i) => new { Value = v, Index = i + 1 }))
        {
            switch (opt.Value.Name)
            {
                case "DisableSkeldDevices" when Main.CurrentMap is not MapNames.Skeld and not MapNames.Dleks:
                case "DisableMiraHQDevices" when Main.CurrentMap != MapNames.MiraHQ:
                case "DisablePolusDevices" when Main.CurrentMap != MapNames.Polus:
                case "DisableAirshipDevices" when Main.CurrentMap != MapNames.Airship:
                case "PolusReactorTimeLimit" when Main.CurrentMap != MapNames.Polus:
                case "AirshipReactorTimeLimit" when Main.CurrentMap != MapNames.Airship:
                case "ImpCanBeRole" or "CrewCanBeRole" or "NeutralCanBeRole" or "CovenCanBeRole" when f1:
                    continue;
            }

            if (deep > 0)
            {
                sb.Append(string.Concat(Enumerable.Repeat("┃", Mathf.Max(deep - 1, 0))));
                sb.Append(opt.Index == option.Children.Count ? "┗ " : "┣ ");
            }

            string value = opt.Value.GetString().Replace("ON", "<#00ffa5>ON</color>").Replace("OFF", "<#ff0000>OFF</color>");
            var name = $"{opt.Value.GetName(disableColor).Replace("color=", string.Empty)}</color>";
            sb.Append($"{name}: <#ffff00>{value}</color>\n");
            if (opt.Value.GetBool()) ShowChildrenSettings(opt.Value, ref sb, deep + 1, disableColor: disableColor);
        }
    }

    public static void ShowLastRoles(byte playerId = byte.MaxValue)
    {
        if (AmongUsClient.Instance.IsGameStarted)
        {
            SendMessage(GetString("CantUse.lastroles"), playerId, sendOption: SendOption.None);
            return;
        }

        StringBuilder sb = new();

        sb.Append("<#ffffff>");
        sb.Append(GetString("RoleSummaryText"));
        sb.Append("</color><size=70%>");

        List<byte> cloneRoles = [.. Main.PlayerStates.Keys];

        foreach (byte id in Main.WinnerList)
        {
            try
            {
                if (EndGamePatch.SummaryText[id].Contains("<INVALID:NotAssigned>")) continue;

                sb.Append("\n<#c4aa02>\u2605</color> ").Append(EndGamePatch.SummaryText[id] /*.RemoveHtmlTags()*/);
                cloneRoles.Remove(id);
            }
            catch (Exception ex) { ThrowException(ex); }
        }

        switch (Options.CurrentGameMode)
        {
            case CustomGameMode.SoloKombat:
                List<(int, byte)> list = [];
                list.AddRange(cloneRoles.Select(id => (SoloPVP.GetRankFromScore(id), id)));

                list.Sort();

                foreach ((int, byte) id in list)
                {
                    try { sb.Append("\n\u3000 ").Append(EndGamePatch.SummaryText[id.Item2]); }
                    catch (Exception ex) { ThrowException(ex); }
                }

                break;
            case CustomGameMode.FFA:
                List<(int, byte)> list2 = [];
                list2.AddRange(cloneRoles.Select(id => (FreeForAll.GetRankFromScore(id), id)));

                list2.Sort();

                foreach ((int, byte) id in list2)
                {
                    try { sb.Append("\n\u3000 ").Append(EndGamePatch.SummaryText[id.Item2]); }
                    catch (Exception ex) { ThrowException(ex); }
                }

                break;
            case CustomGameMode.MoveAndStop:
                List<(int, byte)> list3 = [];
                list3.AddRange(cloneRoles.Select(id => (MoveAndStop.GetRankFromScore(id), id)));

                list3.Sort();

                foreach ((int, byte) id in list3)
                {
                    try { sb.Append("\n\u3000 ").Append(EndGamePatch.SummaryText[id.Item2]); }
                    catch (Exception ex) { ThrowException(ex); }
                }

                break;
            case CustomGameMode.BedWars:
            case CustomGameMode.RoomRush:
            case CustomGameMode.NaturalDisasters:
            case CustomGameMode.KingOfTheZones:
            case CustomGameMode.TheMindGame:
            case CustomGameMode.Quiz:
            case CustomGameMode.CaptureTheFlag:
            case CustomGameMode.Speedrun:
            case CustomGameMode.HotPotato:
            case CustomGameMode.HideAndSeek:
                foreach (byte id in cloneRoles)
                {
                    try { sb.Append("\n\u3000 ").Append(EndGamePatch.SummaryText[id]); }
                    catch (Exception ex) { ThrowException(ex); }
                }

                break;
            default:
                foreach (byte id in cloneRoles)
                {
                    try
                    {
                        if (!EndGamePatch.SummaryText.TryGetValue(id, out string summaryText)) continue;

                        if (summaryText.Contains("<INVALID:NotAssigned>")) continue;

                        sb.Append("\n\u3000 ").Append(summaryText);
                    }
                    catch (Exception ex) { ThrowException(ex); }
                }

                break;
        }

        sb.Append("</size>");

        SendMessage("\n", playerId, sb.ToString());

        if (Options.CurrentGameMode != CustomGameMode.Standard) return;

        sb.Clear();

        foreach ((byte id, PlayerState state) in Main.PlayerStates)
        {
            if (state.RoleHistory.Count > 1)
            {
                string join = string.Join(" > ", state.RoleHistory.ConvertAll(x => x.ToColoredString()));
                sb.AppendLine($"{id.ColoredPlayerName()}: {join} > {state.MainRole.ToColoredString()}");
            }
        }

        if (sb.Length == 0) return;

        sb.Insert(0, $"<size=70%>{GetString("RoleHistoryText")}\n");
        SendMessage("\n", playerId, sb.ToString().Trim() + "</size>");
    }

    public static void ShowKillLog(byte playerId = byte.MaxValue)
    {
        if (GameStates.IsInGame)
        {
            SendMessage(GetString("CantUse.killlog"), playerId, sendOption: SendOption.None);
            return;
        }

        if (EndGamePatch.KillLog != string.Empty) SendMessage(EndGamePatch.KillLog, playerId);
    }

    public static void ShowLastResult(byte playerId = byte.MaxValue)
    {
        if (GameStates.IsInGame)
        {
            SendMessage(GetString("CantUse.lastresult"), playerId, sendOption: SendOption.None);
            return;
        }

        if (Options.CurrentGameMode != CustomGameMode.Standard)
        {
            if (Statistics.WinCountsForOutro.Length > 0) SendMessage("\n", playerId, $"<size=80%>{Statistics.WinCountsForOutro}</size>");
            return;
        }

        StringBuilder sb = new();
        if (SetEverythingUpPatch.LastWinsText != string.Empty) sb.Append($"<size=90%>{GetString("LastResult")} {SetEverythingUpPatch.LastWinsText}</size>");
        if (SetEverythingUpPatch.LastWinsReason != string.Empty) sb.Append($"\n<size=90%>{GetString("LastEndReason")} {SetEverythingUpPatch.LastWinsReason}</size>");
        if (sb.Length > 0) SendMessage("\n", playerId, sb.ToString());
    }

    public static void ShowLastAddOns(byte playerId = byte.MaxValue)
    {
        if (GameStates.IsInGame) return;
        if (Options.CurrentGameMode != CustomGameMode.Standard) return;

        string result = Main.LastAddOns.Values.Join(delimiter: "\n");
        SendMessage("\n", playerId, result);
    }

    public static string GetSubRolesText(byte id, bool disableColor = false, bool intro = false, bool summary = false)
    {
        List<CustomRoles> subRoles = Main.PlayerStates[id].SubRoles;
        if (subRoles.Count == 0) return string.Empty;

        StringBuilder sb = new();

        if (intro)
        {
            bool isLovers = subRoles.Contains(CustomRoles.Lovers) && Main.PlayerStates[id].MainRole is not CustomRoles.LovingCrewmate and not CustomRoles.LovingImpostor;
            subRoles.RemoveAll(x => x is CustomRoles.NotAssigned or CustomRoles.LastImpostor or CustomRoles.Lovers);

            if (isLovers) sb.Append($"{ColorString(GetRoleColor(CustomRoles.Lovers), " ♥")}");

            if (subRoles.Count == 0) return sb.ToString();

            sb.Append("<size=15%>");

            if (subRoles.Count == 1)
            {
                CustomRoles role = subRoles[0];

                string roleText = ColorString(GetRoleColor(role), GetRoleName(role));
                sb.Append($"{ColorString(Color.gray, GetString("Modifier"))}{roleText}");
            }
            else
            {
                sb.Append($"{ColorString(Color.gray, GetString("Modifiers"))}");

                for (var i = 0; i < subRoles.Count; i++)
                {
                    if (i != 0) sb.Append(", ");

                    CustomRoles role = subRoles[i];

                    string roleText = ColorString(GetRoleColor(role), GetRoleName(role));
                    sb.Append(roleText);
                }
            }

            sb.Append("</size>");
        }
        else if (!summary)
        {
            foreach (CustomRoles role in subRoles)
            {
                if (role is CustomRoles.NotAssigned or CustomRoles.LastImpostor) continue;

                string roleText = disableColor ? GetRoleName(role) : ColorString(GetRoleColor(role), GetRoleName(role));
                sb.Append($"{ColorString(Color.gray, " + ")}{roleText}");
            }
        }

        return sb.ToString();
    }

    public static byte MsgToColor(string text, bool isHost = false)
    {
        text = text.ToLowerInvariant();
        text = text.Replace("色", string.Empty);
        int color;

        try { color = int.Parse(text); }
        catch { color = -1; }

        color = text switch
        {
            "0" or "红" or "紅" or "red" or "Red" or "крас" or "Крас" or "красн" or "Красн" or "красный" or "Красный" or "Vermelho" or "vermelho" => 0,
            "1" or "蓝" or "藍" or "深蓝" or "blue" or "Blue" or "син" or "Син" or "синий" or "Синий" or "Azul" or "azul" => 1,
            "2" or "绿" or "綠" or "深绿" or "green" or "Green" or "Зел" or "зел" or "Зелёный" or "Зеленый" or "зелёный" or "зеленый" or "Verde" or "verde" or "Verde-Escuro" or "verde-escuro" => 2,
            "3" or "粉红" or "pink" or "Pink" or "Роз" or "роз" or "Розовый" or "розовый" or "Rosa" or "rosa" => 3,
            "4" or "橘" or "orange" or "Orange" or "оранж" or "Оранж" or "оранжевый" or "Оранжевый" or "Laranja" or "laranja" => 4,
            "5" or "黄" or "黃" or "yellow" or "Yellow" or "Жёлт" or "Желт" or "жёлт" or "желт" or "Жёлтый" or "Желтый" or "жёлтый" or "желтый" or "Amarelo" or "amarelo" => 5,
            "6" or "黑" or "black" or "Black" or "Чёрный" or "Черный" or "чёрный" or "черный" or "Чёрн" or "Черн" or "чёрн" or "черн" or "Preto" or "preto" => 6,
            "7" or "白" or "white" or "White" or "Белый" or "белый" or "Бел" or "бел" or "Branco" or "branco" => 7,
            "8" or "紫" or "purple" or "Purple" or "Фиол" or "фиол" or "Фиолетовый" or "фиолетовый" or "Roxo" or "roxo" => 8,
            "9" or "棕" or "brown" or "Brown" or "Корич" or "корич" or "Коричневый" or "коричевый" or "Marrom" or "marrom" => 9,
            "10" or "青" or "cyan" or "Cyan" or "Голуб" or "голуб" or "Голубой" or "голубой" or "Циановый" or "циановый" or "Циан" or "циан" or "Ciano" or "ciano" => 10,
            "11" or "黄绿" or "黃綠" or "浅绿" or "lime" or "Lime" or "Лайм" or "лайм" or "Лаймовый" or "лаймовый" or "Салатовый" or "салатовый" or "Салат" or "салат" or "Lima" or "lima" or "Verde-Claro" or "verde-claro" => 11,
            "12" or "红褐" or "紅褐" or "深红" or "maroon" or "Maroon" or "Борд" or "борд" or "Бордо" or "бордо" or "Бордовый" or "бордовый" or "Bordô" or "bordô" or "Vinho" or "vinho" => 12,
            "13" or "玫红" or "玫紅" or "浅粉" or "rose" or "Rose" or "Светло роз" or "светло роз" or "Светло розовый" or "светло розовый" or "Сирень" or "сирень" or "Сиреневый" or "сиреневый" or "Rosê" or "rosê" or "rosinha" or "Rosinha" or "Rosa-Claro" or "rosa-claro" => 13,
            "14" or "焦黄" or "焦黃" or "淡黄" or "banana" or "Banana" or "Банан" or "банан" or "Банановый" or "банановый" or "Amarelo-Claro" or "amarelo-claro" => 14,
            "15" or "灰" or "gray" or "Gray" or "Сер" or "сер" or "Серый" or "серый" or "Cinza" or "cinza" => 15,
            "16" or "茶" or "tan" or "Tan" or "Загар" or "загар" or "Загаровый" or "загаровый" or "Беж" or "беж" or "Бежевый" or "бежевый" or "bege" or "bege" or "Creme" or "creme" => 16,
            "17" or "珊瑚" or "coral" or "Coral" or "Корал" or "корал" or "Коралл" or "коралл" or "Коралловый" or "коралловый" => 17,
            "18" or "隐藏" or "?" or "Fortegreen" or "fortegreen" or "Фортгрин" or "фортгрин" or "Форт" or "форт" => 18,
            _ => color
        };

        return !isHost && color == 18 ? byte.MaxValue : color is < 0 or > 18 ? byte.MaxValue : Convert.ToByte(color);
    }

    public static void ShowHelp(byte id)
    {
        PlayerControl player = GetPlayerById(id);
        SendMessage(ChatCommands.AllCommands.Where(x => x.CanUseCommand(player, false) && !x.CommandForms.Contains("help")).Aggregate("<size=70%>", (s, c) => s + $"\n<b>/{c.CommandForms.TakeWhile(f => f.All(char.IsAscii)).MinBy(f => f.Length)}{(c.Arguments.Length == 0 ? string.Empty : $" {c.Arguments.Split(' ').Select((x, i) => id == 0 ? ColorString(GetColor(i), x) : x).Join(delimiter: " ")}")}</b> \u2192 {c.Description}"), id, GetString("CommandList"));
        return;

        Color GetColor(int i) =>
            i switch
            {
                0 => Palette.Orange,
                1 => Color.magenta,
                2 => id == 0 && Main.DarkTheme.Value ? Color.yellow : Color.blue,
                3 => Color.red,
                4 => Color.cyan,
                5 => Color.green,
                6 => Palette.Brown,
                7 => Palette.Purple,

                _ => Color.white
            };
    }

    private static void CheckTerroristWin(NetworkedPlayerInfo terrorist)
    {
        if (!AmongUsClient.Instance.AmHost) return;

        TaskState taskState = GetPlayerById(terrorist.PlayerId).GetTaskState();

        if (taskState.IsTaskFinished && (!Main.PlayerStates[terrorist.PlayerId].IsSuicide || Options.CanTerroristSuicideWin.GetBool()))
        {
            foreach (PlayerControl pc in Main.AllPlayerControls)
            {
                if (pc.Is(CustomRoles.Terrorist))
                    Main.PlayerStates[pc.PlayerId].deathReason = Main.PlayerStates[pc.PlayerId].deathReason == PlayerState.DeathReason.Vote ? PlayerState.DeathReason.etc : PlayerState.DeathReason.Suicide;
                else if (pc.IsAlive()) pc.Suicide(PlayerState.DeathReason.Bombed, terrorist.Object);
            }

            CustomWinnerHolder.ResetAndSetWinner(CustomWinner.Terrorist);
            CustomWinnerHolder.WinnerIds.Add(terrorist.PlayerId);
        }
    }

    public static void CheckAndSpawnAdditionalRefugee(NetworkedPlayerInfo deadPlayer)
    {
        try
        {
            if (Options.CurrentGameMode != CustomGameMode.Standard || deadPlayer == null || deadPlayer.Object.Is(CustomRoles.Refugee) || Main.HasJustStarted || !GameStates.InGame || !Options.SpawnAdditionalRefugeeOnImpsDead.GetBool() || Main.AllAlivePlayerControls.Length < Options.SpawnAdditionalRefugeeMinAlivePlayers.GetInt() || CustomRoles.Refugee.RoleExist(true) || Main.AllAlivePlayerControls == null || Main.AllAlivePlayerControls.Length == 0 || Main.AllAlivePlayerControls.Any(x => x.PlayerId != deadPlayer.PlayerId && (x.Is(CustomRoleTypes.Impostor) || (x.IsNeutralKiller() && !Options.SpawnAdditionalRefugeeWhenNKAlive.GetBool())))) return;

            PlayerControl[] listToChooseFrom = Main.AllAlivePlayerControls.Where(x => x.PlayerId != deadPlayer.PlayerId && x.Is(CustomRoleTypes.Crewmate) && !x.Is(CustomRoles.Loyal)).ToArray();

            if (listToChooseFrom.Length > 0)
            {
                PlayerControl pc = listToChooseFrom.RandomElement();
                pc.RpcSetCustomRole(CustomRoles.Refugee);
                pc.SetKillCooldown();
                Main.PlayerStates[pc.PlayerId].RemoveSubRole(CustomRoles.Madmate);
                Logger.Warn($"{pc.GetRealName()} is now a Refugee since all Impostors are dead", "Add Refugee");
            }
            else
                Logger.Msg("No Player to change to Refugee.", "Add Refugee");
        }
        catch (Exception e) { ThrowException(e); }
    }

    public static void SendMultipleMessages(this IEnumerable<Message> messages, SendOption sendOption = SendOption.Reliable)
    {
        var sender = CustomRpcSender.Create("Utils.SendMultipleMessages", sendOption);
        sender = messages.Aggregate(sender, (current, message) => SendMessage(message.Text, message.SendTo, message.Title, writer: current, multiple: true, sendOption: sendOption));
        sender.SendMessage(dispose: sender.stream.Length <= 3);
    }

    public static CustomRpcSender SendMessage(string text, byte sendTo = byte.MaxValue, string title = "", bool noSplit = false, CustomRpcSender writer = null, bool final = false, bool multiple = false, SendOption sendOption = SendOption.Reliable, bool addtoHistory = true)
    {
        try
        {
            PlayerControl receiver = GetPlayerById(sendTo, false);
            if (sendTo != byte.MaxValue && receiver == null || title.RemoveHtmlTags().Trim().Length == 0 && text.RemoveHtmlTags().Trim().Length == 0) return writer;

            if (!AmongUsClient.Instance.AmHost)
            {
                if (sendTo == PlayerControl.LocalPlayer.PlayerId && !multiple)
                {
                    MessageWriter w = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RequestSendMessage, SendOption.Reliable, AmongUsClient.Instance.HostId);
                    w.Write(text);
                    w.Write(sendTo);
                    w.Write(title);
                    w.Write(noSplit);
                    AmongUsClient.Instance.FinishRpcImmediately(w);
                }

                return writer;
            }

            if (title == "") title = "<color=#8b32a8>" + GetString("DefaultSystemMessageTitle") + "</color>";

            if (title.Count(x => x == '\u2605') == 2 && !title.Contains('\n'))
            {
                if (title.Contains('<') && title.Contains('>') && title.Contains('#'))
                    title = $"{title[..(title.IndexOf('>') + 1)]}\u27a1{title.Replace("\u2605", "")[..(title.LastIndexOf('<') - 2)]}\u2b05";
                else
                    title = "\u27a1" + title.Replace("\u2605", "") + "\u2b05";
            }

            text = text.Replace("color=", string.Empty);
            title = title.Replace("color=", string.Empty);

            PlayerControl sender = !addtoHistory ? PlayerControl.LocalPlayer : Main.AllAlivePlayerControls.MinBy(x => x.PlayerId) ?? Main.AllPlayerControls.MinBy(x => x.PlayerId) ?? PlayerControl.LocalPlayer;

            if (sendTo != byte.MaxValue && receiver.IsLocalPlayer())
            {
                string name = sender.Data.PlayerName;
                sender.SetName(title);
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, text);
                sender.SetName(name);

                try
                {
                    string pureText = text.RemoveHtmlTags();
                    string pureTitle = title.RemoveHtmlTags();
                    Logger.Info($" Message: {pureText[..(pureText.Length <= 300 ? pureText.Length : 300)]} - To: {GetPlayerById(sendTo)?.GetRealName()} - Title: {pureTitle[..(pureTitle.Length <= 300 ? pureTitle.Length : 300)]}", "SendMessage");
                }
                catch { Logger.Info(" Message sent", "SendMessage"); }

                if (addtoHistory) ChatUpdatePatch.LastMessages.Add((text, sendTo, title, TimeStamp));
                return writer;
            }

            int targetClientId = sendTo == byte.MaxValue ? -1 : receiver.OwnerId;

            if (writer == null || writer.CurrentState == CustomRpcSender.State.Finished)
                writer = CustomRpcSender.Create("Utils.SendMessage(1)", sendOption);

            int fullRpcSizeLimit = Options.MessageRpcSizeLimit.GetInt();
            int textRpcSize = text.Length * 2;
            int titleRpcSize = title.Length * 2 + 4;
            int resetNameRpcSize = sender.Data.PlayerName.Length * 2 + 4;
            int fullRpcSize = textRpcSize + titleRpcSize + resetNameRpcSize;

            if (!noSplit)
            {
                int titleRpcSizeLimit = fullRpcSizeLimit - textRpcSize - resetNameRpcSize;

                if ((fullRpcSize <= fullRpcSizeLimit && titleRpcSizeLimit <= titleRpcSize) || title.Length <= 100)
                {
                    writer.AutoStartRpc(sender.NetId, RpcCalls.SetName, targetClientId)
                        .Write(sender.Data.NetId)
                        .Write(title)
                        .EndRpc();
                }
                else
                {
                    titleRpcSizeLimit = (fullRpcSizeLimit - 8 - resetNameRpcSize) * 2;

                    if (titleRpcSizeLimit - 4 < 1)
                    {
                        Logger.SendInGame(GetString("MessageTooLong"), Color.red);
                        if (!multiple) writer.SendMessage(dispose: true);
                        return writer;
                    }

                    string[] lines = title.Split('\n');
                    var shortenedTitle = string.Empty;

                    foreach (string line in lines)
                    {
                        if (shortenedTitle.Length * 2 + line.Length * 2 + 4 < titleRpcSizeLimit)
                        {
                            shortenedTitle += line + "\n";
                            continue;
                        }

                        if (shortenedTitle.Length * 2 >= titleRpcSizeLimit - 4)
                        {
                            foreach (char[] chars in shortenedTitle.Chunk(titleRpcSizeLimit - 4))
                                writer = SendTempTitleMessage(new string(chars));
                        }
                        else
                            writer = SendTempTitleMessage(shortenedTitle);

                        string sentText = shortenedTitle;
                        shortenedTitle = line + "\n";

                        if (Regex.Matches(sentText, "<size").Count > Regex.Matches(sentText, "</size>").Count)
                        {
                            string sizeTag = Regex.Matches(sentText, @"<size=\d+\.?\d*%?>")[^1].Value;
                            shortenedTitle = sizeTag + shortenedTitle;
                        }
                    }

                    if (shortenedTitle.Length > 0 && !shortenedTitle.IsNullOrWhiteSpace())
                        writer = SendTempTitleMessage(shortenedTitle);

                    if (text == "\n") return writer;

                    title = "‎";

                    if (writer.CurrentState == CustomRpcSender.State.Finished)
                        writer = CustomRpcSender.Create("Utils.SendMessage(2)", sendOption);

                    writer.AutoStartRpc(sender.NetId, RpcCalls.SetName, targetClientId)
                        .Write(sender.Data.NetId)
                        .Write(title)
                        .EndRpc();

                    CustomRpcSender SendTempTitleMessage(string tempTitle)
                    {
                        if (writer.CurrentState == CustomRpcSender.State.Finished)
                            writer = CustomRpcSender.Create("Utils.SendMessage.SendTempTitleMessage", sendOption);

                        writer.AutoStartRpc(sender.NetId, RpcCalls.SetName, targetClientId)
                            .Write(sender.Data.NetId)
                            .Write(tempTitle)
                            .EndRpc();

                        writer.AutoStartRpc(sender.NetId, RpcCalls.SendChat, targetClientId)
                            .Write("\n")
                            .EndRpc();

                        writer.AutoStartRpc(sender.NetId, RpcCalls.SetName, targetClientId)
                            .Write(sender.Data.NetId)
                            .Write(Main.AllPlayerNames.GetValueOrDefault(sender.PlayerId, string.Empty))
                            .EndRpc();

                        writer.SendMessage();

                        try
                        {
                            string pureTitle = tempTitle.RemoveHtmlTags();
                            Logger.Info($" Message: \\n - To: {(sendTo == byte.MaxValue ? "Everyone" : $"{GetPlayerById(sendTo)?.GetRealName()}")} - Title: {pureTitle[..(pureTitle.Length <= 300 ? pureTitle.Length : 300)]}", "SendMessage");
                        }
                        catch { Logger.Info(" Message sent", "SendMessage"); }

                        if (addtoHistory) ChatUpdatePatch.LastMessages.Add(("\n", sendTo, tempTitle, TimeStamp));
                        return writer;
                    }
                }
            }

            titleRpcSize = title.Length * 2 + 4;
            int textRpcSizeLimit = fullRpcSizeLimit - titleRpcSize - resetNameRpcSize;

            if (textRpcSizeLimit < 1 && textRpcSize >= textRpcSizeLimit && !noSplit)
            {
                Logger.SendInGame(GetString("MessageTooLong"), Color.red);
                if (!multiple) writer.SendMessage(dispose: true);
                return writer;
            }

            if (textRpcSize >= textRpcSizeLimit && !noSplit)
            {
                string[] lines = text.Split('\n');
                var shortenedText = string.Empty;

                foreach (string line in lines)
                {
                    if (shortenedText.Length * 2 + line.Length * 2 + 4 < textRpcSizeLimit)
                    {
                        shortenedText += line + "\n";
                        continue;
                    }

                    writer = shortenedText.Length * 2 >= textRpcSizeLimit
                        ? shortenedText.Chunk(textRpcSizeLimit).Aggregate(writer, (current, chunk) => SendMessage(new(chunk), sendTo, title, true, current, sendOption: sendOption))
                        : SendMessage(shortenedText, sendTo, title, true, writer, sendOption: sendOption);

                    string sentText = shortenedText;
                    shortenedText = line + "\n";

                    if (Regex.Matches(sentText, "<size").Count > Regex.Matches(sentText, "</size>").Count)
                    {
                        string sizeTag = Regex.Matches(sentText, @"<size=\d+\.?\d*%?>")[^1].Value;
                        shortenedText = sizeTag + shortenedText;
                    }
                }

                if (shortenedText.Length > 0 && !shortenedText.IsNullOrWhiteSpace()) writer = SendMessage(shortenedText, sendTo, title, true, writer, true, sendOption: sendOption);
                else
                {
                    writer.AutoStartRpc(sender.NetId, RpcCalls.SetName, targetClientId)
                        .Write(sender.Data.NetId)
                        .Write(Main.AllPlayerNames.GetValueOrDefault(sender.PlayerId, string.Empty))
                        .EndRpc();

                    if (!multiple) writer.SendMessage();
                    else RestartMessageIfTooLong();
                }

                return writer;
            }

            try
            {
                string pureText = text.RemoveHtmlTags();
                string pureTitle = title.RemoveHtmlTags();
                Logger.Info($" Message: {pureText[..(pureText.Length <= 300 ? pureText.Length : 300)]} - To: {(sendTo == byte.MaxValue ? "Everyone" : $"{GetPlayerById(sendTo)?.GetRealName()}")} - Title: {pureTitle[..(pureTitle.Length <= 300 ? pureTitle.Length : 300)]}", "SendMessage");
            }
            catch { Logger.Info(" Message sent", "SendMessage"); }

            if (noSplit)
            {
                text = text.TrimStart('\n');
                if (!text.EndsWith('\n')) text += "\n";
                text += "‎";
            }

            if (writer.CurrentState == CustomRpcSender.State.Ready)
            {
                writer.AutoStartRpc(sender.NetId, RpcCalls.SetName, targetClientId)
                    .Write(sender.Data.NetId)
                    .Write(title)
                    .EndRpc();
            }

            writer.AutoStartRpc(sender.NetId, RpcCalls.SendChat, targetClientId)
                .Write(text)
                .EndRpc();

            if (sendTo == byte.MaxValue)
            {
                string name = sender.Data.PlayerName;
                sender.SetName(title);
                FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, text);
                sender.SetName(name);
            }

            if ((noSplit && final) || !noSplit)
            {
                writer.AutoStartRpc(sender.NetId, RpcCalls.SetName, targetClientId)
                    .Write(sender.Data.NetId)
                    .Write(Main.AllPlayerNames.GetValueOrDefault(sender.PlayerId, string.Empty))
                    .EndRpc();

                if (!multiple) writer.SendMessage();
                else RestartMessageIfTooLong();
            }
            else
                RestartMessageIfTooLong();
        }
        catch (Exception e) { ThrowException(e); }

        if (addtoHistory) ChatUpdatePatch.LastMessages.Add((text, sendTo, title, TimeStamp));
        return writer;

        void RestartMessageIfTooLong()
        {
            if (writer.stream.Length > 400)
            {
                writer.SendMessage();
                writer = CustomRpcSender.Create("Utils.SendMessage", sendOption);
            }
        }
    }

    public static HashSet<byte> DirtyName = [];

    public static bool ApplySuffix(PlayerControl player, out string name)
    {
        name = string.Empty;
        if (!AmongUsClient.Instance.AmHost || player == null) return false;

        DevManager.TagInfo devUser = player.FriendCode.GetDevUser();
        bool admin = ChatCommands.IsPlayerAdmin(player.FriendCode);
        bool mod = ChatCommands.IsPlayerModerator(player.FriendCode);
        bool vip = ChatCommands.IsPlayerVIP(player.FriendCode);
        bool hasTag = devUser.HasTag();
        bool hasPrivateTag = PrivateTagManager.Tags.TryGetValue(player.FriendCode, out string privateTag);

        if (!player.AmOwner && !hasTag && !mod && !vip && !hasPrivateTag && !DirtyName.Contains(player.PlayerId)) return false;

        if (!Main.AllPlayerNames.TryGetValue(player.PlayerId, out name)) return false;
        if (Main.NickName != string.Empty && player.AmOwner) name = Main.NickName;

        if (name == string.Empty) return false;

        if (AmongUsClient.Instance.IsGameStarted)
        {
            if (Options.FormatNameMode.GetInt() == 1 && Main.NickName == string.Empty)
            {
                string notFormattedName = Palette.GetColorName(player.Data.DefaultOutfit.ColorId);
                name = char.ToUpper(notFormattedName[0]) + notFormattedName[1..].ToLower();
            }
        }
        else
        {
            if (!GameStates.IsLobby) return false;

            if (player.AmOwner)
            {
                if (GameStates.IsOnlineGame || GameStates.IsLocalGame)
                    name = $"<color={GetString("HostColor")}>{GetString("HostText")}</color><color={GetString("IconColor")}>{GetString("Icon")}</color><color={GetString("NameColor")}>{name}</color>";

                var modeText = $"<size=1.8>{GetString($"Mode{Options.CurrentGameMode}")}</size>";

                name = Options.CurrentGameMode switch
                {
                    CustomGameMode.SoloKombat => $"<color=#f55252>{modeText}</color>\r\n{name}",
                    CustomGameMode.FFA => $"<color=#00ffff>{modeText}</color>\r\n{name}",
                    CustomGameMode.MoveAndStop => $"<color=#00ffa5>{modeText}</color>\r\n{name}",
                    CustomGameMode.HotPotato => $"<color=#e8cd46>{modeText}</color>\r\n{name}",
                    CustomGameMode.HideAndSeek => $"<color=#345eeb>{modeText}</color>\r\n{name}",
                    CustomGameMode.CaptureTheFlag => $"<color=#1313c2>{modeText}</color>\r\n{name}",
                    CustomGameMode.NaturalDisasters => $"<color=#03fc4a>{modeText}</color>\r\n{name}",
                    CustomGameMode.RoomRush => $"<color=#ffab1b>{modeText}</color>\r\n{name}",
                    CustomGameMode.KingOfTheZones => $"<color=#ff0000>{modeText}</color>\r\n{name}",
                    CustomGameMode.TheMindGame => $"<color=#ffff00>{modeText}</color>\r\n{name}",
                    CustomGameMode.Speedrun => ColorString(GetRoleColor(CustomRoles.Speedrunner), $"{modeText}\r\n") + name,
                    CustomGameMode.Quiz => ColorString(GetRoleColor(CustomRoles.QuizMaster), $"{modeText}\r\n") + name,
                    CustomGameMode.BedWars => ColorString(GetRoleColor(CustomRoles.BedWarsPlayer), $"{modeText}\r\n") + name,
                    _ => name
                };
            }

            if (hasTag || mod || vip || hasPrivateTag)
            {
                string pTag = hasPrivateTag ? privateTag : string.Empty;
                string tag = hasTag ? devUser.GetTag() : string.Empty;
                if (tag == "null") tag = string.Empty;

                bool host = player.IsHost();
                string separator = player.AmOwner || player.IsModdedClient() ? "\r\n" : " ";
                string adminTag = host ? string.Empty : $"<size=1.7>{GetString("AdminTag")}{separator}</size>";
                string modTag = host ? string.Empty : $"<size=1.7>{GetString("ModeratorTag")}{separator}</size>";
                string vipTag = host ? string.Empty : $"<size=1.7>{GetString("VIPTag")}{separator}</size>";
                name = $"{(hasTag ? tag.Replace("\r\n", separator) : string.Empty)}{(admin ? adminTag : mod ? modTag : string.Empty)}{(vip ? vipTag : string.Empty)}{pTag}{name}";
            }

            if (player.AmOwner)
            {
                name = Options.GetSuffixMode() switch
                {
                    SuffixModes.EHR => $"{name} (<color={Main.ModColor}>EHR v{Main.PluginDisplayVersion}</color>)",
                    SuffixModes.Streaming => $"{name} (<color={Main.ModColor}>{GetString("SuffixMode.Streaming")}</color>)",
                    SuffixModes.Recording => $"{name} (<color={Main.ModColor}>{GetString("SuffixMode.Recording")}</color>)",
                    SuffixModes.RoomHost => $"{name} (<color={Main.ModColor}>{GetString("SuffixMode.RoomHost")}</color>)",
                    SuffixModes.OriginalName => $"{name} (<color={Main.ModColor}>{DataManager.player.Customization.Name}</color>)",
                    SuffixModes.DoNotKillMe => $"{name} (<color={Main.ModColor}>{GetString("SuffixModeText.DoNotKillMe")}</color>)",
                    SuffixModes.NoAndroidPlz => $"{name} (<color={Main.ModColor}>{GetString("SuffixModeText.NoAndroidPlz")}</color>)",
                    SuffixModes.AutoHost => $"{name} (<color={Main.ModColor}>{GetString("SuffixModeText.AutoHost")}</color>)",
                    _ => name
                };
            }
        }

        return DirtyName.Remove(player.PlayerId) || (name != player.name && player.CurrentOutfitType == PlayerOutfitType.Default);
    }

    public static Dictionary<string, int> GetAllPlayerLocationsCount()
    {
        Dictionary<string, int> playerRooms = [];

        foreach (PlayerControl pc in Main.AllAlivePlayerControls)
        {
            if (!pc.IsAlive() || Pelican.IsEaten(pc.PlayerId)) return null;

            Il2CppReferenceArray<PlainShipRoom> rooms = ShipStatus.Instance.AllRooms;
            if (rooms == null) return null;

            foreach (PlainShipRoom room in rooms)
            {
                if (!room.roomArea) continue;

                if (!pc.Collider.IsTouching(room.roomArea)) continue;

                string roomName = GetString($"{room.RoomId}");
                if (!playerRooms.TryAdd(roomName, 1)) playerRooms[roomName]++;
            }
        }

        return playerRooms;
    }

    public static float GetSettingNameAndValueForRole(CustomRoles role, string settingName)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        FieldInfo field = types.SelectMany(x => x.GetFields(flags)).FirstOrDefault(x => x.Name == $"{role}{settingName}");

        if (field == null)
        {
            FieldInfo tempField = null;

            foreach (Type x in types)
            {
                var any = false;

                foreach (FieldInfo f in x.GetFields(flags))
                {
                    if (f.Name.Contains(settingName))
                    {
                        any = true;
                        tempField = f;
                        break;
                    }
                }

                if (any && x.Name == $"{role}")
                {
                    field = tempField;
                    break;
                }
            }
        }

        float add;

        if (field == null)
            add = float.MaxValue;
        else
        {
            if (field.GetValue(null) is OptionItem optionItem)
                add = optionItem.GetFloat();
            else
                add = float.MaxValue;
        }

        return add;
    }

    public static string ColoredPlayerName(this byte id)
    {
        return ColorString(Main.PlayerColors.GetValueOrDefault(id, Color.white), Main.AllPlayerNames.GetValueOrDefault(id, GetPlayerById(id)?.GetRealName() ?? $"Someone (ID {id})"));
    }

    public static PlayerControl GetPlayer(this byte id)
    {
        return GetPlayerById(id);
    }

    public static PlayerControl GetPlayerById(int playerId, bool fast = true)
    {
        if (playerId is > byte.MaxValue or < byte.MinValue) return null;

        if (fast && GameStates.InGame && Main.PlayerStates.TryGetValue((byte)playerId, out PlayerState state) && state.Player != null) return state.Player;

        if (playerId == PlayerControl.LocalPlayer.PlayerId) return PlayerControl.LocalPlayer;

        for (int i = 0; i < PlayerControl.AllPlayerControls.Count; i++)
        {
            PlayerControl pc = PlayerControl.AllPlayerControls[i];

            if (pc.PlayerId == playerId)
                return pc;
        }

        return null;
    }

    public static NetworkedPlayerInfo GetPlayerInfoById(int playerId)
    {
        for (int i = 0; i < GameData.Instance.AllPlayers.Count; i++)
        {
            NetworkedPlayerInfo info = GameData.Instance.AllPlayers[i];

            if (info.PlayerId == playerId)
                return info;
        }

        return null;
    }

    public static void SetupLongRoleDescriptions()
    {
        try
        {
            LongRoleDescriptions.Clear();

            int charsInOneLine = GetUserTrueLang() is SupportedLangs.Russian or SupportedLangs.SChinese or SupportedLangs.TChinese or SupportedLangs.Japanese or SupportedLangs.Korean ? 35 : 50;

            foreach (PlayerControl seer in Main.AllPlayerControls)
            {
                try
                {
                    string longInfo = seer.GetRoleInfo(true).Split("\n\n")[0];
                    if (longInfo.Contains("):\n")) longInfo = longInfo.Split("):\n")[1];

                    var tooLong = false;
                    bool showLongInfo = Options.ShowLongInfo.GetBool();

                    if (showLongInfo)
                    {
                        if (longInfo.Length > 296)
                        {
                            longInfo = longInfo[..296];
                            longInfo += "...";
                            tooLong = true;
                        }

                        for (int i = charsInOneLine; i < longInfo.Length; i += charsInOneLine)
                        {
                            if (tooLong && i > 296) break;

                            int index = longInfo.LastIndexOf(' ', i);
                            if (index != -1) longInfo = longInfo.Insert(index + 1, "\n");
                        }
                    }

                    longInfo = $"<#ffffff>{longInfo}</color>";

                    int lines = longInfo.Count(x => x == '\n');
                    int readTime = 20 + (lines * 5);

                    LongRoleDescriptions[seer.PlayerId] = (longInfo, readTime, tooLong);
                }
                catch (Exception e) { ThrowException(e); }
            }
        }
        catch (Exception e) { ThrowException(e); }
    }

    public static string BuildSuffix(PlayerControl seer, PlayerControl target, bool hud = false, bool meeting = false)
    {
        StringBuilder suffix = new();

        foreach (PlayerState state in Main.PlayerStates.Values)
        {
            string tempSuffix = state.Role.GetSuffix(seer, target, hud, meeting).Trim();

            if (!string.IsNullOrEmpty(tempSuffix))
                suffix.Append($"{tempSuffix}\n");
        }

        suffix.Append(CustomSabotage.GetAllSuffix(seer, target, hud, meeting));

        return suffix.ToString().Trim();
    }

    public static IEnumerator NotifyEveryoneAsync(int speed = 2, bool noCache = true)
    {
        if (GameStates.IsMeeting) yield break;

        var count = 0;
        PlayerControl[] aapc = Main.AllAlivePlayerControls;

        foreach (PlayerControl seer in aapc)
        {
            foreach (PlayerControl target in aapc)
            {
                if (GameStates.IsMeeting) yield break;
                var sender = CustomRpcSender.Create("Utils.NotifyEveryoneAsync", SendOption.Reliable, log: false);
                var hasValue = WriteSetNameRpcsToSender(ref sender, false, noCache, false, false, false, false, seer, [seer], [target], out bool senderWasCleared) && !senderWasCleared;
                sender.SendMessage(!hasValue || sender.stream.Length <= 3);
                if (count++ % speed == 0) yield return null;
            }
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static void NotifyRoles(bool ForMeeting = false, PlayerControl SpecifySeer = null, PlayerControl SpecifyTarget = null, bool NoCache = false, bool ForceLoop = false, bool CamouflageIsForMeeting = false, bool GuesserIsForMeeting = false, bool MushroomMixup = false, SendOption SendOption = SendOption.Reliable)
    {
        try
        {
            if (!SetUpRoleTextPatch.IsInIntro && ((SpecifySeer != null && SpecifySeer.IsModdedClient() && (Options.CurrentGameMode == CustomGameMode.Standard || SpecifySeer.IsHost())) || !AmongUsClient.Instance.AmHost || (GameStates.IsMeeting && !ForMeeting) || GameStates.IsLobby)) return;

            PlayerControl[] apc = Main.AllPlayerControls;
            PlayerControl[] seerList = SpecifySeer != null ? [SpecifySeer] : apc;
            PlayerControl[] targetList = SpecifyTarget != null ? [SpecifyTarget] : apc;

            var sender = CustomRpcSender.Create("NotifyRoles", SendOption, log: false);
            var hasValue = false;

            foreach (PlayerControl seer in seerList)
            {
                hasValue |= WriteSetNameRpcsToSender(ref sender, ForMeeting, NoCache, ForceLoop, CamouflageIsForMeeting, GuesserIsForMeeting, MushroomMixup, seer, seerList, targetList, out bool senderWasCleared, SendOption);
                if (senderWasCleared) hasValue = false;

                if (sender.stream.Length > 500)
                {
                    sender.SendMessage();
                    sender = CustomRpcSender.Create("NotifyRoles", SendOption, log: false);
                    hasValue = false;
                }
            }

            sender.SendMessage(!hasValue || sender.stream.Length <= 3);

            if (Options.CurrentGameMode != CustomGameMode.Standard) return;

            string seers = seerList.Length == apc.Length ? "Everyone" : string.Join(", ", seerList.Select(x => x.GetRealName()));
            string targets = targetList.Length == apc.Length ? "Everyone" : string.Join(", ", targetList.Select(x => x.GetRealName()));

            if (seers.Length == 0) seers = "\u2205";
            if (targets.Length == 0) targets = "\u2205";

            Logger.Info($" Seers: {seers} ---- Targets: {targets}", "NR");
        }
        catch (Exception e) { ThrowException(e); }
    }

    public static bool WriteSetNameRpcsToSender(ref CustomRpcSender sender, bool forMeeting, bool noCache, bool forceLoop, bool camouflageIsForMeeting, bool guesserIsForMeeting, bool mushroomMixup, PlayerControl seer, PlayerControl[] seerList, PlayerControl[] targetList, out bool senderWasCleared, SendOption sendOption = SendOption.Reliable)
    {
        long now = TimeStamp;
        var hasValue = false;
        senderWasCleared = false;

        try
        {
            if (seer == null || seer.Data.Disconnected || (seer.IsModdedClient() && (seer.IsHost() || Options.CurrentGameMode == CustomGameMode.Standard)) || (!SetUpRoleTextPatch.IsInIntro && GameStates.IsLobby))
                return false;

            sender ??= CustomRpcSender.Create("NotifyRoles", sendOption);

            // During the intro scene, set the team name for non-modded clients and skip the rest.
            string selfName;
            Team seerTeam = seer.GetTeam();
            CustomRoles seerRole = seer.GetCustomRole();

            if (SetUpRoleTextPatch.IsInIntro && (seerRole.IsDesyncRole() || seer.Is(CustomRoles.Bloodlust)) && Options.CurrentGameMode == CustomGameMode.Standard)
            {
                const string iconTextLeft = "<color=#ffffff>\u21e8</color>";
                const string iconTextRight = "<color=#ffffff>\u21e6</color>";
                const string roleNameUp = "</size><size=1450%>\n \n</size>";

                var selfTeamName = $"<size=450%>{iconTextLeft} <font=\"VCR SDF\" material=\"VCR Black Outline\">{ColorString(seerTeam.GetColor(), $"{seerTeam}")}</font> {iconTextRight}</size><size=500%>\n \n</size>";
                selfName = $"{selfTeamName}\r\n<size=150%>{seerRole.ToColoredString()}</size>{roleNameUp}";

                sender.RpcSetName(seer, selfName, seer);
                return true;
            }

            if (seer.Is(CustomRoles.Car) && !forMeeting)
            {
                sender.RpcSetName(seer, Car.Name);
                return true;
            }

            if (forMeeting && Magistrate.CallCourtNextMeeting)
            {
                selfName = seer.Is(CustomRoles.Magistrate) ? GetString("Magistrate.CourtName") : GetString("Magistrate.JuryName");
                sender.RpcSetName(seer, selfName);
                return true;
            }

            var fontSize = "1.7";

            if (forMeeting && (seer.GetClient().PlatformData.Platform == Platforms.Playstation || seer.GetClient().PlatformData.Platform == Platforms.Switch))
                fontSize = "70%";

            // Text containing progress, such as tasks
            string selfTaskText = GameStates.IsLobby ? string.Empty : GetProgressText(seer);

            SelfMark.Clear();
            SelfSuffix.Clear();

            if (!GameStates.IsLobby)
            {
                if (Options.CurrentGameMode != CustomGameMode.Standard) goto GameMode0;

                SelfMark.Append(Snitch.GetWarningArrow(seer));
                if (Main.LoversPlayers.Exists(x => x.PlayerId == seer.PlayerId)) SelfMark.Append(ColorString(GetRoleColor(CustomRoles.Lovers), " ♥"));

                if (BallLightning.IsGhost(seer)) SelfMark.Append(ColorString(GetRoleColor(CustomRoles.BallLightning), "■"));

                SelfMark.Append(Medic.GetMark(seer, seer));
                SelfMark.Append(Gaslighter.GetMark(seer, seer, forMeeting));
                SelfMark.Append(Gamer.TargetMark(seer, seer));
                SelfMark.Append(Sniper.GetShotNotify(seer.PlayerId));
                if (Silencer.ForSilencer.Contains(seer.PlayerId)) SelfMark.Append(ColorString(GetRoleColor(CustomRoles.Silencer), "╳"));

                GameMode0:

                List<string> additionalSuffixes = [];

                if (Options.CurrentGameMode is not CustomGameMode.Standard and not CustomGameMode.HideAndSeek) goto GameMode;

                SelfSuffix.Append(BuildSuffix(seer, seer, meeting: forMeeting));

                foreach (var suffix in new[] { Spurt.GetSuffix(seer), Dynamo.GetSuffix(seer), CustomTeamManager.GetSuffix(seer) })
                {
                    var trimmedSuffix = suffix.Trim();
                    if (!string.IsNullOrEmpty(trimmedSuffix)) SelfSuffix.Append("\n" + trimmedSuffix);
                }

                if (!forMeeting)
                {
                    if (Options.UsePets.GetBool() && Main.AbilityCD.TryGetValue(seer.PlayerId, out (long StartTimeStamp, int TotalCooldown) time))
                    {
                        long remainingCD = time.TotalCooldown - (now - time.StartTimeStamp) + 1;
                        SelfSuffix.Append("\n" + string.Format(GetString("CDPT"), remainingCD > 60 ? "> 60" : remainingCD));
                    }

                    if (seer.Is(CustomRoles.Asthmatic)) additionalSuffixes.Add(Asthmatic.GetSuffixText(seer.PlayerId));
                    if (seer.Is(CustomRoles.Sonar)) additionalSuffixes.Add(Sonar.GetSuffix(seer, false));
                    if (seer.Is(CustomRoles.Deadlined)) additionalSuffixes.Add(Deadlined.GetSuffix(seer));
                    if (seer.Is(CustomRoles.Introvert)) additionalSuffixes.Add(Introvert.GetSelfSuffix(seer));
                    if (seer.Is(CustomRoles.Allergic)) additionalSuffixes.Add(Allergic.GetSelfSuffix(seer));

                    additionalSuffixes.Add(Bloodmoon.GetSuffix(seer));
                    additionalSuffixes.Add(Haunter.GetSuffix(seer));

                    switch (seerRole)
                    {
                        case CustomRoles.SuperStar when Options.EveryOneKnowSuperStar.GetBool():
                            SelfMark.Append(ColorString(GetRoleColor(CustomRoles.SuperStar), "★"));
                            break;
                        case CustomRoles.Monitor:
                            if (AntiAdminer.IsAdminWatch) additionalSuffixes.Add($"{GetString("AntiAdminerAD")} <size=70%>({AntiAdminer.PlayersNearDevices.Where(x => x.Value.Contains(AntiAdminer.Device.Admin)).Select(x => x.Key.ColoredPlayerName()).Join()})</size>");
                            if (AntiAdminer.IsVitalWatch) additionalSuffixes.Add($"{GetString("AntiAdminerVI")} <size=70%>({AntiAdminer.PlayersNearDevices.Where(x => x.Value.Contains(AntiAdminer.Device.Vitals)).Select(x => x.Key.ColoredPlayerName()).Join()})</size>");
                            if (AntiAdminer.IsDoorLogWatch) additionalSuffixes.Add($"{GetString("AntiAdminerDL")} <size=70%>({AntiAdminer.PlayersNearDevices.Where(x => x.Value.Contains(AntiAdminer.Device.DoorLog)).Select(x => x.Key.ColoredPlayerName()).Join()})</size>");
                            if (AntiAdminer.IsCameraWatch) additionalSuffixes.Add($"{GetString("AntiAdminerCA")} <size=70%>({AntiAdminer.PlayersNearDevices.Where(x => x.Value.Contains(AntiAdminer.Device.Camera)).Select(x => x.Key.ColoredPlayerName()).Join()})</size>");
                            break;
                        case CustomRoles.AntiAdminer:
                            if (AntiAdminer.IsAdminWatch) additionalSuffixes.Add(GetString("AntiAdminerAD"));
                            if (AntiAdminer.IsVitalWatch) additionalSuffixes.Add(GetString("AntiAdminerVI"));
                            if (AntiAdminer.IsDoorLogWatch) additionalSuffixes.Add(GetString("AntiAdminerDL"));
                            if (AntiAdminer.IsCameraWatch) additionalSuffixes.Add(GetString("AntiAdminerCA"));
                            break;
                    }
                }
                else
                {
                    SelfMark.Append(Witch.GetSpelledMark(seer.PlayerId, true));
                    SelfMark.Append(Wasp.GetStungMark(seer.PlayerId));
                    SelfMark.Append(SpellCaster.HasSpelledMark(seer.PlayerId) ? ColorString(Team.Coven.GetColor(), "\u25c0") : string.Empty);
                }

                GameMode:

                switch (Options.CurrentGameMode)
                {
                    case CustomGameMode.FFA:
                        additionalSuffixes.Add(FreeForAll.GetPlayerArrow(seer));
                        break;
                    case CustomGameMode.SoloKombat:
                        additionalSuffixes.Add(SoloPVP.GetDisplayHealth(seer, true));
                        break;
                    case CustomGameMode.MoveAndStop:
                        additionalSuffixes.Add(MoveAndStop.GetSuffixText(seer));
                        break;
                    case CustomGameMode.HotPotato:
                        additionalSuffixes.Add(HotPotato.GetSuffixText(seer.PlayerId));
                        break;
                    case CustomGameMode.Speedrun:
                        additionalSuffixes.Add(Speedrun.GetSuffixText(seer));
                        break;
                    case CustomGameMode.HideAndSeek:
                        additionalSuffixes.Add(CustomHnS.GetSuffixText(seer, seer));
                        break;
                    case CustomGameMode.CaptureTheFlag:
                        additionalSuffixes.Add(CaptureTheFlag.GetSuffixText(seer, seer));
                        break;
                    case CustomGameMode.NaturalDisasters:
                        additionalSuffixes.Add(NaturalDisasters.SuffixText());
                        break;
                    case CustomGameMode.RoomRush:
                        additionalSuffixes.Add(RoomRush.GetSuffix(seer));
                        break;
                    case CustomGameMode.KingOfTheZones:
                        additionalSuffixes.Add(KingOfTheZones.GetSuffix(seer));
                        break;
                    case CustomGameMode.Quiz:
                        additionalSuffixes.Add(Quiz.GetSuffix(seer));
                        break;
                    case CustomGameMode.TheMindGame:
                        additionalSuffixes.Add(TheMindGame.GetSuffix(seer, seer));
                        break;
                    case CustomGameMode.BedWars:
                        additionalSuffixes.Add(BedWars.GetSuffix(seer, seer));
                        break;
                }

                SelfSuffix.Append(string.Join('\n', additionalSuffixes.ConvertAll(x => x.Trim()).FindAll(x => !string.IsNullOrEmpty(x))));
            }

            string seerRealName = seer.GetRealName(forMeeting);

            if (seer.Is(CustomRoles.BananaMan))
                seerRealName = seerRealName.Insert(0, $"{GetString("Prefix.BananaMan")} ");

            if (!GameStates.IsLobby)
            {
                if ((Options.CurrentGameMode == CustomGameMode.FFA && FreeForAll.FFATeamMode.GetBool()) || Options.CurrentGameMode == CustomGameMode.HotPotato)
                    seerRealName = seerRealName.ApplyNameColorData(seer, seer, forMeeting);

                if (!forMeeting && MeetingStates.FirstMeeting && Options.ChangeNameToRoleInfo.GetBool() && Options.CurrentGameMode is not CustomGameMode.FFA and not CustomGameMode.MoveAndStop and not CustomGameMode.HotPotato and not CustomGameMode.Speedrun and not CustomGameMode.CaptureTheFlag and not CustomGameMode.NaturalDisasters and not CustomGameMode.RoomRush and not CustomGameMode.KingOfTheZones and not CustomGameMode.Quiz and not CustomGameMode.TheMindGame and not CustomGameMode.BedWars)
                {
                    CustomTeamManager.CustomTeam team = CustomTeamManager.GetCustomTeam(seer.PlayerId);

                    if (team != null)
                    {
                        seerRealName = ColorString(
                            team.RoleRevealScreenBackgroundColor == "*" || !ColorUtility.TryParseHtmlString(team.RoleRevealScreenBackgroundColor, out Color teamColor)
                                ? Color.yellow
                                : teamColor,
                            string.Format(
                                GetString("CustomTeamHelp"),
                                team.RoleRevealScreenTitle == "*"
                                    ? team.TeamName
                                    : team.RoleRevealScreenTitle,
                                team.RoleRevealScreenSubtitle == "*"
                                    ? string.Empty
                                    : team.RoleRevealScreenSubtitle));
                    }
                    else if (Options.CurrentGameMode == CustomGameMode.HideAndSeek)
                    {
                        if (GameStartTimeStamp + 40 > now) seerRealName = CustomHnS.GetRoleInfoText(seer);
                    }
                    else if (Options.ChangeNameToRoleInfo.GetBool() && !seer.IsModdedClient() && Options.CurrentGameMode == CustomGameMode.Standard)
                    {
                        bool showLongInfo = LongRoleDescriptions.TryGetValue(seer.PlayerId, out (string Text, int Duration, bool Long) description) && GameStartTimeStamp + description.Duration > now;
                        string mHelp = !showLongInfo || description.Long ? "\n" + GetString("MyRoleCommandHelp") : string.Empty;
                        string color = seerTeam.GetTextColor();
                        string teamStr = seerTeam == Team.Impostor && seer.IsMadmate() ? "Madmate" : seerTeam.ToString();
                        string info = (showLongInfo ? description.Text : seer.GetRoleInfo()) + mHelp;
                        seerRealName = $"<color={color}>{GetString($"YouAre{teamStr}")}</color>\n<size=90%>{info}</size>";
                    }
                }

                if (GameStartTimeStamp + 44 > TimeStamp && Main.HasPlayedGM.TryGetValue(Options.CurrentGameMode, out HashSet<string> playedFCs) && !playedFCs.Contains(seer.FriendCode))
                    SelfSuffix.Append($"\n\n<#ffffff>{GetString($"GameModeTutorial.{Options.CurrentGameMode}")}</color>\n");
            }

            bool noRoleText = GameStates.IsLobby || Options.CurrentGameMode is CustomGameMode.CaptureTheFlag or CustomGameMode.NaturalDisasters or CustomGameMode.RoomRush or CustomGameMode.KingOfTheZones or CustomGameMode.Quiz or CustomGameMode.TheMindGame or CustomGameMode.BedWars;

            // Combine the seer's job title and SelfTaskText with the seer's player name and SelfMark
            string selfRoleName = noRoleText ? string.Empty : $"<size={fontSize}>{seer.GetDisplayRoleName()}{selfTaskText}</size>";
            string selfDeathReason = seer.KnowDeathReason(seer) && !noRoleText ? $"\n<size=1.5>『{ColorString(GetRoleColor(CustomRoles.Doctor), GetVitalText(seer.PlayerId))}』</size>" : string.Empty;
            selfName = $"{ColorString(noRoleText ? Color.white : seer.GetRoleColor(), seerRealName)}{selfDeathReason}{SelfMark}";

            if (Options.CurrentGameMode != CustomGameMode.Standard || GameStates.IsLobby) goto GameMode2;

            selfName = seerRole switch
            {
                CustomRoles.Arsonist when seer.IsDouseDone() => $"{ColorString(seer.GetRoleColor(), GetString("EnterVentToWin"))}",
                CustomRoles.Revolutionist when seer.IsDrawDone() => $">{ColorString(seer.GetRoleColor(), string.Format(GetString("EnterVentWinCountDown"), Revolutionist.RevolutionistCountdown.GetValueOrDefault(seer.PlayerId, 10)))}",
                _ => selfName
            };

            if (Pelican.IsEaten(seer.PlayerId))
                selfName = $"{ColorString(GetRoleColor(CustomRoles.Pelican), GetString("EatenByPelican"))}";

            if (Deathpact.IsInActiveDeathpact(seer))
                selfName = Deathpact.GetDeathpactString(seer);

            // Devourer
            if (Devourer.HideNameOfConsumedPlayer.GetBool() && Devourer.PlayerIdList.Any(x => Main.PlayerStates[x].Role is Devourer { IsEnable: true } dv && dv.PlayerSkinsCosumed.Contains(seer.PlayerId)) && !camouflageIsForMeeting)
                selfName = GetString("DevouredName");

            // Camouflage
            if (Camouflage.IsCamouflage && !camouflageIsForMeeting)
                selfName = $"<size=0>{selfName}</size>";

            GameMode2:

            if (!GameStates.IsLobby)
            {
                if (Options.CurrentGameMode is CustomGameMode.Quiz or CustomGameMode.BedWars)
                    selfName = string.Empty;

                if (Options.CurrentGameMode != CustomGameMode.BedWars && NameNotifyManager.GetNameNotify(seer, out string name) && name.Length > 0)
                    selfName = name;

                switch (Options.CurrentGameMode)
                {
                    case CustomGameMode.SoloKombat:
                        SoloPVP.GetNameNotify(seer, ref selfName);
                        selfName = $"<size={fontSize}>{selfTaskText}</size>\r\n{selfName}";
                        break;
                    case CustomGameMode.FFA:
                        selfName = $"<size={fontSize}>{selfTaskText}</size>\r\n{selfName}";
                        break;
                    default:
                        selfName = $"{selfRoleName}\r\n{selfName}";
                        break;
                }

                selfName += SelfSuffix.ToString() == string.Empty ? string.Empty : $"\r\n{SelfSuffix}";
                if (!forMeeting) selfName += "\r\n";
            }

            selfName = selfName.Trim().Replace("color=", "").Replace("<#ffffff><#ffffff>", "<#ffffff>");
            if (selfName.EndsWith("</size>")) selfName = selfName.Remove(selfName.Length - 7);
            if (selfName.EndsWith("</color>")) selfName = selfName.Remove(selfName.Length - 8);

            sender.RpcSetName(seer, selfName, seer);
            hasValue = true;

            bool onlySelfNameUpdateRequired = Options.CurrentGameMode switch
            {
                CustomGameMode.FFA => !FreeForAll.FFATeamMode.GetBool(),
                CustomGameMode.MoveAndStop => true,
                CustomGameMode.CaptureTheFlag => true,
                CustomGameMode.NaturalDisasters => true,
                CustomGameMode.RoomRush => true,
                CustomGameMode.KingOfTheZones => true,
                CustomGameMode.Quiz => true,
                _ => false
            };

            if (onlySelfNameUpdateRequired) return true;

            // Run the second loop only when necessary, such as when the seer is dead
            if (!seer.IsAlive() || noCache || camouflageIsForMeeting || mushroomMixup || IsActive(SystemTypes.MushroomMixupSabotage) || forceLoop || seerList.Length == 1 || targetList.Length == 1)
            {
                foreach (PlayerControl target in targetList)
                {
                    try
                    {
                        if (target.PlayerId == seer.PlayerId) continue;

                        if ((IsActive(SystemTypes.MushroomMixupSabotage) || mushroomMixup) && !forMeeting && target.IsAlive() && !seer.Is(CustomRoleTypes.Impostor) && seer.HasDesyncRole())
                            sender.RpcSetName(target, "<size=0%>", seer);
                        else
                        {
                            TargetMark.Clear();

                            if (Options.CurrentGameMode != CustomGameMode.Standard || GameStates.IsLobby) goto BeforeEnd2;

                            TargetMark.Append(Witch.GetSpelledMark(target.PlayerId, forMeeting));
                            if (forMeeting) TargetMark.Append(Wasp.GetStungMark(target.PlayerId));
                            if (forMeeting) TargetMark.Append(SpellCaster.HasSpelledMark(seer.PlayerId) ? ColorString(Team.Coven.GetColor(), "\u25c0") : string.Empty);

                            if (target.Is(CustomRoles.SuperStar) && Options.EveryOneKnowSuperStar.GetBool())
                                TargetMark.Append(ColorString(GetRoleColor(CustomRoles.SuperStar), "★"));

                            if (BallLightning.IsGhost(target)) TargetMark.Append(ColorString(GetRoleColor(CustomRoles.BallLightning), "■"));

                            TargetMark.Append(Snitch.GetWarningMark(seer, target));

                            if ((!seer.IsAlive() || Main.LoversPlayers.Exists(x => x.PlayerId == seer.PlayerId)) && Main.LoversPlayers.Exists(x => x.PlayerId == target.PlayerId))
                                TargetMark.Append($"<color={GetRoleColorCode(CustomRoles.Lovers)}> ♥</color>");

                            if (Randomizer.IsShielded(target)) TargetMark.Append(ColorString(GetRoleColor(CustomRoles.Randomizer), "✚"));

                            switch (seerRole)
                            {
                                case CustomRoles.PlagueBearer when PlagueBearer.IsPlagued(seer.PlayerId, target.PlayerId):
                                    TargetMark.Append($"<color={GetRoleColorCode(CustomRoles.PlagueBearer)}>●</color>");
                                    PlagueBearer.SendRPC(seer, target);
                                    break;
                                case CustomRoles.Arsonist:
                                    if (seer.IsDousedPlayer(target))
                                        TargetMark.Append($"<color={GetRoleColorCode(CustomRoles.Arsonist)}>▲</color>");

                                    else if (Arsonist.ArsonistTimer.TryGetValue(seer.PlayerId, out (PlayerControl Player, float Timer) arKvp) && arKvp.Player == target)
                                        TargetMark.Append($"<color={GetRoleColorCode(CustomRoles.Arsonist)}>△</color>");

                                    break;
                                case CustomRoles.Revolutionist:
                                    if (seer.IsDrawPlayer(target)) TargetMark.Append($"<color={GetRoleColorCode(CustomRoles.Revolutionist)}>●</color>");
                                    if (Revolutionist.RevolutionistTimer.TryGetValue(seer.PlayerId, out (PlayerControl Player, float Timer) arKvp1) && arKvp1.Player == target)
                                        TargetMark.Append($"<color={GetRoleColorCode(CustomRoles.Revolutionist)}>○</color>");
                                    break;
                                case CustomRoles.Farseer when Farseer.FarseerTimer.TryGetValue(seer.PlayerId, out (PlayerControl PLAYER, float TIMER) arKvp2) && arKvp2.PLAYER == target:
                                    TargetMark.Append($"<color={GetRoleColorCode(CustomRoles.Farseer)}>○</color>");
                                    break;
                                case CustomRoles.Analyst when (Main.PlayerStates[seer.PlayerId].Role as Analyst).CurrentTarget.ID == target.PlayerId:
                                    TargetMark.Append($"<color={GetRoleColorCode(CustomRoles.Analyst)}>○</color>");
                                    break;
                                case CustomRoles.Samurai when (Main.PlayerStates[seer.PlayerId].Role as Samurai).Target.Id == target.PlayerId:
                                    TargetMark.Append($"<color={GetRoleColorCode(CustomRoles.Samurai)}>○</color>");
                                    break;
                                case CustomRoles.Puppeteer when Puppeteer.PuppeteerList.ContainsValue(seer.PlayerId) && Puppeteer.PuppeteerList.ContainsKey(target.PlayerId):
                                    TargetMark.Append($"<color={GetRoleColorCode(CustomRoles.Impostor)}>◆</color>");
                                    break;
                            }

                            BeforeEnd2:

                            bool shouldSeeTargetAddons = seer.PlayerId == target.PlayerId || new[] { seer, target }.All(x => x.Is(Team.Impostor));

                            string targetRoleText =
                                (!seer.IsAlive() && Options.GhostCanSeeOtherRoles.GetBool()) ||
                                (seer.Is(CustomRoles.Mimic) && !target.IsAlive() && Options.MimicCanSeeDeadRoles.GetBool()) ||
                                (target.Is(CustomRoles.Gravestone) && !target.IsAlive()) ||
                                (Main.LoversPlayers.TrueForAll(x => x.PlayerId == seer.PlayerId || x.PlayerId == target.PlayerId) && Main.LoversPlayers.Count == 2 && Lovers.LoverKnowRoles.GetBool()) ||
                                (seer.Is(CustomRoleTypes.Coven) && target.Is(CustomRoleTypes.Coven)) ||
                                (seer.Is(CustomRoleTypes.Impostor) && target.Is(CustomRoleTypes.Impostor) && Options.ImpKnowAlliesRole.GetBool() && CustomTeamManager.ArentInCustomTeam(seer.PlayerId, target.PlayerId)) ||
                                (seer.IsMadmate() && target.Is(CustomRoleTypes.Impostor) && Options.MadmateKnowWhosImp.GetBool()) ||
                                (seer.Is(CustomRoleTypes.Impostor) && target.IsMadmate() && Options.ImpKnowWhosMadmate.GetBool()) ||
                                (seer.Is(CustomRoles.Crewpostor) && target.Is(CustomRoleTypes.Impostor) && Options.CrewpostorKnowsAllies.GetBool()) ||
                                (seer.Is(CustomRoles.Hypocrite) && target.Is(CustomRoleTypes.Impostor) && Hypocrite.KnowsAllies.GetBool()) ||
                                (seer.Is(CustomRoleTypes.Impostor) && target.Is(CustomRoles.Hypocrite) && Hypocrite.AlliesKnowHypocrite.GetBool()) ||
                                (seer.Is(CustomRoleTypes.Impostor) && target.Is(CustomRoles.Crewpostor) && Options.AlliesKnowCrewpostor.GetBool()) ||
                                (seer.IsMadmate() && target.IsMadmate() && Options.MadmateKnowWhosMadmate.GetBool()) ||
                                ((seer.Is(CustomRoles.Sidekick) || seer.Is(CustomRoles.Recruit) || seer.Is(CustomRoles.Jackal)) && (target.Is(CustomRoles.Sidekick) || target.Is(CustomRoles.Recruit) || target.Is(CustomRoles.Jackal))) ||
                                (target.Is(CustomRoles.Workaholic) && Workaholic.WorkaholicVisibleToEveryone.GetBool()) ||
                                (target.Is(CustomRoles.Doctor) && !target.HasEvilAddon() && Options.DoctorVisibleToEveryone.GetBool()) ||
                                (target.Is(CustomRoles.Mayor) && Mayor.MayorRevealWhenDoneTasks.GetBool() && target.GetTaskState().IsTaskFinished) ||
                                (Marshall.CanSeeMarshall(seer) && target.Is(CustomRoles.Marshall) && target.GetTaskState().IsTaskFinished) ||
                                (Main.PlayerStates[target.PlayerId].deathReason == PlayerState.DeathReason.Vote && Options.SeeEjectedRolesInMeeting.GetBool()) ||
                                (CustomTeamManager.AreInSameCustomTeam(seer.PlayerId, target.PlayerId) && CustomTeamManager.IsSettingEnabledForPlayerTeam(seer.PlayerId, CTAOption.KnowRoles)) ||
                                Main.PlayerStates.Values.Any(x => x.Role.KnowRole(seer, target)) ||
                                Markseeker.PlayerIdList.Any(x => Main.PlayerStates[x].Role is Markseeker { IsEnable: true, TargetRevealed: true } ms && ms.MarkedId == target.PlayerId) ||
                                Options.CurrentGameMode is CustomGameMode.FFA or CustomGameMode.MoveAndStop or CustomGameMode.HotPotato or CustomGameMode.Speedrun ||
                                (Options.CurrentGameMode == CustomGameMode.HideAndSeek && CustomHnS.IsRoleTextEnabled(seer, target)) ||
                                (seer.IsRevealedPlayer(target) && !target.Is(CustomRoles.Trickster)) ||
                                (seer.Is(CustomRoles.God) && God.KnowInfo.GetValue() == 2) ||
                                target.Is(CustomRoles.GM)
                                    ? $"<size={fontSize}>{target.GetDisplayRoleName(seeTargetBetrayalAddons: shouldSeeTargetAddons)}{GetProgressText(target)}</size>\r\n"
                                    : string.Empty;

                            if (IsRevivingRoleAlive() && Main.DiedThisRound.Contains(seer.PlayerId))
                                targetRoleText = string.Empty;

                            if (Options.CurrentGameMode is CustomGameMode.CaptureTheFlag or CustomGameMode.NaturalDisasters or CustomGameMode.RoomRush or CustomGameMode.KingOfTheZones or CustomGameMode.Quiz or CustomGameMode.TheMindGame or CustomGameMode.BedWars)
                                targetRoleText = string.Empty;

                            if (!GameStates.IsLobby)
                            {
                                if (seer.IsAlive() && seer.IsRevealedPlayer(target) && target.Is(CustomRoles.Trickster))
                                {
                                    targetRoleText = Farseer.RandomRole[seer.PlayerId];
                                    targetRoleText += Farseer.GetTaskState();
                                }

                                if (Options.CurrentGameMode == CustomGameMode.SoloKombat) targetRoleText = $"<size={fontSize}>{GetProgressText(target)}</size>\r\n";
                            }
                            else
                                targetRoleText = string.Empty;

                            string targetPlayerName = target.GetRealName(forMeeting);

                            if (target.Is(CustomRoles.BananaMan))
                                targetPlayerName = targetPlayerName.Insert(0, $"{GetString("Prefix.BananaMan")} ");

                            if (GameStates.IsLobby) goto End;

                            if (Options.CurrentGameMode != CustomGameMode.Standard) goto BeforeEnd;

                            if (guesserIsForMeeting || forMeeting || (seerRole == CustomRoles.Mafia && !seer.IsAlive() && Options.MafiaCanKillNum.GetInt() >= 1))
                                targetPlayerName = $"{ColorString(GetRoleColor(seerRole), target.PlayerId.ToString())} {targetPlayerName}";

                            switch (seerRole)
                            {
                                case CustomRoles.EvilTracker:
                                    TargetMark.Append(EvilTracker.GetTargetMark(seer, target));

                                    if (forMeeting && EvilTracker.IsTrackTarget(seer, target) && EvilTracker.CanSeeLastRoomInMeeting)
                                        targetRoleText = $"<size={fontSize}>{EvilTracker.GetArrowAndLastRoom(seer, target)}</size>\r\n";

                                    break;
                                case CustomRoles.Scout:
                                    TargetMark.Append(Scout.GetTargetMark(seer, target));

                                    if (forMeeting && Scout.IsTrackTarget(seer, target) && Scout.CanSeeLastRoomInMeeting)
                                        targetRoleText = $"<size={fontSize}>{Scout.GetArrowAndLastRoom(seer, target)}</size>\r\n";

                                    break;
                                case CustomRoles.Psychic when seer.IsAlive() && Psychic.IsRedForPsy(target, seer) && forMeeting:
                                    targetPlayerName = ColorString(GetRoleColor(CustomRoles.Impostor), targetPlayerName);
                                    break;
                                case CustomRoles.HeadHunter when (Main.PlayerStates[seer.PlayerId].Role as HeadHunter).Targets.Contains(target.PlayerId) && seer.IsAlive():
                                case CustomRoles.BountyHunter when (Main.PlayerStates[seer.PlayerId].Role as BountyHunter).GetTarget(seer) == target.PlayerId && seer.IsAlive():
                                    targetPlayerName = $"<color=#000000>{targetPlayerName}</size>";
                                    break;
                                case CustomRoles.Lookout when seer.IsAlive() && target.IsAlive() && !forMeeting:
                                    targetPlayerName = $"{ColorString(GetRoleColor(CustomRoles.Lookout), $" {target.PlayerId}")} {targetPlayerName}";
                                    break;
                            }

                            BeforeEnd:

                            targetPlayerName = targetPlayerName.ApplyNameColorData(seer, target, forMeeting);

                            if (Options.CurrentGameMode != CustomGameMode.Standard) goto End;

                            if (seer.Is(CustomRoleTypes.Impostor) && target.Is(CustomRoles.Snitch) && target.Is(CustomRoles.Madmate) && target.GetTaskState().IsTaskFinished)
                                TargetMark.Append(ColorString(GetRoleColor(CustomRoles.Impostor), "★"));

                            if (Marshall.CanSeeMarshall(seer) && target.Is(CustomRoles.Marshall) && target.GetTaskState().IsTaskFinished)
                                TargetMark.Append(ColorString(GetRoleColor(CustomRoles.Marshall), "★"));

                            TargetMark.Append(Executioner.TargetMark(seer, target));
                            TargetMark.Append(Gamer.TargetMark(seer, target));
                            TargetMark.Append(Medic.GetMark(seer, target));
                            TargetMark.Append(Gaslighter.GetMark(seer, target, forMeeting));
                            TargetMark.Append(Totocalcio.TargetMark(seer, target));
                            TargetMark.Append(Romantic.TargetMark(seer, target));
                            TargetMark.Append(Lawyer.LawyerMark(seer, target));
                            TargetMark.Append(Deathpact.GetDeathpactMark(seer, target));
                            TargetMark.Append(PlagueDoctor.GetMarkOthers(seer, target));

                            End:

                            TargetSuffix.Clear();

                            if (!GameStates.IsLobby)
                            {
                                List<string> additionalSuffixes = [];

                                switch (Options.CurrentGameMode)
                                {
                                    case CustomGameMode.SoloKombat:
                                        additionalSuffixes.Add(SoloPVP.GetDisplayHealth(target, false));
                                        break;
                                    case CustomGameMode.HideAndSeek:
                                        additionalSuffixes.Add(CustomHnS.GetSuffixText(seer, target));
                                        break;
                                    case CustomGameMode.CaptureTheFlag:
                                        additionalSuffixes.Add(CaptureTheFlag.GetSuffixText(seer, target));
                                        break;
                                    case CustomGameMode.TheMindGame:
                                        additionalSuffixes.Add(TheMindGame.GetSuffix(seer, target));
                                        break;
                                    case CustomGameMode.BedWars:
                                        additionalSuffixes.Add(BedWars.GetSuffix(seer, target));
                                        break;
                                }

                                if (MeetingStates.FirstMeeting && Main.ShieldPlayer == target.FriendCode && !string.IsNullOrEmpty(target.FriendCode) && Options.CurrentGameMode is CustomGameMode.Standard or CustomGameMode.FFA or CustomGameMode.Speedrun)
                                    additionalSuffixes.Add(GetString("DiedR1Warning"));

                                if (!forMeeting)
                                    additionalSuffixes.Add(AFKDetector.GetSuffix(seer, target));

                                TargetSuffix.Append(BuildSuffix(seer, target, meeting: forMeeting));

                                TargetSuffix.Append(string.Join('\n', additionalSuffixes.ConvertAll(x => x.Trim()).FindAll(x => !string.IsNullOrEmpty(x))));
                            }

                            var targetDeathReason = string.Empty;
                            string newLineBeforeSuffix = !(Options.CurrentGameMode == CustomGameMode.BedWars && GameStates.InGame) ? "\r\n" : " - ";
                            if (seer.KnowDeathReason(target) && !GameStates.IsLobby) targetDeathReason = $"{newLineBeforeSuffix}<size=1.7>({ColorString(GetRoleColor(CustomRoles.Doctor), GetVitalText(target.PlayerId))})</size>";

                            // Devourer
                            if (Devourer.HideNameOfConsumedPlayer.GetBool() && !GameStates.IsLobby && Devourer.PlayerIdList.Any(x => Main.PlayerStates[x].Role is Devourer { IsEnable: true } dv && dv.PlayerSkinsCosumed.Contains(target.PlayerId)) && !camouflageIsForMeeting)
                                targetPlayerName = GetString("DevouredName");

                            // Camouflage
                            if (Camouflage.IsCamouflage && !camouflageIsForMeeting) targetPlayerName = $"<size=0>{targetPlayerName}</size>";

                            if (Options.CurrentGameMode == CustomGameMode.KingOfTheZones && Main.IntroDestroyed && !KingOfTheZones.GameGoing)
                                targetPlayerName = EmptyMessage;

                            var targetName = $"{targetRoleText}{targetPlayerName}{targetDeathReason}{TargetMark}";
                            targetName += GameStates.IsLobby || TargetSuffix.ToString() == string.Empty ? string.Empty : $"{newLineBeforeSuffix}{TargetSuffix}";

                            targetName = targetName.Trim().Replace("color=", "").Replace("<#ffffff><#ffffff>", "<#ffffff>");
                            if (targetName.EndsWith("</size>")) targetName = targetName.Remove(targetName.Length - 7);
                            if (targetName.EndsWith("</color>")) targetName = targetName.Remove(targetName.Length - 8);

                            sender.RpcSetName(target, targetName, seer);
                            hasValue = true;
                            senderWasCleared = false;

                            if (sender.stream.Length > 500)
                            {
                                sender.SendMessage();
                                sender = CustomRpcSender.Create(sender.name, sender.sendOption);
                                hasValue = false;
                                senderWasCleared = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (LastNotifyRolesErrorTS != now)
                        {
                            Logger.Error($"Error - seer = {seer.GetNameWithRole()}, target = {target.GetNameWithRole()}:", "NR");
                            ThrowException(ex);
                            LastNotifyRolesErrorTS = now;
                        }
                        else
                            Logger.Error($"Error - seer = {seer.GetNameWithRole()}, target = {target.GetNameWithRole()}: {ex}", "NR");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (LastNotifyRolesErrorTS != now)
            {
                Logger.Error($"Error for {seer.GetNameWithRole()}:", "NR");
                ThrowException(ex);
                LastNotifyRolesErrorTS = now;
            }
            else
                Logger.Error($"Error for {seer.GetNameWithRole()}: {ex}", "NR");
        }

        return hasValue;
    }

    public static void MarkEveryoneDirtySettings()
    {
        PlayerGameOptionsSender.SetDirtyToAll();
    }

    public static void MarkEveryoneDirtySettingsV2()
    {
        PlayerGameOptionsSender.SetDirtyToAllV2();
    }

    public static void MarkEveryoneDirtySettingsV3()
    {
        PlayerGameOptionsSender.SetDirtyToAllV3();
    }

    public static void MarkEveryoneDirtySettingsV4()
    {
        PlayerGameOptionsSender.SetDirtyToAllV4();
    }

    public static void SyncAllSettings()
    {
        PlayerGameOptionsSender.SetDirtyToAll();
        Main.Instance.StartCoroutine(GameOptionsSender.SendAllGameOptionsAsync());
    }

    public static bool RpcChangeSkin(PlayerControl pc, NetworkedPlayerInfo.PlayerOutfit newOutfit, CustomRpcSender writer = null, SendOption sendOption = SendOption.Reliable)
    {
        if (pc.Is(CustomRoles.BananaMan))
            newOutfit = BananaMan.GetOutfit(Main.AllPlayerNames.GetValueOrDefault(pc.PlayerId, "Banana"));
        
        if (newOutfit.Compare(pc.Data.DefaultOutfit)) return false;

        Camouflage.SetPetForOutfitIfNecessary(newOutfit);

        if (newOutfit.Compare(pc.Data.DefaultOutfit)) return false;

        CustomRpcSender sender = writer ?? CustomRpcSender.Create($"Utils.RpcChangeSkin({pc.Data.PlayerName})", sendOption);

        pc.SetName(newOutfit.PlayerName);

        sender.AutoStartRpc(pc.NetId, RpcCalls.SetName)
            .Write(pc.Data.NetId)
            .Write(newOutfit.PlayerName)
            .EndRpc();

        Main.AllPlayerNames[pc.PlayerId] = newOutfit.PlayerName;

        pc.SetColor(newOutfit.ColorId);

        sender.AutoStartRpc(pc.NetId, RpcCalls.SetColor)
            .Write(pc.Data.NetId)
            .Write((byte)newOutfit.ColorId)
            .EndRpc();

        pc.SetHat(newOutfit.HatId, newOutfit.ColorId);
        pc.Data.DefaultOutfit.HatSequenceId += 10;

        sender.AutoStartRpc(pc.NetId, RpcCalls.SetHatStr)
            .Write(newOutfit.HatId)
            .Write(pc.GetNextRpcSequenceId(RpcCalls.SetHatStr))
            .EndRpc();

        pc.SetSkin(newOutfit.SkinId, newOutfit.ColorId);
        pc.Data.DefaultOutfit.SkinSequenceId += 10;

        sender.AutoStartRpc(pc.NetId, RpcCalls.SetSkinStr)
            .Write(newOutfit.SkinId)
            .Write(pc.GetNextRpcSequenceId(RpcCalls.SetSkinStr))
            .EndRpc();

        pc.SetVisor(newOutfit.VisorId, newOutfit.ColorId);
        pc.Data.DefaultOutfit.VisorSequenceId += 10;

        sender.AutoStartRpc(pc.NetId, RpcCalls.SetVisorStr)
            .Write(newOutfit.VisorId)
            .Write(pc.GetNextRpcSequenceId(RpcCalls.SetVisorStr))
            .EndRpc();

        pc.SetPet(newOutfit.PetId);
        pc.Data.DefaultOutfit.PetSequenceId += 10;

        sender.AutoStartRpc(pc.NetId, RpcCalls.SetPetStr)
            .Write(newOutfit.PetId)
            .Write(pc.GetNextRpcSequenceId(RpcCalls.SetPetStr))
            .EndRpc();

        pc.SetNamePlate(newOutfit.NamePlateId);
        pc.Data.DefaultOutfit.NamePlateSequenceId += 10;

        sender.AutoStartRpc(pc.NetId, RpcCalls.SetNamePlateStr)
            .Write(newOutfit.NamePlateId)
            .Write(pc.GetNextRpcSequenceId(RpcCalls.SetNamePlateStr))
            .EndRpc();

        if (writer == null) sender.SendMessage();

        return true;
    }

    public static void SendGameData()
    {
        MessageWriter writer = MessageWriter.Get(SendOption.Reliable);
        writer.StartMessage(5);
        writer.Write(AmongUsClient.Instance.GameId);

        foreach (NetworkedPlayerInfo playerinfo in GameData.Instance.AllPlayers)
        {
            if (writer.Length > 500)
            {
                writer.EndMessage();
                AmongUsClient.Instance.SendOrDisconnect(writer);
                writer.Clear(SendOption.Reliable);
                writer.StartMessage(5);
                writer.Write(AmongUsClient.Instance.GameId);
            }

            writer.StartMessage(1);
            writer.WritePacked(playerinfo.NetId);
            playerinfo.Serialize(writer, false);
            writer.EndMessage();
        }

        writer.EndMessage();
        AmongUsClient.Instance.SendOrDisconnect(writer);
        writer.Recycle();
    }

    public static string GetGameStateData(bool clairvoyant = false)
    {
        Dictionary<Options.GameStateInfo, int> nums = Enum.GetValues<Options.GameStateInfo>().ToDictionary(x => x, _ => 0);

        if (CustomRoles.Romantic.RoleExist(true)) nums[Options.GameStateInfo.RomanticState] = 1;
        if (Romantic.HasPickedPartner) nums[Options.GameStateInfo.RomanticState] = 2;

        foreach (PlayerControl pc in Main.AllAlivePlayerControls)
        {
            if (pc.IsMadmate())
                nums[Options.GameStateInfo.MadmateCount]++;
            else if (pc.IsNeutralKiller())
                nums[Options.GameStateInfo.NKCount]++;
            else if (pc.IsCrewmate())
                nums[Options.GameStateInfo.CrewCount]++;
            else if (pc.Is(Team.Impostor))
                nums[Options.GameStateInfo.ImpCount]++;
            else if (pc.Is(Team.Neutral))
                nums[Options.GameStateInfo.NNKCount]++;
            else if (pc.Is(Team.Coven))
                nums[Options.GameStateInfo.CovenCount]++;

            if (pc.IsConverted()) nums[Options.GameStateInfo.ConvertedCount]++;
            if (Main.LoversPlayers.Exists(x => x.PlayerId == pc.PlayerId)) nums[Options.GameStateInfo.LoversState]++;
            if (pc.Is(CustomRoles.Romantic)) nums[Options.GameStateInfo.RomanticState] *= 3;
            if (Romantic.PartnerId == pc.PlayerId) nums[Options.GameStateInfo.RomanticState] *= 4;
        }

        foreach ((byte id, CustomRoles role) in Forger.Forges)
        {
            if (!Main.PlayerStates.TryGetValue(id, out var state) || state.IsDead)
            {
                if (role.IsMadmate())
                    nums[Options.GameStateInfo.MadmateCount]--;
                else if (role.IsNK())
                    nums[Options.GameStateInfo.NKCount]--;
                else if (role.IsCrewmate())
                    nums[Options.GameStateInfo.CrewCount]--;
                else if (role.Is(Team.Impostor))
                    nums[Options.GameStateInfo.ImpCount]--;
                else if (role.Is(Team.Neutral))
                    nums[Options.GameStateInfo.NNKCount]--;
                else if (role.Is(Team.Coven))
                    nums[Options.GameStateInfo.CovenCount]--;
            }
        }

        // All possible results of RomanticState from the above code:
        // 0: Romantic doesn't exist
        // 1: Romantic exists but hasn't picked a partner (and is dead)
        // 2: Romantic exists and has picked a partner (but both of them are dead)
        // 3: Romantic exists, is alive, but hasn't picked a partner
        // 6: Romantic exists, has picked a partner who is dead, but Romantic is alive
        // 8: Romantic exists, has picked a partner who is alive, but Romantic is dead
        // 24: Romantic exists, has picked a partner who is alive, and Romantic is alive

        StringBuilder sb = new();
        Dictionary<Options.GameStateInfo, OptionItem> checkDict = clairvoyant ? Clairvoyant.Settings : Options.GameStateSettings;
        nums[Options.GameStateInfo.Tasks] = GameData.Instance.CompletedTasks;
        Dictionary<Options.GameStateInfo, object> states = nums.ToDictionary(x => x.Key, x => x.Key == Options.GameStateInfo.RomanticState ? GetString($"GSRomanticState.{x.Value}") : (object)x.Value);
        states.DoIf(x => checkDict[x.Key].GetBool(), x => sb.AppendLine(string.Format(GetString($"GSInfo.{x.Key}"), x.Value, GameData.Instance.TotalTasks)));
        return $"<#ffffff><size=90%>{sb.ToString().TrimEnd()}</size></color>";
    }

    public static void AddAbilityCD(CustomRoles role, byte playerId, bool includeDuration = true)
    {
        if (role.UsesPetInsteadOfKill())
        {
            var kcd = (int)Math.Round(Main.AllPlayerKillCooldown.TryGetValue(playerId, out float killCd) ? killCd : Options.AdjustedDefaultKillCooldown);
            Main.AbilityCD[playerId] = (TimeStamp, kcd);
            SendRPC(CustomRPC.SyncAbilityCD, 1, playerId, kcd);
            return;
        }

        int cd = role switch
        {
            CustomRoles.PortalMaker => 5,
            CustomRoles.Mole => Mole.CD.GetInt(),
            CustomRoles.Monitor => Monitor.VentCooldown.GetInt(),
            CustomRoles.Tether => Tether.VentCooldown.GetInt(),
            CustomRoles.Mayor when Mayor.MayorHasPortableButton.GetBool() => (int)Math.Round(Options.AdjustedDefaultKillCooldown),
            CustomRoles.Paranoia => (int)Math.Round(Options.AdjustedDefaultKillCooldown),
            CustomRoles.Grenadier => Options.GrenadierSkillCooldown.GetInt() + (includeDuration ? Options.GrenadierSkillDuration.GetInt() : 0),
            CustomRoles.Lighter => Options.LighterSkillCooldown.GetInt() + (includeDuration ? Options.LighterSkillDuration.GetInt() : 0),
            CustomRoles.SecurityGuard => Options.SecurityGuardSkillCooldown.GetInt() + (includeDuration ? Options.SecurityGuardSkillDuration.GetInt() : 0),
            CustomRoles.Veteran => Options.VeteranSkillCooldown.GetInt() + (includeDuration ? Options.VeteranSkillDuration.GetInt() : 0),
            CustomRoles.Rhapsode => Rhapsode.AbilityCooldown.GetInt() + (includeDuration ? Rhapsode.AbilityDuration.GetInt() : 0),
            CustomRoles.Whisperer => Whisperer.Cooldown.GetInt() + (includeDuration ? Whisperer.Duration.GetInt() : 0),
            CustomRoles.Astral => Astral.AbilityCooldown.GetInt() + (includeDuration ? Astral.AbilityDuration.GetInt() : 0),
            CustomRoles.TimeMaster => TimeMaster.TimeMasterSkillCooldown.GetInt(),
            CustomRoles.Perceiver => Perceiver.CD.GetInt(),
            CustomRoles.Convener => Convener.CD.GetInt(),
            CustomRoles.DovesOfNeace => Options.DovesOfNeaceCooldown.GetInt(),
            CustomRoles.Alchemist => Alchemist.VentCooldown.GetInt(),
            CustomRoles.NiceHacker => playerId.IsPlayerModdedClient() ? -1 : NiceHacker.AbilityCD.GetInt(),
            CustomRoles.CameraMan => CameraMan.VentCooldown.GetInt(),
            CustomRoles.Tornado => Tornado.TornadoCooldown.GetInt(),
            CustomRoles.Sentinel => Sentinel.PatrolCooldown.GetInt(),
            CustomRoles.Druid => Druid.VentCooldown.GetInt(),
            CustomRoles.Catcher => Catcher.AbilityCooldown.GetInt(),
            CustomRoles.Sentry => Crewmate.Sentry.ShowInfoCooldown.GetInt(),
            CustomRoles.ToiletMaster => ToiletMaster.AbilityCooldown.GetInt(),
            CustomRoles.AntiAdminer => AntiAdminer.AbilityCooldown.GetInt(),
            CustomRoles.Sniper => Options.DefaultShapeshiftCooldown.GetInt(),
            CustomRoles.Assassin => Assassin.AssassinateCooldownOpt.GetInt(),
            CustomRoles.Undertaker => Undertaker.UndertakerAssassinateCooldown.GetInt(),
            CustomRoles.Bomber => Bomber.BombCooldown.GetInt(),
            CustomRoles.Nuker => Bomber.NukeCooldown.GetInt(),
            CustomRoles.Sapper => Sapper.ShapeshiftCooldown.GetInt(),
            CustomRoles.Miner => Miner.MinerSSCD.GetInt(),
            CustomRoles.Escapee => Escapee.EscapeeSSCD.GetInt(),
            CustomRoles.QuickShooter => QuickShooter.ShapeshiftCooldown.GetInt(),
            CustomRoles.Disperser => Disperser.DisperserShapeshiftCooldown.GetInt(),
            CustomRoles.Twister => Twister.ShapeshiftCooldown.GetInt(),
            CustomRoles.Abyssbringer => Abyssbringer.BlackHolePlaceCooldown.GetInt(),
            CustomRoles.Wiper => Wiper.AbilityCooldown.GetInt(),
            CustomRoles.Warlock => Warlock.IsCursed ? -1 : Warlock.ShapeshiftCooldown.GetInt(),
            CustomRoles.Stasis => Stasis.AbilityCooldown.GetInt() + (includeDuration ? Stasis.AbilityDuration.GetInt() : 0),
            CustomRoles.Swiftclaw => Swiftclaw.DashCD.GetInt() + (includeDuration ? Swiftclaw.DashDuration.GetInt() : 0),
            CustomRoles.Hypnotist => Hypnotist.AbilityCooldown.GetInt() + (includeDuration ? Hypnotist.AbilityDuration.GetInt() : 0),
            CustomRoles.Parasite => (int)Parasite.SSCD + (includeDuration ? (int)Parasite.SSDur : 0),
            CustomRoles.Tiger => Tiger.EnrageCooldown.GetInt() + (includeDuration ? Tiger.EnrageDuration.GetInt() : 0),
            CustomRoles.Nonplus => Nonplus.BlindCooldown.GetInt() + (includeDuration ? Nonplus.BlindDuration.GetInt() : 0),
            CustomRoles.Amogus => Amogus.AbilityCooldown.GetInt() + (includeDuration ? Amogus.AbilityDuration.GetInt() : 0),
            CustomRoles.Cherokious => Cherokious.KillCooldown.GetInt(),
            CustomRoles.Shifter => Shifter.KillCooldown.GetInt(),
            CustomRoles.NoteKiller => NoteKiller.AbilityCooldown.GetInt(),
            CustomRoles.Weatherman => Weatherman.AbilityCooldown.GetInt(),
            _ => -1
        };

        if (cd == -1) return;

        if (Main.PlayerStates[playerId].SubRoles.Contains(CustomRoles.Energetic))
            cd = (int)Math.Round(cd * 0.75f);

        Main.AbilityCD[playerId] = (TimeStamp, cd);
        SendRPC(CustomRPC.SyncAbilityCD, 1, playerId, cd);

        if (role.SimpleAbilityTrigger() &&
            (Options.UsePhantomBasis.GetBool() && (!role.IsNeutral() || Options.UsePhantomBasisForNKs.GetBool())) && !role.AlwaysUsesPhantomBase())
            GetPlayerById(playerId)?.RpcResetAbilityCooldown();
    }

    public static (RoleTypes RoleType, CustomRoles CustomRole) GetRoleMap(byte seerId, byte targetId = byte.MaxValue)
    {
        if (targetId == byte.MaxValue) targetId = seerId;
        return StartGameHostPatch.RpcSetRoleReplacer.RoleMap[(seerId, targetId)];
    }

    public static void AfterMeetingTasks()
    {
        if (Lovers.PrivateChat.GetBool() && Main.LoversPlayers.TrueForAll(x => x.IsAlive()))
        {
            Main.LoversPlayers.ForEach(x => x.SetChatVisible(true));
            GameEndChecker.CheckCustomEndCriteria();
        }

        AFKDetector.NumAFK = 0;
        AFKDetector.PlayerData.Clear();

        Camouflage.CheckCamouflage();

        foreach (PlayerControl pc in Main.AllPlayerControls)
        {
            if (pc.IsAlive())
            {
                pc.AddKillTimerToDict();

                if (pc.Is(CustomRoles.Truant))
                {
                    float beforeSpeed = Main.AllPlayerSpeed[pc.PlayerId];
                    Main.AllPlayerSpeed[pc.PlayerId] = Main.MinSpeed;
                    pc.MarkDirtySettings();

                    LateTask.New(() =>
                        {
                            Main.AllPlayerSpeed[pc.PlayerId] = beforeSpeed;
                            pc.MarkDirtySettings();
                        }, Options.TruantWaitingTime.GetFloat(), $"Truant Waiting: {pc.GetNameWithRole()}");
                }

                if (Options.UsePets.GetBool())
                {
                    pc.AddAbilityCD(false);

                    LateTask.New(() =>
                    {
                        string petId = PetsHelper.GetPetId();
                        PetsHelper.SetPet(pc, petId);
                    }, 3f, "No Pet Reassign");
                }

                AFKDetector.RecordPosition(pc);

                if (Camouflage.IsCamouflage)
                    Camouflage.RpcSetSkin(pc);
            }
            else
            {
                TaskState taskState = pc.GetTaskState();

                if (pc.IsCrewmate() && !taskState.IsTaskFinished && taskState.HasTasks)
                    pc.Notify(GetString("DoYourTasksPlease"), 8f);

                GhostRolesManager.NotifyAboutGhostRole(pc);
            }

            Main.PlayerStates[pc.PlayerId].Role.AfterMeetingTasks();

            if (pc.Is(CustomRoles.Specter) || pc.Is(CustomRoles.Haunter))
                pc.RpcResetAbilityCooldown();

            Main.CheckShapeshift[pc.PlayerId] = false;
        }

        LateTask.New(() => Main.ProcessShapeshifts = true, 1f, log: false);

        CopyCat.ResetRoles();

        if (Options.DiseasedCDReset.GetBool())
        {
            Main.KilledDiseased.SetAllValues(0);
            Main.KilledDiseased.Keys.ToValidPlayers().Do(x => x?.ResetKillCooldown());
            Main.KilledDiseased.Clear();
        }

        if (Options.AntidoteCDReset.GetBool())
        {
            Main.KilledAntidote.SetAllValues(0);
            Main.KilledAntidote.Keys.ToValidPlayers().Do(x => x?.ResetKillCooldown());
            Main.KilledAntidote.Clear();
        }

        Damocles.AfterMeetingTasks();
        Stressed.AfterMeetingTasks();
        Circumvent.AfterMeetingTasks();
        Deadlined.AfterMeetingTasks();

        if (Options.AirshipVariableElectrical.GetBool())
            AirshipElectricalDoors.Initialize();

        Main.DontCancelVoteList.Clear();

        DoorsReset.ResetDoors();
        RoleBlockManager.Reset();
        PhantomRolePatch.AfterMeeting();

        if (Main.CurrentMap == MapNames.Airship && AmongUsClient.Instance.AmHost && PlayerControl.LocalPlayer.Is(CustomRoles.GM))
            LateTask.New(() => PlayerControl.LocalPlayer.NetTransform.SnapTo(new(15.5f, 0.0f), (ushort)(PlayerControl.LocalPlayer.NetTransform.lastSequenceId + 8)), 11f, "GM Auto-TP Failsafe"); // TP to Main Hall

        LateTask.New(() => Asthmatic.RunChecks = true, 2f, log: false);
        EAC.InvalidReports.Clear();

        CustomNetObject.AfterMeeting();

        RPCHandlerPatch.RemoveExpiredWhiteList();
    }

    public static void AfterPlayerDeathTasks(PlayerControl target, bool onMeeting = false, bool disconnect = false)
    {
        PlayerControl targetRealKiller = target.GetRealKiller();

        try
        {
            if (!onMeeting) Main.DiedThisRound.Add(target.PlayerId);

            // Record the first death
            if (Main.FirstDied == string.Empty) Main.FirstDied = target.FriendCode;

            switch (target.GetCustomRole())
            {
                case CustomRoles.Hypnotist when disconnect && Hypnotist.DoReportAfterHypnosisEnds.GetBool():
                    ReportDeadBodyPatch.CanReport.SetAllValues(true);
                    break;
                case CustomRoles.Curser:
                    ((Curser)Main.PlayerStates[target.PlayerId].Role).OnDeath();
                    break;
                case CustomRoles.Camouflager when Camouflager.IsActive:
                    Camouflager.IsDead();
                    break;
                case CustomRoles.Terrorist when !disconnect:
                    Logger.Info(target?.Data?.PlayerName + " Terrorist died", "MurderPlayer");
                    CheckTerroristWin(target?.Data);
                    break;
                case CustomRoles.Executioner when Executioner.Target.Remove(target.PlayerId):
                    Executioner.SendRPC(target.PlayerId);
                    break;
                case CustomRoles.Lawyer when Lawyer.Target.Remove(target.PlayerId):
                    Lawyer.SendRPC(target.PlayerId);
                    break;
                case CustomRoles.PlagueDoctor when !disconnect && !onMeeting:
                    PlagueDoctor.OnPDdeath(targetRealKiller, target);
                    break;
                case CustomRoles.SuperStar when !disconnect:
                    if (onMeeting)
                    {
                        (
                            from pc in Main.AllPlayerControls
                            where (Options.ImpKnowSuperStarDead.GetBool() || !pc.GetCustomRole().IsImpostor()) && (Options.NeutralKnowSuperStarDead.GetBool() || !pc.GetCustomRole().IsNeutral()) && (Options.CovenKnowSuperStarDead.GetBool() || !pc.Is(CustomRoleTypes.Coven))
                            select new Message(string.Format(GetString("SuperStarDead"), target.GetRealName()), pc.PlayerId, ColorString(GetRoleColor(CustomRoles.SuperStar), GetString("SuperStarNewsTitle")))
                        ).SendMultipleMessages();
                    }
                    else
                    {
                        if (!Main.SuperStarDead.Contains(target.PlayerId))
                            Main.SuperStarDead.Add(target.PlayerId);
                    }

                    break;
                case CustomRoles.Pelican:
                    Pelican.OnPelicanDied(target.PlayerId);
                    break;
                case CustomRoles.Devourer:
                    Devourer.OnDevourerDied(target.PlayerId);
                    break;
                case CustomRoles.Markseeker when !disconnect:
                    Markseeker.OnDeath(target);
                    break;
                case CustomRoles.Medic:
                    Medic.IsDead(target);
                    break;
                case CustomRoles.Dreamweaver:
                    ((Dreamweaver)Main.PlayerStates[target.PlayerId].Role).InsanePlayers.Clear();
                    Main.PlayerStates.Values.Do(x => x.RemoveSubRole(CustomRoles.Insane));
                    break;
            }

            if (target == null) return;

            if (!disconnect && !onMeeting) Randomizer.OnAnyoneDeath(target);
            if (Executioner.Target.ContainsValue(target.PlayerId)) Executioner.ChangeRoleByTarget(target);
            if (Lawyer.Target.ContainsValue(target.PlayerId)) Lawyer.ChangeRoleByTarget(target);
            if (!disconnect && !onMeeting && target.Is(CustomRoles.Stained)) Stained.OnDeath(target, targetRealKiller);
            if (!disconnect && target.Is(CustomRoles.Spurt)) Spurt.DeathTask(target);

            Postman.CheckAndResetTargets(target, !onMeeting && !disconnect);
            Hitman.CheckAndResetTargets();
            Reaper.OnAnyoneDead(target);
            Wyrd.OnAnyoneDeath(target);

            if (!onMeeting && !disconnect)
            {
                Hacker.AddDeadBody(target);
                Mortician.OnPlayerDead(target);
                Tracefinder.OnPlayerDead(target);
                Scout.OnPlayerDeath(target);
                Dad.OnAnyoneDeath(target);
                Crewmate.Sentry.OnAnyoneMurder(target);
                Soothsayer.OnAnyoneDeath(targetRealKiller);

                TargetDies(targetRealKiller, target);
            }

            if (!onMeeting)
            {
                Amogus.OnAnyoneDead(target);
                Adventurer.OnAnyoneDead(target);
                Whisperer.OnAnyoneDied(target);
            }

            if (QuizMaster.On) QuizMaster.Data.NumPlayersDeadThisRound++;

            FixedUpdatePatch.LoversSuicide(target.PlayerId, guess: onMeeting);

            if (!target.HasGhostRole())
            {
                Main.AllPlayerSpeed[target.PlayerId] = Main.RealOptionsData.GetFloat(FloatOptionNames.PlayerSpeedMod);
                target.MarkDirtySettings();
            }
        }
        catch (Exception ex)
        {
            Logger.CurrentMethod();
            Logger.Exception(ex, "AfterPlayerDeathTasks");
        }

        if (target == null || (Main.DiedThisRound.Contains(target.PlayerId) && IsRevivingRoleAlive()) || Options.CurrentGameMode != CustomGameMode.Standard) return;

        if (targetRealKiller != null)
            target.Notify($"<#ffffff>{string.Format(GetString("DeathCommand"), targetRealKiller.PlayerId.ColoredPlayerName(), (targetRealKiller.Is(CustomRoles.Bloodlust) ? $"{CustomRoles.Bloodlust.ToColoredString()} " : string.Empty) + targetRealKiller.GetCustomRole().ToColoredString())}</color>", 10f);
    }

    public static void CountAlivePlayers(bool sendLog = false)
    {
        try
        {
            int aliveImpostorCount = Main.AllAlivePlayerControls.Count(pc => pc.Is(CustomRoleTypes.Impostor));

            if (Main.AliveImpostorCount != aliveImpostorCount)
            {
                Logger.Info("Number of living Impostors: " + aliveImpostorCount, "CountAliveImpostors");
                Main.AliveImpostorCount = aliveImpostorCount;
                LastImpostor.SetSubRole();
            }

            if (sendLog)
            {
                StringBuilder sb = new(100);

                if (Options.CurrentGameMode == CustomGameMode.Standard)
                {
                    foreach (CountTypes countTypes in Enum.GetValues<CountTypes>())
                    {
                        int playersCount = PlayersCount(countTypes);
                        if (playersCount == 0) continue;

                        sb.Append($"{countTypes}: {AlivePlayersCount(countTypes)}/{playersCount}, ");
                    }
                }

                sb.Append($"All: {AllAlivePlayersCount}/{AllPlayersCount}");
                Logger.Info(sb.ToString(), "CountAlivePlayers");
            }

            if (AmongUsClient.Instance.AmHost && Main.IntroDestroyed)
                GameEndChecker.CheckCustomEndCriteria();
        }
        catch (Exception e) { ThrowException(e); }
    }

    public static string GetVoteName(byte num)
    {
        PlayerControl player = GetPlayerById(num);

        return num switch
        {
            < 128 when player != null => player.GetNameWithRole().RemoveHtmlTags(),
            253 => "Skip",
            254 => "None",
            255 => "Dead",
            _ => "invalid"
        };
    }

    public static string PadRightV2(this object text, int num)
    {
        var t = text.ToString();
        if (t == null) return string.Empty;

        int bc = t.Sum(c => Encoding.GetEncoding("UTF-8").GetByteCount(c.ToString()) == 1 ? 1 : 2);

        return t.PadRight(Mathf.Max(num - (bc - t.Length), 0));
    }

    public static void DumpLog(bool open = true, bool finish = true)
    {
        try
        {
            if (finish) CustomLogger.Instance.Finish();

            var t = DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss");
#if ANDROID
            var f = $"{Main.DataPath}/EHR_Logs/{t}";
#else
            var f = $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}/EHR_Logs/{t}";
#endif
            if (!Directory.Exists(f)) Directory.CreateDirectory(f);

            var filename = $"{f}/EHR-v{Main.PluginVersion}-LOG";
#if ANDROID
            var directory = Main.DataPath;
#else
            string directory = Environment.CurrentDirectory;
#endif
            FileInfo[] files = [new($"{directory}/BepInEx/LogOutput.log"), new($"{directory}/BepInEx/log.html")];
            files.Do(x => x.CopyTo($"{filename}{x.Extension}"));

            if (!open) return;

            if (PlayerControl.LocalPlayer != null)
                FastDestroyableSingleton<HudManager>.Instance?.Chat?.AddChat(PlayerControl.LocalPlayer, string.Format(GetString("Message.DumpfileSaved"), "EHR" + filename.Split("EHR")[1]));

#if !ANDROID
            Process.Start("explorer.exe", f.Replace("/", "\\"));
#endif
        }
        catch (Exception e) { ThrowException(e); }
    }

    public static (int Doused, int All) GetDousedPlayerCount(byte playerId)
    {
        int doused = 0, all = 0;

        foreach (PlayerControl pc in Main.AllAlivePlayerControls)
        {
            if (pc.PlayerId == playerId) continue;

            all++;

            if (Arsonist.IsDoused.TryGetValue((playerId, pc.PlayerId), out bool isDoused) && isDoused)
                doused++;
        }

        return (doused, all);
    }

    public static (int Drawn, int All) GetDrawPlayerCount(byte playerId, out List<PlayerControl> winnerList)
    {
        int all = Revolutionist.RevolutionistDrawCount.GetInt();
        int max = Main.AllAlivePlayerControls.Length;

        if (!Main.PlayerStates[playerId].IsDead) max--;
        if (all > max) all = max;

        winnerList = Main.AllPlayerControls.Where(pc => Revolutionist.IsDraw.TryGetValue((playerId, pc.PlayerId), out bool isDraw) && isDraw).ToList();
        return (winnerList.Count, all);
    }

    public static string SummaryTexts(byte id, bool disableColor = true, bool check = false)
    {
        try
        {
            string name = Main.AllPlayerNames[id].RemoveHtmlTags().Replace("\r\n", string.Empty);

            if (id == PlayerControl.LocalPlayer.PlayerId)
                name = DataManager.player.Customization.Name;
            else
                name = GetPlayerById(id)?.Data.PlayerName ?? name;

            TaskState taskState = Main.PlayerStates[id].TaskState;
            string taskCount;

            if (taskState.HasTasks)
            {
                NetworkedPlayerInfo info = GetPlayerInfoById(id);
                Color taskCompleteColor = HasTasks(info) ? Color.green : Color.cyan;
                Color nonCompleteColor = HasTasks(info) ? Color.yellow : Color.white;

                if (Workhorse.IsThisRole(id)) nonCompleteColor = Workhorse.RoleColor;

                Color normalColor = taskState.IsTaskFinished ? taskCompleteColor : nonCompleteColor;

                if (Main.PlayerStates.TryGetValue(id, out PlayerState ps))
                {
                    normalColor = ps.MainRole switch
                    {
                        CustomRoles.Hypocrite => Color.red,
                        CustomRoles.Crewpostor => Color.red,
                        CustomRoles.Cherokious => GetRoleColor(CustomRoles.Cherokious),
                        CustomRoles.Pawn => GetRoleColor(CustomRoles.Cherokious),
                        _ => normalColor
                    };
                }

                Color textColor = normalColor;
                var completed = $"{taskState.CompletedTasksCount}";
                taskCount = ColorString(textColor, $" ({completed}/{taskState.AllTasksCount})");
            }
            else
                taskCount = string.Empty;

            var summary = $"{ColorString(Main.PlayerColors[id], name)} - {GetDisplayRoleName(id, true)}{taskCount}{GetKillCountText(id)} ({GetVitalText(id, true)})";

            switch (Options.CurrentGameMode)
            {
                case CustomGameMode.SoloKombat:
                    summary = $"{ColorString(Main.PlayerColors[id], name)} - {SoloPVP.GetSummaryStatistics(id)}";
                    break;
                case CustomGameMode.FFA:
                    summary = $"{ColorString(Main.PlayerColors[id], name)} {GetKillCountText(id, true)}";
                    break;
                case CustomGameMode.Speedrun:
                case CustomGameMode.MoveAndStop:
                    summary = $"{ColorString(Main.PlayerColors[id], name)} -{taskCount.Replace("(", string.Empty).Replace(")", string.Empty)}  ({GetVitalText(id, true)})";
                    break;
                case CustomGameMode.HotPotato:
                    int time = HotPotato.GetSurvivalTime(id);
                    summary = $"{ColorString(Main.PlayerColors[id], name)} - <#e8cd46>{GetString("SurvivedTimePrefix")}: <#ffffff>{(time == 0 ? $"{GetString("SurvivedUntilTheEnd")}</color>" : $"{time}</color>s")}</color>  ({GetVitalText(id, true)})";
                    break;
                case CustomGameMode.NaturalDisasters:
                    int time2 = NaturalDisasters.SurvivalTime(id);
                    summary = $"{ColorString(Main.PlayerColors[id], name)} - <#e8cd46>{GetString("SurvivedTimePrefix")}: <#ffffff>{(time2 == 0 ? $"{GetString("SurvivedUntilTheEnd")}</color>" : $"{time2}</color>s")}</color>  ({GetVitalText(id, true)})";
                    break;
                case CustomGameMode.RoomRush:
                    int rrSurvivalTime = RoomRush.GetSurvivalTime(id);
                    string rrSurvivalTimeText = rrSurvivalTime == 0 ? $"{GetString("SurvivedUntilTheEnd")}</color>" : $"{rrSurvivalTime}</color>s";
                    string rrSurvivedText = RoomRush.PointsSystem ? RoomRush.GetPoints(id) : $"{GetString("SurvivedTimePrefix")}: <#ffffff>{rrSurvivalTimeText}</color>";
                    string vitalText = RoomRush.PointsSystem ? string.Empty : $" ({GetVitalText(id, true)})";
                    summary = $"{ColorString(Main.PlayerColors[id], name)} - <#e8cd46>{rrSurvivedText}{vitalText}";
                    break;
                case CustomGameMode.CaptureTheFlag:
                    summary = $"{ColorString(Main.PlayerColors[id], name)}: {CaptureTheFlag.GetStatistics(id)}";
                    if (CaptureTheFlag.IsDeathPossible) summary += $"  ({GetVitalText(id, true)})";
                    break;
                case CustomGameMode.KingOfTheZones:
                    summary = $"{ColorString(Main.PlayerColors[id], name)} - {KingOfTheZones.GetStatistics(id)}";
                    break;
                case CustomGameMode.Quiz:
                    summary = $"{ColorString(Main.PlayerColors[id], name)} - {Quiz.GetStatistics(id)}";
                    break;
                case CustomGameMode.TheMindGame:
                    summary = $"{ColorString(Main.PlayerColors[id], name)} - {TheMindGame.GetStatistics(id)}";
                    break;
                case CustomGameMode.BedWars:
                    summary = $"{ColorString(Main.PlayerColors[id], name)} - {BedWars.GetStatistics(id)}";
                    break;
            }

            return check && GetDisplayRoleName(id, true).RemoveHtmlTags().Contains("INVALID:NotAssigned")
                ? "INVALID"
                : disableColor
                    ? summary.RemoveHtmlTags()
                    : summary;
        }
        catch (Exception e)
        {
            ThrowException(e);
            return $"{id.ColoredPlayerName()} - ERROR";
        }
    }

    public static string GetRemainingKillers(bool notify = false, bool showAll = false, byte excludeId = byte.MaxValue)
    {
        var impnum = 0;
        var neutralnum = 0;
        var covenNum = 0;

        bool impShow = showAll || Options.ShowImpRemainOnEject.GetBool();
        bool nkShow = showAll || Options.ShowNKRemainOnEject.GetBool();
        bool covenShow = showAll || Options.ShowCovenRemainOnEject.GetBool();

        if (!impShow && !nkShow && !covenShow) return string.Empty;

        foreach (PlayerControl pc in Main.AllPlayerControls)
        {
            bool exclude = excludeId != byte.MaxValue && pc.PlayerId == excludeId;

            if (Forger.Forges.TryGetValue(pc.PlayerId, out var forgedRole) && (exclude || ExileController.Instance || !pc.IsAlive()))
            {
                if (impShow && forgedRole.Is(Team.Impostor)) impnum--;
                else if (nkShow && forgedRole.IsNK()) neutralnum--;
                else if (covenShow && forgedRole.Is(Team.Coven)) covenNum--;
            }
            else if (pc.IsAlive() && !exclude)
            {
                if (impShow && pc.Is(Team.Impostor)) impnum++;
                else if (nkShow && pc.IsNeutralKiller()) neutralnum++;
                else if (covenShow && pc.Is(Team.Coven)) covenNum++;
            }
        }

        impShow &= impnum > 0;
        nkShow &= neutralnum > 0;
        covenShow &= covenNum > 0;

        if (!impShow && !nkShow && !covenShow) return string.Empty;

        StringBuilder sb = new();

        sb.Append(notify ? "<#777777>" : string.Empty);

        int numberToUse = impShow ? impnum : nkShow ? neutralnum : covenNum;
        sb.Append(numberToUse == 1 ? GetString("RemainingText.Prefix.Single") : GetString("RemainingText.Prefix.Plural"));
        sb.Append(notify ? " " : "\n");

        if (impShow)
        {
            sb.Append(notify ? "<#ffffff>" : "<b>");
            sb.Append(impnum);
            sb.Append(notify ? "</color>" : "</b>");
            sb.Append(' ');
            sb.Append($"<#ff1919>{(impnum == 1 ? GetString("RemainingText.ImpSingle") : GetString("RemainingText.ImpPlural"))}</color>");

            if (nkShow ^ covenShow) sb.Append(" & ");
            else if (nkShow) sb.Append(", ");
        }

        if (nkShow)
        {
            sb.Append(notify ? "<#ffffff>" : "<b>");
            sb.Append(neutralnum);
            sb.Append(notify ? "</color>" : "</b>");
            sb.Append(' ');
            sb.Append($"<#ffab1b>{(neutralnum == 1 ? GetString("RemainingText.NKSingle") : GetString("RemainingText.NKPlural"))}</color>");
            if (covenShow) sb.Append(" & ");
        }

        if (covenShow)
        {
            sb.Append(notify ? "<#ffffff>" : "<b>");
            sb.Append(covenNum);
            sb.Append(notify ? "</color>" : "</b>");
            sb.Append(' ');
            sb.Append($"<#7b3fbb>{(covenNum == 1 ? GetString("RemainingText.CovenSingle") : GetString("RemainingText.CovenPlural"))}</color>");
        }

        sb.Append(GetString("RemainingText.Suffix"));
        sb.Append('.');
        sb.Append(notify ? "</color>" : string.Empty);

        return sb.ToString();
    }

    public static string RemoveHtmlTags(this string str)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        return Regex.Replace(str, "<[^>]*?>", string.Empty);
    }

    public static void FlashColor(Color color, float duration = 1f)
    {
        HudManager hud = FastDestroyableSingleton<HudManager>.Instance;
        if (hud.FullScreen == null) return;

        GameObject obj = hud.transform.FindChild("FlashColor_FullScreen")?.gameObject;

        if (obj == null)
        {
            obj = Object.Instantiate(hud.FullScreen.gameObject, hud.transform);
            obj.name = "FlashColor_FullScreen";
        }

        hud.StartCoroutine(Effects.Lerp(duration, new Action<float>(t =>
        {
            obj.SetActive(Math.Abs(t - 1f) > 0.1f);
            obj.GetComponent<SpriteRenderer>().color = new(color.r, color.g, color.b, Mathf.Clamp01(((-2f * Mathf.Abs(t - 0.5f)) + 1) * color.a / 2));
        })));
    }

    public static Sprite LoadSprite(string path, float pixelsPerUnit = 1f)
    {
        try
        {
            if (CachedSprites.TryGetValue(path + pixelsPerUnit, out Sprite sprite)) return sprite;

            Texture2D texture = LoadTextureFromResources(path);
            sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new(0.5f, 0.5f), pixelsPerUnit);
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
            return CachedSprites[path + pixelsPerUnit] = sprite;
        }
        catch { Logger.Error($"Error loading texture from: {path}", "LoadImage"); }

        return null;
    }

    [SuppressMessage("ReSharper", "MustUseReturnValue")]
    private static unsafe Texture2D LoadTextureFromResources(string path)
    {
        try
        {
            Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(path);

            if (stream != null)
            {
                long length = stream.Length;
                var byteTexture = new Il2CppStructArray<byte>(length);
                stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
                texture.LoadImage(byteTexture, false);
            }

            return texture;
        }
        catch { Logger.Error($"Error loading texture: {path}", "LoadImage"); }

        return null;
    }

    public static string ColorString(Color32 color, string str)
    {
        return $"<#{color.r:x2}{color.g:x2}{color.b:x2}{color.a:x2}>{str}</color>";
    }

    /// <summary>
    ///     Darkness:Mix black and original color in a ratio of 1. If it is negative, it will be mixed with white.
    /// </summary>
    public static Color ShadeColor(this Color color, float darkness = 0)
    {
        bool isDarker = darkness >= 0;
        if (!isDarker) darkness = -darkness;

        float weight = isDarker ? 0 : darkness;
        float r = (color.r + weight) / (darkness + 1);
        float g = (color.g + weight) / (darkness + 1);
        float b = (color.b + weight) / (darkness + 1);
        return new(r, g, b, color.a);
    }

    public static void SetChatVisibleForAll()
    {
        if (!GameStates.IsInGame) return;
        Main.AllAlivePlayerControls.Do(x => x.SetChatVisible(true));
    }

    public static bool TryCast<T>(this Il2CppObjectBase obj, out T casted) where T : Il2CppObjectBase
    {
        casted = obj.TryCast<T>();
        return casted != null;
    }

    public static string GetRegionName(IRegionInfo region = null)
    {
        region ??= ServerManager.Instance.CurrentRegion;

        string name = region.Name;

        if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
        {
            name = "Local Game";
            return name;
        }

        if (region.PingServer.EndsWith("among.us", StringComparison.Ordinal))
        {
            // Official server
            name = name switch
            {
                "North America" => "NA",
                "Europe" => "EU",
                "Asia" => "AS",
                _ => name
            };

            return name;
        }

        string ip = region.Servers.FirstOrDefault()?.Ip ?? string.Empty;

        if (ip.Contains("aumods.us", StringComparison.Ordinal) || ip.Contains("duikbo.at", StringComparison.Ordinal))
        {
            // Official Modded Server
            if (ip.Contains("au-eu"))
                name = "MEU";
            else if (ip.Contains("au-as"))
                name = "MAS";
            else
                name = "MNA";

            return name;
        }

        if (name.Contains("Niko", StringComparison.OrdinalIgnoreCase))
            name = name.Replace("233(", "-").TrimEnd(')');

        return name;
    }

    private static (int AddonsProgress, int RolesProgress) QuickSetupProgress;

    public static void EnterQuickSetupRoles(bool addons)
    {
        int progress = addons ? QuickSetupProgress.AddonsProgress : QuickSetupProgress.RolesProgress;
        bool continuation = progress > 0;
        
        int all = Options.CustomRoleSpawnChances.Keys.Count(x => addons ? x.IsAdditionRole() : !x.IsAdditionRole());
        var count = 0;

        foreach ((CustomRoles role, StringOptionItem option) in Options.CustomRoleSpawnChances)
        {
            if (addons ? !role.IsAdditionRole() : role.IsAdditionRole() || role.IsVanilla() || role.IsForOtherGameMode()) continue;

            count++;

            if (continuation && progress >= count) continue;

            string str = GetString($"{role}InfoLong");
            string infoLong;

            try { infoLong = CustomHnS.AllHnSRoles.Contains(role) ? str : str[(str.IndexOf('\n') + 1)..str.Split("\n\n")[0].Length]; }
            catch { infoLong = str; }

            string rotStr;

            if (!role.IsAdditionRole())
            {
                RoleOptionType rot = role.GetRoleOptionType();
                rotStr = ColorString(rot.GetRoleOptionTypeColor(), GetString($"ROT.{rot}"));
            }
            else
            {
                AddonTypes at = Options.GroupedAddons.First(x => x.Value.Contains(role)).Key;
                rotStr = ColorString(at.GetAddonTypeColor(), GetString($"ROT.AddonType.{at}"));
            }

            Action increment = () =>
            {
                if (addons) QuickSetupProgress.AddonsProgress++;
                else QuickSetupProgress.RolesProgress++;

                if (addons && QuickSetupProgress.AddonsProgress >= all)
                    QuickSetupProgress = (0, 0);
            };

            Prompt.Show(
                string.Format(GetString("Promt.EnableRole"), count, all, rotStr, role.ToColoredString(), infoLong),
                (() => option.SetValue(1)) + increment,
                (() => option.SetValue(0)) + increment);
        }

        if (addons) return;
        Prompt.Show(GetString("Promt.ContinueWithQuickSetupAddons"), () => EnterQuickSetupRoles(true), () => { });
    }

    private static int PlayersCount(CountTypes countTypes)
    {
        var count = 0;

        foreach (PlayerState state in Main.PlayerStates.Values)
        {
            if (state.countTypes == countTypes)
                count++;
        }

        return count;
    }

    public static int AlivePlayersCount(CountTypes countTypes)
    {
        return Main.AllAlivePlayerControls.Count(pc => pc.Is(countTypes));
    }

    public static bool IsPlayerModdedClient(this byte id)
    {
        return Main.PlayerVersion.ContainsKey(id);
    }

    // The minimum number of seconds that should be waited between two CheckMurder calls
    public static float CalculatePingDelay()
    {
        // The value of AmongUsClient.Instance.Ping is in milliseconds (ms), so ÷1000 to convert to seconds
        float divice = Options.CurrentGameMode switch
        {
            CustomGameMode.BedWars => 3000f,
            CustomGameMode.SoloKombat => 3000f,
            CustomGameMode.CaptureTheFlag => 1500f,
            CustomGameMode.KingOfTheZones => 1500f,
            _ => 1000f
        };

        float minTime = Mathf.Max(0.2f, AmongUsClient.Instance.Ping / divice * 6f);
        return minTime;
    }

    // Next 2: From MoreGamemodes by Rabek009

    public static void CreateDeadBody(Vector3 position, byte colorId, PlayerControl deadBodyParent)
    {
        int baseColorId = deadBodyParent.Data.DefaultOutfit.ColorId;
        deadBodyParent.Data.DefaultOutfit.ColorId = colorId;
        DeadBody deadBody = Object.Instantiate(GameManager.Instance.DeadBodyPrefab);
        deadBody.enabled = false;
        deadBody.ParentId = deadBodyParent.PlayerId;
        foreach (SpriteRenderer b in deadBody.bodyRenderers)
            deadBodyParent.SetPlayerMaterialColors(b);
        deadBodyParent.SetPlayerMaterialColors(deadBody.bloodSplatter);
        Vector3 vector = position + deadBodyParent.KillAnimations[0].BodyOffset;
        vector.z = vector.y / 1000f;
        deadBody.transform.position = vector;
        deadBodyParent.Data.DefaultOutfit.ColorId = baseColorId;
    }

    public static void RpcCreateDeadBody(Vector3 position, byte colorId, PlayerControl deadBodyParent, SendOption sendOption = SendOption.Reliable)
    {
        if (deadBodyParent == null || !Main.IntroDestroyed) return;
        CreateDeadBody(position, colorId, deadBodyParent);
        PlayerControl playerControl = Object.Instantiate(AmongUsClient.Instance.PlayerPrefab, Vector2.zero, Quaternion.identity);
        playerControl.PlayerId = deadBodyParent.PlayerId;
        playerControl.isNew = false;
        playerControl.notRealPlayer = true;
        playerControl.NetTransform.SnapTo(position);
        AmongUsClient.Instance.NetIdCnt += 1U;
        var sender = CustomRpcSender.Create("Utils.RpcCreateDeadBody", sendOption, true, false);
        MessageWriter writer = sender.stream;
        sender.StartMessage();
        writer.StartMessage(4);
        SpawnGameDataMessage item = AmongUsClient.Instance.CreateSpawnMessage(playerControl, -2, SpawnFlags.None);
        item.SerializeValues(writer);
        writer.EndMessage();

        if (GameStates.CurrentServerType == GameStates.ServerType.Vanilla)
        {
            for (uint i = 1; i <= 3; ++i)
            {
                writer.StartMessage(4);
                writer.WritePacked(2U);
                writer.WritePacked(-2);
                writer.Write((byte)SpawnFlags.None);
                writer.WritePacked(1);
                writer.WritePacked(AmongUsClient.Instance.NetIdCnt - i);
                writer.StartMessage(1);
                writer.EndMessage();
                writer.EndMessage();
            }
        }

        if (PlayerControl.AllPlayerControls.Contains(playerControl))
            PlayerControl.AllPlayerControls.Remove(playerControl);

        int baseColorId = playerControl.Data.DefaultOutfit.ColorId;
        sender.StartRpc(playerControl.NetId, RpcCalls.SetColor)
            .Write(playerControl.Data.NetId)
            .Write(colorId)
            .EndRpc();
        sender.StartRpc(playerControl.NetId, RpcCalls.MurderPlayer)
            .WriteNetObject(playerControl)
            .Write((int)MurderResultFlags.Succeeded)
            .EndRpc();
        sender.StartRpc(playerControl.NetId, RpcCalls.SetColor)
            .Write(playerControl.Data.NetId)
            .Write(baseColorId)
            .EndRpc();
        writer.StartMessage(1);
        writer.WritePacked(playerControl.Data.NetId);
        playerControl.Data.Serialize(writer, false);
        writer.EndMessage();
        writer.StartMessage(5);
        writer.WritePacked(playerControl.NetId);
        writer.EndMessage();
        AmongUsClient.Instance.RemoveNetObject(playerControl);
        Object.Destroy(playerControl.gameObject);
        sender.EndMessage();
        sender.SendMessage();
    }
}

public class Message(string text, byte sendTo = byte.MaxValue, string title = "")
{
    public string Text { get; } = text;
    public byte SendTo { get; } = sendTo;
    public string Title { get; } = title;
}
