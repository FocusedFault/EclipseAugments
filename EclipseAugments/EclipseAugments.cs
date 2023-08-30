using BepInEx;
using RoR2;
using R2API;
using HarmonyLib;
using MonoMod.Cil;
using UnityEngine;

namespace EclipseAugments
{
  [BepInPlugin("com.Nuxlar.EclipseAugments", "EclipseAugments", "1.0.0")]

  public class EclipseAugments : BaseUnityPlugin
  {

    public void Awake()
    {
      IncreaseScaling();
      ChangeDescriptions();
      IL.RoR2.CharacterMaster.OnBodyStart += RemoveE1Modifier;
      IL.RoR2.HoldoutZoneController.FixedUpdate += RemoveE2Modifier;
      IL.RoR2.DeathRewards.OnKilledServer += RemoveE6Modifier;
      Harmony.CreateAndPatchAll(this.GetType(), null);
    }

    [HarmonyPatch(typeof(Run), "RecalculateDifficultyCoefficent")]
    [HarmonyPostfix]
    private static void RecalculateDifficultyCoefficient(Run __instance)
    {
      if (__instance.selectedDifficulty < DifficultyIndex.Eclipse1)
        return;
      float num = (float)((0.75 * __instance.participatingPlayerCount + 1.75) / 15.0);
      __instance.compensatedDifficultyCoefficient += num;
      __instance.difficultyCoefficient += num;
    }

    private void IncreaseScaling()
    {
      DifficultyCatalog.GetDifficultyDef(DifficultyIndex.Eclipse2).scalingValue *= 1.25f;
      DifficultyCatalog.GetDifficultyDef(DifficultyIndex.Eclipse3).scalingValue *= 1.25f;
      DifficultyCatalog.GetDifficultyDef(DifficultyIndex.Eclipse4).scalingValue *= 1.25f;
      DifficultyCatalog.GetDifficultyDef(DifficultyIndex.Eclipse5).scalingValue *= 1.25f;
      DifficultyCatalog.GetDifficultyDef(DifficultyIndex.Eclipse6).scalingValue *= 1.25f;
      DifficultyCatalog.GetDifficultyDef(DifficultyIndex.Eclipse7).scalingValue *= 1.25f;
      DifficultyCatalog.GetDifficultyDef(DifficultyIndex.Eclipse8).scalingValue *= 1.25f;
    }

    private void ChangeDescriptions()
    {
      string str1 = "Starts at baseline Monsoon difficulty.\n";
      string str2 = "\n<mspace=0.5em>(1)</mspace> Initial Difficulty: <style=cIsHealth>+25%</style></style>";
      string str3 = "\n<mspace=0.5em>(2)</mspace> Difficulty Scaling: <style=cIsHealth>+25%</style></style>";
      string str4 = "\n<mspace=0.5em>(3)</mspace> Ally Fall Damage: <style=cIsHealth>+100% and lethal</style></style>";
      string str5 = "\n<mspace=0.5em>(4)</mspace> Enemies: <style=cIsHealth>+40% Faster</style></style>";
      string str6 = "\n<mspace=0.5em>(5)</mspace> Ally Healing: <style=cIsHealth>-50%</style></style>";
      string str7 = "\n<mspace=0.5em>(6)</mspace> Enemy Gold Drops: <style=cIsHealth>-10%</style></style>";
      string str8 = "\n<mspace=0.5em>(7)</mspace> Enemy Cooldowns: <style=cIsHealth>-50%</style></style>";
      string str9 = "\n<mspace=0.5em>(8)</mspace> Allies recieve <style=cIsHealth>permanent damage</style></style>";
      string str10 = "\"You only celebrate in the light... because I allow it.\" \n\n";
      LanguageAPI.Add("ECLIPSE_1_DESCRIPTION", str1 + str2);
      LanguageAPI.Add("ECLIPSE_2_DESCRIPTION", str1 + str2 + str3);
      LanguageAPI.Add("ECLIPSE_3_DESCRIPTION", str1 + str2 + str3 + str4);
      LanguageAPI.Add("ECLIPSE_4_DESCRIPTION", str1 + str2 + str3 + str4 + str5);
      LanguageAPI.Add("ECLIPSE_5_DESCRIPTION", str1 + str2 + str3 + str4 + str5 + str6);
      LanguageAPI.Add("ECLIPSE_6_DESCRIPTION", str1 + str2 + str3 + str4 + str5 + str6 + str7);
      LanguageAPI.Add("ECLIPSE_7_DESCRIPTION", str1 + str2 + str3 + str4 + str5 + str6 + str7 + str8);
      LanguageAPI.Add("ECLIPSE_8_DESCRIPTION", str10 + str1 + str2 + str3 + str4 + str5 + str6 + str7 + str8 + str9);
    }

    private void RemoveE1Modifier(ILContext il)
    {
      ILCursor ilCursor = new ILCursor(il);
      if (ilCursor.TryGotoNext(MoveType.Before, x => ILPatternMatchingExt.MatchLdcR4(x, 0.5f)))
        ilCursor.Next.Operand = 1f;
      else
        Debug.LogError("EclipseAugments: RemoveE1Modifier IL Hook failed");
    }

    private void RemoveE2Modifier(ILContext il)
    {
      ILCursor ilCursor = new ILCursor(il);
      if (ilCursor.TryGotoNext(MoveType.Before, x => ILPatternMatchingExt.MatchLdcR4(x, 0.5f)))
        ilCursor.Next.Operand = 1f;
      else
        Debug.LogError("EclipseAugments: RemoveE2Modifier IL Hook failed");
    }

    private void RemoveE6Modifier(ILContext il)
    {
      ILCursor c = new(il);
      if (c.TryGotoNext(MoveType.Before, x => x.MatchLdcR4(0.8f)))
        c.Next.Operand = 1f;
      else
        Debug.LogError("EclipseAugments: RemoveE6Modifier IL Hook failed");
    }

  }
}