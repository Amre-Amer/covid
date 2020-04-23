using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Person
{
    public GameObject go;
    public int dayInfected;
    public bool ynInfected;
    public bool ynImmune;
    public int othersInfected;
    public Person(GameObject go0)
    {
        go = go0;
    }
}

public class Covid2 : MonoBehaviour
{
    int numPersons;
    float side;
    float scalePerson;
    float speed;
    float nearDist;
    int daysInfected;
    Color colorInfected;
    Color colorImmune;
    Color colorNotInfected;
    List<Person> persons;
    GameObject goPersons;
    GameObject goPersonPrefab;
    GameObject goBorders;
    GameObject goBorderPrefab;
    GameObject goGraphs;
    GameObject goGraphPrefab;
    GameObject goInfo;
    GameObject goGraphNotInfectedLast;
    GameObject goGraphInfectedLast;
    GameObject goGraphImmuneLast;
    int day;
    int cntInfected;
    int cntImmune;
    int cntNotInfected;
    int cntMaxInfected;
    bool ynDone;
    int numOthersInfected;
    int numOthersInfectedDenominator;
    int dayLockDownEnds;
    float lockDownIncrease;
    int cycle;

    // Start is called before the first frame update
    void Start()
    {
        numPersons = 500;
        side = 200;
        scalePerson = 3;
        speed = 2.25f;
        nearDist = scalePerson;
        daysInfected = 21;
        dayLockDownEnds = 90;
        lockDownIncrease = 1.5f;
        //
        persons = new List<Person>();
        colorNotInfected = Color.white;
        colorInfected = Color.red;
        colorImmune = Color.green;
        goPersons = GameObject.Find("Persons");
        goPersonPrefab = GameObject.Find("Person");
        goBorders = GameObject.Find("Borders");
        goBorderPrefab = GameObject.Find("Border");
        goGraphs = GameObject.Find("Graphs");
        goGraphPrefab = GameObject.Find("Graph");
        goInfo = GameObject.Find("Info");
        LoadBorders();
        LoadPersons();
        Infect();
    }

    void Infect()
    {
        int n = UnityEngine.Random.Range(0, numPersons);
        n = persons.Count - 1;
        persons[n].go.transform.position = new Vector3(side * .75f, 0, side/2);
        SetColor(persons[n].go, colorInfected);
        persons[n].dayInfected = day;
        persons[n].ynInfected = true;
    }

    void LoadPersons() {
        for(int n = 0; n < numPersons; n++)
        {
            float x = UnityEngine.Random.Range(0, side - scalePerson);
            float z = UnityEngine.Random.Range(0, side - scalePerson);
            Vector3 pos = new Vector3(x, 0, z);
            LoadPerson(pos);
        }
        //Destroy(goPersonPrefab);
        Debug.Log("LoadPersons()\n");
    }

    Person LoadPerson(Vector3 pos)
    {
        GameObject go = Instantiate(goPersonPrefab, goPersons.transform);
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * scalePerson;
        go.transform.Rotate(0, UnityEngine.Random.Range(0, 360), 0);
        Person person = new Person(go);
        persons.Add(person);
        SetColor(go, colorNotInfected);
        return person;
    }

    void LoadBorders()
    {
        for (int n = 0; n < 5; n++)
        {
            GameObject go;
            go = Instantiate(goBorderPrefab, goBorders.transform);
            Vector3 pos = Vector3.zero;
            Vector3 sca = Vector3.zero;
            float thick = 1f;
            switch (n)
            {
                case 0:
                    pos = new Vector3(side/2, 0, 0);
                    sca = new Vector3(side, thick, thick);
                    break;    
                case 1:
                    pos = new Vector3(side, 0, side/2);
                    sca = new Vector3(thick, thick, side);
                    break;
                case 2:
                    pos = new Vector3(side/2, 0, side);
                    sca = new Vector3(side, thick, thick);
                    break;
                case 3:
                    pos = new Vector3(0, 0, side/2);
                    sca = new Vector3(thick, thick, side);
                    break;
                case 4:
                    pos = new Vector3(side / 2, 0, side / 2);
                    sca = new Vector3(thick, thick, side);
                    break;
                default:
                    break;
            }
            go.transform.position = pos;
            go.transform.localScale = sca;
        }
        //Destroy(goBorderPrefab);
    }

    void Move()
    {
        foreach(Person person in persons)
        {
            GameObject go = person.go;
            go.transform.position += go.transform.forward * speed;
            Bounce(person);
        }
    }

    void Bounce(Person personCheck)
    {
        GameObject goCheck = personCheck.go;
        if (personCheck.ynInfected)
        {
            if (day - personCheck.dayInfected > daysInfected)
            {
                personCheck.ynInfected = false;
                personCheck.ynImmune = true;
                SetColor(goCheck, colorImmune);
            }
        }
        foreach (Person person in persons)
        {
            GameObject go = person.go;
            if (go != goCheck)
            {
                if (IsNearPerson(goCheck, go))
                {
                    if (personCheck.ynInfected && !person.ynInfected && !person.ynImmune)
                    {
                        person.dayInfected = day;
                        person.ynInfected = true;
                        SetColor(person.go, colorInfected);
                        personCheck.othersInfected++;
                        personCheck.go.name = personCheck.othersInfected.ToString();
                    }
                    ReflectPerson(personCheck, person);
                }
            }
        }
        IsNearBorderReflect(goCheck);
    }

    void SetColor(GameObject go, Color color)
    {
        go.GetComponent<Renderer>().material.color = color;
    }

    void IsNearBorderReflect(GameObject goCheck)
    {
        Vector3 posCheck = goCheck.transform.position;
        Vector3 eulCheck = goCheck.transform.eulerAngles;
        for (int n = 0; n < goBorders.transform.childCount; n++)
        {
            GameObject go = goBorders.transform.GetChild(n).gameObject;
            Vector3 pos = go.transform.position;
            switch (n)
            {
                case 0: // bottom
                    if (posCheck.z <= pos.z + scalePerson)
                    {
                        eulCheck.y += 180;
                        goCheck.transform.eulerAngles = eulCheck;
                    }
                    break;
                case 1: // right
                    if (posCheck.x >= pos.x - scalePerson)
                    {
                        eulCheck.y += 180;
                        goCheck.transform.eulerAngles = eulCheck;
                    }
                    break;
                case 2: // up
                    if (posCheck.z >= pos.z - scalePerson)
                    {
                        eulCheck.y += 180;
                        goCheck.transform.eulerAngles = eulCheck;
                    }
                    break;
                case 3: // left
                    if (posCheck.x <= pos.x + scalePerson)
                    {
                        eulCheck.y += 180;
                        goCheck.transform.eulerAngles = eulCheck;
                    }
                    break;
                case 4: // middle
                    if (day < dayLockDownEnds)
                    {
                        if (posCheck.x >= pos.x - scalePerson && posCheck.x <= pos.x + scalePerson)
                        {
                            if (posCheck.z >= pos.z - side / 2 && posCheck.z <= pos.z + side / 2)
                            {
                                eulCheck.y += 180;
                                goCheck.transform.eulerAngles = eulCheck;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    bool IsNearPerson(GameObject goCheck, GameObject go)
    {
        float dist = Vector3.Distance(goCheck.transform.position, go.transform.position);
        if (dist <= nearDist)
        {
            return true;
        }
        return false;
    }

    void ReflectPerson(Person personCheck, Person person)
    {
        GameObject goCheck = personCheck.go;
        GameObject go = person.go;
        Vector3 pos = goCheck.transform.position;
        goCheck.transform.position = go.transform.position;
        goCheck.transform.LookAt(pos);
        goCheck.transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (ynDone) return;
        Move();
        Graph();
        if (day == dayLockDownEnds)
        {
            GameObject go = goBorders.transform.GetChild(4).gameObject;
            go.SetActive(false);
//            speed *= 1.25f;
        }
        day++;
    }

    void Graph()
    {
        GetCounts();
        //
        GameObject go;
        Vector3 sca;
        Vector3 pos;
        //
        go = Instantiate(goGraphPrefab, goGraphs.transform);
        go.name = "not infected";
        pos = new Vector3(side + day, 0, 0 + cntNotInfected/2f);
        sca = new Vector3(1, 1, 2);
        go.transform.position = pos;
        if (goGraphNotInfectedLast != null)
        {
            go.transform.position = (goGraphNotInfectedLast.transform.position + pos) / 2;
            go.transform.LookAt(pos);
            sca.z = Vector3.Distance(pos, goGraphNotInfectedLast.transform.position);
        }
        go.transform.localScale = sca;
        SetColor(go, colorNotInfected);
        goGraphNotInfectedLast = go;
        //
        go = Instantiate(goGraphPrefab, goGraphs.transform);
        go.name = "infected";
        pos = new Vector3(side + day, 0, 0 + cntInfected / 2f);
        sca = new Vector3(1, 1, 2);
        go.transform.position = pos;
        if (goGraphInfectedLast != null)
        {
            go.transform.position = (goGraphInfectedLast.transform.position + pos) / 2;
            go.transform.LookAt(pos);
            sca.z = Vector3.Distance(pos, goGraphInfectedLast.transform.position);
        }
        go.transform.localScale = sca;
        SetColor(go, colorInfected);
        goGraphInfectedLast = go;
        //
        go = Instantiate(goGraphPrefab, goGraphs.transform);
        go.name = "immune";
        pos = new Vector3(side + day, 0, 0 + cntImmune / 2f);
        sca = new Vector3(1, 1, 2);
        go.transform.position = pos;
        if (goGraphImmuneLast != null)
        {
            go.transform.position = (goGraphImmuneLast.transform.position + pos) / 2;
            go.transform.LookAt(pos);
            sca.z = Vector3.Distance(pos, goGraphImmuneLast.transform.position);
        }
        go.transform.localScale = sca;
        SetColor(go, colorImmune);
        goGraphImmuneLast = go;
        //
        float percentNotInfected = cntNotInfected / (float)numPersons * 100;
        float percentInfected = cntInfected / (float)numPersons * 100;
        float percentImmune = cntImmune / (float)numPersons * 100;
        float percentMaxInfected = cntMaxInfected / (float)numPersons * 100;
        float month = day / 30f;
        float aveOthersInfected = numOthersInfected / (float)numOthersInfectedDenominator;
        string txt = "";
        txt += "not infected " + percentNotInfected.ToString("F2") + " %\n";
        txt += "infected " + percentInfected.ToString("F2") + " % + (" + percentMaxInfected.ToString("F2") + " %)\n";
        txt += "immune " + percentImmune.ToString("F2") + " %\n";
        txt += "month " + month.ToString("F1") + "\n";
        txt += "days sick " + daysInfected + "\n";
        txt += "R factor " + aveOthersInfected.ToString("F2") + "\n";
        txt += "dayLockDownEnds " + dayLockDownEnds; // + " (increase " + ((lockDownIncrease - 1) * 100).ToString("F0") + " %)\n";
//        Debug.Log(txt);
        goInfo.GetComponent<TextMesh>().text = txt;
        //
        if (day % 30 == 0)
        {
            go = Instantiate(goGraphPrefab, goGraphs.transform);
            go.name = "month";
            pos = new Vector3(side + day, 0, 0);
            sca = new Vector3(1, 1, 10);
            go.transform.position = pos;
            go.transform.localScale = sca;
            SetColor(go, Color.green);
        }
        if (day == dayLockDownEnds)
        {
            go = Instantiate(goGraphPrefab, goGraphs.transform);
            go.name = "dayLockDownEnd";
            pos = new Vector3(side + day, 0, 0);
            sca = new Vector3(4, 1, 10);
            go.transform.position = pos;
            go.transform.localScale = sca;
            SetColor(go, Color.red);
        }
    }

    void GetCounts()
    {
        cntNotInfected = 0;
        cntInfected = 0;
        cntImmune = 0;
        numOthersInfected = 0;
        numOthersInfectedDenominator = 0;
        foreach (Person person in persons)
        {
            if (!person.ynInfected && !person.ynImmune)
            {
                cntNotInfected++;
            }
            else
            {
                if (person.ynInfected)
                {
                    cntInfected++;
                    if (cntInfected > cntMaxInfected)
                    {
                        cntMaxInfected = cntInfected;
                    }
                }
                else
                {
                    if (person.ynImmune)
                    {
                        cntImmune++;
                    }
                }
            }
            if (person.othersInfected > 0)
            {
                numOthersInfected += person.othersInfected;
                numOthersInfectedDenominator++;
            }
        }
        if (cntInfected == 0)
        {
            ynDone = true;
            DateTime dt = DateTime.UtcNow;
            string txt = "Covid " + dt.ToString("yyyy-MM-dd-HH-mm-ss");
            ScreenCapture.CaptureScreenshot(txt);
            Debug.Log("saving image " + txt + "\n");
            Invoke("Restart", 5);
        }
    }

    void Restart()
    {
        day = 0;
        cntMaxInfected = 0;
        for(int n = 0; n < persons.Count; n++)
        {
            Person person = persons[n];
            Destroy(person.go);
        }
        persons.Clear();
        //
        for(int n = 0; n < goBorders.transform.childCount; n++)
        {
            GameObject go = goBorders.transform.GetChild(n).gameObject;
            Destroy(go);
        }
        //
        if (cycle % 5 == 0)
        {
            for (int n = 0; n < goGraphs.transform.childCount; n++)
            {
                GameObject go = goGraphs.transform.GetChild(n).gameObject;
                Destroy(go);
            }
        }
        goGraphImmuneLast = null;
        goGraphNotInfectedLast = null;
        goGraphInfectedLast = null;
        //
        cycle++;
        Start();
        ynDone = false;
    }
}
