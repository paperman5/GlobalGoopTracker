﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GlobalGoopTracker
{
    public static class GoopPatches
    {
        // Class that contains the patches related to goop and litter collection.

        public static BiomeManager nonBiomeManager;
        public static int prevPlasticsInCloud = 0;

        [HarmonyPatch(typeof(BubbleGunController))]
        public static class BubbleGunController_Patch
        {
            [HarmonyPatch(nameof(BubbleGunController.BubbleItem))]
            [HarmonyPostfix]
            public static void BubbleItem_Postfix(GameObject itemToBubble)
            {
                if (itemToBubble != null)
                {
                    Pickup component = itemToBubble.GetComponent<Pickup>();
                    if (component.surroundingBiome == null)
                    {
                        nonBiomeManager.StopTrackingPickupLitter(itemToBubble);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MicroplasticsCloud))]
        public static class MicroPlasticsCloud_Patch
        {
            [HarmonyPatch(nameof(MicroplasticsCloud.SubtractFromParticleCount))]
            [HarmonyPrefix]
            public static void SubtractFromParticleCount_Prefix(MicroplasticsCloud __instance, int amount)
            {
                if (__instance.parentBiome == null)
                {
                    prevPlasticsInCloud = __instance.currentParticlesInCloud;
                }
            }
            [HarmonyPatch(nameof(MicroplasticsCloud.SubtractFromParticleCount))]
            [HarmonyPostfix]
            public static void SubtractFromParticleCount_Postfix(MicroplasticsCloud __instance, int amount)
            {
                if (__instance.parentBiome == null)
                {
                    nonBiomeManager.AddToPlasticCloudPollution((float)(__instance.currentParticlesInCloud - prevPlasticsInCloud), false);
                }
            }
            [HarmonyPatch(nameof(MicroplasticsCloud.InitializeFromLoad))]
            [HarmonyPrefix]
            public static void InitializeFromLoad_Prefix(MicroplasticsCloud __instance, PlasticCloudSaveData loadedData)
            {
                if (__instance.parentBiome == null)
                {
                    prevPlasticsInCloud = __instance.currentParticlesInCloud;
                }
            }
            [HarmonyPatch(nameof(MicroplasticsCloud.InitializeFromLoad))]
            [HarmonyPostfix]
            public static void InitializeFromLoad_Postfix(MicroplasticsCloud __instance, PlasticCloudSaveData loadedData)
            {
                if (__instance.parentBiome == null)
                {
                    nonBiomeManager.AddToPlasticCloudPollution((float)(__instance.currentParticlesInCloud - prevPlasticsInCloud), false);
                }
            }
        }

        [HarmonyPatch(typeof(Chunk))]
        public static class Chunk_Patch
        {
            [HarmonyPatch(nameof(Chunk.DestroyChunk))]
            [HarmonyPostfix]
            public static void DestroyChunk_Postfix(Chunk __instance)
            {
                BiomeManager biomeManager = __instance.surroundingBiome;
                if (biomeManager == null)
                {
                    nonBiomeManager.StopTrackingChunkLitter(__instance.gameObject);
                }
            }
        }

        [HarmonyPatch(typeof(FlatGoop))]
        public static class FlatGoop_Patch
        {
            [HarmonyPatch(nameof(FlatGoop.Dissolve))]
            [HarmonyPostfix]
            public static void Dissolve_Postfix(FlatGoop __instance, bool instant = false)
            {
                BiomeManager biomeManager = __instance.surroundingBiome;
                if (biomeManager == null)
                {
                    nonBiomeManager.AddToGoopPollution(-1f, instant);
                }
            }
            [HarmonyPatch(nameof(FlatGoop.Undissolve))]
            [HarmonyPostfix]
            public static void Undissolve_Postfix(FlatGoop __instance, bool instant = false, bool affectBiomePollution = false)
            {
                BiomeManager biomeManager = __instance.surroundingBiome;
                if (biomeManager == null && affectBiomePollution)
                {
                    nonBiomeManager.AddToGoopPollution(1f, instant);
                }
            }
            [HarmonyPatch(nameof(FlatGoop.InitializeFromLoad))]
            [HarmonyPrefix]
            public static void InitializeFromLoad_Prefix(FlatGoop __instance, FlatGoopSaveData loadedData)
            {
                if (!__instance.hasBeenDissolved)
                {
                    BiomeManager biomeManager = __instance.surroundingBiome;
                    if (biomeManager == null)
                    {
                        nonBiomeManager.AddToGoopPollution(-1f, true);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(FloraRegrowthZone))]
        public static class FloraRegrowthZone_Patch
        {
            [HarmonyPatch(nameof(FloraRegrowthZone.AddToPollutionLevel))]
            [HarmonyPostfix]
            public static void AddToPollutionLevel_Postfix(FloraRegrowthZone __instance, int goopToAdd, bool skipEffects)
            {
                BiomeManager biomeManager = __instance.parentBiome;
                if (biomeManager == null)
                {
                    nonBiomeManager.AddToGoopPollution((float)goopToAdd, skipEffects);
                }
            }
        }

        [HarmonyPatch(typeof(FoodPlant))]
        public static class FoodPlant_Patch
        {
            [HarmonyPatch(nameof(FoodPlant.AddToPollutionLevel))]
            [HarmonyPostfix]
            public static void AddToPollutionLevel_Postfix(FoodPlant __instance, int goopToAdd, bool skipEffects)
            {
                BiomeManager biomeManager = __instance.parentBiome;
                if (biomeManager == null)
                {
                    nonBiomeManager.AddToGoopPollution((float)goopToAdd, skipEffects);
                }
            }
        }
    }
}