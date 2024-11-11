/// <summary>
/// ResourceManager 类用于管理游戏中的资源加载和缓存。
/// 它通过使用 Unity 的 Resources 系统加载资源，并支持同步和异步加载。
/// 同时，该类还提供了资源卸载和缓存清理功能，以优化内存使用。
/// </summary>
/// <remarks>
/// 主要功能：
/// 1. 加载资源：提供同步和异步的资源加载方法，支持通过相对路径加载资源。
/// 2. 缓存机制：资源加载后会被缓存，以避免重复加载相同资源，提高性能。
/// 3. 资源卸载：可以根据路径卸载特定资源，或清空所有缓存资源。
/// 4. 路径处理：支持自动去除文件路径中的后缀名，以符合 Resources.Load 的要求。
/// </remarks>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResourceModule
{
    public class ResourceManager : MonoSingleton<ResourceManager>
    {
        // 资源缓存字典，避免重复加载
        private Dictionary<string, Object> resourceCache = new Dictionary<string, Object>();

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">要加载的资源类型</typeparam>
        /// <param name="path">资源的相对路径，不包含文件后缀名</param>
        /// <returns>加载的资源对象，如果加载失败返回 null</returns>
        public T LoadResource<T>(string path) where T : Object
        {
            // 去除路径中的文件后缀名，确保路径符合 Resources.Load 的要求
            string pathWithoutExtension = System.IO.Path.ChangeExtension(path, null);

            // 检查资源是否已被缓存
            if (resourceCache.ContainsKey(pathWithoutExtension))
            {
                return resourceCache[pathWithoutExtension] as T;
            }

            // 加载资源
            T resource = Resources.Load<T>(pathWithoutExtension);
            if (resource != null)
            {
                // 如果加载成功，将资源缓存
                resourceCache[pathWithoutExtension] = resource;
            }
            else
            {
                // 如果加载失败，记录错误日志
                LogManager.LogError($"资源加载失败: {pathWithoutExtension}");
            }

            return resource;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">要加载的资源类型</typeparam>
        /// <param name="path">资源的相对路径，路径有文件后缀名输入会自动去除</param>
        /// <param name="callback">资源加载完成后的回调函数</param>
        /// <returns>IEnumerator，用于协程控制</returns>
        public IEnumerator LoadResourceAsync<T>(string path, System.Action<T> callback) where T : Object
        {
            // 去除路径中的文件后缀名，确保路径符合 Resources.LoadAsync 的要求
            string pathWithoutExtension = System.IO.Path.ChangeExtension(path, null);

            // 检查资源是否已被缓存
            if (resourceCache.ContainsKey(pathWithoutExtension))
            {
                callback?.Invoke(resourceCache[pathWithoutExtension] as T);
                yield break;
            }

            // 异步加载资源
            ResourceRequest request = Resources.LoadAsync<T>(pathWithoutExtension);
            yield return request;

            if (request.asset != null)
            {
                // 如果加载成功，将资源缓存，并调用回调函数
                resourceCache[pathWithoutExtension] = request.asset;
                callback?.Invoke(request.asset as T);
            }
            else
            {
                // 如果加载失败，记录错误日志，并调用回调函数传递 null
                LogManager.LogError($"资源异步加载失败: {pathWithoutExtension}");
                callback?.Invoke(null);
            }
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="path">要卸载的资源的相对路径，路径有文件后缀名输入会自动去除</param>
        public void UnloadResource(string path)
        {
            // 去除路径中的文件后缀名
            string pathWithoutExtension = System.IO.Path.ChangeExtension(path, null);

            // 检查资源是否在缓存中
            if (resourceCache.ContainsKey(pathWithoutExtension))
            {
                // 卸载资源并从缓存中移除
                Resources.UnloadAsset(resourceCache[pathWithoutExtension]);
                resourceCache.Remove(pathWithoutExtension);
            }
        }

        /// <summary>
        /// 清空所有缓存资源
        /// </summary>
        public void ClearCache()
        {
            // 清空缓存字典
            resourceCache.Clear();

            // 卸载未使用的资源
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 根据类型和文件名返回可以加载的文件路径
        /// </summary>
        /// <typeparam name="T">资源的类型，例如 Texture2D, GameObject 等</typeparam>
        /// <param name="fileName">资源文件名，不包含后缀名</param>
        /// <returns>构造的相对路径</returns>
        public string GetResourcePathByType<T>(string fileName) where T : Object
        {
            string directory = string.Empty;

            // 根据资源类型选择合适的路径前缀
            if (typeof(T) == typeof(Texture2D))
            {
                directory = "Textures/";
            }
            else if (typeof(T) == typeof(AudioClip))
            {
                directory = "Audio/Audios";
            }
            else if (typeof(T) == typeof(Material))
            {
                directory = "Materials/";
            }
            else if (typeof(T) == typeof(GameObject))
            {
                directory = "Prefabs/";
            }
            else
            {
                LogManager.LogWarning($"未知的资源类型: {typeof(T)}，无法确定路径前缀。");
                return null;
            }

            // 返回完整的资源路径，不含后缀名
            return directory + fileName;
        }
    }

}
