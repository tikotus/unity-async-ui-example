using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ExampleView3 : MonoBehaviour
{
    public Button buttonIncrement;
    public Button buttonDecrement;
    public Button buttonClose;
    public Text textCounter;

    public static async Task Open(CancellationToken ct)
    {
        int counter = 0;

        ExampleView3 view = await Utils.LoadScene<ExampleView3>("Example View 3", ct);
        
        try 
        {
            while (true)
            {
                CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                Task pressButtonIncrement = Utils.PressButton(view.buttonIncrement, cts.Token);
                Task pressButtonDecrement = Utils.PressButton(view.buttonDecrement, cts.Token);
                Task pressButtonClose = Utils.PressButton(view.buttonClose, cts.Token);
                Task pressBackButton = Utils.PressBackButton(cts.Token);
                
                Task finishedTask = await Task.WhenAny(pressButtonIncrement, pressButtonDecrement, pressButtonClose, pressBackButton);
                await finishedTask;
                cts.Cancel();
                cts.Dispose();

                if (finishedTask == pressButtonIncrement) {
                    counter += 3;
                    view.textCounter.text = counter.ToString();
                }
                else if (finishedTask == pressButtonDecrement) {
                    counter -= 3;
                    view.textCounter.text = counter.ToString();
                }
                else {
                    return;
                }
            }
        }
        finally 
        {
            SceneManager.UnloadScene(view.gameObject.scene);
        }
    }
}
