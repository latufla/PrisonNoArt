using System;
using TMPro;
using UnityEngine;


namespace Honeylab.Utils
{
    public class TimePanel : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI _label;

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }


        public void SetTime(double seconds, string customFormat = null, int timeAmount = 0)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);

            string text;
            if (customFormat != null)
            {
                text = time.ToString(customFormat);
            }
            else
            {
                string[] formatArray = new string[4];
                if (time.Days > 0)
                {
                    formatArray[0] = @"dd\d\ ";
                }
                if (time.Hours > 0)
                {
                    formatArray[1] = @"hh\h\ ";
                }
                if (time.Minutes > 0)
                {
                    formatArray[2] = @"mm\m\ ";
                }
                if (time.Seconds >= 0)
                {
                    formatArray[3] = @"ss\s";
                }

                text = string.Empty;
                int amount = timeAmount > 0 ? timeAmount : formatArray.Length;
                for (int i = 0; i < formatArray.Length; i++)
                {
                    if (formatArray[i] != null && amount > 0)
                    {
                        text += time.ToString(formatArray[i]);
                        amount--;
                    }
                }
            }

            _label.text = text;
        }
    }
}
