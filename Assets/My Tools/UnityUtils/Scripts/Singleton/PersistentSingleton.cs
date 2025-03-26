using UnityEngine;

namespace UnityUtils {
    /// <summary>
    /// PersistentSingleton 클래스는 씬 전환 간 유지와 독립적인 GameObject 특성을 가진 싱글톤입니다.
    /// - DontDestroyOnLoad: 씬 전환 시에도 인스턴스를 유지합니다.
    /// - AutoUnparentOnAwake: 초기화 시 부모 GameObject에서 분리하여 독립적으로 관리됩니다.
    /// - 중복된 인스턴스가 발견되면 기존 인스턴스를 즉시 삭제합니다.
    /// - 글로벌 상태를 유지하며 독립적인 싱글톤 관리가 필요한 경우에 사용됩니다.
    /// </summary>
    public class PersistentSingleton<T> : MonoBehaviour where T : Component {
        public bool AutoUnparentOnAwake = true;

        protected static T instance;

        public static bool HasInstance => instance != null;
        public static T TryGetInstance() => HasInstance ? instance : null;

        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindAnyObjectByType<T>();
                    if (instance == null) {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake() {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton() {
            if (!Application.isPlaying) return;

            if (AutoUnparentOnAwake) {
                transform.SetParent(null);
            }

            if (instance == null) {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            } else {
                if (instance != this) {
                    Destroy(gameObject);
                }
            }
        }
    }
}