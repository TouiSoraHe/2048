namespace CrossPlatformJson
{
    public class JavaScriptObjectFactory
    {
        private static IJsonString2JavaScriptObjectHandle jsonParse = null;

        static JavaScriptObjectFactory()
        {
            jsonParse = new JavaScriptObjectWithJsonParse();
        }

        public static JavaScriptObject CreateJavaScriptObject(string json)
        {
            return jsonParse.ToJavaScriptObject(json);
        }
    }

}