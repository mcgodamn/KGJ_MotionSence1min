using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTeller : MonoBehaviour
{
    //line
    [SerializeField]
    GameObject phaseContainer;

    [SerializeField]
    GameObject phaseR, phaseL;
    //

    public enum WHO
    {
        ME = 1,
        HIM = 2
    }

    public static StoryTeller instance;
    //Phases
    List<GameObject> phases;
    float posRx = 102;
    float posLx = -693;
    float start_y = 369;
    float step = 80;
    float maxY = -276;

    float nowY = 369;
    //

    void Awake()
    {
        instance = this;
        phases = new List<GameObject>();

        // StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        while(true)
        {
            AddPhase(WHO.HIM,"123");
            AddPhase(WHO.ME, "123");
            yield return new WaitForSeconds(1);
        }
    }

    public void AddPhase(WHO who,string _s)
    {

        if (nowY < maxY)
        {
            var gb = phases[0];
            phases.RemoveAt(0);
            Destroy(gb);

            foreach(var phase in phases)
            {
                var rt = phase.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x,rt.anchoredPosition.y + step);
            }
            nowY += step;
        }

        var p = Instantiate(
            ((who == WHO.ME)?phaseR:phaseL),
            Vector2.zero,
            Quaternion.identity,
            phaseContainer.transform
        );
        p.GetComponent<RectTransform>().anchoredPosition = new Vector2(((who == WHO.ME) ? posRx : posLx), nowY);
        nowY -= step;

        p.GetComponent<WordSetter>().SetText(_s);
        phases.Add(p);
    }
}
