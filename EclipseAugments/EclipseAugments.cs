using BepInEx;
using RoR2;
using R2API;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine;
using System;

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
      IL.RoR2.HealthComponent.TakeDamage += ReduceAdaptiveArmor;
      On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
      On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
      On.RoR2.GlobalEventManager.OnCharacterHitGroundServer += GlobalEventManager_OnCharacterHitGroundServer;
    }

    private void IncreaseScaling()
    {
      DifficultyCatalog.GetDifficultyDef(DifficultyIndex.Eclipse1).scalingValue *= 1.25f;
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
      string str2 = "\n<mspace=0.5em>(1)</mspace> Difficulty Scaling: <style=cIsHealth>+25%</style></style>";
      string str3 = "\n<mspace=0.5em>(2)</mspace> Elite Bias: <style=cIsHealth>+25%</style></style>";
      string str4 = "\n<mspace=0.5em>(3)</mspace> Ally Fall Damage: <style=cIsHealth>+100%</style></style>";
      string str5 = "\n<mspace=0.5em>(4)</mspace> Enemies: <style=cIsHealth>+40% Faster</style></style>";
      string str6 = "\n<mspace=0.5em>(5)</mspace> Ally Healing: <style=cIsHealth>-50%</style></style>";
      string str7 = "\n<mspace=0.5em>(6)</mspace> Enemy Gold Drops: <style=cIsHealth>-10%</style></style>";
      string str8 = "\n<mspace=0.5em>(7)</mspace> Boss Armor: <style=cIsHealth>Adaptive</style></style>";
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
      ILCursor c = new ILCursor(il);
      if (c.TryGotoNext(MoveType.Before, x => ILPatternMatchingExt.MatchLdcR4(x, 0.5f)))
        c.Next.Operand = 1f;
      else
        Debug.LogError("EclipseAugments: RemoveE1Modifier IL Hook failed");
    }

    private void RemoveE2Modifier(ILContext il)
    {
      ILCursor c = new ILCursor(il);
      if (c.TryGotoNext(MoveType.Before, x => ILPatternMatchingExt.MatchLdcR4(x, 0.5f)))
        c.Next.Operand = 1f;
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

    private void ReduceAdaptiveArmor(ILContext il)
    {
      ILCursor c = new(il);
      if (c.TryGotoNext(MoveType.After,
          x => x.MatchLdcR4(400)))
      {
        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Func<float, HealthComponent, float>>((armorCap, self) =>
        {
          if (self.body.name.Contains("Brother"))
            return armorCap;
          else
            return 100f;
        });
      }
      else
        Logger.LogError("EclipseAugments: ReduceAdaptiveArmor IL Hook failed");
    }

    private void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
    {
      orig(self, body);
      if (body.inventory && body.isBoss && !body.name.Contains("Brother"))
        body.inventory.GiveItemString("AdaptiveArmor");
    }

    private void CombatDirector_Awake(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
    {
      orig(self);
      if (Run.instance && Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse2)
        self.eliteBias /= 1.25f;
    }

    private void GlobalEventManager_OnCharacterHitGroundServer(On.RoR2.GlobalEventManager.orig_OnCharacterHitGroundServer orig, GlobalEventManager self, CharacterBody characterBody, Vector3 impactVelocity)
    {
      bool flag1 = RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.weakAssKneesArtifactDef);
      float num1 = Mathf.Abs(impactVelocity.y);
      Inventory inventory = characterBody.inventory;
      CharacterMaster master = characterBody.master;
      CharacterMotor characterMotor = characterBody.characterMotor;
      bool flag2 = false;
      if (((bool)(UnityEngine.Object)inventory ? inventory.GetItemCount(RoR2Content.Items.FallBoots) : 0) <= 0 && (characterBody.bodyFlags & CharacterBody.BodyFlags.IgnoreFallDamage) == CharacterBody.BodyFlags.None)
      {
        float num2 = Mathf.Max(num1 - (characterBody.jumpPower + 20f), 0.0f);
        if ((double)num2 > 0.0)
        {
          HealthComponent component = characterBody.GetComponent<HealthComponent>();
          if ((bool)(UnityEngine.Object)component)
          {
            flag2 = true;
            float num3 = num2 / 60f;
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.attacker = (GameObject)null;
            damageInfo.inflictor = (GameObject)null;
            damageInfo.crit = false;
            damageInfo.damage = num3 * characterBody.maxHealth;
            damageInfo.damageType = DamageType.NonLethal | DamageType.FallDamage;
            damageInfo.force = Vector3.zero;
            damageInfo.position = characterBody.footPosition;
            damageInfo.procCoefficient = 0.0f;
            if (flag1 || characterBody.teamComponent.teamIndex == TeamIndex.Player && Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3)
              damageInfo.damage *= 2f;
            component.TakeDamage(damageInfo);
          }
        }
      }
      if (!(bool)(UnityEngine.Object)characterMotor || (double)(Run.FixedTimeStamp.now - characterMotor.lastGroundedTime) <= 0.200000002980232)
        return;
      Vector3 footPosition = characterBody.footPosition;
      float radius = characterBody.radius;
      RaycastHit hitInfo;
      if (!Physics.Raycast(new Ray(footPosition + Vector3.up * 1.5f, Vector3.down), out hitInfo, 4f, (int)LayerIndex.world.mask | (int)LayerIndex.water.mask, QueryTriggerInteraction.Collide))
        return;
      SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(hitInfo.collider, hitInfo.point);
      if (!(bool)(UnityEngine.Object)objectSurfaceDef)
        return;
      EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData()
      {
        origin = footPosition,
        scale = radius,
        color = (Color32)objectSurfaceDef.approximateColor
      }, true);
      if ((bool)(UnityEngine.Object)objectSurfaceDef.footstepEffectPrefab)
        EffectManager.SpawnEffect(objectSurfaceDef.footstepEffectPrefab, new EffectData()
        {
          origin = hitInfo.point,
          scale = radius * 3f
        }, false);
      SfxLocator component1 = characterBody.GetComponent<SfxLocator>();
      if (!(bool)(UnityEngine.Object)component1)
        return;
      if (objectSurfaceDef.materialSwitchString != null && objectSurfaceDef.materialSwitchString.Length > 0)
      {
        int num4 = (int)AkSoundEngine.SetSwitch("material", objectSurfaceDef.materialSwitchString, characterBody.gameObject);
      }
      else
      {
        int num5 = (int)AkSoundEngine.SetSwitch("material", "dirt", characterBody.gameObject);
      }
      int num6 = (int)Util.PlaySound(component1.landingSound, characterBody.gameObject);
      if (!flag2)
        return;
      int num7 = (int)Util.PlaySound(component1.fallDamageSound, characterBody.gameObject);
    }
  }
}