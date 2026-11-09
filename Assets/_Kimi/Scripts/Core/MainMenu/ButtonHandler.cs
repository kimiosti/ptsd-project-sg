using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public void StartDurakGame()
    {
        StartCoroutine(ChangeScene("CardGameScene"));
    }

    private IEnumerator ChangeScene(string sceneName)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
