using UnityEngine;
using UnityEngine.UIElements;

//Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
public class TabbedMenu : MonoBehaviour
{
    private TabbedMenuController controller;
    private RotatingCube _rotatingCube;

    private void OnEnable()
    {
        UIDocument menu = GetComponent<UIDocument>();
        VisualElement root = menu.rootVisualElement;

        controller = new(root);
        GameObject cubeObj;
        cubeObj = GameObject.Find("Cube");
        _rotatingCube = cubeObj.GetComponent<RotatingCube>();
        //_rotatingCube = GetComponent<RotatingCube>();

        controller.RegisterTabCallbacks(_rotatingCube);
    }
}