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
    public StemManager stemManager;

    Button addStemBtn, removeStemBtn;
    Button saveSessionBtn, loadSessionBtn, newSessionBtn;
    Button playBtn, stopBtn;
    ListView stemContainer;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        if (uiDocument == null || stemManager == null)
        {
            Debug.LogError("StemUIManager: Assign both a UIDocument and StemManager!");
            enabled = false;
            return;
        }

        var root = uiDocument.rootVisualElement;
        addStemBtn = root.Q<Button>("AddStemBtn");
        removeStemBtn = root.Q<Button>("RemoveStemBtn");
        saveSessionBtn = root.Q<Button>("SaveSessionBtn");
        loadSessionBtn = root.Q<Button>("LoadSessionBtn");
        newSessionBtn = root.Q<Button>("NewSessionBtn");
        playBtn = root.Q<Button>("PlayBtn");
        stopBtn = root.Q<Button>("StopBtn");

        stemContainer = root.Q<ListView>("StemContainer");

        addStemBtn.RegisterCallback<ClickEvent>(evt => AddStem());
        removeStemBtn.RegisterCallback<ClickEvent>(evt => RemoveStem());
        saveSessionBtn.RegisterCallback<ClickEvent>(evt => SaveSession());
        loadSessionBtn.RegisterCallback<ClickEvent>(evt => LoadSession());
        newSessionBtn.RegisterCallback<ClickEvent>(evt => NewSession());
        playBtn.RegisterCallback<ClickEvent>(evt =>
        {
            stemManager.Play();
            playBtn.style.display = DisplayStyle.None;
            stopBtn.style.display = DisplayStyle.Flex;
        });
        stopBtn.RegisterCallback<ClickEvent>(evt =>
        {
            stemManager.Pause();
            playBtn.style.display = DisplayStyle.Flex;
            stopBtn.style.display = DisplayStyle.None;
        });
        stopBtn.style.display = DisplayStyle.None;

        stemContainer.makeItem = () =>
        {
            var newStemUI = stemUITemplate.Instantiate();
            var newStem = stemManager.stems.Last();
            newStem.SetVisualElements(newStemUI);
            newStemUI.userData = newStem;
            return newStemUI;
        };
        stemContainer.bindItem = (item, index) =>
        {
            stemManager.stems[index].SetVisualElements(item);
            item.userData = stemManager.stems[index];
        };
        stemContainer.selectionChanged += OnStemChange;
    }

    public void RefreshItems()
    {
        stemContainer.itemsSource = stemManager.stems;
        stemContainer.RefreshItems();
    }

    void AddStem()
    {
        stemManager.AddNewStem();
        RefreshItems();
    }

    void RemoveStem()
    {
        stemManager.RemoveStem(stemContainer.selectedIndex);
        RefreshItems();
        stemContainer.ClearSelection();
    }

    void OnStemChange(IEnumerable<object> selectedItems)
    {
        var stemItem = stemContainer.selectedItem as StemItem;

        // Update stem data on UI
        removeStemBtn.SetEnabled(!!stemItem);
        optionUIManager.SetStemData(stemItem);
    }

    public void SaveSession()
    {
        SessionManager.Instance.SaveSession();
    }

    public void LoadSession()
    {
        SessionManager.Instance.LoadSession();
        RefreshItems();
    }

    public void NewSession()
    {
        SessionManager.Instance.NewSession();
        stemContainer.ClearSelection();
        RefreshItems();
    }
}
