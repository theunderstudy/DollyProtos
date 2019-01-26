
using UnityEngine;
using DG.Tweening;
public class TimeController : MonoBehaviour
{
    public enum TimeState { Normal, Back, Paused }

    public static TimeState CurrentTimeState = TimeState.Normal;
    public Color NormalColor, BackColor;
    public float TransitionTime = 0.1f;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            CurrentTimeState = TimeState.Paused;
            DOTween.Kill("camColor");
            Camera.main.DOColor(BackColor, TransitionTime).SetId("camColor").OnComplete(
                () =>
                CurrentTimeState = TimeState.Back
            );
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            CurrentTimeState = TimeState.Paused;
            DOTween.Kill("camColor");
            Camera.main.DOColor(NormalColor, TransitionTime).SetId("camColor").OnComplete(()=> CurrentTimeState = TimeState.Normal);
        }
    }
}
