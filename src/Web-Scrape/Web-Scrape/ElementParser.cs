using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Web_Scrape
{
    public class ElementParser
    {
        public static IEnumerable<GenericElement> FindElements(string elementName, string html)
        {
            List<GenericElement> list = new List<GenericElement>();
            var elementMatches = Regex.Matches(html, string.Format(@"<{0}([^<]*?)>(.*?)</{0}>|<{0}([^<]*?)/>|<{0}([^<]*?)>", elementName), RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match elementMatch in elementMatches)
            {
                var t = new GenericElement(elementName.ToLower());
                if (elementMatch.Groups[2].Success)
                    t.Text = elementMatch.Groups[2].Value;

                string attributeValues = "";
                if (elementMatch.Groups[1].Success)
                    attributeValues = elementMatch.Groups[1].Value;
                else if (elementMatch.Groups[3].Success)
                    attributeValues = elementMatch.Groups[3].Value;
                else if (elementMatch.Groups[4].Success)
                    attributeValues = elementMatch.Groups[4].Value;

                var attributeMatches = Regex.Matches(attributeValues, @"([A-Za-z]*)\s*=\s*(\""(.*?)\"")|('(.*?)')", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match attributeMatch in attributeMatches)
                {
                    if (!attributeMatch.Groups[1].Success ||
                        attributeMatch.Groups[1].Value.StartsWith("'") ||
                        attributeMatch.Groups[1].Value.StartsWith(("\"")))
                        continue;
                    t.Attributes[attributeMatch.Groups[1].Value.ToLower()] = attributeMatch.Groups[3].Success ? attributeMatch.Groups[3].Value : attributeMatch.Groups[5].Value;
                }
                list.Add(t);
            }

            return list;
        }

        public static IEnumerable<GenericElement> FindElementsLike(string elementName, string html, string tagRegex)
        {
            List<GenericElement> list = new List<GenericElement>();
            var elementMatches = Regex.Matches(html, string.Format(@"<{0}([^<]*{1}[^<]*?)>(.*?)</{0}>|<{0}([^<]*?{1}[^<]*?)/>|<{0}([^<]*?{1}[^<]*?)>", elementName, tagRegex), RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match elementMatch in elementMatches)
            {
                var t = new GenericElement(elementName.ToLower());
                if (elementMatch.Groups[2].Success)
                    t.Text = elementMatch.Groups[2].Value;

                string attributeValues = "";
                if (elementMatch.Groups[1].Success)
                    attributeValues = elementMatch.Groups[1].Value;
                else if (elementMatch.Groups[3].Success)
                    attributeValues = elementMatch.Groups[3].Value;
                else if (elementMatch.Groups[4].Success)
                    attributeValues = elementMatch.Groups[4].Value;

                var attributeMatches = Regex.Matches(attributeValues, @"([A-Za-z]*)\s*=\s*(\""(.*?)\"")|('(.*?)')", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match attributeMatch in attributeMatches)
                {
                    if (!attributeMatch.Groups[1].Success ||
                        attributeMatch.Groups[1].Value.StartsWith("'") ||
                        attributeMatch.Groups[1].Value.StartsWith(("\"")))
                        continue;
                    t.Attributes[attributeMatch.Groups[1].Value.ToLower()] = attributeMatch.Groups[3].Success ? attributeMatch.Groups[3].Value : attributeMatch.Groups[5].Value;
                }
                list.Add(t);
            }

            return list;
        }

        public static Dictionary<string, IEnumerable<GenericElement>> FindAllElements(IEnumerable<string> elementNames, string html)
        {
            Dictionary<string, IEnumerable<GenericElement>> output = new Dictionary<string, IEnumerable<GenericElement>>();
            foreach (string elementName in elementNames)
            {
                output[elementName] = FindElements(elementName, html);
            }
            return output;
        }

        public static Dictionary<string, IEnumerable<GenericElement>> FindAllElements(string html)
        {
            var elementNames = FindTags(html);
            Dictionary<string, IEnumerable<GenericElement>> output = new Dictionary<string, IEnumerable<GenericElement>>();
            foreach (string elementName in elementNames)
            {
                output[elementName] = FindElements(elementName, html);
            }
            return output;
        }

        public static IEnumerable<string> FindTags(string html)
        {
            List<string> output = new List<string>();
            var tagMatches = Regex.Matches(html, string.Format(@"<\s*([A-Za-z]+)[^>]*?>"), RegexOptions.Singleline);
            foreach (Match tagMatch in tagMatches)
            {
                if (tagMatch.Groups[1].Success && !output.Contains(tagMatch.Groups[1].Value.ToLower()))
                    output.Add(tagMatch.Groups[1].Value.ToLower());
            }
            return output;
        }
    }
}
