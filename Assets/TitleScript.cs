using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleScript : MonoBehaviour {
    
    public string[] menuStrings;
    public Text[] menuText;
    public Text highlight;
    public Text carrot;
    public Text loadingText;

    System.Text.StringBuilder sb;
    int[] counters;
    string[] loadingAnim;

    bool titleGenerated = false;
    int menuSelected = 2;

    // Use this for initialization
    void Start () {
        counters = new int[]
        {
            1, 6, 12, 8, 16, 16, 10, 6, 14, 10
        };

        loadingAnim = new string[]
        {
            "\\", "!", "/", "-"
        };

        StartCoroutine(GenerateMenuItem());
        StartCoroutine(AnimateCarrot());
        StartCoroutine(AnimateLoading());
    }

    void Update()
    {
        if (titleGenerated && Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                menuSelected--;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                menuSelected++;
            }
            
            if (menuSelected >= menuText.Length)
            {
                menuSelected = 2;
            }
            else if (menuSelected < 2)
            {
                menuSelected = menuText.Length - 1;
            }

            var pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(menuText[menuSelected].gameObject, pointer, ExecuteEvents.pointerEnterHandler);
        }
    }

    public void SelectMenuItem(int index)
    {
        menuSelected = index;
        if (menuSelected >= menuText.Length)
        {
            menuSelected = 2;
        }
        else if (menuSelected < 2)
        {
            menuSelected = menuText.Length - 1;
        }

        var pointer = new PointerEventData(EventSystem.current);
        for (int i = 0; i < menuText.Length; i++)
        {
            if (i != menuSelected)
            {
                ExecuteEvents.Execute(menuText[i].gameObject, pointer, ExecuteEvents.pointerExitHandler);
            }
        }
        carrot.transform.SetParent(menuText[menuSelected].transform);
        carrot.rectTransform.anchoredPosition = new Vector2(0, 0);
    }

    // Update is called once per frame
    IEnumerator GenerateMenuItem() {
        int tries = 0;
        int index = 0;
        int menuIndex = 0;
        int offset = 2;
        while (menuIndex < menuText.Length)
        {
            sb = new System.Text.StringBuilder();
            sb.Append('>');
            sb.Append(' ');

            if (menuIndex > 1)
            {
                sb.Append((". . . "));
                offset = 8;
            }

            carrot.transform.SetParent(menuText[menuIndex].transform);
            carrot.rectTransform.anchoredPosition = new Vector2(0, 0);

            highlight.transform.SetParent(menuText[menuIndex].transform);
            highlight.rectTransform.anchoredPosition = new Vector2(0, 0);

            while (index < menuStrings[menuIndex].Length)
            {
                if (index >= sb.Length - offset)
                {
                    sb.Append(' ');
                }
                if (menuIndex == 0 && index < counters.Length && tries < counters[index])
                {
                    char c = (char)('A' + Random.Range(0, 26));
                    sb[index + offset] = c;
                    highlight.text = c.ToString();
                    tries++;
                }
                else
                {
                    sb[index + offset] = menuStrings[menuIndex][index];
                    highlight.text = menuStrings[menuIndex][index].ToString();
                    tries = 0;
                    index++;
                }
                menuText[menuIndex].text = sb.ToString();
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.5f);
            index = 0;
            menuIndex++;
        }

        var pointer = new PointerEventData(EventSystem.current);
        for (int i = 0; i < menuText.Length; i++)
        {
            ExecuteEvents.Execute(menuText[i].gameObject, pointer, ExecuteEvents.pointerExitHandler);
            if(menuText[i].GetComponent<Selectable>() != null)
            {
                menuText[i].GetComponent<Selectable>().enabled = true;
            }
        }
        ExecuteEvents.Execute(menuText[menuSelected].gameObject, pointer, ExecuteEvents.pointerEnterHandler);

        highlight.enabled = false;
        titleGenerated = true;
    }

    IEnumerator AnimateCarrot()
    {
        while (true)
        {
            carrot.enabled = !carrot.enabled;
            yield return new WaitForSeconds(60f / 136f);
        }
    }

    IEnumerator AnimateLoading()
    {
        int count = 0;
        while (!titleGenerated)
        {
            loadingText.text = "BOOTING . . . " + loadingAnim[count];
            count++;
            if (count >= loadingAnim.Length)
            {
                count = 0;
            }
            yield return new WaitForSeconds(60f / 136f);
        }
        count = 0;
        yield return new WaitForSeconds(1f);
        while (titleGenerated)
        {
            string periods = "";
            for (int i = 0; i < count; i++)
            {
                periods += " .";
            }

            count++;
            if (count >= 4)
            {
                count = 0;
            }

            loadingText.text = "COMMAND SUCCESSFUL! AWAITING FURTHER INSTRUCTION" + periods;
            yield return new WaitForSeconds(60f / 136f);
        }
    }
}
