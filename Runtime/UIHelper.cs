//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using EP.U3D.LIBRARY.BASE;

namespace EP.U3D.LIBRARY.UI
{
    public class UIHelper
    {
        protected static readonly object[] NIL_OBJECT_ARR = new object[0];

        public static Transform GetTransform(Object rootObj)
        {
            if (rootObj == null)
            {
                return null;
            }
            if (rootObj is Transform)
            {
                return rootObj as Transform;
            }
            else if (rootObj is GameObject)
            {
                GameObject go = rootObj as GameObject;
                if (go)
                {
                    return go.transform;
                }
            }
            else if (rootObj is Behaviour)
            {
                Behaviour behaviour = rootObj as Behaviour;
                if (behaviour)
                {
                    return behaviour.transform;
                }
            }
            return null;
        }

        public static Transform GetTransform(Object parentObj, string path)
        {
            Transform parent = GetTransform(parentObj);
            if (string.IsNullOrEmpty(path))
            {
                return parent;
            }
            else if (parent)
            {
                return parent.Find(path);
            }
            else
            {
                return null;
            }
        }

        public static void SetButtonEvent(Object rootObj, System.Action<GameObject> func)
        {
            SetButtonEvent(rootObj, func);
        }

        public static void SetButtonEvent(Object parentObj, string path, System.Action<GameObject> func)
        {
            Button listener = GetComponent(parentObj, path, typeof(Button)) as Button;
            if (listener)
            {
                listener.onClick.AddListener(() => { if (func != null) { func(listener.gameObject); } });
            }
        }

        public static void SetEventEnabled(Object rootObj, bool status)
        {
            Button btn = GetComponent(rootObj, typeof(Button)) as Button;
            if (btn)
            {
                btn.interactable = status;
            }
        }

        public static void SetEventEnabled(Object parentObj, string path, bool status)
        {
            Button btn = GetComponent(parentObj, path, typeof(Button)) as Button;
            if (btn)
            {
                btn.interactable = status;
            }
        }

        public static Text SetLabelText(Object rootObj, object content)
        {
            if (content == null)
            {
                return null;
            }
            Text label = GetComponent(rootObj, typeof(Text)) as Text;
            if (label)
            {
                label.text = content.ToString();
                return label;
            }
            else
            {
                return null;
            }
        }

        public static Text SetLabelText(Object parentObj, string path, object content)
        {
            if (content == null)
            {
                return null;
            }
            Text label = GetComponent(parentObj, path, typeof(Text)) as Text;
            if (label)
            {
                label.text = content.ToString();
                return label;
            }
            else
            {
                return null;
            }
        }

        public static void SetActiveState(Object rootObj, bool state)
        {
            Transform root = GetTransform(rootObj);
            if (root && root.gameObject)
            {
                root.gameObject.SetActive(state);
            }
        }

        public static void SetActiveState(Object parentObj, string path, bool state)
        {
            Transform parent = GetTransform(parentObj);
            if (parent != null)
            {
                SetActiveState(parent.Find(path), state);
            }
        }

        public static Image SetSpriteName(Object rootObj, string name)
        {
            Image image = GetComponent(rootObj, typeof(Image)) as Image;
            if (image)
            {
                if (image.atlas)
                {
                    Sprite s = image.atlas.GetSprite(name);
                    image.sprite = s;
                }
                return image;
            }
            else
            {
                return null;
            }
        }

        public static Image SetSpriteName(Object parentObj, string path, string name)
        {
            Image image = GetComponent(parentObj, path, typeof(Image)) as Image;
            if (image)
            {
                if (image.atlas)
                {
                    Sprite s = image.atlas.GetSprite(name);
                    image.sprite = s;
                }
                return image;
            }
            else
            {
                return null;
            }
        }

        public static Image SetSpriteName(Object parentObj, string path, string name, Atlas atlas)
        {
            Image image = GetComponent(parentObj, path, typeof(Image)) as Image;
            if (image)
            {
                if (atlas)
                {
                    image.atlas = atlas;
                }
                if (image.atlas)
                {
                    Sprite s = image.atlas.GetSprite(name);
                    image.sprite = s;
                }
                return image;
            }
            else
            {
                return null;
            }
        }

        public static Image SetSpriteGray(Object rootObj, bool status)
        {
            // TODO
            //UISprite sprite = GetComponent(rootObj, typeof(UISprite)) as UISprite;
            //if (sprite)
            //{
            //    sprite.SetGray(status);
            //}
            //return sprite;
            return null;
        }

        public static Image SetSpriteGray(Object parentObj, string path, bool status)
        {
            // TODO
            //UISprite sprite = GetComponent(parentObj, path, typeof(UISprite)) as UISprite;
            //if (sprite)
            //{
            //    sprite.SetGray(status);
            //}
            //return sprite;
            return null;
        }

        public static Image SetSpriteAlpha(Object rootObj, int alpha)
        {
            Image image = GetComponent(rootObj, typeof(Image)) as Image;
            if (image)
            {
                Color nc = new Color(image.color.r, image.color.g, image.color.b, (float)alpha / 255);
                image.color = nc;
            }
            return image;
        }

        public static Image SetSpriteAlpha(Object parentObj, string path, int alpha)
        {
            Image image = GetComponent(parentObj, path, typeof(Image)) as Image;
            if (image)
            {
                Color nc = new Color(image.color.r, image.color.g, image.color.b, (float)alpha / 255);
                image.color = nc;
            }
            return image;
        }

        public static RawImage SetUITexture(Object rootObj, string url)
        {
            return SetUITexture(rootObj, url, true);
        }

        public static RawImage SetUITexture(Object rootObj, string url, bool useCached)
        {
            RawImage texture = GetComponent(rootObj, typeof(RawImage)) as RawImage;
            if (texture)
            {
                texture.texture = null;
                bool done = false;
                if (useCached)
                {
                    Texture2D tex2d;
                    cachedTextures.TryGetValue(url, out tex2d);
                    if (tex2d)
                    {
                        texture.texture = tex2d;
                        done = true;
                    }
                }
                if (done == false)
                {
                    Loom.StartCR(ProcessWWWTexture(url, texture, null));
                }
            }
            return texture;
        }

        public static RawImage SetUITexture(Object parentObj, string path, string url)
        {
            return SetUITexture(parentObj, path, url, true);
        }

        public static RawImage SetUITexture(Object parentObj, string path, string url, bool useCached)
        {
            RawImage texture = GetComponent(parentObj, path, typeof(RawImage)) as RawImage;
            if (texture)
            {
                texture.texture = null;
                bool done = false;
                if (useCached)
                {
                    Texture2D tex2d;
                    cachedTextures.TryGetValue(url, out tex2d);
                    if (tex2d)
                    {
                        texture.texture = tex2d;
                        done = true;
                    }
                }
                if (done == false)
                {
                    Loom.StartCR(ProcessWWWTexture(url, texture, null));
                }
            }
            return texture;
        }

        public static Texture2D GetTexture(string url)
        {
            Texture2D tex2d = null;
            if (string.IsNullOrEmpty(url) == false)
            {
                cachedTextures.TryGetValue(url, out tex2d);
            }
            return tex2d;
        }

        public static void WWWTexture(string url, bool useCached, System.Action callback)
        {
            bool done = false;
            if (useCached)
            {
                Texture2D tex2d = null;
                cachedTextures.TryGetValue(url, out tex2d);
                if (tex2d)
                {
                    done = true;
                    if (callback != null)
                    {
                        callback();
                    }
                }
            }
            if (done == false)
            {
                Loom.StartCR(ProcessWWWTexture(url, null, callback));
            }
        }

        private static Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

        private static IEnumerator ProcessWWWTexture(string url, RawImage texture, System.Action callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (callback != null)
                {
                    callback();
                }
                yield break;
            }
            WWW www = new WWW(url);
            yield return www;
            if (string.IsNullOrEmpty(www.error) == false)
            {
                Helper.LogError(Constants.RELEASE_MODE ? null : "error info: {0}, url is {1}.", www.error, url);
                if (callback != null)
                {
                    callback();
                }
                yield break;
            }
            if (www.isDone == false)
            {
                if (callback != null)
                {
                    callback();
                }
                yield break;
            }
            Texture2D tex2d = www.texture;
            if (tex2d == null)
            {
                if (callback != null)
                {
                    callback();
                }
                yield break;
            }
            if (cachedTextures.ContainsKey(url))
            {
                cachedTextures.Remove(url);
            }
            cachedTextures.Add(url, tex2d);
            if (texture)
            {
                texture.texture = tex2d;
            }
            if (callback != null)
            {
                callback();
            }
        }

        public static object GetComponentInParent(Object rootObj, System.Type type)
        {
            return GetComponentInParent(rootObj, null, type);
        }

        public static object GetComponentInParent(Object parentObj, string path, System.Type type)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return null;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                return root.gameObject.GetComponentInParent(type);
            }
            return null;
        }

        public static object GetComponent(Object rootObj, System.Type type)
        {
            return GetComponent(rootObj, null, type);
        }

        public static object GetComponent(Object parentObj, string path, System.Type type)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return null;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                return root.gameObject.GetComponent(type);
            }
            return null;
        }

        public static object GetComponentInChildren(Object rootObj, System.Type type, bool includeInactive = false)
        {
            return GetComponentInChildren(rootObj, null, type, includeInactive);
        }

        public static object GetComponentInChildren(Object parentObj, string path, System.Type type, bool includeInactive = false)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return null;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                return root.gameObject.GetComponentInChildren(type, includeInactive);
            }
            return null;
        }

        public static object[] GetComponentsInParent(Object rootObj, System.Type type, bool includeInactive = false)
        {
            return GetComponentsInParent(rootObj, null, type, includeInactive);
        }

        public static object[] GetComponentsInParent(Object parentObj, string path, System.Type type, bool includeInactive = false)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return NIL_OBJECT_ARR;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                return root.gameObject.GetComponentsInParent(type, includeInactive);
            }
            else
            {
                return NIL_OBJECT_ARR;
            }
        }

        public static object[] GetComponents(Object rootObj, System.Type type)
        {
            return GetComponents(rootObj, null, type);
        }

        public static object[] GetComponents(Object parentObj, string path, System.Type type)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return NIL_OBJECT_ARR;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                return root.gameObject.GetComponents(type);
            }
            else
            {
                return NIL_OBJECT_ARR;
            }
        }

        public static object[] GetComponentsInChildren(Object rootObj, System.Type type, bool includeInactive = false)
        {
            return GetComponentsInChildren(rootObj, null, type, includeInactive);
        }

        public static object[] GetComponentsInChildren(Object parentObj, string path, System.Type type, bool includeInactive = false)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return NIL_OBJECT_ARR;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                return root.gameObject.GetComponentsInChildren(type, includeInactive);
            }
            else
            {
                return NIL_OBJECT_ARR;
            }
        }

        public static object AddComponent(Object rootObj, System.Type type)
        {
            return AddComponent(rootObj, null, type);
        }

        public static object AddComponent(Object parentObj, string path, System.Type type)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return null;
            }
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                return root.gameObject.AddComponent(type);
            }
            else
            {
                return null;
            }
        }

        public static void RemoveComponent(Object rootObj, System.Type type)
        {
            RemoveComponent(rootObj, null, type, false);
        }

        public static void RemoveComponent(Object rootObj, System.Type type, bool immediate)
        {
            RemoveComponent(rootObj, null, type, immediate);
        }

        public static void RemoveComponent(Object parentObj, string path, System.Type type)
        {
            RemoveComponent(parentObj, path, type, false);
        }

        public static void RemoveComponent(Object parentObj, string path, System.Type type, bool immediate)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return;
            }
            Object obj = GetComponent(parentObj, path, type) as Object;
            if (obj)
            {
                if (immediate)
                {
                    Object.DestroyImmediate(obj);
                }
                else
                {
                    Object.Destroy(obj);
                }
            }
        }

        public static object SetComponentEnabled(Object rootObj, System.Type type, bool enabled)
        {
            return SetComponentEnabled(rootObj, null, type, enabled);
        }

        public static object SetComponentEnabled(Object parentObj, string path, System.Type type, bool enabled)
        {
            if (type == null)
            {
                Helper.LogError("missing type argument");
                return null;
            }
            Behaviour behaviour = GetComponent(parentObj, path, type) as Behaviour;
            if (behaviour)
            {
                behaviour.enabled = enabled;
            }
            return behaviour;
        }

        public static void SetPosition(Object rootObj, Vector3 position)
        {
            Transform root = GetTransform(rootObj);
            if (root && root.gameObject)
            {
                root.position = position;
            }
        }

        public static void SetPosition(Object parentObj, string path, Vector3 position)
        {
            Transform parent = GetTransform(parentObj);
            if (parent && parent.gameObject)
            {
                Transform root = parent.Find(path);
                if (root && root.gameObject)
                {
                    root.position = position;
                }
            }
        }

        public static void SetLocalPosition(Object rootObj, Vector3 position)
        {
            Transform root = GetTransform(rootObj);
            if (root && root.gameObject)
            {
                root.localPosition = position;
            }
        }

        public static void SetLocalPosition(Object parentObj, string path, Vector3 position)
        {
            Transform parent = GetTransform(parentObj);
            if (parent && parent.gameObject)
            {
                Transform root = parent.Find(path);
                if (root && root.gameObject)
                {
                    root.localPosition = position;
                }
            }
        }

        public static void SetRotation(Object rootObj, Vector3 eulerAngles)
        {
            Transform root = GetTransform(rootObj);
            if (root && root.gameObject)
            {
                root.rotation = Quaternion.Euler(eulerAngles);
            }
        }

        public static void SetRotation(Object parentObj, string path, Vector3 eulerAngles)
        {
            Transform parent = GetTransform(parentObj);
            if (parent && parent.gameObject)
            {
                Transform root = parent.Find(path);
                if (root && root.gameObject)
                {
                    root.rotation = Quaternion.Euler(eulerAngles);
                }
            }
        }

        public static void SetLocalRotation(Object rootObj, Vector3 eulerAngles)
        {
            Transform root = GetTransform(rootObj);
            if (root && root.gameObject)
            {
                root.localRotation = Quaternion.Euler(eulerAngles);
            }
        }

        public static void SetLocalRotation(Object parentObj, string path, Vector3 eulerAngles)
        {
            Transform parent = GetTransform(parentObj);
            if (parent && parent.gameObject)
            {
                Transform root = parent.Find(path);
                if (root && root.gameObject)
                {
                    root.localRotation = Quaternion.Euler(eulerAngles);
                }
            }
        }

        public static void SetLocalScale(Object rootObj, Vector3 scale)
        {
            Transform root = GetTransform(rootObj);
            if (root && root.gameObject)
            {
                root.localScale = scale;
            }
        }

        public static void SetLocalScale(Object parentObj, string path, Vector3 scale)
        {
            Transform parent = GetTransform(parentObj);
            if (parent && parent.gameObject)
            {
                Transform root = parent.Find(path);
                if (root && root.gameObject)
                {
                    root.localScale = scale;
                }
            }
        }

        public static Transform SetParent(Object childObj, Object parentObj)
        {
            return SetParent(childObj, parentObj, false);
        }

        public static Transform SetParent(Object childObj, Object parentObj, bool worldPositionStavs)
        {
            Transform child = GetTransform(childObj);
            Transform parent = GetTransform(parentObj);
            if (child && child.gameObject && parent && parent.gameObject)
            {
                child.SetParent(parent, worldPositionStavs);
            }
            return parent;
        }

        public static Transform SetParent(Object childObj, Object rootObj, string parentPath)
        {
            return SetParent(childObj, rootObj, parentPath);
        }

        public static Transform SetParent(Object childObj, Object rootObj, string parentPath, bool worldPositionStavs = false)
        {
            Transform child = GetTransform(childObj);
            Transform root = GetTransform(rootObj);
            Transform parent = null;
            if (root && root.gameObject)
            {
                parent = root.Find(parentPath);
            }
            if (child && child.gameObject && parent && parent.gameObject)
            {
                child.SetParent(parent, worldPositionStavs);
            }
            return parent;
        }

        public static void DestroyGO(Object rootObj)
        {
            DestroyGO(rootObj, false);
        }

        public static void DestroyGO(Object rootObj, bool immediate)
        {
            Transform root = GetTransform(rootObj);
            if (root && root.gameObject)
            {
                if (immediate)
                {
                    Object.DestroyImmediate(root.gameObject);
                }
                else
                {
                    Object.Destroy(root.gameObject);
                }
            }
        }

        public static void DestroyGO(Object parentObj, string path)
        {
            DestroyGO(parentObj, path, false);
        }

        public static void DestroyGO(Object parentObj, string path, bool immediate)
        {
            Transform root = GetTransform(parentObj, path);
            if (root && root.gameObject)
            {
                if (immediate)
                {
                    Object.DestroyImmediate(root.gameObject);
                }
                else
                {
                    Object.Destroy(root.gameObject);
                }
            }
        }

        public static GameObject CloneGO(Object rootObj)
        {
            Transform root = GetTransform(rootObj);
            if (root && root.gameObject)
            {
                string goName = root.name;
                var go = Object.Instantiate(root.gameObject);
                if (go)
                {
                    go.name = goName;
                }
                return go;
            }
            else
            {
                return null;
            }
        }

        public static void SetLayer(Object rootObj, string layerName)
        {
            Transform root = GetTransform(rootObj);
            if (root && root.gameObject)
            {
                int layer = LayerMask.NameToLayer(layerName);
                SetLayer(root.gameObject, layer);
            }
        }

        public static void SetLayer(Object parentObj, string path, string layerName)
        {
            Transform parent = GetTransform(parentObj);
            if (parent && parent.gameObject)
            {
                Transform root = parent.Find(path);
                if (root && root.gameObject)
                {
                    int layer = LayerMask.NameToLayer(layerName);
                    SetLayer(root.gameObject, layer);
                }
            }
        }

        public static void SetLayer(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                SetLayer(child.gameObject, layer);
            }
        }

        public static GameObject AddChild(GameObject parent) { return AddChild(parent, true, -1); }

        public static GameObject AddChild(GameObject parent, int layer) { return AddChild(parent, true, layer); }

        public static GameObject AddChild(GameObject parent, bool undo) { return AddChild(parent, undo, -1); }

        public static GameObject AddChild(GameObject parent, bool undo, int layer)
        {
            GameObject go = new GameObject();
#if UNITY_EDITOR
            if (undo && !Application.isPlaying)
                UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                if (layer == -1) go.layer = parent.layer;
                else if (layer > -1 && layer < 32) go.layer = layer;
            }
            return go;
        }

        public static GameObject AddChild(GameObject parent, GameObject prefab) { return AddChild(parent, prefab, -1); }

        public static GameObject AddChild(GameObject parent, GameObject prefab, int layer)
        {
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                if (layer == -1) go.layer = parent.layer;
                else if (layer > -1 && layer < 32) go.layer = layer;
            }
            return go;
        }

        public static void GOCSubItem(Transform parent, int count, bool enable, GameObject prefab)
        {
            if (parent.childCount < count)
            {
                count -= parent.childCount;
                if (prefab == null)
                {
                    prefab = parent.GetChild(0).gameObject;
                }
                for (int i = 0; i < count; i++)
                {
                    var item = AddChild(parent.gameObject, prefab, prefab.layer);
                    item.SetActive(enable);
                }
            }
        }

        public static void HideAllSubItem(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive(false);
            }
        }

        public static void ShowAllSubItem(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive(true);
            }
        }

        public static void RefreshObSort(Object obj, string path)
        {
            ILayoutController obSort = GetComponent(obj, path, typeof(ContentSizeFitter)) as ContentSizeFitter;
            if (obSort == null)
            {
                obSort = GetComponent(obj, path, typeof(HorizontalLayoutGroup)) as HorizontalLayoutGroup;
            }
            if (obSort != null)
            {
                obSort.SetLayoutHorizontal();
                obSort.SetLayoutVertical();
            }
        }
    }
}
