using EFT.UI.DragAndDrop;
using EFT.UI.WeaponModding;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeaponBuilderSearch.UI
{
    internal sealed class AttachmentSearchController : MonoBehaviour
    {
        private static readonly FieldInfo ItemsContainerField =
            AccessTools.Field(typeof(DropDownMenu), "_itemsContainer");

        private static readonly FieldInfo NoneItemContainerField =
            AccessTools.Field(typeof(DropDownMenu), "_noneItemContainer");

        private static readonly MethodInfo RepositionMethod =
            AccessTools.Method(typeof(DropDownMenu), "method_1");

        private readonly List<ItemView> _itemViews = new List<ItemView>();
        private TMP_InputField _searchField;
        private GameObject _searchRoot;
        private DropDownMenu _menu;

        public void OnMenuOpened(DropDownMenu menu)
        {
            _menu = menu;
            _itemViews.Clear();

            var container = (RectTransform)ItemsContainerField.GetValue(menu);
            for (var i = 0; i < container.childCount; i++)
            {
                var view = container.GetChild(i).GetComponent<ItemView>();
                if (view != null)
                    _itemViews.Add(view);
            }

            var minItems = Plugin.MinItemsForSearch.Value;
            if (_itemViews.Count < minItems)
            {
                HideSearchField();
                ApplyFilter(string.Empty);
                return;
            }

            EnsureSearchField(menu);
            UpdateSearchFieldLayout(menu);
            _searchField.SetTextWithoutNotify(string.Empty);
            _searchRoot.SetActive(true);
            ApplyFilter(string.Empty);
        }

        public void OnMenuClosed()
        {
            HideSearchField();
            _itemViews.Clear();
            _menu = null;
        }

        private void HideSearchField()
        {
            if (_searchRoot != null)
                _searchRoot.SetActive(false);
        }

        private void EnsureSearchField(DropDownMenu menu)
        {
            if (_searchField != null)
                return;

            var font = FindUIFont(menu.transform);

            _searchRoot = new GameObject("AttachmentSearchField");
            _searchRoot.transform.SetParent(menu.transform, false);
            _searchRoot.transform.SetAsLastSibling();

            var rootRect = _searchRoot.AddComponent<RectTransform>();
            var layoutElement = _searchRoot.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            var background = _searchRoot.AddComponent<Image>();
            background.color = new Color(0.08f, 0.08f, 0.08f, 0.92f);
            background.raycastTarget = true;

            var textArea = new GameObject("Text Area");
            textArea.transform.SetParent(_searchRoot.transform, false);
            var textAreaRect = textArea.AddComponent<RectTransform>();
            StretchRect(textAreaRect, 12f, 4f, 12f, 4f);

            var placeholderGo = new GameObject("Placeholder");
            placeholderGo.transform.SetParent(textArea.transform, false);
            var placeholderRect = placeholderGo.AddComponent<RectTransform>();
            StretchRect(placeholderRect, 0f, 0f, 0f, 0f);
            var placeholderText = placeholderGo.AddComponent<TextMeshProUGUI>();
            placeholderText.text = "Search attachments...";
            placeholderText.font = font;
            placeholderText.fontSize = 16f;
            placeholderText.color = new Color(0.65f, 0.65f, 0.65f, 0.75f);
            placeholderText.alignment = TextAlignmentOptions.MidlineLeft;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(textArea.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            StretchRect(textRect, 0f, 0f, 0f, 0f);
            var inputText = textGo.AddComponent<TextMeshProUGUI>();
            inputText.font = font;
            inputText.fontSize = 16f;
            inputText.color = new Color(0.92f, 0.92f, 0.92f, 1f);
            inputText.alignment = TextAlignmentOptions.MidlineLeft;

            _searchField = _searchRoot.AddComponent<TMP_InputField>();
            _searchField.textViewport = textAreaRect;
            _searchField.textComponent = inputText;
            _searchField.placeholder = placeholderText;
            _searchField.lineType = TMP_InputField.LineType.SingleLine;
            _searchField.onValueChanged.AddListener(ApplyFilter);
        }

        private void UpdateSearchFieldLayout(DropDownMenu menu)
        {
            if (_searchRoot == null)
                return;

            _searchRoot.transform.SetParent(menu.transform, false);
            _searchRoot.transform.SetAsLastSibling();

            var rootRect = _searchRoot.GetComponent<RectTransform>();
            var items = (RectTransform)ItemsContainerField.GetValue(menu);
            var targetWidth = GetDropdownContentWidth(menu) * 0.5f;
            var left = GetLeftOffset(menu.RectTransform, items, -5f);
            var scrollActive = menu.GetComponent<ScrollRect>() != null;

            rootRect.anchorMin = new Vector2(0f, 0f);
            rootRect.anchorMax = new Vector2(0f, 0f);
            rootRect.sizeDelta = new Vector2(targetWidth, 36f);

            if (scrollActive)
            {
                rootRect.pivot = new Vector2(0f, 0f);
                rootRect.anchoredPosition = new Vector2(left, 0f);
            }
            else
            {
                rootRect.pivot = new Vector2(0f, 1f);
                rootRect.anchoredPosition = new Vector2(left, 0f);
            }
        }

        private static float GetLeftOffset(RectTransform menu, RectTransform items, float adjust)
        {
            if (items == null)
                return adjust;

            var corners = new Vector3[4];
            items.GetWorldCorners(corners);
            return menu.InverseTransformPoint(corners[0]).x + adjust;
        }

        private static float GetDropdownContentWidth(DropDownMenu menu)
        {
            var items = (RectTransform)ItemsContainerField.GetValue(menu);
            var none = (RectTransform)NoneItemContainerField.GetValue(menu);
            var menuRect = menu.RectTransform;

            return Mathf.Max(
                GetRectWidth(items),
                GetRectWidth(none),
                GetRectWidth(menuRect),
                520f);
        }

        private static float GetRectWidth(RectTransform rect)
        {
            if (rect == null)
                return 0f;

            var width = rect.rect.width;
            if (width > 1f)
                return width;

            return Mathf.Max(rect.sizeDelta.x, 0f);
        }

        private void ApplyFilter(string query)
        {
            foreach (var view in _itemViews)
            {
                if (view == null)
                    continue;

                var visible = AttachmentSearchMatcher.Matches(view.Item, query);
                view.gameObject.SetActive(visible);
            }

            if (_menu != null && _menu.Open && RepositionMethod != null)
                RepositionMethod.Invoke(_menu, null);
        }

        private static TMP_FontAsset FindUIFont(Transform root)
        {
            var sample = root.GetComponentInChildren<TextMeshProUGUI>(true);
            if (sample != null && sample.font != null)
                return sample.font;

            return TMP_Settings.defaultFontAsset;
        }

        private static void StretchRect(RectTransform rect, float left, float bottom, float right, float top)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }
    }
}
