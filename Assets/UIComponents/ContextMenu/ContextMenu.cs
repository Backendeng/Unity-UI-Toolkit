using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ContextMenu : MonoBehaviour 
{

    /* 
     * Define member variables
    **/
    private const string menuItem = "Item";

    /*
     * UI Builder Elements
    **/
    private VisualElement root;
    private VisualElement menuPanel;
    private Button rightClickBtn;  
    
    // Start is call
    private void OnEnable() 
    {
        UIDocument menu = GetComponent<UIDocument>();
        root = menu.rootVisualElement;

        rightClickBtn = root.Q<Button>("button");
        menuPanel = root.Q<VisualElement>("menuPanel");
        rightClickBtn.clickable.activators.Clear();
        rightClickBtn.RegisterCallback<MouseDownEvent>(e => OnShowMenu(e));

        // Initial

        createItems();
    }

    /*
     * Click Event when click button.
    **/
    private void OnShowMenu(MouseDownEvent evt) {
        // click right button
        if(evt.button == 1) {
            menuPanel.style.left  = Input.mousePosition.x;
            menuPanel.style.display = DisplayStyle.Flex;
        }
    }

    private void createItems() {
        for (int i = 0 ; i < 10; i++ ){
            Label newItem = new Label();
            newItem.text = "Test " + i.ToString();
            newItem.AddToClassList(menuItem);
            AddMenuItemEvent(newItem);
            menuPanel.Insert(menuPanel.childCount, newItem);
        }

        menuPanel.style.height = menuPanel.childCount * 33;
        menuPanel.style.display = DisplayStyle.None;
    }

    /*
     * Add Tags when click search tag
    **/
    private void AddMenuItemEvent(Label tags) {
        tags.RegisterCallback<ClickEvent>(OnClickMenuItem);
    }

    private void OnClickMenuItem(ClickEvent evt){
        Label clickItem = evt.currentTarget as Label;

        print(clickItem.text);
        menuPanel.style.display = DisplayStyle.None;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
