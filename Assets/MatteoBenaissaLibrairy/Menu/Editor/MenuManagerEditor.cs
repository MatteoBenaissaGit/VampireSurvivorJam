namespace Menu.Editor
{
    
    using UnityEditor;
    using UnityEngine;

#if UNITY_EDITOR
    [CustomEditor(typeof(MenuManager))]
    public class MenuManagerEditor : UnityEditor.Editor
    {
        private MenuManager menuManagerScript;

        private void OnEnable()
        {
            menuManagerScript = (MenuManager)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawDefaultInspector();

            //buttons
            if (GUILayout.Button("Set Menu Type Simple"))
            {
                menuManagerScript.SetMenuTypeSimple();
            }

            if (GUILayout.Button("Set Menu Type Side Slide"))
            {
                menuManagerScript.SetMenuTypeSideSlide();
            }

            EditorUtility.SetDirty(menuManagerScript);
            
            #region Warnings

            //references check
            MenuReferences simpleMenuReferences = menuManagerScript.SimpleMenuReferences;
            MenuReferences sideSlideMenuReferences = menuManagerScript.SideSlideMenuReferences;

            if (simpleMenuReferences.MenuGameObject == null ||
                simpleMenuReferences.PlayButton == null ||
                simpleMenuReferences.CreditsButton == null ||
                simpleMenuReferences.QuitButton == null ||
                sideSlideMenuReferences.MenuGameObject == null ||
                sideSlideMenuReferences.PlayButton == null ||
                sideSlideMenuReferences.CreditsButton == null ||
                sideSlideMenuReferences.QuitButton == null)
            {
                EditorGUILayout.HelpBox("References missing", MessageType.Warning, true);
            }

            if (string.IsNullOrEmpty(menuManagerScript.PlaySceneName) ||
                string.IsNullOrEmpty(menuManagerScript.CreditSceneName))
            {
                EditorGUILayout.HelpBox("Scenes names missing", MessageType.Warning, true);
            }

            #endregion

            serializedObject.ApplyModifiedProperties();
        }
    }

#endif
}