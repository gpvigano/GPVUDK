using UnityEngine;
using UnityEngine.UI;

namespace GPVUDK
{
    public class LoadingManager : SceneLoader
    {
        /// <summary>
        /// Image that will spin to notify a waiting.
        /// </summary>
        [Tooltip("Image that will spin to notify a waiting.")]
        public RawImage uiSpinner;

        /// <summary>
        /// Image with fill mode set, used to show the loading progress.
        /// </summary>
        [Tooltip("Image with fill mode set, used to show the loading progress.")]
        public Image uiProgressBar;

		
        private void Awake()
        {
            uiSpinner.gameObject.SetActive(false);
        }

		
        protected override void Update()
        {
            if (loadingStarted)
            {
                if(!uiSpinner.gameObject.activeSelf)
                {
                    Transform[] siblings = uiSpinner.transform.parent.GetComponentsInChildren<Transform>();
                    uiSpinner.gameObject.SetActive(true);
                    foreach(Transform tr in siblings)
                    {
                        if(tr!=uiSpinner.transform && tr!=uiSpinner.transform.parent)
                        {
                            tr.gameObject.SetActive(false);
                        }
                    }
                }
                uiSpinner.rectTransform.Rotate(Vector3.forward, 90.0f * Time.deltaTime);
            }
            base.Update();
            if (!loadingStarted)
            {
                if (uiProgressBar != null && loadingDelay>0)
                {
                    uiProgressBar.fillAmount = elapsedTime / loadingDelay;
                }
            }
        }

    }
}
