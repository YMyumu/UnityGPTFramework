using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventModule;
using UIModule;

namespace SceneLoaderModule
{
    public class SceneLoader : MonoSingleton<SceneLoader>
    {
        private bool sceneReadyToActivate = false;

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName">要加载的场景名称</param>
        public void LoadSceneAsync(string sceneName)
        {
            sceneReadyToActivate = false;
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        /// <summary>
        /// 场景加载的协程
        /// </summary>
        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            // 场景加载前事件
            EventManager.Instance.Dispatch(EventDefine.BEFORE_SCENE_LOAD);

            UIManager.Instance.OpenPanel(UIDefine.LoadingUIPanel);


            // 开始异步加载场景
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;  // 禁止场景立即激活

            float fakeProgress = 0.0f;

            // 在加载过程中，持续更新进度条
            while (!asyncOperation.isDone)
            {
                // 场景加载进度（0.0 到 0.9 表示加载中，0.9 完成后等待激活）
                float realProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                fakeProgress = Mathf.MoveTowards(fakeProgress, realProgress, Time.deltaTime * 0.5f);

                EventManager.Instance.Dispatch<float>(EventDefine.ON_SCENE_LOAD_PROGRESS_FOR_UIPANEL, fakeProgress);

                // 如果加载进度到达100%，等待界面准备好再激活场景
                if (fakeProgress >= 1.0f && sceneReadyToActivate)
                {
                    asyncOperation.allowSceneActivation = true;
                    // 场景加载完成后事件
                    EventManager.Instance.Dispatch(EventDefine.AFTER_SCENE_LOAD);
                }

                yield return null;
            }
        }

        // 场景准备激活
        public void OnSceneUIReady()
        {
            sceneReadyToActivate = true;
        }
    }

}
