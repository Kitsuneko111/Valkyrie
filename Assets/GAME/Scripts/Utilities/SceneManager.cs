// https://bergstrand-niklas.medium.com/simple-loading-screen-in-unity-99a9bcbfb6c4
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Project.Utilities{
    
    public class SceneManager : MonoSingleton<SceneManager>
    {
        Canvas loadingCanvas;
        Image loadingImage;
        [SerializeField]
        GameObject loadingPrefab;
        [SerializeField]
        GameObject loadingScreen;
        public void Quit()
        {
            Application.Quit();
        }

        public void NextScene()
        {
            StartCoroutine(LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1));
        }

        public void Restart()
        {
            StartCoroutine(LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex));
        }

        public void Load(int index)
        {
            StartCoroutine(LoadScene(index));
        }

        private IEnumerator LoadScene(int index)
        {
            int currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            yield return null;
            loadingScreen.SetActive(true);
            loadingCanvas = loadingScreen.GetComponentInChildren<Canvas>();
            loadingImage = GameObject.FindGameObjectWithTag("LoadingImage").GetComponent<Image>();

            AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index);

            while (!load.isDone)
            {
                if (loadingImage)
                {
                    loadingImage.fillAmount = (int)(Mathf.Clamp01(load.progress)*20)/20f;
                }
                yield return null;
            }
            Destroy(loadingScreen);
            Destroy(this);
        }

        private void Start()
        {
            loadingPrefab = Resources.Load<GameObject>("Prefabs/Loading/LoadingScreen");
            loadingScreen = Instantiate(loadingPrefab);
            loadingScreen.SetActive(false);
        }
    }
    
}
