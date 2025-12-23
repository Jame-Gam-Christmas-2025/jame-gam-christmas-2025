using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class DialogueView : MonoBehaviour, ISubmitHandler,ICancelHandler, IPointerClickHandler
    {
        public Sprite AvatarSprite { get; private set; }
        public string NpcName { get; private set; }

#if UNITY_EDITOR
        [Header("Editor only")]
        [SerializeField] private TestDialogue testDialogue;
#endif

        [Header("Dialogue view")]
        [SerializeField] private TextMeshProUGUI dialogueTextMesh;
        [SerializeField] private TextMeshProUGUI npcNameTextMesh; 
        [SerializeField] private Image avatarImage;
        [SerializeField, Range(1, 100), Tooltip("Letter rate in char per second")] private int letterRate = 70;

        [Header("Choices")]
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private Transform choicesParent;

        private bool _initialized = false;


        private DialogueData _currentDialogue;
        private List<DialogueChoice> _choices = new List<DialogueChoice>();

        // Player objects variables
        private GameObject _player;
        private PlayerInteractor _playerInteractor;
        private PlayerCombatController _playerCombatController;
        private PlayerMovement _playerMovement;

        // Dialogue animation variables
        private Coroutine _textAnimCoroutine;
        private string _dialogueText = "";
        private bool _dialogueAnim = false;

        private void Start()
        {
#if UNITY_EDITOR
            if (SceneManager.GetActiveScene().name == "Feature-Dialogue_System" 
                || SceneManager.GetActiveScene().name == "FeaturePlayerAlignment")
            {
                EditorStartDialogue();
            }
#endif
        }

        private void Initialize()
        {
            // Store player objects in variables
            _player = GameObject.FindGameObjectWithTag("Player");

            if (_player)
            {
                _playerInteractor = _player.GetComponent<PlayerInteractor>();
                _playerCombatController = _player.GetComponent<PlayerCombatController>();
                _playerMovement = _player.GetComponent<PlayerMovement>();
            }          

            _initialized = true;
        }

        /// <summary>
        /// Show dialogue UI.
        /// </summary>
        public void Show()
        {
            // Initalize object if not initialized
            if (!_initialized) Initialize();

            Cursor.lockState = CursorLockMode.None;

            if (_player)
            {
                // Disable interaction, combat and movement
                _playerInteractor.DisableInteraction();
                _playerCombatController.DisableAttack();
                _playerMovement.DisableInput();
            }

            // Show UI
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide dialogue UI.
        /// </summary>
        public void Hide()
        {
            Cursor.lockState = CursorLockMode.Locked;

            if (_player)
            {
                // Enable interaction, combat and movement
                _playerInteractor.EnableInteraction();
                _playerCombatController.EnableAttack();
                _playerMovement.EnableInput();
            }

            // Hide UI
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Call this method to launch a whole new dialogue.
        /// </summary>
        /// <param name="dialogueData"></param>
        /// <param name="avatarSprite"></param>
        /// <param name="npcName"></param>
        public void StartNewDialogue(DialogueData dialogueData)
        {
            Show();

            _currentDialogue = dialogueData;
            CharacterData character = dialogueData.characterData;
            if (character.avatar)
                AvatarSprite = character.avatar;
            NpcName = character.characterName;

            StartDialogue(dialogueData);
        }

        /// <summary>
        /// Begins dialogue and sets text, sprite and choices
        /// </summary>
        public void StartDialogue(DialogueData nextDialogue)
        {
            _currentDialogue = nextDialogue;

            if (_currentDialogue.characterData)
            {
                CharacterData character = _currentDialogue.characterData;

                AvatarSprite = character.avatar;
                NpcName = character.characterName;
            }

            EventSystem.current.SetSelectedGameObject(gameObject);

            string currentText = "";

            // Set language depending from LocalizationManager
            switch (LocalizationManager.s_Instance.Language)
            {
                case Language.English:
                    currentText = _currentDialogue.dialogueText.english;
                    break;
                case Language.French:
                    currentText = _currentDialogue.dialogueText.french;
                    break;
            }

            // 
            dialogueTextMesh.text = currentText;

            avatarImage.sprite = AvatarSprite;
            avatarImage.preserveAspect = true;

            npcNameTextMesh.text = NpcName;

            ClearChoices();

            foreach (ChoiceData c in _currentDialogue.choices)
            {
                AddChoice(c);
            }

            Canvas.ForceUpdateCanvases();

            ShowChoices(false);

            _textAnimCoroutine = StartCoroutine(TextAnimation());
        }

        /// <summary>
        /// Text animation coroutine.
        /// </summary>
        /// <returns></returns>
        private IEnumerator TextAnimation()
        {
            _dialogueAnim = true;

            float letterDelay = 1f / letterRate;
            _dialogueText = dialogueTextMesh.text;

            dialogueTextMesh.text = "";

            foreach (char c in _dialogueText)
            {
                dialogueTextMesh.text += c;
                yield return new WaitForSeconds(letterDelay);
            }

            OnTextAnimEnd();
        }

        /// <summary>
        /// Called when text animation ends.
        /// </summary>
        private void OnTextAnimEnd()
        {
            StopCoroutine(_textAnimCoroutine);

            _dialogueAnim = false;

            ShowChoices(true);

            dialogueTextMesh.text = _dialogueText;

            if (_choices.Count > 0)
            {
                EventSystem.current.SetSelectedGameObject(_choices[0].gameObject);
            }
        }

        /// <summary>
        /// Instantiate choice on dialogue panel.
        /// </summary>
        /// <param name="choiceData">Choice data containing text and next dialogue.</param>
        private void AddChoice(ChoiceData choiceData)
        {
            DialogueChoice newChoice = Instantiate(choicePrefab, choicesParent).GetComponent<DialogueChoice>();

            newChoice.SetChoice(choiceData);

            _choices.Add(newChoice);
        }

        /// <summary>
        /// Clear every choice from dialogue panel.
        /// </summary>
        public void ClearChoices()
        {
            _choices.Clear();

            foreach (Transform child in choicesParent)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Show choices on dialogue panel.
        /// </summary>
        /// <param name="enabled"></param>
        public void ShowChoices(bool enabled)
        {
            foreach (var choice in _choices)
            {
                choice.gameObject.SetActive(enabled);
            }

            Canvas.ForceUpdateCanvases();
        }

        /// <summary>
        /// Called when clicking on dialogue panel.
        /// </summary>
        private void OnClick()
        {
            if (_dialogueAnim)
            {
                OnTextAnimEnd();
                return;
            }

            if (_choices.Count == 0)
            {
                if (_currentDialogue.nextDialogue)
                {
                    StartDialogue(_currentDialogue.nextDialogue);
                    return;
                }
                
                // Show player view

                Hide();        
            }
        }

        #region Event system events
        public void OnSubmit(BaseEventData eventData)
        {
            OnClick();
        }

        public void OnCancel(BaseEventData eventData)
        {
            OnClick();            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        private enum TestDialogue
        {
            YuleCat,
            Krampus,
            Namahage,
            FlavorText,
            SantaEnding1,
            SantaEnding2,
            Example
        }

        [ContextMenu("Start example dialogue")]
        public void EditorStartDialogue()
        {
            DialogueData dialogueData = Resources.Load<DialogueData>($"Data/DialogueSystem/{testDialogue.ToString()}/Dialogue_{testDialogue.ToString()}_0");

            StartNewDialogue(dialogueData);
        }
#endif
#endregion
    }
}