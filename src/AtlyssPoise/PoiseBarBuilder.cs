using UnityEngine;
using UnityEngine.UI;

namespace AtlyssPoise
{
    public class PoiseBarBuilder : MonoBehaviour
    {
        public Sprite barSprite;
        private Slider poiseSlider;
        private CanvasGroup canvasGroup;
        private float visibleTimer = 0f;
        private float visibleDuration = 3f;

        void Start()
        {
            barSprite = SpriteLoader.LoadSpriteFromFile("Bar.png");
            if (barSprite == null)
                Debug.LogWarning("[Poise] Bar sprite failed to load!");
            CreatePoiseBarUI();
        }
        
        void Update()
        {
            if (visibleTimer > 0f)
            {
                visibleTimer -= Time.deltaTime;
                if (visibleTimer <= 0f)
                {
                    canvasGroup.alpha = 0f; // Hide
                }
            }
        }
        
        public void ShowTemporarily()
        {
            visibleTimer = visibleDuration;
            canvasGroup.alpha = 1f; // Show
        }


        private void CreatePoiseBarUI()
        {
            // Create Canvas
            GameObject canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            canvasGroup = canvasObj.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f; // Start hidden

            // Adjust Canvas scaler for resolution independence
            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Create HealthBar
            GameObject healthBarObj = new GameObject("HealthBar", typeof(RectTransform), typeof(Slider));
            healthBarObj.transform.SetParent(canvasObj.transform, false);
            RectTransform healthBarRect = healthBarObj.GetComponent<RectTransform>();
            healthBarRect.sizeDelta = new Vector2(40, 200); // Width x Height
            healthBarRect.anchorMin = new Vector2(0.275f, 0.01f); // Bottom-left-ish
            healthBarRect.anchorMax = new Vector2(0.275f, 0.01f);
            healthBarRect.pivot = new Vector2(0.5f, 0f);
            healthBarRect.anchoredPosition = Vector2.zero;

            // Create Slider
            Slider slider = healthBarObj.GetComponent<Slider>();
            slider.direction = Slider.Direction.BottomToTop;
            slider.transition = Selectable.Transition.None;
            slider.interactable = false;
            slider.navigation = new Navigation { mode = Navigation.Mode.None };

            // Create "Fill" image
            GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObj.transform.SetParent(healthBarObj.transform, false);
            RectTransform fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            Image fillImage = fillObj.GetComponent<Image>();
            fillImage.color = new Color(1f, 0.5f, 0f, 0.8f); // Semi-dark orange
            
            Texture2D whiteTex = Texture2D.whiteTexture;
            Sprite whiteSprite = Sprite.Create(whiteTex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            fillImage.sprite = whiteSprite;

            // Create Border image (Bar.png)
            GameObject borderObj = new GameObject("Border", typeof(RectTransform), typeof(Image));
            borderObj.transform.SetParent(healthBarObj.transform, false);
            RectTransform borderRect = borderObj.GetComponent<RectTransform>();
            borderRect.anchorMin = new Vector2(0, 0);
            borderRect.anchorMax = new Vector2(1, 1);
            borderRect.offsetMin = Vector2.zero;
            borderRect.offsetMax = Vector2.zero;

            Image borderImage = borderObj.GetComponent<Image>();
            borderImage.sprite = barSprite;
            borderImage.type = Image.Type.Sliced;

            // Assign fill to slider
            slider.fillRect = fillRect;
            slider.targetGraphic = fillImage;

            // Set initial values
            slider.minValue = 0;
            slider.maxValue = 100;
            slider.value = 100;

            // Reference
            poiseSlider = slider;
        }
        
        public void SetPoisePercent(float value01)
        {
            if (poiseSlider != null)
                poiseSlider.value = Mathf.Clamp01(value01) * poiseSlider.maxValue;
        }
    }
}
