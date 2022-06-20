//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using EP.U3D.LIBRARY.ASSET;
using EP.U3D.LIBRARY.BASE;
using System.Collections.Generic;
using UnityEngine;

namespace EP.U3D.LIBRARY.UI
{
    public class UIManager
    {
        public static Transform UIRoot;
        public static string UIPath;
        public static Camera Camera;
        public static Canvas Canvas;
        public static List<UIWindow> CachedWindows = new List<UIWindow>();
        public static List<UIWindow> OpenedWindows = new List<UIWindow>();

        public static void Initialize(Transform root, string uiPath)
        {
            UIRoot = root;
            UIPath = uiPath;
            Camera = root.GetComponent<Camera>();
            Canvas = root.GetComponent<Canvas>();
        }

        public static UIWindow EnsureWindow(UIMeta meta, bool removeFromOpened)
        {
            UIWindow window = null;
            for (int i = 0; i < OpenedWindows.Count; i++)
            {
                UIWindow record = OpenedWindows[i];
                if (record.Meta.Name() == meta.Name())
                {
                    window = record;
                    if (removeFromOpened)
                    {
                        OpenedWindows.RemoveAt(i);
                    }
                    break;
                }
            }
            if (window == null)
            {
                for (int i = 0; i < CachedWindows.Count; i++)
                {
                    UIWindow record = OpenedWindows[i];
                    if (record.Meta.Name() == meta.Name())
                    {
                        window = record;
                        CachedWindows.RemoveAt(i);
                        break;
                    }
                }
            }
            if (window == null)
            {
                string path = UIPath + (string.IsNullOrEmpty(meta.Path()) ? meta.Name() : meta.Path());
                GameObject go = AssetManager.LoadAsset(path, typeof(GameObject)) as GameObject;
                go = UIHelper.CloneGO(go);
                Transform trans = go.transform;
                Canvas panel = trans.GetComponent<Canvas>();
                if (panel)
                {
                    if (!meta.NoRoot()) UIHelper.SetParent(panel.gameObject, UIRoot);
                    UIHelper.SetLayer(panel.gameObject, "UI");
                    panel.overrideSorting = true;
                    window = new UIWindow();
                    window.Meta = meta;
                    window.Panel = panel;
                }
                else
                {
                    UIHelper.DestroyGO(go, true);
                }
            }
            return window;
        }

        public static UIWindow FindOpenedWindow(UIMeta meta)
        {
            for (int i = 0; i < OpenedWindows.Count; i++)
            {
                UIWindow record = OpenedWindows[i];
                if (record.Meta.Name() == meta.Name())
                {
                    return record;
                }
            }
            return null;
        }

        public static UIWindow OpenWindow(UIMeta target, UIMeta below = null, UIMeta above = null)
        {
            UIWindow window = EnsureWindow(target, true);
            if (window == null)
            {
                Helper.LogError(Constants.RELEASE_MODE ? null : "OpenWindow error caused by nil window obj,please check it {0}.", target.Name());
            }
            else
            {
                UIHelper.SetActiveState(window.Panel, true);
            }
            UIWindow belowWindow = FindOpenedWindow(below);
            UIWindow aboveWindow = FindOpenedWindow(above);
            ArrangeWindow(window, belowWindow, aboveWindow);
            return window;
        }

        public static void ArrangeWindow(UIWindow window, UIWindow below, UIWindow above)
        {
            if (window != null)
            {
                bool inserted = false;
                if (below != null)
                {
                    for (int i = 0; i < OpenedWindows.Count; i++)
                    {
                        UIWindow record = OpenedWindows[i];
                        if (record == below)
                        {
                            OpenedWindows.Insert(i, window);
                            inserted = true;
                        }
                    }
                }
                else if (above != null)
                {
                    for (int i = 0; i < OpenedWindows.Count; i++)
                    {
                        UIWindow record = OpenedWindows[i];
                        if (record == above)
                        {
                            OpenedWindows.Insert(i + 1, window);
                            inserted = true;
                        }
                    }
                }
                if (!inserted)
                {
                    OpenedWindows.Add(window);
                }
            }

            int index = OpenedWindows.Count - 1;
            bool lastWindowFocused = false;
            while (index >= 0)
            {
                UIWindow record = OpenedWindows[index];
                record.Panel.sortingOrder = record.Meta.FixedRQ();
                if (lastWindowFocused || record.Meta.Focus())
                {
                    UIHelper.SetComponentEnabled(record.Panel, typeof(UnityEngine.UI.GraphicRaycaster), false);
                }
                else
                {
                    UIHelper.SetComponentEnabled(record.Panel, typeof(UnityEngine.UI.GraphicRaycaster), true);
                    lastWindowFocused = true;
                }
                index--;
            }
            for (int i = 0; i < OpenedWindows.Count; i++)
            {
                UIWindow record = OpenedWindows[i];
                if (record.Focus)
                {
                    FocusWindow(record.Meta, true);
                }
            }
        }

        public static void FocusWindow(UIMeta meta, bool always)
        {
            if (meta != null)
            {
                for (int i = 0; i < OpenedWindows.Count; i++)
                {
                    UIWindow record = OpenedWindows[i];
                    if (record.Meta.Name() == meta.Name())
                    {
                        record.Focus = always;
                        UIHelper.SetComponentEnabled(record.Panel, typeof(UnityEngine.UI.GraphicRaycaster), true);
                        break;
                    }
                }
            }
        }

        public static void ResumeWindow()
        {
            UIWindow window = null;
            for (int i = OpenedWindows.Count - 1; i >= 0; i++)
            {
                UIWindow record = OpenedWindows[i];
                if (window == null && record.Meta.Focus())
                {
                    window = record;
                    break;
                }
            }
            if (window != null)
            {
                ArrangeWindow(null, null, null);
            }
        }

        public static void CloseWindow(UIMeta meta, bool resume = true)
        {
            if (meta != null)
            {
                for (int i = OpenedWindows.Count - 1; i >= 0; i--)
                {
                    UIWindow record = OpenedWindows[i];
                    if (record.Meta.Name() == meta.Name())
                    {
                        OpenedWindows.RemoveAt(i);
                        UIHelper.SetActiveState(record.Panel, false);
                        if (!record.Meta.Cached())
                        {
                            UIHelper.DestroyGO(record.Panel, true);
                        }
                        else
                        {
                            CachedWindows.Add(record);
                        }
                        break;
                    }
                }
                if (resume) ResumeWindow();
            }
        }

        public static void CloseAllWindows()
        {
            while (OpenedWindows.Count > 0)
            {
                UIWindow record = OpenedWindows[OpenedWindows.Count - 1];
                CloseWindow(record.Meta, false);
            }
        }

        public static void CloseAllWindowsExcept(params UIMeta[] filter)
        {
            int index = 0;
            while (index < OpenedWindows.Count)
            {
                UIWindow record = OpenedWindows[index];
                bool needsClose = true;
                for (int i = 0; i < filter.Length; i++)
                {
                    if (record.Meta.Name() == filter[i].Name())
                    {
                        needsClose = false;
                        break;
                    }
                }
                if (needsClose)
                {
                    CloseWindow(record.Meta, false);
                }
                else
                {
                    index++;
                }
            }
            ResumeWindow();
        }

        public static UIWindow IsWindowOpened(UIMeta meta)
        {
            for (int i = 0; i < OpenedWindows.Count; i++)
            {
                UIWindow record = OpenedWindows[i];
                if (record.Meta.Name() == meta.Name())
                {
                    return record;
                }
            }
            return null;
        }
    }
}