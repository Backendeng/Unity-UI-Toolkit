using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class TabbedMenuController
{
    /* Define member variables*/
    private const string tabClassName = "tab";
    private const string newGroupTabClassName = "newGroup";
    private const string currentlySelectedTabClassName = "currentlySelectedTab";
    /*private const string unselectedContentClassName = "unselectedContent";*/
    // Tab and tab content have the same prefix but different suffix
    // Define the suffix of the tab name
    private const string tabNameSuffix = "Tab";
    // Define the suffix of the tab content name
    /*private const string contentNameSuffix = "Content";*/

    private readonly VisualElement root = default;
    private RotatingCube _rotatingCube;

    public TabbedMenuController(in VisualElement root)
    {
        this.root = root;
    }

    public void RegisterTabCallbacks(RotatingCube rotatingCube)
    {
        _rotatingCube = rotatingCube;
        UQueryBuilder<Label> tabs = GetAllTabs();
        tabs.ForEach(RegisterTabCallbacks);

        _ = FillCharacters(tabs.First());

        Label newGroup = GetNewGroupTab();
        newGroup.RegisterCallback<ClickEvent>(NewGroupOnClick);
    }

    private void RegisterTabCallbacks(Label tab)
    {
        tab.RegisterCallback<ClickEvent>(TabOnClick);
    }

    private void NewGroupOnClick(ClickEvent evt)
    {
        Label clickedTab = evt.currentTarget as Label;
        VisualElement groupHolder = clickedTab.parent;
        Label newGroup = new Label() { text = "Test" };
        newGroup.AddToClassList("tab");
        RegisterTabCallbacks(newGroup);
        groupHolder.Insert(groupHolder.childCount - 1, newGroup);
        SwitchTab(newGroup);
    }

    private void SwitchTab(Label clickedTab)
    {
        if (_rotatingCube.editRotation == null)
        {
            _rotatingCube.editRotation = new Vector3(0, 1, 0);
        }
        _rotatingCube.editRotation = new Vector3(_rotatingCube.editRotation.x, _rotatingCube.editRotation.y * -1, _rotatingCube.editRotation.z);
        if (!TabIsCurrentlySelected(clickedTab))
        {
            UQueryBuilder<Label> tabs = GetAllTabs();
            UQueryBuilder<Label> otherSelectedTabs =
                tabs.Where((Label tab) => tab != clickedTab && TabIsCurrentlySelected(tab));
            otherSelectedTabs.ForEach(UnselectTab);
            SelectTab(clickedTab);
            _ = FillCharacters(clickedTab);
        }
    }

    /* Method for the tab on-click event: 
	   
	   - If it is not selected, find other tabs that are selected, unselect them 
	   - Then select the tab that was clicked on
	*/
    private void TabOnClick(ClickEvent evt)
    {
        Label clickedTab = evt.currentTarget as Label;
        SwitchTab(clickedTab);
    }
    //Method that returns a Boolean indicating whether a tab is currently selected
    private static bool TabIsCurrentlySelected(in Label tab)
    {
        return tab.ClassListContains(currentlySelectedTabClassName);
    }

    private UQueryBuilder<Label> GetAllTabs()
    {
        return root.Query<Label>(className: tabClassName);
    }

    private Label GetNewGroupTab()
    {
        return root.Q<Label>(className: newGroupTabClassName);
    }

    public static async Task<Texture2D> GetRemoteTexture(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            // begin request:
            var asyncOp = www.SendWebRequest();

            // await until it's done: 
            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);//30 hertz

            // read results:
            if (www.result != UnityWebRequest.Result.Success)
            {
                // log error:
#if DEBUG
                UnityEngine.Debug.Log($"{www.error}, URL:{www.url}");
#endif

                // nothing to return on error:
                return null;
            }
            else
            {
                // return valid results:
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }

    private async Task FillCharacters(Label tab)
    {
        var characterList = root.Q<VisualElement>(name: "characterList").Q<ScrollView>();
        characterList.Clear();
        const int rndMaxValue = 30;
        const int minNumber = 5;
        System.Random rnd = new System.Random();
        var random = rnd.Next(0, rndMaxValue + 1) + minNumber;
        UnityEngine.Debug.Log($"Element count: {random}");
        for (int i = 0; i < random; i++)
        {
            characterList.Add(new Image()
            {
                image = await GetRemoteTexture(url: "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRLOsHTY44hPAQf3DuLI6OgqwAWY9uBZXJHb8KmDTQ&s")
            });
        }
    }

    /* Method for the selected tab: 
       -  Takes a tab as a parameter and adds the currentlySelectedTab class
       -  Then finds the tab content and removes the unselectedContent class */
    private void SelectTab(in Label tab)
    {
        tab.AddToClassList(currentlySelectedTabClassName);
        /*VisualElement content = FindContent(tab);
        content.RemoveFromClassList(unselectedContentClassName);*/
    }

    /* Method for the unselected tab: 
       -  Takes a tab as a parameter and removes the currentlySelectedTab class
       -  Then finds the tab content and adds the unselectedContent class */
    private void UnselectTab(Label tab)
    {
        tab.RemoveFromClassList(currentlySelectedTabClassName);
        /*VisualElement content = FindContent(tab);
        content.AddToClassList(unselectedContentClassName);*/
    }

    // Method to generate the associated tab content name by for the given tab name
    /*private static string GenerateContentName(in Label tab)
    {
        int prefixLength = tab.name.Length - tabNameSuffix.Length;
        string prefix = tab.name.Substring(0, prefixLength);
        return prefix + contentNameSuffix;
    }*/
    // Method that takes a tab as a parameter and returns the associated content element
    /*private VisualElement FindContent(in Label tab)
    {
        return root.Q(GenerateContentName(tab));
    }*/
}
