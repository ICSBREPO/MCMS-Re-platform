﻿namespace McmsApp.Helpers
{
    public class RestApi
    {
        public static string Hostname = "";
        public static string MaximoUrl = "";

        public static string MaximoUrlGetter()
        {
            MaximoUrlSetter();
            return MaximoUrl;
        }

        public static async void MaximoUrlSetter()
        {
            //_ = SecureStorage.SetAsync("Hostname", "http://10.215.73.60/");
            var checkHostname = SecureStorage.GetAsync("Hostname").Result;
            Hostname = await SecureStorage.GetAsync("Hostname");
            if (checkHostname != null)
            {
                Hostname = await SecureStorage.GetAsync("Hostname");
                //MaximoUrl = Hostname + "/maxrest/oslc";
                MaximoUrl = Hostname + "/maximo/oslc";
            }
            else
            {
                _ = SecureStorage.SetAsync("Hostname", "http://10.215.73.60");
                Hostname = await SecureStorage.GetAsync("Hostname");
                //MaximoUrl = Hostname + "/maxrest/oslc";
                MaximoUrl = Hostname + "/maximo/oslc";
            }
        }

        public static string HostnameUrlGetter()
        {
            HostnameUrlSetter();
            return Hostname;
        }

        public static async void HostnameUrlSetter()
        {
            var checkHostname = SecureStorage.GetAsync("Hostname").Result;
            Hostname = await SecureStorage.GetAsync("Hostname");
            if (checkHostname != null)
            {
                Hostname = await SecureStorage.GetAsync("Hostname");
            }
            else
            {
                _ = SecureStorage.SetAsync("Hostname", "http://10.215.73.60");
                Hostname = await SecureStorage.GetAsync("Hostname");
            }
        }
    }

    public static class GoogleMapsApikey
    {
        public const string ApiKey = "AIzaSyBhbuN3jBWhheOY5iXYy54Ovu2C8Ul8-sw";
    }

    public static class Base64Files
    {
        public const string pdfimage = "iVBORw0KGgoAAAANSUhEUgAAAgAAAAIACAYAAAD0eNT6AAAABHNCSVQICAgIfAhkiAAAAAlwSFlzAAAN1wAADdcBQiibeAAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAB90SURBVHja7d0JlCR1fcDxX8997Mmy7MXCuotcEqIIymGEsF4RIooIiiheSDwSkyiYYBTxERUx8cCHCkESDCAqBpCIF4gHeCwgcoPcuyyLBPaeq69U9Y7GwAI72z0z1V2fz0ul8flsZn7dU/9vV3dXFarVatC6lq9c1Z/cHJxsS5Jt/ma2qaYEk+O+B1b8+MX77X2QSTAZCgKgJRf92cnNq5Lt1cn2kmTrMRXInhtuui1mTp8mAhAA1LXodyU3b0+2o5Nt/2RrMxXIfgCkRACTwSLR/At/IdnekPzj7cl2ZrK9yOMKzWX12nUH/uTn111tEggAtnTxT9/bX5ZsFyTbYhMBEQACoLUX/rnJ9p3kH69MtuebCIgAEACtv/jvNfqq/y9MA0QACIB8LP6vS25+mmzbmwaIABAArb/wpx/0+2jyjxclW5+JgAgAAZCDxT+5+Y9kOznZCiYCIgAEQD58JNneZAwgAkAA5OfV/xGjr/wBESACEAA5WfzTT/unh/4d9gdEAAIgJ4v/3OTm0vCBP0AEIABy5ezwVT9ABCAAcvXq/6Dk5lCTAEQAAiA/i3/6fv+nTAIQAQiAfDky2fYxBkAEIADy8+q/K7n5uEkAIgABkC9vD5f0BUQAAiB3jjYCQAQgAHJk+cpVs5Ob/U0CEAEIgHx5lccCEAEIgPx5tREAIgABkCPLV67qT25eYhKACEAA5MvBydZjDIAIQADkyxIjAEQAAiB/5hsBIAIQAAIAQAQgAAQAgAhAAAgAABGAABAAACIAAdCcphoBIAIQAACIAAQAACIAAQCACEAAACACEAAAiAAEAAAiAAEAgAhAAAAgAgQAAIgAAQAAIkAAACACEAAAiAAEAAAiAAEAgAhAAAAgAhAAAIgABAAAIgABAIAIQAAAIAIQAACIAAQAACIAAQCACEAAACACEAAAiAAEAAAiQAAAgAgQAAAgAgQAAIgAAQAAIkAAAIAIEAAAIAIEAACIAAEAACJAAAAgAkSAAABABCAAABABCAAARAACAAARgAAAQAQgAAAQAQgAAEQAAgAAESAAAEAECAAAEAECAABEgAAAABEgAABABAgAABABAgAARIAAAAARIAAAQAQIAAAQAQIAAESAAAAAESAAAEAECAAAEAECAABEgAAAQAQgAAAQAQgAAESAAAAAESAAAEAECAAAEAECAIB6tLfZBYsAAQCQO52dHYYgAgQAQP4CoNMQRIAAAMhdAHQ4AiACBABADo8ACAARIAAAchgA3gIQAQIAIHe6uwSACBAAALkzdUp/tBUKBiECBABAnrS1tcXUqVMMQgQIAIC8mTFtqiGIAAEAkDfTBYAIEAAA+dPR0R5T+vsMQgQIAIC8mTljuiGIAAEAkDfbbjMjuru6DEIECACAPCkUCjF/7nYGIQIEAEDezJwxLfr7eg1CBAgAgLxZMG+OIYgAAQCQN+m3AZwXQAQIAIAc2mH7+dHlGgEiQAAA5Et6XoAlixbWThOMCBAAADnS29MTixbONwgRIAAA8mbG9Gkxb85sgxABAgAgb9IA2GamswSKAAEAkDuLFi5wJEAECACAvB4JWLzj9j4YKAIEAEDepJ8J2HnJoujs9BVBESAAAHKlr7cndn32s2K6kwWJgAYoVKtVj9AkWr5ylQcAGLP1GzbGQ6t+FwMDg4bRBGZOn/bjF++390ECAAEANOYV5pp1sTIJgeGREcMQAQJAAAB5ku7H/+fxNUkMrI0NGwcMRAQIAAEA5E2pVI6169bHmmRbv35DVOzjRYAAEABAvlQqldpnBYZHilEsplspiqXSH/65nPz35DcCOjwMAK0pPW/A031jYMPAULING9TkOHDSnx8eA4B8mtLXk2zdBpHXQDQCABGAAABABCAAABABCAAARAACAAARgAAAQAQgAAAQAQgAAEQAAgAAEYAAAEAEIAAAEAEIAABEAAIAABGAAABABCAAABABCAAARAACAAARgAAAQAQIAAAQAQIAAESAAAAAESAAAEAECAAAEAECAAARIAIEAAAiAAEAgAhAAAAgAhAAAIgABAAAIgABAIAIQAAAIAIQAACIAAQAACIAAQCACEAAACACBAAAiAABAAAiQAAAgAgQAAAgAgQAAIgAAQAAIkAAAIAIEAAAIAIEAACIAAEAACJAAACACBAAACACBAAAIgABAIAIQAAAIAIQAACIAAQAACJAAACACBAAACACBAAAiAABAAAiQAAAgAgQAAAgAgQAAIgAAQAAIkAA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";
        public const string downloadimage = "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";
        public static string GetFileExtension(string base64String)
        {
            if (base64String == null)
            {
                base64String = downloadimage;
            }
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }
    }
}
