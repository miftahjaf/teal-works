using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TexDrawLib
{
    // Creates boxes containing delimeter symbol that exists in different sizes.
    public static class DelimiterFactory
    {
        public static Box CreateBox(string symbol, float minHeight, TexStyle style)
        {
            var charInfo = TEXPreference.main.GetCharMetric(symbol, style);

            // Find first version of character that has at least minimum height.
            var totalHeight = charInfo.height + charInfo.depth;
            while (totalHeight < minHeight && charInfo.ch.nextLargerExist)
            {
                charInfo = TEXPreference.main.GetCharMetric(charInfo.ch.nextLarger, style);
                totalHeight = charInfo.height + charInfo.depth;
            }

            if (totalHeight >= minHeight)
            {
                // Character of sufficient height was found.
                return CharBox.Get(style, charInfo);
            }
            else if (charInfo.ch.extensionExist && !charInfo.ch.extensionHorizontal)
            {
                var resultBox = VerticalBox.Get();
                resultBox.ExtensionMode = true;
                // Construct box from extension character.
                var extension = charInfo.ch.GetExtentMetrics(style);
                if (extension[0] != null)
                    resultBox.Add(CharBox.Get(style, extension[0]));
                if (extension[1] != null)
                    resultBox.Add(CharBox.Get(style, extension[1]));
                if (extension[2] != null)
                    resultBox.Add(CharBox.Get(style, extension[2]));

                // Insert repeatable part multiple times until box is high enough.
                if (extension[3] != null)
                {
                    var repeatBox = CharBox.Get(style, extension[3]);
                    if(repeatBox.totalHeight <= 0)
                        throw new ArgumentOutOfRangeException("PULL CHAR DEL ZERO");
                    do
                    {
                        if (extension[0] != null && extension[2] != null)
                        {
                            resultBox.Add(1, repeatBox);
                            if (extension[1] != null)
                                resultBox.Add(resultBox.Children.Count - 1, repeatBox);
                        }
                        else if (extension[2] != null)
                        {
                            resultBox.Add(0, repeatBox);
                        }
                        else
                        {
                            resultBox.Add(repeatBox);
                        }
                    }
                    while (resultBox.height + resultBox.depth < minHeight);
                }
                return resultBox;
            }
            else
            {
                // No extensions available, so use tallest available version of character.
                return CharBox.Get(style, charInfo);
            }
        }

        public static Box CreateBoxHorizontal(string symbol, float minWidth, TexStyle style)
        {
            var charInfo = TEXPreference.main.GetCharMetric(symbol, style);

            // Find first version of character that has at least minimum width.
            var totalWidth = charInfo.width + charInfo.italic;
            while (totalWidth < minWidth && charInfo.ch.nextLargerExist)
            {
                charInfo = TEXPreference.main.GetCharMetric(charInfo.ch.nextLarger, style);
                totalWidth = charInfo.width + charInfo.italic;
            }

            if (totalWidth >= minWidth)
            {
                // Character of sufficient height was found.
                return CharBox.Get(style, charInfo);
            }
            else if (charInfo.ch.extensionExist)
            {
                var resultBox = HorizontalBox.Get();
                resultBox.ExtensionMode = true;
                // Construct box from extension character.
                var extension = charInfo.ch.GetExtentMetrics(style);
                if (extension[0] != null)
                    resultBox.Add(CharBox.Get(style, extension[0]));
                if (extension[1] != null)
                    resultBox.Add(CharBox.Get(style, extension[1]));
                if (extension[2] != null)
                    resultBox.Add(CharBox.Get(style, extension[2]));

                // Insert repeatable part multiple times until box is high enough.
                if (extension[3] != null)
                {
                    var repeatBox = CharBox.Get(style, extension[3]);
                    do
                    {
                        if (extension[0] != null && extension[2] != null)
                        {
                            resultBox.Add(1, repeatBox);
                            if (extension[1] != null)
                                resultBox.Add(resultBox.Children.Count - 1, repeatBox);
                        }
                        else if (extension[2] != null)
                        {
                            resultBox.Add(0, repeatBox);
                        }
                        else
                        {
                            resultBox.Add(repeatBox);
                        }
                    }
                    while (resultBox.width < minWidth);
                }
                return resultBox;
            }
            else
            {
                // No extensions available, so use tallest available version of character.
                return CharBox.Get(style, charInfo);
            }
        }
   
    }
}