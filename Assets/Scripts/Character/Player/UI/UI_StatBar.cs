using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

namespace SA
{
    public class UI_StatBar : MonoBehaviour
    {
        private Slider slider;
        protected RectTransform rectTransform;

        [Header("Bar Options")]
        [SerializeField] protected bool scaleBarLengthWithStat = true;
        [SerializeField] protected float widthMultiplier = 1;

        protected virtual void Awake()
        {
            slider = GetComponent<Slider>();
            rectTransform = GetComponent<RectTransform>();
        }

        public virtual void SetStat(int newValue)
        {
            slider.value = newValue;
        }

        public virtual void SetMaxStat(int maxValue)
        {
            slider.maxValue = maxValue;
            slider.value = maxValue;

            if (scaleBarLengthWithStat)
            {
                rectTransform.sizeDelta = new Vector2(maxValue * widthMultiplier, rectTransform.sizeDelta.y);

                // 레이아웃 그룹의 설정에 따라 막대의 위치를 재설정  
                PlayerUIManager.instance.playerUIHudManager.RefeshHUD();
            }
        }
    }
}