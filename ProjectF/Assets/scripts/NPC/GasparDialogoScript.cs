using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class GasparDialogoScript : MonoBehaviour, IInteractable
{
    public NPCDialogo dialogueData;
    public GameObject coruja;
    private DialogueController dialogueUI;
    private int dialogueIndex;
    private bool isTyping, dialogoAtivo;
    private Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        dialogueUI = DialogueController.Instance;
        coruja.SetActive(false);
    }
    public bool PodeInteract()
    {
        return !dialogoAtivo;
    }
    public void Interact()
    {
        if (dialogueData == null || (PauseController.JogoPausado &&!dialogoAtivo))
        {
            return;
        }
        else if (dialogoAtivo)
        {
            NextLine();
        }
        else 
        {
            StartDialogo();
        }
    }
    void StartDialogo() 
    {
        PauseController.SetPause(true);
        dialogoAtivo = true;
        dialogueIndex = 0;

        dialogueUI.SetNPCInfo(dialogueData.npcNome, dialogueData.npcRetrato);
        dialogueUI.ShowDialogueUI(true);
        

         DisplayCurrentLine();
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogolinhas[dialogueIndex]);
            isTyping = false;
        }
        dialogueUI.ClearChoices();
        if(dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogo();
            return;
        }

        foreach(DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if(dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogolinhas.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogo();
        }
    }
    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach(char letter in dialogueData.dialogolinhas[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogText.text +=letter);
            yield return new WaitForSeconds(dialogueData.speed);
        }

        isTyping = false;

        if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    

    void DisplayChoices(DialogueChoice choice)
    {
        for(int i = 0; i <choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            dialogueUI.CreateButton(choice.choices[i], () => ChooseOption(nextIndex));
        }
    }

    void ChooseOption(int nextIndex)
    {
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialogo()
    {
        StopAllCoroutines();
        dialogoAtivo = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);

        DestroyGaspar();
        coruja.SetActive(true);
    }

    public void DestroyGaspar(){
        GameObject gaspar = GameObject.FindGameObjectWithTag("Gaspar");
        Destroy(gaspar);
    }    
    
}

