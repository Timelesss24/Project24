using UnityEngine;

namespace UnityUtils {
    /// <summary>
    /// RegulatorSingleton 클래스는 싱글톤에 중복 관리 기능을 추가한 확장된 클래스입니다.
    /// - DontDestroyOnLoad: 씬 전환 간 인스턴스를 유지합니다.
    /// - 중복된 인스턴스가 발견되면 오래된(초기화 시간이 더 앞선) 기존 인스턴스를 삭제합니다.
    /// - 규칙에 따라 하나의 인스턴스만 유지하며, 싱글톤 관리에 안정성을 제공합니다.
    /// </summary>
    public class RegulatorSingleton<T> : MonoBehaviour where T : Component {
        protected static T instance;

        public static bool HasInstance => instance != null;

        public float InitializationTime { get; private set; }

        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindAnyObjectByType<T>();
                    if (instance == null) {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        go.hideFlags = HideFlags.HideAndDontSave;
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// `Awake()`를 재정의할 경우, 반드시 `base.Awake()`를 호출해야 합니다.
        /// </summary>
        protected virtual void Awake() {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton() {
            if (!Application.isPlaying) return;
            InitializationTime = Time.time;
            DontDestroyOnLoad(gameObject);

            T[] oldInstances = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (T old in oldInstances) {
                if (old.GetComponent<RegulatorSingleton<T>>().InitializationTime < InitializationTime) {
                    Destroy(old.gameObject);
                }
            }

            if (instance == null) {
                instance = this as T;
            }
        }
    }
}