using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

/// Airship screen. Loads weather.json from the GitHub Pages feed (github.io is a VRChat trusted domain,
/// so no "Allow Untrusted URLs" needed). Shows a static Academy City advert until/unless the feed loads.
/// Set feedUrl in the inspector (VRCUrl must be a literal, it cannot be built at runtime).
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class AirshipWeather : UdonSharpBehaviour
{
    public VRCUrl feedUrl;
    public TMP_Text label;
    public float refreshSeconds = 600f;
    [TextArea] public string fallbackText = "ACADEMY CITY\nTOKYO WEST  -  CLEAR SKIES";

    void Start()
    {
        if (label != null) label.text = fallbackText;
        Load();
    }

    public void Load()
    {
        if (feedUrl != null && !string.IsNullOrEmpty(feedUrl.Get()))
            VRCStringDownloader.LoadUrl(feedUrl, (IUdonEventReceiver)this);
        SendCustomEventDelayedSeconds(nameof(Load), refreshSeconds); // one request per 5 s is the SDK limit; 10 min is plenty
    }

    public override void OnStringLoadSuccess(IVRCStringDownload r)
    {
        DataToken t;
        if (!VRCJson.TryDeserializeFromJson(r.Result, out t) || t.TokenType != TokenType.DataDictionary) return;
        DataDictionary d = t.DataDictionary;
        DataToken city, temp, hi, lo, desc, wind;
        if (!d.TryGetValue("city", out city) || !d.TryGetValue("temp_f", out temp) ||
            !d.TryGetValue("desc", out desc)) return;
        d.TryGetValue("hi_f", out hi); d.TryGetValue("lo_f", out lo); d.TryGetValue("wind_mph", out wind);
        if (label != null)
            label.text = city.String.ToUpper() + "   " + Mathf.RoundToInt((float)temp.Double) + "°F\n"
                       + desc.String.ToUpper() + "   H " + Mathf.RoundToInt((float)hi.Double)
                       + "  L " + Mathf.RoundToInt((float)lo.Double);
    }

    public override void OnStringLoadError(IVRCStringDownload r)
    {
        if (label != null) label.text = fallbackText; // keep the advert on any failure
    }
}
