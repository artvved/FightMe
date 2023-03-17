using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SpellButtonCDView : MonoBehaviour
    {
        public Image ProgressImage;
        public void SetValue(float p)
        {
            var transformLocalScale = ProgressImage.transform.localScale;
            transformLocalScale.x = p;
            ProgressImage.transform.localScale = transformLocalScale;
        }

    }
}