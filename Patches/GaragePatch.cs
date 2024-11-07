using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace AWDPatches
{
    internal class GarageCarUIPatch
    {
        private static bool _addAWDPostfix = false;
        private static bool _addFWDPostfix = false;

        [HarmonyPatch(typeof(homegarageSpot), "homeGarage_3DUI")]
        [HarmonyPrefix]
        public static void BeforeUpdateGarageCarInfoUI(homegarageSpot __instance)
        {
            _addAWDPostfix = false;
            _addFWDPostfix = false;

            var god = GodConstant.Instance;

            // Early exit if certain conditions are met
            if (god.gameIsPaused || __instance.noInput_ui() || god.phoneCall_currentActive || __instance.customization_Mode != homegarageSpot.Customization_Mode.null_type)
            {
                return;
            }

            var ui = __instance.carInfo_infoScript;
            if (!ui || !ui.gameObject)
            {
                return;
            }

            // Check if the UI is not active
            if (!ui.gameObject.activeSelf)
            {
                // Cast ray to check for the car
                var cast = __instance.meetSpot_PlayerScript.player_rayCast();
                if (!cast || !cast.carData)
                {
                    return;
                }

                // Check if the car is AWD
                if (cast.carData.drivetrain == CarData.Drivetrain.AWD)
                {
                    _addAWDPostfix = true;
                }

                // Check if the car is FWD
                if (cast.carData.drivetrain == CarData.Drivetrain.FWD)
                {
                    _addFWDPostfix = true;
                }
            }
        }

        [HarmonyPatch(typeof(homegarageSpot), "homeGarage_3DUI")]
        [HarmonyPostfix]
        public static void UpdateGarageCarInfoUI(homegarageSpot __instance)
        {
            var ui = __instance.carInfo_infoScript;
            if (ui.gameObject.activeSelf)
            {
                // Append the postfix if conditions are met
                if (_addAWDPostfix)
                {
                    ui.textLines[0].textLine.text += " [CSK AWD]";
                }
                if (_addFWDPostfix)
                {
                    ui.textLines[0].textLine.text += " [CSK FWD]";
                }
            }
        }

        private static bool IsAWD(int saveID)
        {
            // Implementation for checking AWD status, currently returning true for testing
            return true;
        }

        private static bool IsFWD(int saveID)
        {
            // Implementation for checking FWD status, currently returning true for testing
            return true;
        }
    }
}
