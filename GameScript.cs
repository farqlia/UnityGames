using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using UnityEngine.InputSystem;

public class GameScript : MonoBehaviour
{

    string[,] questions = {
        {"Które z tych państw leży w Europie?",
        "Który pierwiastek chemiczny oznaczony jest symbolem 'O'?",
        " Które z poniższych zwierząt jest drapieżnikiem?"},
        {
            "Kto napisał 'Romeo i Julia'?", 
            "Ile planet w Układzie Słonecznym jest większych od Ziemi?",
            "Która planeta jest znana jako 'Czerwona Planeta'?"
        }, {
            "Który ocean jest największy pod względem powierzchni?",
            "Który z następujących kolorów nie jest widoczny na tęczy?",
            " Które z poniższych miast jest stolicą Francji?"
        },
        {
            " Które z poniższych zwierząt jest ssakiem jajorodnym?",
            "Który pierwiastek chemiczny ma symbol \"H\"?",
            "Który z poniższych sportów jest rozgrywany na lodzie?"
        },
        {
            "W którym roku rozpoczęła się II wojna światowa?",
            "Która z planet jest znana jako \"Poranna Gwiazda\" lub \"Wieczorna Gwiazda\"?",
            "Które państwo jest największym producentem kawy na świecie?"
        },
        {
            "Kto jest autorem obrazu \"Mona Lisa\"?",
            "Ile lat trwa jeden mandat prezydencki w Stanach Zjednoczonych?",
            "Które z poniższych zwierząt jest gadem?"
        },
        {"Kto odkrył penicylinę, pierwszy antybiotyk?", "Która rzeka jest najdłuższą na świecie?", "Jak nazywa się proces, w wyniku którego rośliny wytwarzają własne substancje odżywcze?" }
    };

    string[,,] answers = {
        {{"Brazylia", "Japonia", " Australia", "Polska"},
        {"Węgiel", "Tlen", "Miedź", "Sód"},
        {"Koń", "Krokodyl", "Krowa", "Papuga"}},
        {{"William Shakespeare", "Jane Austen", "Charles Dickens", "Leo Tolstoy"},
        {"1", "2", "3", "4"}, {"Wenus", "Mars", "Jowisz", "Saturn"}},
        {{"Ocean Spokojny", "Atlantycki", "Indyjski", "Południowy"}, {"Zielony", "Fioletowy", "Biały", "Czarny"},
        {"Berlin", "Madryt", "Rzym", "Paryż"}},
        {{"Kangur", "Delfin", "Wiewiórka", "Ornitorink"}, {"Hel", " Hydrogen", "Hafn", "Hahnium"}, { "Piłka nożna", "Baseball", "Hokej na lodzie", "Koszykówka"} },
        {{"1935", "1939", "1941", "1945"}, { "Wenus", "Mars", "Merkury", "Jowisz"}, { "Brazylia", "Kolumbia", "Wietnam", "Etiopia"} },
        {{"Vincent van Gogh", "Pablo Picasso", "Leonardo da Vinci", "Claude Monet"}, {"2 lata", "4 lata", "6 lat", "8 lat"},
        {"Krokodyl", "Komar", "Ropucha", "Żaba"}},

        {{"Alexander Fleming", "Marie Curie", "Albert Einstein", "Louis Pasteur"},
        {"Nil", "Amazonka", "Missouri", "Yangtze" }, {"Fotosynteza", "Fermentacja", "Degradacja", "Biodegradacja" } }
    };

    int[,] correct = {
        {3, 1, 1}, {0, 0, 1}, {0, 3, 3}, {3, 1, 2}, {1, 0, 0 }, {2, 1, 0}, {0, 1, 0}
    };

    int[] stakes =
    {
        500, 1000, 5000, 40000, 250000, 500000, 1000000
    };

    int[] levelsWithPrices = { -1, -1, 1, 1, 3, 3, 6};

    static int nLevels = 7;

    public Button gameButton;
    public Button resultsButton;
    public Button descriptionButton;
    public Button contactButton;

    public GameObject menuPanel;

    public GameObject answersPanel;

    public GameObject inputNickPanel;

    public GameObject questionPanel;

    public GameObject gameResultPanel;

    public GameObject promptPanel;

    public TMP_InputField enteredNick;

    public TMP_Text showedNick;
   
    string[] buttonNames = {"A", "B", "C", "D"};

    public TMP_Text question;

    public Button[] buttons;

    public GameObject chancesPanel;

    public Button[] chanceButtons;

    public TMP_InputField[] levelLabels;

    System.Random rand = new System.Random();

    public InputAction finishAction;
    public InputAction chanceAction;

    bool isPlaying = false;

    int currentQuestion;
    int currentLevel = 0;

    public Color[] colors;

    private void OnEnable()
    {
        finishAction.Enable();
        chanceAction.Enable();
    }

    private void OnDisable()
    {
        finishAction.Disable();
        chanceAction.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        showMenu();
        prepareLabels();
        gameResultPanel.SetActive(false);
        promptPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (finishAction.triggered && isPlaying)
        {
            endGameFromInputSystem();
        }
        if (chanceAction.triggered && isPlaying)
        {
            halfChance();
            disableButton(chanceButtons[0]);
        }
    }

    void prepareLabels()
    {
        colors = new Color[nLevels];
        for (int i = 0; i < nLevels; i++)
        {
            levelLabels[i].enabled = false;
            levelLabels[i].text = "" + stakes[i];
            colors[i] = levelLabels[i].GetComponent<Image>().color;
        }
        
    }

    void setButtonsInteractable(Button[] buttons, bool interactable)
    {
        foreach (Button bt in buttons)
        {
            bt.interactable = interactable;
        }
    }

    public void closeMenu(){
        menuPanel.SetActive(false);
        inputNickPanel.SetActive(false);
        showedNick.text = enteredNick.text;        
    }

    public void showMenu(){
        setButtonsInteractable(chanceButtons, false);
        menuPanel.SetActive(true);
        inputNickPanel.SetActive(true);
        answersPanel.SetActive(false);
        questionPanel.SetActive(false);
        showedNick.text = "";
    }
    public void startGame()
     {
        isPlaying = true;
        closeMenu();
        showGame();
     }
    public void showGame(){
        setButtonsInteractable(chanceButtons, true);
        answersPanel.SetActive(true);
        questionPanel.SetActive(true);
        gameResultPanel.SetActive(false);
        setNext();
    }

    void hideGame(){
        answersPanel.SetActive(false);
        questionPanel.SetActive(false);
    }

    void setNext(){
        setSelectedButton();
        changeLevelColors();
        currentQuestion = rand.Next(0, 3);
        setAnswers(currentLevel, currentQuestion);
        setQuestion(currentLevel, currentQuestion);
    }

    void changeLevelColors()
    {
        if (currentLevel > 0)
        {
            setLabelColor(currentLevel - 1);
        }
        setHighlightedLabelColor(currentLevel);
        
    }

    void setLabelColor(int idx)
    {
        levelLabels[idx].GetComponent<Image>().color = colors[idx];
    }

    void setHighlightedLabelColor(int idx)
    {
        levelLabels[idx].GetComponent<Image>().color = Color.green;
    }
    
    void nextLevel() { 
        if (currentLevel + 1 == nLevels){
            endGame();
        } else {
            currentLevel++;
            setNext();
        }
        clear();
    }

    void showGameResult(string status)
    {
        gameResultPanel.SetActive(true);
        var gameResult = gameResultPanel.GetComponentInChildren<TextMeshProUGUI>();
        gameResult.text = status;
    }


    public void endGame(){
        int price = priceForCurrentLevel();
        showGameResult("Wygrana: " + price);
        setLabelColor(currentLevel);
        currentLevel = 0;
        isPlaying = false;
        clear();
        hideGame();
        showMenu();
    }

    void setQuestion(int level, int n){
        question.text = questions[level, n];
    }

    void setAnswers(int level, int n){
        for (int i=0; i < 4; i++){
            var butText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            butText.text = buttonNames[i] + ") " + answers[level, n, i];
        }
    }

    void setButtonText(int butIdx, string text)
    {
        var butText = buttons[butIdx].GetComponentInChildren<TextMeshProUGUI>();
        butText.text = text;
    }

    IEnumerator processAnswer(int buttonClicked)
    {

        bool isCorrect = correct[currentLevel, currentQuestion] == buttonClicked;
        setButtonColor(correct[currentLevel, currentQuestion], Color.green);
        if (!isCorrect)
        {
            setButtonColor(buttonClicked, Color.red);
        }

        setButtonsInteractable(buttons, false);

        yield return new WaitForSecondsRealtime(0.5F);

        setButtonColor(correct[currentLevel, currentQuestion], Color.white);
        setButtonColor(buttonClicked, Color.white);


        if (isCorrect)
        {
            nextLevel();
        } else
        {
            endGame();
        }
        setButtonsInteractable(buttons, true);

    }

    public void answerChosen(int buttonClicked){
        StartCoroutine(processAnswer(buttonClicked));
    }

    public void setButtonColor(int buttonId, Color c)
    {
        buttons[buttonId].GetComponent<Image>().color = c;
    }

    void clear()
    {
        promptPanel.SetActive(false);
    }

    public void halfChance()
    {
        int correctIdx = correct[currentLevel, currentQuestion];
        HashSet<int> takenIdx = new HashSet<int>() { correctIdx };
        int firstIdx = randDifferentThan(4, takenIdx);
        takenIdx.Add(firstIdx);
        int secIdx = randDifferentThan(4, takenIdx);

        setButtonText(firstIdx, "");
        setButtonText(secIdx, "");
        disableButton(buttons[firstIdx]);
        disableButton(buttons[secIdx]);
        clear();
    }

    int randDifferentThan(int to, HashSet<int> differentFrom)
    {
        int randNum = rand.Next(to);
        while (differentFrom.Contains(randNum)){
            randNum = rand.Next(to);
        }
        return randNum;
    }

    public void anotherQuestion()
    {
        int anotherQuestion = randDifferentThan(3, new HashSet<int> () { currentQuestion});
        currentQuestion = anotherQuestion;
        setAnswers(currentLevel, anotherQuestion);
        setQuestion(currentLevel, anotherQuestion);
        clear();
        setButtonsInteractable(buttons, true);
    }

    public void disableButton(Button but)
    {
        but.interactable = false;
    }

    public void friendCall()
    {
        promptPanel.SetActive(true);
        double friendAccuracy = 0.8;
        int friendPrompt;
        int correctIndx = correct[currentLevel, currentQuestion];
        if (rand.NextDouble() > friendAccuracy)
        {
            friendPrompt = correctIndx;
        } else
        {
            friendPrompt = randDifferentThan(4,
                new HashSet<int>() { correctIndx });
        }
        var prmptText = promptPanel.GetComponentInChildren<TextMeshProUGUI>();
        prmptText.text = "Przyjaciel podpowiada " + buttonNames[friendPrompt];
        setButtonsInteractable(buttons, true);
    }

    public void audienceHelp()
    {
        int[] probabilities = new int[] { 0, 0, 0, 0 };
        int cummulativeProb = 0;

        for (int i = 0; i < 4; i++)
        {
            probabilities[i] = rand.Next(101 - cummulativeProb);
            cummulativeProb += probabilities[i];
        }
      
        string audienceAnswers = "";
        for (int i = 0; i < 4; i++)
        {
            audienceAnswers += buttonNames[i] + ": " + probabilities[i] + "% ";
        }
        promptPanel.SetActive(true);
        var prmptText = promptPanel.GetComponentInChildren<TextMeshProUGUI>();
        prmptText.text = audienceAnswers;
        setButtonsInteractable(buttons, true);
    }

    int priceForCurrentLevel()
    {
        if (levelsWithPrices[currentLevel] > 0)
        {
            return stakes[levelsWithPrices[currentLevel]];
        }
        return 0;
    }


    public void endGameFromInputSystem()
    {
        if (currentLevel > 0)
        {
            endGame();
        }
         
    }

    public void setSelectedButton()
    {
        buttons[0].Select();
    }
}
