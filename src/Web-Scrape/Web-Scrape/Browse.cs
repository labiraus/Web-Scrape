using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Text.RegularExpressions;


namespace Web_Scrape
{
    public class Browse
    {
        Scraper scraper;
        string lastResponse;
        public string lastUrl;

        public Browse()
            : this("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.22 (KHTML, like Gecko) Chrome/25.0.1364.172 Safari/537.22")
        {
        }

        public Browse(string userAgent)
        {
            this.scraper = new Scraper(userAgent);
        }

        public string ClickImageButton(string imageButtonName)
        {
            return SubmitExclusive(new Dictionary<string, string>(), imageButtonName);
        }

        public string ClickImageButton(string url, string imageButtonName)
        {
            return SubmitExclusive(url, new Dictionary<string, string>(), imageButtonName);
        }

        public string SubmitAll(string[,] args)
        {
            return submitInputs(checkUrl(lastResponse, lastUrl), updateElements(getElementsForSubmit(lastResponse), args), null);
        }

        public string SubmitAll(string[,] args, string imageButtonName)
        {
            return submitInputs(checkUrl(lastResponse, lastUrl), updateElements(getElementsForSubmit(lastResponse), args), imageButtonName);
        }

        public string SubmitAll(string url, string[,] args)
        {
            string response = scraper.HttpGet(url).AsString();
            return submitInputs(checkUrl(response, url), updateElements(getElementsForSubmit(response), args), null);
        }

        public string SubmitAll(string url, string[,] args, string imageButtonName)
        {
            string response = scraper.HttpGet(url).AsString();
            return submitInputs(checkUrl(response, url), updateElements(getElementsForSubmit(response), args), imageButtonName);
        }

        public string SubmitAll(Dictionary<string, string> args)
        {
            return submitInputs(checkUrl(lastResponse, lastUrl), updateElements(getElementsForSubmit(lastResponse), args), null);
        }

        public string SubmitAll(Dictionary<string, string> args, string imageButtonName)
        {
            return submitInputs(checkUrl(lastResponse, lastUrl), updateElements(getElementsForSubmit(lastResponse), args), imageButtonName);
        }

        public string SubmitAll(string url, Dictionary<string, string> args)
        {
            string response = scraper.HttpGet(url).AsString();
            return submitInputs(checkUrl(response, url), updateElements(getElementsForSubmit(response), args), null);
        }

        public string SubmitAll(string url, Dictionary<string, string> args, string imageButtonName)
        {
            string response = scraper.HttpGet(url).AsString();
            return submitInputs(checkUrl(response, url), updateElements(getElementsForSubmit(response), args), imageButtonName);
        }

        public string SubmitExclusive(string[,] args)
        {
            return submitInputs(checkUrl(lastResponse, lastUrl), updateExclusiveElements(getElementsForSubmit(lastResponse), args), null);
        }

        public string SubmitExclusive(string[,] args, string imageButtonName)
        {
            return submitInputs(checkUrl(lastResponse, lastUrl), updateExclusiveElements(getElementsForSubmit(lastResponse), args, imageButtonName), imageButtonName);
        }

        public string SubmitExclusive(string url, string[,] args)
        {
            string response = scraper.HttpGet(url).AsString();
            return submitInputs(checkUrl(response, url), updateExclusiveElements(getElementsForSubmit(response), args), null);
        }

        public string SubmitExclusive(string url, string[,] args, string imageButtonName)
        {
            string response = scraper.HttpGet(url).AsString();
            return submitInputs(checkUrl(response, url), updateExclusiveElements(getElementsForSubmit(response), args), imageButtonName);
        }

        public string SubmitExclusive(Dictionary<string, string> args)
        {
            return submitInputs(checkUrl(lastResponse, lastUrl), updateExclusiveElements(getElementsForSubmit(lastResponse), args), null);
        }

        public string SubmitExclusive(Dictionary<string, string> args, string imageButtonName)
        {
            return submitInputs(checkUrl(lastResponse, lastUrl), updateExclusiveElements(getElementsForSubmit(lastResponse), args, imageButtonName), imageButtonName);
        }

        public string SubmitExclusive(string url, Dictionary<string, string> args)
        {
            string response = scraper.HttpGet(url).AsString();
            return submitInputs(checkUrl(response, url), updateExclusiveElements(getElementsForSubmit(response), args), null);
        }

        public string SubmitExclusive(string url, Dictionary<string, string> args, string imageButtonName)
        {
            string response = scraper.HttpGet(url).AsString();
            return submitInputs(checkUrl(response, url), updateExclusiveElements(getElementsForSubmit(response), args), imageButtonName);
        }

        public string Navigate(string url)
        {
            this.lastResponse = scraper.HttpPost(url, "").AsString();
            this.lastUrl = url;
            return lastResponse;
        }

        public IEnumerable<GenericElement> ReadElementLike(string tagType, string tagRegex)
        {
            return ElementParser.FindElementsLike(tagType, lastResponse, tagRegex);
        }

        public IEnumerable<GenericElement> ReadElementLike(string url, string tagType, string tagRegex)
        {
            this.lastResponse = scraper.HttpGet(url).AsString();
            this.lastUrl = url;

            IEnumerable<GenericElement> tags = ElementParser.FindElements(tagType, lastResponse);

            return tags.Where(x => Regex.IsMatch(x.Name, tagRegex));
        }

        private string submitInputs(string url, IEnumerable<GenericElement> inputs, string imageButtonName)
        {
            string postValue = string.Join("&", inputs.Where(x => (!x.Attributes.ContainsKey("type") || x.Attributes["type"] != "image") && (!x.Attributes.ContainsKey("disabled") || x.Attributes["disabled"] != "disabled")).Select(x => HttpUtility.UrlEncode(x.Name) + "=" + HttpUtility.UrlEncode(x.Attributes.ContainsKey("value") ? x.Attributes["value"] : "")).ToArray());

            if (!string.IsNullOrEmpty(imageButtonName))
            {
                if (inputs.Where(x => x.Attributes.ContainsKey("type") && x.Attributes["type"] == "image").Count() > 0 && inputs.Where(x => (!x.Attributes.ContainsKey("type") || x.Attributes["type"] != "image") && (!x.Attributes.ContainsKey("disabled") || x.Attributes["disabled"] != "disabled")).Count() > 0)
                    postValue += "&";
                postValue += string.Join("&", inputs.Where(x => x.Attributes.ContainsKey("type") && x.Attributes["type"] == "image" && Regex.IsMatch(x.Name, imageButtonName)).Select(x => HttpUtility.UrlEncode(x.Name) + ".x=1&" + HttpUtility.UrlEncode(x.Name) + ".y=1").ToArray());
            }
            var reponse = scraper.HttpPost(url, postValue);
            this.lastResponse = reponse.AsString();
            this.lastUrl = reponse.ResponseUri.AbsoluteUri;
            return lastResponse;
        }

        private string checkUrl(string response, string url)
        {
            while (response.Contains("<script"))
            {
                int index = response.IndexOf("<script");
                response = response.Remove(index, response.IndexOf("</script>", index) - index + 9);
            }
            while (response.Contains("<style"))
            {
                int index = response.IndexOf("<style");
                response = response.Remove(index, response.IndexOf("</style>", index) - index + 9);
            }
            GenericElement form = ElementParser.FindElements("form", response).First();

            if (form.Attributes.ContainsKey("action"))
                return url.Remove(url.LastIndexOf('/'), url.Length - url.LastIndexOf('/')) + "/" + form.Attributes["action"];
            return url;
        }

        private IEnumerable<GenericElement> getElementsForSubmit(string response)
        {
            while (response.Contains("<script"))
            {
                int index = response.IndexOf("<script");
                response = response.Remove(index, response.IndexOf("</script>", index) - index + 9);
            }
            while (response.Contains("<style"))
            {
                int index = response.IndexOf("<style");
                response = response.Remove(index, response.IndexOf("</style>", index) - index + 9);
            }
            List<GenericElement> inputs = ElementParser.FindElements("input", response).ToList();
            foreach (GenericElement select in ElementParser.FindElements("select", response))
            {
                IEnumerable<GenericElement> subOptions = ElementParser.FindElements("option", select.Text);
                if (subOptions.Count() > 0)
                {
                    GenericElement selectedOption;
                    if (subOptions.Where(x => x.Attributes.ContainsKey("selected") && x.Attributes["selected"] == "selected").Count() > 0)
                        selectedOption = subOptions.Where(x => x.Attributes.ContainsKey("selected") && x.Attributes["selected"] == "selected").First();
                    else
                        selectedOption = subOptions.First();
                    select.Attributes["value"] = selectedOption.Attributes.ContainsKey("value") ? selectedOption.Attributes["value"] : selectedOption.Text;
                }
                inputs.Add(select);
            }
            foreach (GenericElement textArea in ElementParser.FindElements("textarea", response))
            {
                if (!textArea.Attributes.ContainsKey("value"))
                    textArea.Attributes["value"] = textArea.Text;
                inputs.Add(textArea);
            }
            return inputs;
        }

        private IEnumerable<GenericElement> updateElements(IEnumerable<GenericElement> inputs, string[,] args)
        {
            for (int i = 0; i < args.Length / 2; i++)
            {
                inputs.Where(x => Regex.IsMatch(x.Name, args[i, 0])).First().Attributes["value"] = args[i, 1];
            }
            return inputs;
        }

        private IEnumerable<GenericElement> updateElements(IEnumerable<GenericElement> inputs, Dictionary<string, string> args)
        {
            foreach (KeyValuePair<string, string> pair in args)
            {
                inputs.Where(x => Regex.IsMatch(x.Name, pair.Key)).First().Attributes["value"] = pair.Value;
            }
            return inputs;
        }

        private IEnumerable<GenericElement> updateExclusiveElements(IEnumerable<GenericElement> inputs, string[,] args)
        {
            var viewState = inputs.Where(x => Regex.IsMatch(x.Name, ".*?viewstate", RegexOptions.IgnoreCase)).First();
            List<GenericElement> usableInputs = new List<GenericElement>();
            usableInputs.Add(viewState);
            for (int i = 0; i < args.Length / 2; i++)
            {
                var input = inputs.Where(x => Regex.IsMatch(x.Name, args[i, 0])).First();
                input.Attributes["value"] = args[i, 1];
                usableInputs.Add(input);
            }
            return usableInputs;
        }

        private IEnumerable<GenericElement> updateExclusiveElements(IEnumerable<GenericElement> inputs, string[,] args, string imageButtonName)
        {
            List<GenericElement> output = updateExclusiveElements(inputs, args).ToList();
            output.Add(inputs.FirstOrDefault(x => x.Attributes.ContainsKey("type") && x.Attributes["type"] == "image" && Regex.IsMatch(x.Name, imageButtonName)));
            return output;
        }

        private IEnumerable<GenericElement> updateExclusiveElements(IEnumerable<GenericElement> inputs, Dictionary<string, string> args)
        {
            foreach (KeyValuePair<string, string> pair in args)
            {
                inputs.Where(x => Regex.IsMatch(x.Name, pair.Key)).First().Attributes["value"] = pair.Value;
            }
            var viewState = inputs.Where(x => Regex.IsMatch(x.Name, ".*?viewstate", RegexOptions.IgnoreCase)).First();
            List<GenericElement> usableInputs = new List<GenericElement>();
            usableInputs.Add(viewState);
            foreach (KeyValuePair<string, string> pair in args)
            {
                var input = inputs.Where(x => Regex.IsMatch(x.Name, pair.Key)).First();
                input.Attributes["value"] = pair.Value;
                usableInputs.Add(input);
            }
            return usableInputs;
        }

        private IEnumerable<GenericElement> updateExclusiveElements(IEnumerable<GenericElement> inputs, Dictionary<string, string> args, string imageButtonName)
        {
            List<GenericElement> output = updateExclusiveElements(inputs, args).ToList();
            output.Add(inputs.FirstOrDefault(x => x.Attributes.ContainsKey("type") && x.Attributes["type"] == "image" && Regex.IsMatch(x.Name, imageButtonName)));
            return output;
        }
    }
}
