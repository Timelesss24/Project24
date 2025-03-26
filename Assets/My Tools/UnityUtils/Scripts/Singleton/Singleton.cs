using UnityEngine;

namespace UnityUtils {
    /// <summary>
    /// Singleton 클래스는 특정 컴포넌트의 단일 인스턴스만 존재하도록 보장합니다.
    /// - 단순한 싱글톤 패턴을 구현한 기본 클래스입니다.
    /// - 씬 전환 간 인스턴스를 유지하지 않습니다.
    /// - 중복 관리 기능이나 추가적인 로직 없이 단순한 싱글톤 관리에 적합합니다.
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : Component {
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

            instance = this as T;
        }
    }
}
