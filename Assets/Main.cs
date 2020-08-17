using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Main : MonoBehaviour
{
    async void Start()
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += state => {
            if (state == PlayModeStateChange.ExitingPlayMode) cts.Cancel();
        };
#endif

        try {
            await ExampleView1.Open(cts.Token);            
            Debug.Log("Application quit");
        }
        catch (System.OperationCanceledException e)
        {
            Debug.Log("Editor play mode ended");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Application crashed: " + e.ToString());
        }
    }
}
