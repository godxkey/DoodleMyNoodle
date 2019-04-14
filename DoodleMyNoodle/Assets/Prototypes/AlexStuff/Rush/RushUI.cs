using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RushUI : MonoBehaviour
{
    public Text myHp;
    public Text ennemyHp;
    public GridLayoutGroup currentList;
    public GridLayoutGroup nextList;

    public GameObject endGameDisplay;
    public Text endGameText;

    public void ResetUI()
    {
        endGameDisplay.SetActive(false);
    }

    public void DisplayEnnemyWin()
    {
        endGameText.text = "YOU LOST";
        endGameText.color = Color.red;
        endGameDisplay.SetActive(true);
    }

    public void DisplayYouWin()
    {
        endGameText.text = "YOU WIN";
        endGameText.color = Color.green;
        endGameDisplay.SetActive(true);
    }

    public void NextTurnToyListAnimation(System.Action onComplete)
    {
        RectTransform rectTransform = currentList.GetComponent<RectTransform>();
        GridLayoutGroup backupGridInfo = currentList;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(currentList.GetComponent<CanvasGroup>().DOFade(0, 1));
        sequence.Join(nextList.GetComponent<RectTransform>().DOLocalMove(rectTransform.transform.localPosition, 1));
        GridLayoutGroup newList = Instantiate(nextList, nextList.transform.parent.transform).GetComponent<GridLayoutGroup>();
        newList.GetComponent<CanvasGroup>().alpha = 0;
        sequence.Join(newList.GetComponent<CanvasGroup>().DOFade(1, 1));
        sequence.OnComplete(()=> {
            Destroy(currentList.gameObject);
            currentList = nextList;
            nextList = newList;
            onComplete?.Invoke();
        });
    }

    public void ModifyEnnemyHealthDisplay(float newHp)
    {
        ennemyHp.text = "" + newHp;
    }

    public void ModifyMyHealthDisplay(float newHp)
    {
        myHp.text = "" + newHp;
    }
}
