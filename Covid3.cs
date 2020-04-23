using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Covid3 : MonoBehaviour
{
    float cntInfected;
    float cntInfectedNew;
    float cntInfected2;
    float cntInfectedNew2;
    float cntNotInfected;
    float cntImmune;
    float cntPopulation;
    float day;
    float dayDelta;
    float rFactor;
    float rFactor2;
    float interval;
    float graphWidth;
    float intervalCycle;
    float graphHeight;
    float cntInfectedNewMax;
    float cntInfectedNewMax2;
    GameObject goNotInfected;
    GameObject goInfected;
    GameObject goInfectedNew;
    GameObject goImmune;
    GameObject goGraphs;
    GameObject goGraphBackground;
    GameObject goGraphInfected;
    GameObject goGraphInfectedNew;
    GameObject goGraphNotInfected;
    GameObject goGraphImmune;
    TextMesh tmNotInfected;
    TextMesh tmInfected;
    TextMesh tmInfectedNew;
    TextMesh tmImmune;
    TextMesh tmDay;
    TextMesh tmRfactor;
    TextMesh tmRfactor2;
    TextMesh tmPopulation;
    Vector3 posInfected;
    Vector3 posInfectedNew;
    Vector3 posNotInfected;
    Vector3 posImmune;
    Vector3 posInfectedLast;
    Vector3 posInfectedNewLast;
    Vector3 posNotInfectedLast;
    Vector3 posImmuneLast;

    // Start is called before the first frame update
    void Start()
    {
        day = 0;
        dayDelta = 1f;
        interval = .15f;
        cntPopulation = 328000000; // 728000; // 328000000;
        graphWidth = 50;
        intervalCycle = 3;
        rFactor = 1.53f; // 2.63f; 
        rFactor2 = 1.9f; // 2.63f; 
        cntInfected = 1;
        cntInfected2 = 1;
        cntNotInfected = cntPopulation;
        cntImmune = 0;
        cntInfectedNewMax = 0;
        cntInfectedNewMax2 = 0;
        //
        goNotInfected = GameObject.Find("NotInfected");
        goInfected = GameObject.Find("Infected");
        goInfectedNew = GameObject.Find("InfectedNew");
        goImmune = GameObject.Find("Immune");
        //
        goGraphInfected = GameObject.Find("GraphInfected");
        goGraphInfectedNew = GameObject.Find("GraphInfectedNew");
        goGraphNotInfected = GameObject.Find("GraphNotInfected");
        goGraphImmune = GameObject.Find("GraphImmune");
        goGraphs = GameObject.Find("Graphs");
        goGraphBackground = GameObject.Find("GraphBackground");
        //
        tmNotInfected = GameObject.Find("LabNotInfected").GetComponent<TextMesh>();
        tmInfected = GameObject.Find("LabInfected").GetComponent<TextMesh>();
        tmInfectedNew = GameObject.Find("LabInfectedNew").GetComponent<TextMesh>();
        tmImmune = GameObject.Find("LabImmune").GetComponent<TextMesh>();
        tmDay = GameObject.Find("LabDay").GetComponent<TextMesh>();
        tmRfactor = GameObject.Find("LabRfactor").GetComponent<TextMesh>();
        tmRfactor2 = GameObject.Find("LabRfactor2").GetComponent<TextMesh>();
        tmPopulation = GameObject.Find("LabPopulation").GetComponent<TextMesh>();
        //
        tmPopulation.text = "population " + cntPopulation.ToString("N0");
        graphHeight = goGraphBackground.transform.localScale.z;
        //
        Regen();
    }

    void Regen()
    {
        tmRfactor.text = "Rfactor " + (rFactor + 1).ToString("F2");
        tmRfactor2.text = "Rfactor2 " + (rFactor2 + 1).ToString("F2");
        //
        float cntInfectedLast = cntInfected;
        cntInfected += (rFactor - 1) * (1 - cntInfected / cntPopulation) * cntInfected * dayDelta;
        cntInfectedNew = cntInfected - cntInfectedLast;
        if(cntInfectedNew > cntInfectedNewMax)
        {
            cntInfectedNewMax = cntInfectedNew;
        }
        //
        float cntInfectedLast2 = cntInfected2;
        cntInfected2 += (rFactor2 - 1) * (1 - cntInfected2 / cntPopulation) * cntInfected2 * dayDelta;
        cntInfectedNew2 = cntInfected2 - cntInfectedLast2;
        if (cntInfectedNew2 > cntInfectedNewMax2)
        {
            cntInfectedNewMax2 = cntInfectedNew2;
        }
        //
        cntNotInfected = cntPopulation - cntInfected;
        cntImmune = cntInfected;
        //
        AdjustBar(goInfected, cntInfected);
        AdjustBar(goInfectedNew, cntInfectedNew);
        AdjustBar(goImmune, cntImmune);
        AdjustBar(goNotInfected, cntNotInfected);
        //
        tmNotInfected.text = cntNotInfected.ToString("N0") + " Not Infected";
        tmInfected.text = cntInfected.ToString("N0") + " Infected";
        tmInfectedNew.text = cntInfectedNew.ToString("N0") + " Infected New";
        tmImmune.text = cntImmune.ToString("N0") + " Immune";
        tmDay.text = "Day " + day.ToString("F0");
        GameObject go;
        //
        go = Instantiate(goGraphInfected, goGraphs.transform);
        posInfected = getGraphPos(cntInfectedNew2);
        AdjustGraphLine(go, posInfectedLast, posInfected);
        //
        go = Instantiate(goGraphInfectedNew, goGraphs.transform);
        posInfectedNew = getGraphPos(cntInfectedNew);
        AdjustGraphLine(go, posInfectedNewLast, posInfectedNew);
        //
        go = Instantiate(goGraphNotInfected, goGraphs.transform);
        posNotInfected = getGraphPos(cntNotInfected);
        AdjustGraphLine(go, posNotInfectedLast, posNotInfected);
        //
        go = Instantiate(goGraphImmune, goGraphs.transform);
        posImmune = getGraphPos(cntImmune);
        AdjustGraphLine(go, posImmuneLast, posImmune);
        //
        posNotInfectedLast = posNotInfected;
        posInfectedLast = posInfected;
        posInfectedNewLast = posInfectedNew;
        posImmuneLast = posImmune;
        //
        if (Mathf.RoundToInt(cntInfectedNew) == 0)
        {
            Debug.Log("cntInfectedNewMax " + cntInfectedNewMax.ToString("N0") + "\n");
            Debug.Log("percent " + (cntInfectedNewMax / cntPopulation * 100).ToString("F1") + "\n");
            Debug.Log("cntInfectedNewMax2 " + cntInfectedNewMax2.ToString("N0") + "\n");
            Debug.Log("percent2 " + (cntInfectedNewMax2 / cntPopulation * 100).ToString("F1") + "\n");
            Invoke("Restart", intervalCycle);
            return;
        }
        day += dayDelta;
        Invoke("Regen", interval);
    }

    Vector3 getGraphPos(float cnt)
    {
        Vector3 pos = new Vector3(day, 1, -graphHeight + cnt / cntPopulation * graphHeight);
        return pos;
    }

    void AdjustBar(GameObject go, float cnt)
    {
        Vector3 pos = go.transform.position;
        Vector3 sca = go.transform.localScale;
        sca.x = cnt / cntPopulation * graphWidth;
        if (sca.x < .1f) sca.x = .1f;
        pos.x = sca.x / 2;
        go.transform.position = pos;
        go.transform.localScale = sca;
    }

    void AdjustGraphLine(GameObject go, Vector3 posLast, Vector3 pos)
    {
        if (day == 0)
        {
            go.transform.position = pos;
            go.transform.localScale = new Vector3(.5f, 1, .5f);
            return;
        }
        go.transform.position = posLast;
        go.transform.LookAt(pos);
        go.transform.position = (pos + posLast) / 2;
        go.transform.localScale = new Vector3(.5f, 1, Vector3.Distance(pos, posLast));
    }

    private void Restart()
    {
        Destroy(goGraphs);
        goGraphs = new GameObject("Graphs");
        Invoke("Start", interval);
    }
}
