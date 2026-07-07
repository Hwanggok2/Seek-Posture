using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    // Inspector에서 연결할 UI 요소
    public Slider progressBar;
    public GameObject tapToStartPanel; // 로딩 완료 후 활성화할 UI 패널

    private AsyncOperation asyncOperation; // 비동기 로드를 위한 핵심 객체

    void Start()
    {
        // 로딩 시작
        tapToStartPanel.SetActive(false); // 처음엔 비활성화
        StartCoroutine(LoadSceneAsync("Stage_1"));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        // 1. 다음 씬을 백그라운드에서 로드 시작
        asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // 씬 로드가 완료되자마자 활성화되는 것을 방지 (로딩 화면 유지)
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // 2. 로딩 진행률 업데이트 (0.9f는 로드 완료 직전까지)
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            progressBar.value = progress;

            if (asyncOperation.progress >= 0.9f)
            {
                // 3. 로드가 90% 완료되면 (실제 로드는 여기서 완료됨)
                progressBar.value = 1f; // 로딩 바를 100%로 채움
                tapToStartPanel.SetActive(true); // TAP TO START 버튼 활성화

                // 사용자의 터치/클릭을 기다림 (UX 개선)
                if (Input.GetMouseButtonDown(0)) // 모바일 터치 입력 감지
                {
                    // 4. 사용자가 터치하면 씬 전환을 허용
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null; // 다음 프레임까지 대기
        }
    }
}