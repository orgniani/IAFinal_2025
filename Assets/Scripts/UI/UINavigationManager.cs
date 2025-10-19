using System.Collections.Generic;
using UnityEngine;
using UI.Menus;
using Events;
using Helpers;
using UI.Buttons;
using DataSource;
using Scenery;
using System;
using Audio;

namespace UI
{
    public class UINavigationManager : MonoBehaviour
    {
        [Header("Data Source")]
        [SerializeField] private DataSource<FlowManager> flowManagerDataSource;

        [Header("Subscribe to Events")]
        [SerializeField] private EmptyAction onLoseEvent;
        [SerializeField] private EmptyAction onLoadEndEvent;

        [Header("Invoke Events")]
        [SerializeField] private AudioEvent onPlayAudio;

        [Header("Logs")]
        [SerializeField] private bool enableLogs = true;

        private UIMenuID _currentMenuID = UIMenuID.Main;
        private UIMenuID _backTargetID = UIMenuID.Main;

        private FlowManager _flowManager;
        private UINavigationFlowHandler _flowHandler;

        private readonly Dictionary<UIMenuID, IMenu> _menus = new();
        private readonly Dictionary<UIButtonAction, Action<UIMenuID?>> _buttonHandlers = new();

        /// <summary>Sets the default back target for shared UI menus (e.g., Settings/Shop).</summary>
        public void SetBackTarget(UIMenuID id) => _backTargetID = id;

        private void Awake()
        {
            ValidateReferences();
        }

        private void OnEnable()
        {
            onLoseEvent?.Subscribe(() => HandleNavigation(UIMenuID.Lose));

            if (flowManagerDataSource.DataInstance)
                _flowManager = flowManagerDataSource.DataInstance;
        }

        private void Start()
        {
            InitializeMenusFromChildren();
            InitializeButtonHandlers();

            _flowHandler = new UINavigationFlowHandler(this, onLoadEndEvent);
        }

        private void OnDisable()
        {
            onLoseEvent?.Unsubscribe(() => HandleNavigation(UIMenuID.Lose));

            foreach (var menu in _menus.Values)
            {
                if (menu is UIMenu uiMenu)
                    uiMenu.OnButtonAction -= HandleButtonAction;
            }
        }

        /// <summary>
        /// Finds and registers all UIMenus in children, hides them, then shows the main menu.
        /// </summary>
        private void InitializeMenusFromChildren()
        {
            _menus.Clear();
            var allMenus = GetComponentsInChildren<UIMenu>(includeInactive: true);

            foreach (var menu in allMenus)
            {
                var id = menu.MenuID;

                if (_menus.ContainsKey(id))
                {
                    if (enableLogs) Debug.LogWarning($"{name}: Duplicate menu id {id}. Skipping.");
                    continue;
                }

                menu.Setup();
                menu.OnButtonAction += HandleButtonAction;
                menu.ToggleVisibility(false);

                _menus[id] = menu;
            }

            if (_menus.TryGetValue(UIMenuID.Main, out var main))
            {
                main.ToggleVisibility(true);
                _currentMenuID = UIMenuID.Main;
            }
        }

        /// <summary>
        /// Initializes the button actions and binds them to their associated logic.
        /// </summary>
        private void InitializeButtonHandlers()
        {
            _buttonHandlers[UIButtonAction.None] = id =>
            { if (id.HasValue) HandleNavigation(id.Value); };

            _buttonHandlers[UIButtonAction.Play] = id =>
                BeginLevelTransition(id, () => _flowManager?.LoadCurrentLevel());

            _buttonHandlers[UIButtonAction.Restart] = id =>
                BeginLevelTransition(id, () => _flowManager?.ReloadCurrentLevel());

            _buttonHandlers[UIButtonAction.BackFromGame] = id =>
                BeginLevelTransition(id, () => _flowManager?.UnloadCurrentLevel());

            _buttonHandlers[UIButtonAction.BackFromMenu] = _ =>
            {
                var target = GetMenuFromID(_backTargetID);
                if (target != null)
                {
                    CloseAllMenus();
                    target.ToggleVisibility(true);
                    _currentMenuID = _backTargetID;
                }
            };

        }

        /// <summary>Handles button clicks passed from UIMenus.</summary>
        private void HandleButtonAction(UIButtonAction action, UIMenuID? menuID)
        {
            onPlayAudio?.InvokeEvent(AudioKey.Button_Click);

            if (_buttonHandlers.TryGetValue(action, out var handler))
                handler.Invoke(menuID);
        }

        /// <summary>
        /// Handles full-screen and popup menu toggling depending on menu type.
        /// </summary>
        private void HandleNavigation(UIMenuID id)
        {
            if (!_menus.TryGetValue(id, out var menu)) return;

            switch (menu.MenuType)
            {
                case UIMenuType.Full:
                    SwitchToMenu(id);
                    break;

                case UIMenuType.PopUp:
                    menu.ToggleVisibility(!menu.IsVisible);
                    break;
            }
        }

        /// <summary>
        /// Switches to a new menu and hides all others. 
        /// Updates back target if applicable.
        /// </summary>
        private void SwitchToMenu(UIMenuID newID)
        {
            var newMenu = GetMenuFromID(newID);
            if (newMenu == null)
                return;

            if (_currentMenuID == newID)
            {
                newMenu.ToggleVisibility(!newMenu.IsVisible);
                return;
            }

            if (newMenu.SupportsBackNavigation)
                SetBackTarget(_currentMenuID);

            CloseAllMenus();

            newMenu.ToggleVisibility(true);
            _currentMenuID = newID;
        }

        /// <summary>
        /// Begins level flow operation, defers menu opening until the level finishes loading/unloading.
        /// </summary>
        private void BeginLevelTransition(UIMenuID? menuID, Action flowAction)
        {
            _flowHandler.Begin(menuID, flowAction, () =>
            {
                CloseAllMenus();
                if (menuID.HasValue)
                    HandleNavigation(menuID.Value);
            });
        }

        private void CloseAllMenus()
        {
            foreach (var menu in _menus.Values)
                menu.ToggleVisibility(false);
        }

        private IMenu GetMenuFromID(UIMenuID id)
        {
            if (_menus.TryGetValue(id, out var menu))
                return menu;

            if (enableLogs) Debug.LogWarning($"{name}: Menu with ID '{id}' not found.");
            return null;
        }

        private void ValidateReferences()
        {
            ReferenceValidator.Validate(flowManagerDataSource, nameof(flowManagerDataSource), this);
            ReferenceValidator.Validate(onLoadEndEvent, nameof(onLoadEndEvent), this);
            ReferenceValidator.Validate(onLoseEvent, nameof(onLoseEvent), this);
        }
    }
}