﻿using Celeste;
using Celeste.Mod;
using Monocle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TAS.EverestInterop {
    public class Hotkeys {

        public class Hotkey {
            private List<Keys> keys;
            private List<Buttons> buttons;
            private bool keyCombo;
            public bool pressed;
            public bool wasPressed;
            public Hotkey(List<Keys> keys, List<Buttons> buttons, bool keyCombo) {
                this.keys = keys;
                this.buttons = buttons;
                this.keyCombo = keyCombo;
            }

            public void Update() {
                wasPressed = pressed;
                pressed = IsKeyDown(keys, keyCombo) || IsButtonDown(buttons, keyCombo);
            } 
        }

        public static Hotkeys instance;
        public static CelesteTASModuleSettings Settings => CelesteTASModule.Settings;

        private static KeyboardState kbState;
        private static GamePadState padState;

        public static Hotkey hotkeyHitboxes;
        public static Hotkey hotkeyGraphics;
        public static Hotkey hotkeyCamera;
        public static Hotkey hotkeyStart;
        public static Hotkey hotkeyFastForward;
        public static Hotkey hotkeyFrameAdvance;
        public static Hotkey hotkeyPause;

        public static Hotkey[] hotkeys;


        public void Load() {
            Everest.Events.Input.OnInitialize += OnInputInitialize;
        }

        public void Unload() {
            Everest.Events.Input.OnInitialize -= OnInputInitialize;
        }

        public void OnInputInitialize() {
            if (Settings.KeyStart.Count == 0) {
                Settings.KeyStart = new List<Keys> { Keys.RightControl, Keys.OemOpenBrackets };
                Settings.KeyFastForward = new List<Keys> { Keys.RightControl, Keys.RightShift };
                Settings.KeyFrameAdvance = new List<Keys> { Keys.OemOpenBrackets };
                Settings.KeyPause = new List<Keys> { Keys.OemCloseBrackets };
            }


            hotkeyHitboxes = new Hotkey(Settings.KeyHitboxes, Settings.ButtonHitboxes, false);
            hotkeyGraphics = new Hotkey(Settings.KeyGraphics, Settings.ButtonGraphics, false);
            hotkeyCamera = new Hotkey(Settings.KeyCamera, Settings.ButtonCamera, false);
            hotkeyStart = new Hotkey(Settings.KeyStart, Settings.ButtonStart, true);
            hotkeyFastForward = new Hotkey(Settings.KeyFastForward, Settings.ButtonFastForward, true);
            hotkeyFrameAdvance = new Hotkey(Settings.KeyFrameAdvance, Settings.ButtonFrameAdvance, true);
            hotkeyPause = new Hotkey(Settings.KeyPause, Settings.ButtonPause, true);
            hotkeys = new Hotkey[7] { hotkeyHitboxes, hotkeyGraphics, hotkeyCamera, hotkeyStart, hotkeyFastForward, hotkeyFrameAdvance, hotkeyPause };
        }

        public static bool IsKeyDown(List<Keys> keys, bool keyCombo = true) {
            if (keys == null || keys.Count == 0)
                return false;
            if (keyCombo) {
                foreach (Keys key in keys) {
                    if (!kbState.IsKeyDown(key))
                        return false;
                }
                return true;
            }
            else {
                foreach (Keys key in keys) {
                    if (kbState.IsKeyDown(key))
                        return true;
                }
                return false;
            }
        }
        public static bool IsButtonDown(List<Buttons> buttons, bool keyCombo = true) {
            if (buttons == null || buttons.Count == 0)
                return false;
            if (keyCombo) {
                foreach (Buttons button in buttons) {
                    if (!padState.IsButtonDown(button))
                        return false;
                }
                return true;
            }
            else {
                foreach (Buttons button in buttons) {
                    if (padState.IsButtonDown(button))
                        return true;
                }
                return false;
            }
        }

        public static GamePadState GetGamePadState() {
            GamePadState padState = MInput.GamePads[0].CurrentState;
            for (int i = 0; i < 4; i++) {
                padState = GamePad.GetState((PlayerIndex)i);
                if (padState.IsConnected)
                    break;
            }
            return padState;
        }

        public void Update() {
            kbState = Keyboard.GetState();
            padState = GetGamePadState();

            foreach (Hotkey hotkey in hotkeys) {
                hotkey?.Update();
            }
            if (hotkeyHitboxes.pressed && !hotkeyHitboxes.wasPressed)
                Settings.ShowHitboxes = !Settings.ShowHitboxes;
            if (hotkeyGraphics.pressed && !hotkeyGraphics.wasPressed)
                Settings.SimplifiedGraphics = !Settings.SimplifiedGraphics;
            if (hotkeyCamera.pressed && !hotkeyCamera.wasPressed)
                Settings.CenterCamera = !Settings.CenterCamera;
        }
    }
}
