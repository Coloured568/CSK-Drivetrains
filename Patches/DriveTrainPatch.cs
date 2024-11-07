using System;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using UnityEngine;
using UnityEngine.UI;
using ChassisType = Il2Cpp.car_carOrigin.ChassisType;
using WheelType = Il2Cpp.RCC_CarControllerV3.WheelType;
using System.Reflection;
using UnityEngine.Playables;
using Il2CppRewired;

namespace AWDPatches
{
    internal class CarDrivetrainPatch
    {
        public static float _stockAWDCarChance = 0.33f;
        // public static float _tunedAWDCarChance = 0.25f;
        public static float _stockFWDCarChance = 0.25f;
        // public static float _tunedFWDCarChance = 0.33f;
        private const string Legacy_AWDSaveFlagPrefix = "shopCar_isAWD";
        private const string Legacy_FWDSaveFlagPrefix = "shopCar_isFWD";
        // private const string StockFWDCarNote = "This car is actually FWD, hopefully I can patch the stupid circle out next update!";
        private const string ColouredsNote = "Made with SWAG by Coloured, with best wishes from Silver Stars!";
        
        private static readonly string[] StockFWDCarNote = { "Actually a FWD car, patching sucks!!" };
        private static readonly string[] ColouredsCarNote = { "CSK Drivetrains Made with swag by Coloured and Scoolnik!" };

        private static readonly ChassisType[] _awdChassis = new ChassisType[]
        {
            ChassisType.sannis_sykina_2door_1998,
            ChassisType.sannis_sykina_2door_1989,
            ChassisType.kymoto_sprecia_1996_2001,
            ChassisType.korschen_911_1989_1994,
            ChassisType.korschen_911_turbo_1989_1994,
            ChassisType.sannis_livisa_coupe_1989,
            ChassisType.sannis_livisa_hatch_1989,
            ChassisType.sannis_livisa_1999
        };

        private static readonly ChassisType[] _fwdChassis = new ChassisType[]
        {
            ChassisType.sannis_sykina_2door_1998,
            ChassisType.sannis_sykina_2door_1989,
            ChassisType.kymoto_sprecia_1996_2001,
            ChassisType.korschen_911_1989_1994,
            ChassisType.korschen_911_turbo_1989_1994,
            ChassisType.sannis_livisa_coupe_1989,
            ChassisType.sannis_livisa_hatch_1989,
            ChassisType.sannis_livisa_1999
        };

        private const float AWDPriceMultiplier = 1.5f;
        private const float FWDPriceMultiplier = 0.8f;

        [HarmonyPatch(typeof(GodConstant), "shopCarsResetDealerCars")]
        [HarmonyPrefix]
        public static void RemoveShopCarsSave(string __0)
        {
        }

        [HarmonyPatch(typeof(GodConstant), "CarDeleteSave")]
        [HarmonyPrefix]
        public static void RemoveShopCarSave(int __0)
        {
        }

        

        [HarmonyPatch(typeof(GodConstant), "shopCarSave")]
        [HarmonyPrefix]
        public static void SaveShopCar(car_overwrite __0, string __1)
        {
            var carOrigin = __0.carOrigin;

            // Check for AWD chassis and apply modification
            var isAWDChassis = _awdChassis.Contains(carOrigin.chassisType);
            var isFWDChassis = _fwdChassis.Contains(carOrigin.chassisType);
            if (isAWDChassis && Roll(_stockAWDCarChance))
            {
                Console.WriteLine($"Saving AWD car: {carOrigin.name}");
                carOrigin = GetAWDCarOrigin(carOrigin);
                __0.carAuction_Price *= AWDPriceMultiplier;
                __0.carOrigin = carOrigin;
                carOrigin.transform.SetParent(__0.transform);
                __0.carAuction_notes = (Il2CppStringArray)ColouredsCarNote;
            }
            else if (isFWDChassis && Roll(_stockFWDCarChance))
            {
                Console.WriteLine($"Saving FWD car: {carOrigin.name}");
                carOrigin = GetFWDCarOrigin(carOrigin);
                __0.carAuction_Price *= FWDPriceMultiplier;
                __0.carOrigin = carOrigin;
                carOrigin.transform.SetParent(__0.transform);
                // will be used in another project.
                //__0.inductionType = CarData.InductionType.SUPERCHARGE;
                // var message = isAWDChassis ? StockFWDCarNote : ColouredsNote;

                __0.carAuction_notes = (Il2CppStringArray)StockFWDCarNote;

                //ES3.Save(Legacy_FWDSaveFlagPrefix + __1, true, GodConstant.Instance.es3_settings);
            }
        }

        [HarmonyPatch(typeof(CarParent), "shopCarLoad")]
        [HarmonyPostfix]
        public static void LoadShopCar(GodConstant.ShopCar_SaveElement __0, car_overwrite __result)
        {
            // Ensure the loaded car has the correct drivetrain
            if (__result.carOrigin.stockDrivetrain == CarData.Drivetrain.AWD && !_awdChassis.Contains(__result.carOrigin.chassisType))
            {
                Console.WriteLine($"Loaded car {__result.carOrigin.name} should be AWD but is not in AWD chassis list.");
                __result.carOrigin.stockDrivetrain = CarData.Drivetrain.RWD; // Set to RWD or handle accordingly    
            }
            else if (__result.carOrigin.stockDrivetrain == CarData.Drivetrain.FWD && !_fwdChassis.Contains(__result.carOrigin.chassisType))
            {
                Console.WriteLine($"Loaded car {__result.carOrigin.name} should be FWD but is not in FWD chassis list.");
                __result.carOrigin.stockDrivetrain = CarData.Drivetrain.RWD; // Set to RWD or handle accordingly
            }
        }

        [HarmonyPatch(typeof(CarParent), "loadPerformanceParts")]
        [HarmonyPostfix]
        public static void LoadCarPerformanceParts(CarLocalCustom __0)
        {
            var carData = __0.carData;
            var controller = carData.rcc;
            controller._wheelTypeOriginal = (WheelType)carData.drivetrain;
        }

        [HarmonyPatch(typeof(CarData), "getCar_value")]
        [HarmonyPostfix]
        public static void GetCarValue(CarData __instance, ref int __result)
        {
            if (__instance.drivetrain == CarData.Drivetrain.AWD)
            {
                __result = (int)(__result * AWDPriceMultiplier);
                __instance.carPrice = __result;
            }
            else if (__instance.drivetrain == CarData.Drivetrain.FWD)
            {
                __result = (int)(__result * FWDPriceMultiplier);
                __instance.carPrice = __result;
            }
        }

        private static car_carOrigin GetAWDCarOrigin(car_carOrigin origin)
        {
            var newOrigin = GameObject.Instantiate(origin);
            newOrigin.stockDrivetrain = CarData.Drivetrain.AWD;

            Console.WriteLine($"New AWD car created: {newOrigin.name}");
            return newOrigin;
        }
        private static car_carOrigin GetFWDCarOrigin(car_carOrigin origin)
        {
            var newOrigin = GameObject.Instantiate(origin);
            newOrigin.stockDrivetrain = CarData.Drivetrain.FWD;

            Console.WriteLine($"New FWD car created: {newOrigin.name}");
            return newOrigin;
        }

        private static bool Roll(float chance)
        {
            var ch = UnityEngine.Random.Range(0, 101);
            return ch < chance * 100;
        }
    }
}
