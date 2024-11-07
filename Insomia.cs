using UnityEngine;
using Il2Cpp;
using MelonLoader;
using System.IO;
using Harmony;
using HarmonyLib;
using AWDPatches;
using AWDPatches_Model;

namespace Insomia
{
    public class InsomiaMod : MelonMod
    {
        // public float drivetrainbias; quite literally useless

        public string textToDisplay = "Game modified using CSK Drive trains, DO NOT SUBMIT ANY RUNS TO THE LEADERBOARD USING THIS MOD!";
        public int fontSize = 12;
        private bool showOverlay = false;

        private GUIStyle guiStyle;
        private BiasedDrivetrainOptions drivetrainOptions;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Made by Scoolnik and Coloured!");
            LoggerInstance.Msg("Ever wanted a FWD livisa?");
        }

        public override void OnApplicationStart()
        {
            Harmony.PatchAll(typeof(CarDrivetrainPatch));
            Harmony.PatchAll(typeof(GarageCarUIPatch));

            guiStyle = new GUIStyle
            {
                fontSize = fontSize,
                alignment = TextAnchor.MiddleRight,
                normal = new GUIStyleState { textColor = Color.white }
            };

            drivetrainOptions = new BiasedDrivetrainOptions();
            LoggerInstance.Msg("DrivetrainOptions initialized."); 
        }

        public override void OnUpdate()
        {
            // Check for right shift key press
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                showOverlay = !showOverlay; // Toggle overlay visibility

                if (showOverlay)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

        public override void OnGUI()
        {
            // who would've guessed the peoples of nightcordia don't like a watermark
            
            /*
            Vector2 textSize = guiStyle.CalcSize(new GUIContent(textToDisplay));
            float x = Screen.width - textSize.x - 10; // 10 pixels from the right edge
            float y = Screen.height - textSize.y - 10; // 10 pixels from the bottom edge
            GUI.Label(new Rect(x, y, textSize.x, textSize.y), textToDisplay, guiStyle);
            */

            // i love "if" statements
            if (showOverlay)
            {
                // i hate having to restart the game every time just to fix the overlay
                GUI.Box(new Rect(10, 10, 200, 250), "CSK Drivetrains");

                // Slider for BiasedTorque
                AWDPatches.CarDrivetrainPatch._stockFWDCarChance = GUI.HorizontalSlider(new Rect(20, 60, 160, 20), AWDPatches.CarDrivetrainPatch._stockFWDCarChance, 0.0f, 1.0f);
                AWDPatches.CarDrivetrainPatch._stockAWDCarChance = GUI.HorizontalSlider(new Rect(20, 100, 160, 20), AWDPatches.CarDrivetrainPatch._stockAWDCarChance, 0.0f, 1.0f);
                //AWDPatches.CarDrivetrainPatch._tunedFWDCarChance = GUI.HorizontalSlider(new Rect(20, 140, 160, 20), AWDPatches.CarDrivetrainPatch._tunedFWDCarChance, 0.0f, 1.0f);
                //AWDPatches.CarDrivetrainPatch._tunedAWDCarChance = GUI.HorizontalSlider(new Rect(20, 180, 160, 20), AWDPatches.CarDrivetrainPatch._tunedAWDCarChance, 0.0f, 1.0f);


                // math (ew)
                float FWD= Mathf.Round(AWDPatches.CarDrivetrainPatch._stockFWDCarChance * 100 / 10) * 10;
                float AWD = Mathf.Round(AWDPatches.CarDrivetrainPatch._stockAWDCarChance * 100 / 10) * 10;
                //float displayedValue3 = Mathf.Round(AWDPatches.CarDrivetrainPatch._stockFWDCarChance * 100 / 10) * 10;
                //float displayedValue4 = Mathf.Round(AWDPatches.CarDrivetrainPatch._stockAWDCarChance * 100 / 10) * 10;

                // text!!
                GUI.Label(new Rect(20, 40, 160, 20), $"Stock FWD Odds: {FWD:F0}%", guiStyle);
                GUI.Label(new Rect(20, 80, 160, 20), $"Stock AWD Odds: {AWD:F0}%", guiStyle);

                // maybe these will come later? I'm to lazy for ts

                // GUI.Label(new Rect(20, 120, 160, 20), $"Tuned FWD Odds: {displayedValue3:F0}%", guiStyle);
                // GUI.Label(new Rect(20, 160, 160, 20), $"Tuned FWD Odds: {displayedValue4:F0}%", guiStyle);
            }
        }
    }
}
