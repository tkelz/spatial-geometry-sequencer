using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class StemUIManager : MonoBehaviour
{
    public static StemUIManager Instance;

    [Header("The UI Document that holds your sliders")]
    public UIDocument uiDocument;
    public VisualTreeAsset stemUITemplate;
    public OptionUIManager optionUIManager;

    [Header("Game Manager")]
    public GameManager gameManager;
    public StemManager stemManager;

    Button addStemBtn, removeStemBtn;
    ListView stemContainer;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        if (uiDocument == null || gameManager == null)
        {
            Debug.LogError("StemUIManager: Assign both a UIDocument and GameManager!");
            enabled = false;
            return;
        }

        var root = uiDocument.rootVisualElement;
        addStemBtn = root.Q<Button>("AddStemBtn");
        removeStemBtn = root.Q<Button>("RemoveStemBtn");

        stemContainer = root.Q<ListView>("StemContainer");

        addStemBtn.RegisterCallback<ClickEvent>(evt => AddStem());
        removeStemBtn.RegisterCallback<ClickEvent>(evt => RemoveStem());

        stemContainer.makeItem = () =>
        {
            var newStemUI = stemUITemplate.Instantiate();
            var newStem = stemManager.stems.Last();
            newStem.SetVisualElements(newStemUI);
            newStemUI.userData = newStem;
            return newStemUI;
        };
        stemContainer.bindItem = (item, index) => {
            stemManager.stems[index].SetVisualElements(item);
            item.userData = stemManager.stems[index];
        };
        stemContainer.selectionChanged += OnStemChange;
    }

    public void RefreshItems () {
        stemContainer.itemsSource = stemManager.stems;
        print(stemManager.stems);
        stemContainer.RefreshItems();
    }

    void AddStem() {
        stemManager.AddNewStem();
        RefreshItems();
    }

    void RemoveStem() {
        print("UI:" + stemContainer.selectedIndex);
        stemManager.RemoveStem(stemContainer.selectedIndex);
        RefreshItems();
        stemContainer.ClearSelection();
    }

    void OnStemChange (IEnumerable<object> selectedItems) {
        var stemItem = stemContainer.selectedItem as StemItem;

        // Update stem data on UI
        removeStemBtn.SetEnabled(!!stemItem);
        optionUIManager.SetStemData(stemItem);
    }
}
