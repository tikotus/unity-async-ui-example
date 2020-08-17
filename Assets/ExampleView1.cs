using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ExampleView1 : MonoBehaviour
{
    public Button buttonIncrement;
    public Button buttonDecrement;
    public Button buttonOpenSubView;
    public Button buttonClose;
    public Text textCounter;

    public static async Task Open(CancellationToken ct)
    {
        int counter = 0;

        ExampleView1 view = await Utils.LoadScene<ExampleView1>("Example View 1", ct);
        
        try 
        {
            while (true)
            {
                CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                Task pressButtonIncrement = Utils.PressButton(view.buttonIncrement, cts.Token);
                Task pressButtonDecrement = Utils.PressButton(view.buttonDecrement, cts.Token);
                Task pressButtonOpenSubView = Utils.PressButton(view.buttonOpenSubView, cts.Token);
                Task pressButtonClose = Utils.PressButton(view.buttonClose, cts.Token);
                Task pressBackButton = Utils.PressBackButton(cts.Token);
                
                Task finishedTask = await Task.WhenAny(pressButtonIncrement, pressButtonDecrement, pressButtonOpenSubView, pressButtonClose, pressBackButton);
                await finishedTask;
                cts.Cancel();
                cts.Dispose();

                if (finishedTask == pressButtonIncrement) {
                    counter++;
                    view.textCounter.text = counter.ToString();
                }
                else if (finishedTask == pressButtonDecrement) {
                    counter--;
                    view.textCounter.text = counter.ToString();
                }
                else if (finishedTask == pressButtonOpenSubView) {
                    view.gameObject.SetActive(false);
                    await ExampleView2.Open(ct);
                    view.gameObject.SetActive(true);
                }
                else {
                    return;
                }
            }
        }
        finally 
        {
            await Task.Yield(); // You can play a close animation here
            SceneManager.UnloadScene(view.gameObject.scene);
        }
    }
}
