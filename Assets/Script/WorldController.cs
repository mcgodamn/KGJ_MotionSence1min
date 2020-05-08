using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;
using UnityEngine.Networking;

static class ListExtension
{
    public static T PopAt<T>(this List<T> list, int index)
    {
        T r = list[index];
        list.RemoveAt(index);
        return r;
    }
}

public class WorldController : MonoBehaviour
{
    // bg
    [SerializeField]
    GameObject no_sound, no_net;

    //yt
    [SerializeField]
    GameObject yt_bg;
    [SerializeField]
    VideoPlayer videoPlayer;

    //line
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip phaseClip, ringClip;

    [SerializeField]
    GameObject line_bg, not_stable1, notstable2,phoneRing;
    [SerializeField]
    Button b1, b2;
    [SerializeField]
    Text b1T,b2T;

    [SerializeField]
    string start;

    [SerializeField]
    Image interval_img;
    [SerializeField]
    Text intervalText;

    [SerializeField]
    Text DateTimeText;

    List<string> AllPlay;
    void Awake()
    {
        AllPlay = new List<string>();
    }

    bool playing = false;

    // Start is called before the first frame update
    void Start()
    {
        DateTimeText.text = 
            System.DateTime.Now.ToShortTimeString() + "\n" + System.DateTime.Now.ToShortDateString();
        InqueueFile(start);
        StartCoroutine(PlayStart());
    }

    void inqueue(string _play)
    {
        char[] splitChar = { ' ', '\n', (char)13};
        List<string> play = _play.Split(splitChar).ToList();

        AllPlay.AddRange(play);
    }

    bool countingStart = false;
    float countingNow = 0;
    // Update is called once per frame
    void Update()
    {
        if (countingStart)
        {
            countingNow += Time.deltaTime;
        }
    }

    IEnumerator PlayStart()
    {
        playing = true;
        while (playing)
        {
            if (AllPlay.Any())
            {
                yield return handleCommand(AllPlay.PopAt(0));
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator handleCommand(string command)
    {
        if (command == "") yield break;

        switch (command)
        {
            case "startcounting":
                StartCounting();
                break;
            case "interval":
                yield return interval(AllPlay.PopAt(0));
                break;
            case "wait":
                yield return wait(AllPlay.PopAt(0));
                break;
            case "end":
                yield return ToggleTextFade(1);
                end();
                break;
            case "close":
                yield return close(AllPlay.PopAt(0));
                break;
            case "show":
                yield return show(AllPlay.PopAt(0));
                break;
            case "play":
                yield return play(AllPlay.PopAt(0));
                break;
            case "pause":
                yield return pause(AllPlay.PopAt(0));
                break;
            case "phase":
                yield return phase(AllPlay.PopAt(0), AllPlay.PopAt(0));
                break;
            case "option":
                var n = int.Parse(AllPlay.PopAt(0));
                List<(string, string)> map = new List<(string, string)>();
                for (int i = 0; i < n; i++)
                {
                    map.Add((AllPlay.PopAt(0), AllPlay.PopAt(0)));
                }
                option(map);
                break;
            case "random":
                var r = int.Parse(AllPlay.PopAt(0));
                Dictionary<int, string> rmap = new Dictionary<int, string>();
                var chance = 0;
                for (int i = 0; i < r; i++)
                {
                    chance += int.Parse(AllPlay.PopAt(0));
                    rmap.Add(chance, AllPlay.PopAt(0));
                }
                random(rmap);
                break;
        }
        yield break;
    }

    IEnumerator interval(string s)
    {
        yield return ToggleInterval(true, s);
    }


    IEnumerator ToggleTextFade(float time = 0.5f)
    {
        if (intervalText.color.a >= 1)
            time = -time;
        float timer = Mathf.Abs(time);
        while (timer > 0)
        {
            intervalText.color = new Color(
                intervalText.color.r,
                intervalText.color.g,
                intervalText.color.b,
                intervalText.color.a + (1 / time) * Time.deltaTime
            );
            timer -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ToggleFade(float time = 2)
    {
        if (interval_img.color.a >= 1)
            time = -time;
        float timer = Mathf.Abs(time);
        while (timer > 0)
        {
            interval_img.color = new Color(
                interval_img.color.r,
                interval_img.color.g,
                interval_img.color.b,
                interval_img.color.a + (1 / time) * Time.deltaTime
            );
            timer -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator ToggleInterval(bool b, string s ="")
    {
        if (b)
        {
            if (interval_img.gameObject.activeSelf == false)
            {
                interval_img.gameObject.SetActive(true);
                yield return ToggleFade();
                yield return ToggleInterval(b, s);
            }
            else
            {
                var c = intervalText.color;
                c.a = 0;
                intervalText.color = c;
                intervalText.text = s;
                yield return ToggleTextFade();
            }
        }
        else
        {
            yield return ToggleTextFade();
            intervalText.text = "";
            yield return ToggleFade();
            interval_img.gameObject.SetActive(false);
        }
        
        yield return null;
    }

    [SerializeField]
    float DIA_STEP = 0.3f;
    bool skipDia = false;
    IEnumerator wait(string s)
    {
        if (s == "DIA_STEP")
        {
            float t = DIA_STEP;
            while(t > 0)
            {
                if (skipDia)
                {
                    skipDia = false;
                    yield break;
                }
                yield return null;
                t -= Time.deltaTime;
            }
        }
        else
        {
            yield return new WaitForSeconds(float.Parse(s));
        }
    }

    void end()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("end");
    }

    IEnumerator close(string s)
    {
        yield return toggle(s, false);
    }

    IEnumerator show(string s)
    {
        yield return toggle(s, true);
    }

    IEnumerator toggle(string s, bool b)
    {
        switch (s)
        {
            case "all":
                yield return toggle("yt_bg", b);
                yield return toggle("line_bg", b);
                yield return toggle("interval", b);
                break;
            case "yt_bg":
                if (b)
                {
                    yield return close("line_bg");
                    videoPlayer.Play();
                }
                else
                {
                    videoPlayer.Pause();
                }

                yt_bg.SetActive(b);
                break;
            case "line_bg":

                if (b)
                {
                    videoPlayer.Pause();
                }
                else
                {
                    if (yt_bg.activeSelf)
                        videoPlayer.Play();
                }
                line_bg.SetActive(b);
                break;
            case "interval":
                yield return ToggleInterval(b);
                break;
            case "ring":
                phoneRing.SetActive(b);
                break;
        }
    }

    IEnumerator play(string s, System.Action afterPlay = null)
    {
        if (s == "video")
            videoPlayer.Play();
        else if (s == "phase")
        {
            audioSource.PlayOneShot(phaseClip);
        }
        else if (s == "ring")
        {
            yield return show("ring");
            audioSource.clip = ringClip;
            audioSource.loop = true;
            audioSource.Play();
            inqueue("wait 6");
        }
        afterPlay?.Invoke();
    }
    IEnumerator pause(string s)
    {
        if (s == "all")
        {
            yield return pause("video");
            yield return pause("phase");
            yield return pause("ring");
        }
        if (s == "video")
        {
            videoPlayer.Pause();
        }
        else if (s == "phase" || s == "ring")
        {
            // yield return close("ring");
            audioSource.Pause();
        }
    }
    public bool netEnable = true;
    IEnumerator phase(string _who, string s)
    {
        if (netEnable)
        {
            not_stable1.SetActive(false);
        }
        else
        {
            not_stable1.SetActive(true);
            yield break;
        }

        yield return show("line_bg");

        var who = (StoryTeller.WHO)(int.Parse(_who));

        if (who == StoryTeller.WHO.HIM && line_bg.activeSelf)
        {
            yield return play("phase");
        }

        StoryTeller.instance.AddPhase(who,s);
    }

    void InqueueFile(string _path)
    {
        var path = Application.streamingAssetsPath + "/" + _path + ".txt";
        inqueue(System.IO.File.ReadAllText(path));
    }

    void option(List<(string, string)> options)
    {

        IEnumerator dontBother()
        {
            yield return new WaitForSeconds(30);
            InqueueFile("dontbother/dontbother");
        }

        var c = StartCoroutine(dontBother());

        if (netEnable)
        {
            not_stable1.SetActive(false);
        }
        else
        {
            not_stable1.SetActive(true);
            return;
        }

        void DeactiveAllB()
        {
            b1.gameObject.SetActive(false);
            b2.gameObject.SetActive(false);
        }

        void SetButton(Button b, Text bt, (string, string) inp)
        {
            b.gameObject.SetActive(true);
            b.onClick.RemoveAllListeners();
            bt.text = inp.Item1;
            b.onClick.AddListener(()=>{
                StopCoroutine(c);
                inqueue("phase 1 " + inp.Item1);
                InqueueFile(inp.Item2);
                DeactiveAllB();
            });
        }

        SetButton(b1,b1T,options[0]);
        if (options.Count > 1)
            SetButton(b2, b2T, options[1]);
    }

    void random(Dictionary<int, string> randoms)
    {
        var r = Random.Range(1,100);
        Debug.Log(r);
        foreach(var kv in randoms)
        {
            if (kv.Key > r)
            {
                InqueueFile(kv.Value);
                break;
            }
        }
    }

    void StartCounting()
    {
        countingNow = 0;
        countingStart = true;
    }

    public void onClikRing()
    {
        void handleResult()
        {
            if (countingStart == false)
            {
                InqueueFile("notalkyet/notalkyet");
                return;
            }
            countingStart = false;
            int count = (int)Mathf.Floor(countingNow);

            if (count < 5)
            {
                InqueueFile("extreamlyshort/extreamlyshort");
            }
            else if (count < 50)
            {
                InqueueFile("short/short");
            }
            else if (count < 60)
            {
                InqueueFile("fit_short/fit_short");
            }
            else if (count == 60)
            {
                InqueueFile("perfect/perfect");
            }
            else if (count < 70)
            {
                InqueueFile("fit_long/fit_long");
            }
            else if (count < 1800)
            {
                InqueueFile("long/long");
            }
            else
            {
                InqueueFile("extreamlylong/extreamlylong");
            }
        }

        if (!netEnable)
        {
            notstable2.SetActive(true);
            return;
        }
        else
        {
            notstable2.SetActive(false);
        }

        AllPlay.Clear();
        StartCoroutine(play("ring", handleResult));
    }

    public void onClickToggleLine()
    {
        if (line_bg.activeSelf)
        {
            StartCoroutine(close("line_bg"));
        }
        else
        {
            StartCoroutine(show("line_bg"));
        }
    }

    public void onClickToggleYT()
    {
        if (yt_bg.activeSelf)
        {
            StartCoroutine(close("yt_bg"));
        }
        else
        {
            StartCoroutine(show("yt_bg"));
        }
    }

    public void onClickToggleNet()
    {
        netEnable = !netEnable;
        no_net.SetActive(!netEnable);
        if (netEnable)
        {
            not_stable1.SetActive(false);
            notstable2.SetActive(false);
        }
    }

    float sound;
    public void onClickToggleSound()
    {
        var soundEnable = audioSource.volume != 0;

        no_sound.SetActive(soundEnable);
        if (soundEnable)
        {
            sound = audioSource.volume;
            audioSource.volume = 0;
            videoPlayer.SetDirectAudioVolume(0,0);
        }
        else
        {
            audioSource.volume = sound;
            videoPlayer.SetDirectAudioVolume(0, sound);
        }
    }

    public void onClickYTBG()
    {
        if (yt_bg.activeSelf) StartCoroutine(show("yt_bg"));
    }

    public void onClickLineBG()
    {
        skipDia = true;
    }
}
