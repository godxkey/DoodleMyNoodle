using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIDataTransferWidgetElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _incomingText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private Slider _progressSlider;

    public AutoResetDirtyValue<bool> IsIncoming;
    public AutoResetDirtyValue<string> Description;
    public AutoResetDirtyValue<int> TotalDataSize;
    public AutoResetDirtyValue<int> CurrentDataSize;
    public AutoResetDirtyValue<string> State;

    public void UpdateDisplay()
    {
        if (IsIncoming.IsDirty)
        {
            _incomingText.text = IsIncoming.Get() ? "Receiving:" : "Sending:";
        }

        if (Description.IsDirty)
        {
            _descriptionText.text = Description.Get();
        }

        if (TotalDataSize.IsDirty || CurrentDataSize.IsDirty || State.IsDirty)
        {
            if(CurrentDataSize.Get() == -1)
            {
                _progressSlider.gameObject.SetActive(false);

                _progressText.text = $"{State} - {StringUtility.ByteCountToReadableString(TotalDataSize.Get())}";
            }
            else
            {
                _progressSlider.gameObject.SetActive(true);

                float ratio = (float)CurrentDataSize.Get() / TotalDataSize.Get();
                _progressSlider.value = ratio;
                _progressText.text = $"{StringUtility.ByteCountRatioToReadableString(CurrentDataSize.Get(), TotalDataSize.Get())} ({(ratio * 100).Rounded()}%)";
            }


        }
    }


    // could be moved in a standalone file in CCC.Core
    static class StringUtility
    {
        public static string ByteCountToReadableString(int byteCount)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            float len = byteCount;
            while (len >= 1000f && order < sizes.Length - 1)
            {
                order++;
                len /= 1000f;
            }

            return $"{NumberWithFixedCharacters(len, 3)} {sizes[order]}";
        }

        public static string ByteCountRatioToReadableString(int nominator, int denominator)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            float lenA = nominator;
            float lenB = denominator;
            while ((lenB >= 1000f || lenA >= 1000f) && order < sizes.Length - 1)
            {
                order++;
                lenA /= 1000f;
                lenB /= 1000f;
            }

            return $"{NumberWithFixedCharacters(lenA, 3)} / {NumberWithFixedCharacters(lenB, 3)} {sizes[order]}";
        }

        public static string NumberWithFixedCharacters(float v, int digits)
        {
            string s = v.ToString();
            int commaIndex = s.IndexOf('.');

            if (commaIndex == -1)
                return s;

            if (commaIndex > 0 && commaIndex < digits)
            {
                return s.RemoveFrom(digits + 1);
            }
            else
            {
                return s.RemoveFrom(commaIndex);
            }
        }
    }
}
