using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class Utils
{
    public static async Task<T> LoadScene<T>(string name, CancellationToken ct) where T : Object {
        SceneManager.LoadScene(name, LoadSceneMode.Additive);
        
        // Synchronous scene loading takes one full frame to finish. One yield is not enough because it only yields until end of current frame.
        await Task.Yield();
        await Task.Yield();
        ct.ThrowIfCancellationRequested();

        return GameObject.FindObjectOfType<T>();
    }

    public static Task PressButton(Button button, CancellationToken ct) {
        TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
        CancellationTokenRegistration ctr = ct.Register(OnCancel);
        button.onClick.AddListener(OnClick);

        return tcs.Task;

        void OnClick() {
            button.onClick.RemoveListener(OnClick);
            ctr.Dispose();
            tcs.SetResult(1);
        }

        void OnCancel() {
            button.onClick.RemoveListener(OnClick);
            ctr.Dispose();
            tcs.SetException(new System.OperationCanceledException());
        }
    }

    public static async Task PressBackButton(CancellationToken ct) {
        while (true) {
            await Task.Yield();
            ct.ThrowIfCancellationRequested();

            if (Input.GetKeyDown(KeyCode.Escape))
                return;
        }
    }
}
