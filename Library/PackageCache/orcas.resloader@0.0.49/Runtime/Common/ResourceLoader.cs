using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Orcas.Resources
{
    public static class ResourceLoader 
    {
        /// <summary>
        /// 实例化物体
        /// </summary>
        /// <param name="go"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static GameObject InstantiateGameObject(GameObject go, Transform trans)
        {
            return GameObject.Instantiate(go, trans);
        }
        
        /// <summary>
        /// 找到GameObject
        /// </summary>
        public static GameObject GetGameObjectInParentByPath(string path, GameObject parent)
        {
            if (parent == null) return null;
            Transform trans = parent.transform.Find(path);
            GameObject obj = trans == null ? null : trans.gameObject;
            if (obj == null)
            {
                string[] pathStrs = path.Split('/');
                obj = GetGameObjectInParentByName(pathStrs[pathStrs.Length - 1], parent);
            }
            return obj;
        }

        /// <summary>
        /// 找到物体
        /// </summary>
        public static GameObject GetGameObjectInParentByName(string name, GameObject parent)
        {
            if (parent == null) return null;

            foreach (Transform child in parent.GetComponentsInChildren<Transform>())
            {
                if (child.name == name) return child.gameObject;
            }

            return null;
        }
        
        public static bool AudioClipUseResourcesLoader = false;
        public static AudioClip LoadAudioClip(string file)
        {
            if (AudioClipUseResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadOriginalAsset<AudioClip>(file);
            }
            return ResourceLoaderManager.AssetLoader.LoadOriginalAsset<AudioClip>(file);
        }



        public static bool SpriteUseResourcesLoader = false;
        public static Sprite GetUISprite(string atlasName, string spriteName)
        {
            UnityEngine.Object[] spriteList;
            if (SpriteUseResourcesLoader)
            {
                spriteList = ResourceLoaderManager.ResourceLoader.LoadAllOriginalAsset<UnityEngine.Object>(atlasName);
            }
            else
            {
                spriteList = ResourceLoaderManager.AssetLoader.LoadAllOriginalAsset<UnityEngine.Object>(atlasName);
            }
            foreach (var sprite in spriteList)
            {
                if (sprite.name == spriteName)
                {
                    return sprite as Sprite;
                }
            }
            return null;
        }

        public static bool TextureUseResourcesLoader = false;
        public static Texture2D GetUITexture(string texPath)
        {
            if (TextureUseResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadOriginalAsset<Texture2D>(texPath);
            }
            return ResourceLoaderManager.AssetLoader.LoadOriginalAsset<Texture2D>(texPath);
        }
        
        
        public static bool PrefabUseResourcesLoader = false;
        public static GameObject LoadPrefab(string path)
        {
            if (PrefabUseResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadOriginalAsset<GameObject>(path);
            }
            return ResourceLoaderManager.AssetLoader.LoadOriginalAsset<GameObject>(path);
        }

        public static bool ObjectUseResourcesLoader = false;
        public static UnityEngine.Object LoadObjectPrefab(string path)
        {
            if (ObjectUseResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadOriginalAsset<UnityEngine.Object>(path);
            }
            return ResourceLoaderManager.AssetLoader.LoadOriginalAsset<UnityEngine.Object>(path);
        }


        public static bool GameObjectUseResourcesLoader = false;
        public static GameObject LoadGameObject(string path)
        {
            if (GameObjectUseResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadInstantiatedObject<GameObject>(path);
            }
            return ResourceLoaderManager.AssetLoader.LoadInstantiatedObject<GameObject>(path);
        }
        

        public static bool AnimationClipUseResourcesLoader = false;

        public static AnimationClip LoadAnimationClip(string path)
        {
            if (AnimationClipUseResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadOriginalAsset<AnimationClip>(path);
            }

            return ResourceLoaderManager.AssetLoader.LoadOriginalAsset<AnimationClip>(path);
        }


        public static bool MaterialUseResourcesLoader = false;

        public static Material LoadMaterial(string path)
        {
            if (MaterialUseResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadOriginalAsset<Material>(path);
            }

            return ResourceLoaderManager.AssetLoader.LoadOriginalAsset<Material>(path);
        }

        
        public static bool VideoClipUseResourcesLoader = false;
        public static VideoClip LoadVideoClip(string path)
        {
            if (VideoClipUseResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadOriginalAsset<VideoClip>(path);
            }

            return ResourceLoaderManager.AssetLoader.LoadOriginalAsset<VideoClip>(path);
        }

        public static bool TextAssetUseResourcesLoader = false;

        public static TextAsset LoadTextAsset(string path)
        {
            if (TextAssetUseResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadOriginalAsset<TextAsset>(path);
            }

            return ResourceLoaderManager.AssetLoader.LoadOriginalAsset<TextAsset>(path);
        }


        public static bool AnimatorUseResourcesLoader = false;

        public static RuntimeAnimatorController LoadRuntimeAnimatorController(string path)
        {
            if (AnimatorUseResourcesLoader)
            {
                return ResourceLoaderManager.ResourceLoader.LoadOriginalAsset<RuntimeAnimatorController>(path);
            }
            
            return ResourceLoaderManager.AssetLoader.LoadOriginalAsset<RuntimeAnimatorController>(path);
        }
    }
}

