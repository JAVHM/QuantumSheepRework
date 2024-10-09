using UnityEngine;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public TextMeshProUGUI _timeText;
    public GameObject _gameIlumination;

    public void SetTimeText(int time)
    {
        _timeText.text = time.ToString();
    }

    public void SetTimeText()
    {
        _timeText.gameObject.SetActive(false);
    }

    public void SetGameIlumination(DayCycle part)
    {
        if (part == DayCycle.Day)
        {
            _gameIlumination.SetActive(false);
        }
        else
        {
            _gameIlumination.SetActive(true);
        }  
    }
}
