using System.Reflection;
using UnityEngine;

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
        [SerializeField] private float meterWidth = 0.55f;
        [SerializeField] private float meterHeight = 0.045f;

        private Component recorder;
        private PropertyInfo levelMeterProperty;
        private PropertyInfo peakProperty;
        private float targetLevel;
        private float smoothedLevel;
        private Transform meterRoot;
        private Transform fill;
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
            UpdateMeter();
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
            var recorderType = System.Type.GetType("Photon.Voice.Unity.Recorder, PhotonVoice")
                ?? System.Type.GetType("Photon.Voice.Unity.Recorder, PhotonVoiceLibs");
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
            var peak = peakProperty.GetValue(meter);
            if (peak == null) return 0f;
            return Mathf.Clamp01((float)peak * sensitivity);
        }

        private void CreateMeter()
        {
            meterRoot = new GameObject("VoiceMeter").transform;
            meterRoot.SetParent(headAnchor, false);
            meterRoot.localPosition = Vector3.up * 0.3f;
            meterRoot.localScale = Vector3.one * 0.35f;

            CreateQuad(meterRoot, "Background", new Vector3(meterWidth, meterHeight, 1f));
            fill = CreateQuad(meterRoot, "Fill", new Vector3(meterWidth, meterHeight, 0.02f));
        }

        private void UpdateMeter()
        {
            if (meterRoot == null || fill == null || mainCamera == null) return;
            var distance = Vector3.Distance(mainCamera.transform.position, headAnchor.position);
            meterRoot.gameObject.SetActive(distance <= maxVisibleDistance);
            if (!meterRoot.gameObject.activeSelf) return;

            meterRoot.forward = mainCamera.transform.position - meterRoot.position;
            var level = Mathf.Clamp01(smoothedLevel);
            fill.localScale = new Vector3(meterWidth * level, meterHeight, 0.02f);
            fill.localPosition = new Vector3((level - 1f) * meterWidth * 0.5f, 0f, -0.02f);
        }

        private static Transform CreateQuad(Transform parent, string name, Vector3 scale)
        {
            var primitive = GameObject.CreatePrimitive(PrimitiveType.Quad);
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            primitive.transform.localScale = scale;
            var collider = primitive.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            return primitive.transform;
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
