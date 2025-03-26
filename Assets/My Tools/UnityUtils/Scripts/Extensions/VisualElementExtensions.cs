using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUtils
{
    public static class VisualElementExtensions
    {
        /// <summary>
        /// 새로운 자식 VisualElement를 생성하고 이를 부모에 추가합니다.
        /// </summary>
        /// <param name="parent">자식을 추가할 부모 VisualElement입니다.</param>
        /// <param name="classes">자식 요소에 추가할 CSS 클래스들입니다.</param>
        /// <returns>생성된 자식 VisualElement를 반환합니다.</returns>
        public static VisualElement CreateChild(this VisualElement parent, params string[] classes)
        {
            var child = new VisualElement();
            child.AddClass(classes).AddTo(parent);
            return child;
        }

        /// <summary>
        /// 새로 생성된 T 타입의 자식을 부모에 추가합니다.
        /// </summary>
        /// <typeparam name="T">자식 VisualElement의 타입입니다.</typeparam>
        /// <param name="parent">자식을 추가할 부모 VisualElement입니다.</param>
        /// <param name="classes">자식에게 추가할 CSS 클래스들입니다.</param>
        /// <returns>생성된 T 타입의 자식 VisualElement입니다.</returns>
        public static T CreateChild<T>(this VisualElement parent, params string[] classes)
            where T : VisualElement, new()
        {
            var child = new T();
            child.AddClass(classes).AddTo(parent);
            return child;
        }

        /// <summary>
        /// 자식 VisualElement를 부모에 추가하고 자식을 반환합니다.
        /// </summary>
        /// <typeparam name="T">자식 VisualElement의 유형입니다.</typeparam>
        /// <param name="child">추가할 자식 VisualElement입니다.</param>
        /// <param name="parent">자식을 추가할 부모 VisualElement입니다.</param>
        /// <returns>추가된 자식 VisualElement를 반환합니다.</returns>
        public static T AddTo<T>(this T child, VisualElement parent) where T : VisualElement
        {
            parent.Add(child);
            return child;
        }

        /// <remarks>
        /// See <see cref="AddTo{T}(T, VisualElement)"/> for adding a child to a parent.
        /// </remarks>
        public static void RemoveFrom<T>(this T child, VisualElement parent)
            where T : VisualElement => parent.Remove(child);

        /// <summary>
        /// 지정된 ㅕSS 클래스를 VisualElement에 추가합니다.
        /// </summary>
        /// <typeparam name="T">VisualElement의 유형입니다.</typeparam>
        /// <param name="visualElement">CSS 클래스를 추가할 VisualElement입니다.</param>
        /// <param name="classes">추가할 CSS 클래스들입니다.</param>
        /// <returns>CSS 클래스가 추가된 VisualElement를 반환합니다.</returns>
        public static T AddClass<T>(this T visualElement, params string[] classes) where T : VisualElement
        {
            foreach (string cls in classes)
            {
                if (!string.IsNullOrEmpty(cls))
                {
                    visualElement.AddToClassList(cls);
                }
            }
            return visualElement;
        }

        /// <remarks>
        /// 클래스 추가에 대해선 <see cref="AddClass{T}(T, string[])"/>를 참조하세요.
        /// </remarks>
        public static void RemoveClass<T>(this T visualElement, params string[] classes) where T : VisualElement
        {
            foreach (string cls in classes)
            {
                if (!string.IsNullOrEmpty(cls))
                {
                    visualElement.RemoveFromClassList(cls);
                }
            }
        }

        /// <summary>
        /// VisualElement에 조작기를 추가합니다.
        /// </summary>
        /// <typeparam name="T">VisualElement의 유형입니다.</typeparam>
        /// <param name="visualElement">조작기를 추가할 VisualElement입니다.</param>
        /// <param name="manipulator">추가할 조작기입니다.</param>
        /// <returns>조작기가 추가된 VisualElement를 반환합니다.</returns>
        public static T WithManipulator<T>(this T visualElement, IManipulator manipulator) where T : VisualElement
        {
            visualElement.AddManipulator(manipulator);
            return visualElement;
        }

        /// <summary>
        /// 지정된 Sprite를 사용하여 VisualElement의 배경 이미지를 설정합니다.
        /// </summary>
        /// <param name="imageContainer">배경 이미지가 설정될 VisualElement입니다.</param>
        /// <param name="sprite">배경 이미지로 사용할 Sprite입니다.</param>
        public static void SetImageFromSprite(this VisualElement imageContainer, Sprite sprite)
        {
            var texture = sprite.texture;
            if (texture)
            {
                imageContainer.style.backgroundImage = new StyleBackground(texture);
            }
        }
    }
}