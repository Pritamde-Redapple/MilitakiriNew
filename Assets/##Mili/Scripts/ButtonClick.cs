
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        MilitakiriAudioManager.Instance.PlayUISound(AudioTag.BUTTON_CLICK_2);
    }
}
