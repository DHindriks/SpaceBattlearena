using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Characters
{
    Zero,
    GenericPilot,
    BossPilot
}

public class DialogueManager : MonoBehaviour {

    Queue<KeyValuePair<string, GameObject>> Dialoguequeue = new Queue<KeyValuePair<string, GameObject>>();
     
    bool Playing = false;

    [SerializeField]
    Animator DialoguePanel;

    [SerializeField]
    GameObject CharacterContainer;

    [SerializeField]
    GameObject MissionControl;

    [SerializeField]
    Text DialogueText;

    public void AddDialogue(string Text, Characters Character = Characters.Zero)
    {
        GameObject ChosenChar;

        ChosenChar = Resources.Load("Characters/" + Character.ToString()) as GameObject;

        KeyValuePair<string, GameObject> DialogueToAdd = new KeyValuePair<string, GameObject>(Text, ChosenChar);
        Debug.Log("Added: " + Text + " to queue");
        Dialoguequeue.Enqueue(DialogueToAdd);
        CheckQueue();
    }

    void CheckQueue()
    {
        if (Dialoguequeue.Count > 0 && Playing == false)
        {
            StartCoroutine("PlayNext");
        }
        else {
            Debug.LogWarning("Coroutine not started " + Dialoguequeue.Count + " " + Playing);
        }
    }

    IEnumerator PlayNext()
    {
        DialoguePanel.SetBool("Open", true);
        DialoguePanel.gameObject.SetActive(true);
        Playing = true;
        KeyValuePair<string, GameObject> DialogueToPlay = Dialoguequeue.Dequeue();
        GameObject Character = Instantiate(DialogueToPlay.Value, CharacterContainer.transform);
        SetLayer(Character);
        Character.transform.position = CharacterContainer.transform.position;
        Character.transform.rotation = CharacterContainer.transform.rotation;

        DialogueText.text = null;

        for (int i = 0; i < DialogueToPlay.Key.Length; i++)
        {
            DialogueText.text += DialogueToPlay.Key[i];
            DialogueText.gameObject.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(0.075f);
        }
        yield return new WaitForSeconds(3);
        Debug.Log(DialoguePanel.GetBool("Open"));
        DialoguePanel.SetBool("Open", false);
        Playing = false;
        Debug.Log("Closing Window");

        foreach (Transform child in CharacterContainer.transform)
        {
            Destroy(child.gameObject,   0.333f);
        }

        CheckQueue();
    }

    void SetLayer(GameObject gameObject)
    {
        gameObject.layer = 9;

        foreach(Transform child in gameObject.transform)
        {
            SetLayer(child.gameObject);
        }
    }
}
