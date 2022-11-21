using System.Collections.Generic;

namespace Menu
{
    
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using DG.Tweening;

    [Serializable]
    public enum MenuType
    {
        Simple = 0,
        SideSlide = 1
    }

    public enum ButtonType
    {
        Play = 0,
        Credits = 1,
        Quit = 2
    }

    [Serializable]
    public struct MenuReferences
    {
        [Tooltip("Reference menu game object")]
        public GameObject MenuGameObject;
        
        [Tooltip("Reference menu play button transform")]
        public ButtonManager PlayButton;
        
        [Tooltip("Reference menu credits button transform")]
        public ButtonManager CreditsButton;
        
        [Tooltip("Reference menu quit button transform")]
        public ButtonManager QuitButton;
    }
    
    [Serializable]
    public class MenuManager : MonoBehaviour
    {
        #region References & Parameters

        //serialized
        
        [Header("Menus references")]
        
        [SerializeField, Tooltip("Reference the simple menu's references")]
        public MenuReferences SimpleMenuReferences;

        [SerializeField, Tooltip("Reference the side-slide menu's references")]
        public MenuReferences SideSlideMenuReferences;

        
        [Space(10), Header("Scenes names")]
        
        [SerializeField, Tooltip("name of the play scene")]
        public string PlaySceneName = string.Empty;
        
        [SerializeField, Tooltip("name of the credit scene")]
        public string CreditSceneName = string.Empty;

        
        [Space(10), Header("Parameters")]

        [SerializeField, Tooltip("If true, button will complete their animation")]
        private bool _makeButtonAnimation;
        
        [SerializeField, Tooltip("Chose the seconds to wait before launching the scene")]
        private float _launchSceneTimeInSeconds;
        
        [SerializeField, Range(0,3), Tooltip("The time it takes for the button animation to end")]
        private float _buttonAnimationTimeInSeconds;
        
        [Space(10), Header("Menu Type")]

        [SerializeField, ReadOnly, Tooltip("The current menu type, debug only !")]
        private MenuType _currentMenuType;

        //private

        private bool _buttonClicked;
        private List<ButtonManager> _buttonManagerList = new List<ButtonManager>();

        #endregion

        private void Start()
        {
            _buttonManagerList.Add(SimpleMenuReferences.PlayButton);
            _buttonManagerList.Add(SimpleMenuReferences.CreditsButton);
            _buttonManagerList.Add(SimpleMenuReferences.QuitButton);
            _buttonManagerList.Add(SideSlideMenuReferences.PlayButton);
            _buttonManagerList.Add(SideSlideMenuReferences.CreditsButton);
            _buttonManagerList.Add(SideSlideMenuReferences.QuitButton);
        }

        #region Methods

        public void SetMenuTypeSimple()
        {
            SetMenuType(MenuType.Simple);
        }
        public void SetMenuTypeSideSlide()
        {
            SetMenuType(MenuType.SideSlide);
        }

        private void SetMenuType(MenuType menuType)
        {
            SimpleMenuReferences.MenuGameObject.SetActive(menuType == MenuType.Simple);
            SideSlideMenuReferences.MenuGameObject.SetActive(menuType == MenuType.SideSlide);

            _currentMenuType = menuType;
        }

        public void ButtonAction(int buttonType)
        {
            //guard
            if (_buttonClicked) return;
            
            //block other animations
            _buttonClicked = true;
            _buttonManagerList.ForEach(x => x.CanAnimate = false);
            
            //quit
            if (buttonType == (int)ButtonType.Quit)
            {
                Application.Quit();
            }

            bool isPlayButton = buttonType == (int)ButtonType.Play;

            //button animation
            if (_makeButtonAnimation)
            {
                switch (_currentMenuType)
                {
                    case MenuType.Simple:
                        SimpleButtonClickAnimation(isPlayButton ?
                            SimpleMenuReferences.PlayButton.transform :
                            SimpleMenuReferences.CreditsButton.transform);
                        break;
                
                    case MenuType.SideSlide:
                        SlideButtonClickAnimation(isPlayButton ?
                            SideSlideMenuReferences.PlayButton.transform :
                            SideSlideMenuReferences.CreditsButton.transform);
                        break;
                }
            }

            //change scene
            StartCoroutine(GoToScene(isPlayButton ? PlaySceneName : CreditSceneName));
        }

        private void SimpleButtonClickAnimation(Transform buttonTransform)
        {
            Vector3 punchScaleForce = Vector3.one * 0.25f;

            buttonTransform.DOPunchScale(punchScaleForce, _buttonAnimationTimeInSeconds);
        }
        
        private void SlideButtonClickAnimation(Transform buttonTransform)
        {
            const float animationRangeX = 1f;

            buttonTransform.DOMoveX(buttonTransform.position.x + animationRangeX, _buttonAnimationTimeInSeconds);
        }

        private IEnumerator GoToScene(string sceneName)
        {
            yield return new WaitForSeconds(_launchSceneTimeInSeconds);
            
            if (string.IsNullOrEmpty(sceneName) == false)
            {
                Debug.Log($"Go to scene : {sceneName}");
                SceneManager.LoadScene(sceneName);
            }
        }

        #endregion

    }
}
