using UnityEngine;
using UnityEngine.UI;

public class RunJavaScriptInWebSceneManager : MonoBehaviour {
    public Text result;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8

    private UniWebView _webView;
    private string _fileName = "Cerebro/youtube.html";

    public void LoadFromFile() {
        if (_webView != null) {
            return;
        }
        
        _webView = CreateWebView();
        _webView.url = UniWebViewHelper.streamingAssetURLForPath(_fileName);

        // Set the height of webview half of the screen height.
        int bottomInset = UniWebViewHelper.screenHeight;
        _webView.insets = new UniWebViewEdgeInsets(0, 0, 0, 0);

        // `OnEvalJavaScriptFinished` will be called after you invoking some JavaScript function.
        _webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
		_webView.OnLoadComplete += _webView_OnLoadComplete;

		_webView.OnReceivedMessage += OnReceivedMessage;

        _webView.OnWebViewShouldClose += (webView) => {
            _webView = null;
            return true;
        };

        _webView.Load();
        _webView.Show();


    }

    void _webView_OnLoadComplete (UniWebView webView, bool success, string errorMessage)
    {
		print ("LOAD COMPLETE");
		_webView.EvaluatingJavaScript("loadVideo('NybHckSEQBI')");
    }

	void OnReceivedMessage(UniWebView webView, UniWebViewMessage message) {

		if (message.path == "close") {
			result.text = "";
			Destroy(webView);
			_webView = null;
		}

		if (message.path == "ytEvent") {
			string state = message.args ["state"];
			print ("Player returned " + state);
		}
	}

    void OnEvalJavaScriptFinished(UniWebView webView, string r) {
		print ("Javascript eval finished with " + r);
    }
    
    UniWebView CreateWebView() {
        var webViewGameObject = GameObject.Find("WebView");
        if (webViewGameObject == null) {
            webViewGameObject = new GameObject("WebView");
        }
        
        var webView = webViewGameObject.AddComponent<UniWebView>();
        
        webView.toolBarShow = true;
        return webView;
    }
#else
    void Start() {
        Debug.LogWarning("UniWebView only works on iOS/Android/WP8. Please switch to these platforms in Build Settings.");
    }
#endif
}
