using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ObjectiveCriterion))]
public class ObjectiveCriterionDrawer: PropertyDrawer
{

    private const float LINE_SPACING = 6f;
    private const float LINE_HEIGHT = 8f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        EditorGUI.BeginProperty(position, label, property);

        var fieldProp           = property.FindPropertyRelative("progressionField");
        var fieldTypeProp       = property.FindPropertyRelative("progressionFieldType");
        var comparisonProp      = property.FindPropertyRelative("valueComparison");
        var negateProp          = property.FindPropertyRelative("negate");
        var boolTargetProp      = property.FindPropertyRelative("boolTarget");
        var intTargetProp       = property.FindPropertyRelative("intTarget");
        var floatTargetProp     = property.FindPropertyRelative("floatTarget");
        var stringTargetProp    = property.FindPropertyRelative("stringTarget");

        var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // 0) Progression field
        EditorGUI.PropertyField(line, fieldProp);
        line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;

        // 1) Progression field type + comparison on first line
        EditorGUI.PropertyField(line, fieldTypeProp);
        line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;
        EditorGUI.PropertyField(line, comparisonProp);
        line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;

        // 2) Decide what to show based on field + comparison
        var fieldType   = (EnumProgressionFieldType)fieldTypeProp.enumValueIndex;
        var comparison  = (EnumValueComparison)comparisonProp.enumValueIndex;

        bool showBool   = false;
        bool showInt    = false;
        bool showFloat  = false;
        bool showString = false;

        switch (fieldType)
        {
            case EnumProgressionFieldType.BOOL:
                showBool = true;
                break;
            case EnumProgressionFieldType.INT:
                showInt = true;
                break;
            case EnumProgressionFieldType.FLOAT:
                showFloat = true;
                break;
            case EnumProgressionFieldType.STRING:
                showString = true;
                break;
            case EnumProgressionFieldType.STRING_LIST:
                showString = true;
                break;
        }

        // Constrain comparisons
        if (fieldType == EnumProgressionFieldType.BOOL)
        {
            if (comparison != EnumValueComparison.EQUAL)
            {
                EditorGUI.HelpBox(line, "EQUAL is the only valid comparison for the BOOL value type", MessageType.Error);
                line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;
            }
        }
        if (fieldType == EnumProgressionFieldType.INT)
        {
            if (comparison == EnumValueComparison.CONTAINS)
            {
                EditorGUI.HelpBox(line, "CONTAINS is not a valid comparison for the INT value type", MessageType.Error);
                line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;
            }
        }
        if (fieldType == EnumProgressionFieldType.FLOAT)
        {
            if (comparison == EnumValueComparison.CONTAINS)
            {
                EditorGUI.HelpBox(line, "CONTAINS is not a valid comparison for the FLOAT value type", MessageType.Error);
                line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;
            }
        }
        if (fieldType == EnumProgressionFieldType.STRING)
        {
            if (comparison != EnumValueComparison.EQUAL)
            {
                EditorGUI.HelpBox(line, "EQUAL is the only valid comparison for the STRING value type", MessageType.Error);
                line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;
            }
        }
        if (fieldType == EnumProgressionFieldType.STRING_LIST)
        {
            if (comparison != EnumValueComparison.CONTAINS)
            {
                EditorGUI.HelpBox(line, "CONTAINS is the only valid comparison for the STRING_LIST value type", MessageType.Error);
                line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;
            }
        }


        // Negate toggle
        EditorGUI.PropertyField(line, negateProp);
        line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;


        // 3) Draw only the relevant target field
        if (showBool)
        {
            EditorGUI.PropertyField(line, boolTargetProp, new GUIContent("Bool Target"));
            line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING * 2f;
        }
        if (showInt)
        {
            EditorGUI.PropertyField(line, intTargetProp, new GUIContent("Int Target"));
            line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;
        }
        if (showFloat)
        {
            EditorGUI.PropertyField(line, floatTargetProp, new GUIContent("Float Target"));
            line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;
        }
        if (showString)
        {
            EditorGUI.PropertyField(line, stringTargetProp, new GUIContent("String Target"));
            line.y += EditorGUIUtility.singleLineHeight + LINE_SPACING;
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * LINE_HEIGHT;
    }
}
