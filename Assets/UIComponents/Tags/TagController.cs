using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class TagController : MonoBehaviour
{
    /* 
     * Define member variables
    **/
    private const string inputTagClassName = "inputTag";
    private const string inputTagVisualElement = "tagVE";
    private const string inputTagTextField = "tagTextField";
    private const string inputTagCloseLabel = "closeLabel";
    private const string inputTagButton = "buttonTag";

    /* 
     * when click InputTag
    **/
    private bool isTyping;
    private bool isClose;

    /*
     * UI Builder Elements
    **/
    private VisualElement root;
    private VisualElement tagVE;
    private VisualElement inputTagVE;
    private VisualElement createTagVE;
    private VisualElement newTagVE;
    private VisualElement searchTagVE;
    private TextField tagTextField;
    private TextField tagCreateTextField;
    private Label newTagLabel;

    List<string> listTags = new List<string>();

    /*
     * Start is called before the first frame update
    **/
    void Start()
    {
        isTyping = false;
        isClose = false;
        listTags.Add("Test 1");
    }
    
    private void OnEnable() 
    {
        UIDocument menu = GetComponent<UIDocument>();
        root = menu.rootVisualElement;

        // Initial
        tagVE = root.Q<VisualElement>(className: inputTagClassName);
        createTagVE = root.Q<VisualElement>("createTag");
        newTagVE = root.Q<VisualElement>("NewTag");
        searchTagVE = root.Q<VisualElement>("searchTag");
        newTagVE.style.display = DisplayStyle.None;

        TagEvent();
        TagTextFeildEvent();
    }

    /*
     * Click Event when click multi tag input feild.
    **/
    private void TagEvent() {
        tagVE.RegisterCallback<ClickEvent>(AddNewInputGroup);
    }

    private void AddNewInputGroup(ClickEvent evt) {
        
        if (!isClose) {
            if (!isTyping)
            {
                searchTagVE.Clear();

                inputTagVE = new VisualElement();
                inputTagVE.AddToClassList(inputTagVisualElement);

                tagTextField = new TextField();
                tagTextField.AddToClassList(inputTagTextField);
                inputTagVE.Insert(inputTagVE.childCount, tagTextField);

                Label closeLabel = new Label() {text = "x"};
                closeLabel.AddToClassList(inputTagCloseLabel);
                RemoveInputTagCallbacks(closeLabel);
                inputTagVE.Insert(inputTagVE.childCount, closeLabel);
                
                tagVE.Insert(tagVE.childCount, inputTagVE);


                // Adding new Label in CreateTag VisualElement
                newTagLabel = new Label();
                newTagLabel.AddToClassList(inputTagButton);
                newTagVE.style.display = DisplayStyle.Flex;
                createTagVE.Insert(createTagVE.childCount, newTagLabel);

                isTyping = true;
            } 
        } else {
            isClose = false;
        }
    }

    /*
     * Keyboard event
    **/
    private void TagTextFeildEvent() {
        tagVE.RegisterCallback<KeyUpEvent>(OnKeyUp, TrickleDown.TrickleDown);
    }

    private void OnKeyUp(KeyUpEvent evt) {
        int tagTextFieldWidth = tagTextField.text.Length;

        // when click enter key
        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter )
        {
            if (tagTextField.text != null && tagTextField.text != "")
            {
                VisualElement addTagVE = new VisualElement();
                addTagVE.AddToClassList(inputTagVisualElement);

                Label newLabel = new Label() { text = tagTextField.text };
                newLabel.AddToClassList(inputTagTextField);
                newTagLabel.style.width = 15 + tagTextFieldWidth*8.5f;
                addTagVE.Insert(addTagVE.childCount, newLabel);

                Label closeLabel = new Label() {text = "x"};
                closeLabel.AddToClassList(inputTagCloseLabel);
                addTagVE.Insert(addTagVE.childCount, closeLabel);

                addTagVE.style.width = 30 + tagTextFieldWidth*8.5f;

                RemoveTagCallbacks(addTagVE);
                
                tagVE.Insert(tagVE.childCount-1, addTagVE);

                tagVE.RemoveAt(tagVE.childCount-1);
                createTagVE.RemoveAt(1);

                isTyping = false;

                if (!MatchTagList()) {
                    listTags.Add(newLabel.text);
                }

                searchTagVE.Clear();
                
                newTagVE.style.display = DisplayStyle.None;

            }
        }

        // change TextFeild width

        tagTextField.style.width = 15 + tagTextFieldWidth*8.5f;
        newTagLabel.style.width = 15 + tagTextFieldWidth*8.5f;
        inputTagVE.style.width = 30 + tagTextFieldWidth*8.5f;
        
        newTagLabel.text = tagTextField.text;


        if (tagTextFieldWidth > 0) {
            
            searchTagVE.Clear();
            SearchTagList();
        } else {
            searchTagVE.Clear();
        }
    }

    private void SearchTagList() {
        for (int i = 0 ; i < listTags.Count; i++ ){
            if (listTags[i] != null && listTags[i] != "") {
                if (listTags[i].Contains(tagTextField.text)){
                    Label newSearchLabel = new Label() { text = listTags[i] };
                    newSearchLabel.AddToClassList(inputTagButton);
                    newSearchLabel.style.width = 20 + listTags[i].Length*8.5f;
                    searchTagVE.Insert(searchTagVE.childCount, newSearchLabel);
                }
            }
        }
    }

    private bool MatchTagList() {
        for (int i = 0 ; i < listTags.Count; i++ ){
            if (listTags[i] == tagTextField.text) {
                return true;
            }
        }
        return false;
    }

    private void DeleteSearchTagList() {
        int count = searchTagVE.childCount;
        for (int i=0; i< count; i++)
        {
            searchTagVE.RemoveAt(i);
        }
    }
    /*
     * Delete tag when click tag
    **/
    private void RemoveTagCallbacks(VisualElement tag) {
        tag.RegisterCallback<ClickEvent>(TagOnClick);
    }

    /*
     * Delete inputTag when click close (x)
    **/
    private void RemoveInputTagCallbacks(Label close) {
        close.RegisterCallback<ClickEvent>(CloseOnClick);
    }

    private void CloseOnClick(ClickEvent evt)
    {
        Label clickedTag = evt.currentTarget as Label;
        clickedTag.parent.style.display = DisplayStyle.None;
        createTagVE.RemoveAt(1);
        newTagVE.style.display = DisplayStyle.None;
        isClose = true;
        isTyping = false;
    }

    private void TagOnClick(ClickEvent evt)
    {
        VisualElement clickedTag = evt.currentTarget as VisualElement;
        clickedTag.style.display = DisplayStyle.None;
        isClose = true;
    }

    /*
     * Update is called once per frame
    **/
    void Update()
    {
        
    }
}
