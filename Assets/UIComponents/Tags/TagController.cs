using System.Collections;
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
    private const string inputTagTextField = "tagTextField";
    private const string inputTagButton = "buttonTag";

    /* 
     * when click InputTag
    **/
    private bool isTyping;

    /*
     * UI Builder Elements
    **/
    private VisualElement root;
    private VisualElement tagVE;
    private VisualElement createTagVE;
    private VisualElement newTagVE;
    private TextField tagTextField;
    private TextField tagCreateTextField;
    private Label newTagLabel;

    /*
     * Start is called before the first frame update
    **/
    void Start()
    {
        isTyping = false;
    }
    
    private void OnEnable() 
    {
        UIDocument menu = GetComponent<UIDocument>();
        root = menu.rootVisualElement;

        // Initial
        tagVE = root.Q<VisualElement>(className: inputTagClassName);
        createTagVE = root.Q<VisualElement>("createTag");
        newTagVE = root.Q<VisualElement>("NewTag");
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
        
        if (!isTyping)
        {
            // Adding new TextFeild in InputTag VisualElement
            tagTextField = new TextField();
            tagTextField.AddToClassList(inputTagTextField);
            tagVE.Insert(tagVE.childCount, tagTextField);

            // Adding new Label in CreateTag VisualElement
            newTagLabel = new Label();
            newTagLabel.AddToClassList(inputTagButton);
            newTagVE.style.display = DisplayStyle.Flex;
            createTagVE.Insert(createTagVE.childCount, newTagLabel);

            isTyping = true;
        } 
    }

    /*
     * Keyboard event
    **/
    private void TagTextFeildEvent() {
        tagVE.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
    }

    private void OnKeyDown(KeyDownEvent evt) {
        // when click enter key
        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter )
        {
            if (tagTextField.text != null && tagTextField.text != "")
            {
                // Add new Tag
                Button newTagItem = new Button() { text = tagTextField.text };
                newTagItem.AddToClassList(inputTagButton);
                newTagItem.style.width = 20 + tagTextField.text.Length*8;
                RegisterTabCallbacks(newTagItem);
                tagVE.Insert(tagVE.childCount-1, newTagItem);

                tagVE.RemoveAt(tagVE.childCount-1);
                createTagVE.RemoveAt(1);

                isTyping = false;
                newTagVE.style.display = DisplayStyle.None;
            }
        }

        // change TextFeild width
        tagTextField.style.width = 20 + tagTextField.text.Length*8.5f;
        newTagLabel.style.width = 10 + tagTextField.text.Length*8.5f;

        newTagLabel.text = tagTextField.text;
    }

    /*
     * Delete tag when click tag
    **/
    private void RegisterTabCallbacks(Button tag) {
        tag.RegisterCallback<ClickEvent>(TagOnClick);
    }

    private void TagOnClick(ClickEvent evt)
    {
        Button clickedTag = evt.currentTarget as Button;
        clickedTag.style.display = DisplayStyle.None;
    }

    /*
     * Update is called once per frame
    **/
    void Update()
    {
        
    }
}
