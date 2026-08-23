using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif

namespace KaijuGame.Voice
{
#if PHOTON_UNITY_NETWORKING
    public sealed class VoiceMeterNetwork : MonoBehaviourPun, IPunObservable
#else
    public sealed class VoiceMeterNetwork : MonoBehaviour
#endif
    {
        [SerializeField] private Transform headAnchor;
        [SerializeField] private float sensitivity = 7f;
        [SerializeField] private float smoothing = 14f;
        [SerializeField] private float maxVisibleDistance = 30f;
        [SerializeField] private bool showMeter = true;

        private Component recorder;
        private PropertyInfo levelMeterProperty;
        private PropertyInfo peakProperty;
        private float targetLevel;
        private float smoothedLevel;
        private Slider slider;
        private Camera mainCamera;

        public float VoiceLevel => smoothedLevel;
        public bool IsSpeaking => smoothedLevel > 0.08f;

        private void Awake()
        {
            mainCamera = Camera.main;
            ResolveRecorder();
            if (headAnchor == null) headAnchor = transform;
            if (showMeter) CreateMeter();
        }

        private void Update()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (IsLocal()) targetLevel = ReadRecorderLevel();

            smoothedLevel = Mathf.MoveTowards(smoothedLevel, targetLevel, smoothing * Time.deltaTime);
            if (slider != null)
            {
                slider.value = smoothedLevel;
                slider.gameObject.SetActive(mainCamera != null && Vector3.Distance(mainCamera.transform.position, headAnchor.position) <= maxVisibleDistance);
            }
        }

        private bool IsLocal()
        {
#if PHOTON_UNITY_NETWORKING
            return photonView.IsMine;
#else
            return true;
#endif
        }

        private void ResolveRecorder()
        {
            var recorderType = System.Type.GetType("Photon.Voice.Unity.Recorder, PhotonVoice");
            if (recorderType == null)
                recorderType = System.Type.GetType("Photon.Voice.Unity.Recorder, PhotonVoiceLibs");
            if (recorderType == null) return;

            foreach (var component in GetComponentsInChildren<Component>(true))
            {
                if (component != null && recorderType.IsInstanceOfType(component))
                {
                    recorder = component;
                    break;
                }
            }

            if (recorder == null) return;
            levelMeterProperty = recorderType.GetProperty("LevelMeter", BindingFlags.Instance | BindingFlags.Public);
            var meterType = levelMeterProperty?.PropertyType;
            peakProperty = meterType?.GetProperty("CurrentPeakAmp", BindingFlags.Instance | BindingFlags.Public);
        }

        private float ReadRecorderLevel()
        {
            if (recorder == null || levelMeterProperty == null || peakProperty == null) return 0f;
            var meter = levelMeterProperty.GetValue(recorder);
            if (meter == null) return 0f;
            var peak = (float)peakProperty.GetValue(meter);
            return Mathf.Clamp01(peak * sensitivity);
        }

        private void CreateMeter()
        {
            var root = new GameObject("VoiceMeter", typeof(Canvas));
            root.transform.SetParent(headAnchor, false);
            root.transform.localPosition = Vector3.up * 0.3f;
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100;
            var scaler = root.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10f;

            var back = CreateImage(root.transform, "Background");
            back.rectTransform.sizeDelta = new Vector2(0.5f, 0.06f);
            var fill = CreateImage(back.transform, "Fill");
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = Vector2.one;
            fill.rectTransform.offsetMin = Vector2.zero;
            fill.rectTransform.offsetMax = Vector2.zero;

            slider = back.gameObject.AddComponent<Slider>();
            slider.fillRect = fill.rectTransform;
            slider.interactable = false;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 0f;
            root.transform.localScale = Vector3.one * 0.35f;
        }

        private static Image CreateImage(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            return go.GetComponent<Image>();
        }

#if PHOTON_UNITY_NETWORKING
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
                stream.SendNext(smoothedLevel);
            else
                targetLevel = Mathf.Lerp(targetLevel, (float)stream.ReceiveNext(), 0.7f);
        }
#endif
    }
}
