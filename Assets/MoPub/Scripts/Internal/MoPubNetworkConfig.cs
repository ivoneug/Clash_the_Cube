using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MJ = MoPubInternal.ThirdParty.MiniJSON;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// A base class for mediated network packages to use in extending the MoPubManager "control panel" prefab.
/// Add any fields desired to present a configuration interface to the user in the Unity editor.
/// At runtime, the MoPubManager script iterates over all NetworkConfigs on the same gameobject, and retrieves
/// the NetworkOptions property of each one.  These are added into the SdkConfiguration object it builds.
/// </summary>
/// <remarks>The fields need to be JSON-serializable types.</remarks>
public abstract class MoPubNetworkConfig : MonoBehaviour
{
    // To get the 'enable' toggle to show up in the inspector, the script has to have either a Start() or
    // an Update() function, even if it does nothing.
    void Start() {}


    /// <summary>
    /// Define this property in the derived class to return the Java/Objective-C class name of the
    /// AdapterConfiguration-derived class.  For Android, this must include the full package name.
    /// </summary>
    public abstract string AdapterConfigurationClassName { get; }


    /// <summary>
    /// Override this property in the derived class to return the Java/Objective-C class name of the
    /// class that implements IGlobalMediationSettings (if there is one).
    /// </summary>
    public virtual string MediationSettingsClassName { get { return null; } }


    /// <summary>
    /// Returns a MoPub.MediatedNetwork object filled in with information from the fields of the derived class.
    /// You can override this property in the derived class in case you need to replace or add to the logic here.
    /// </summary>
    public virtual MoPub.MediatedNetwork NetworkOptions {
        get {
            // Fill in the class name and dictionary entries.
            var options = new MoPub.MediatedNetwork {
                AdapterConfigurationClassName = AdapterConfigurationClassName,
                MediationSettingsClassName = MediationSettingsClassName,
                NetworkConfiguration = new Dictionary<string, string>(),
                MediationSettings = new Dictionary<string, object>(),
                MoPubRequestOptions = new Dictionary<string, string>()
            };
            // Find fields marked with the attributes defined further below, and add them as well.
            FillInMarkedFields(options);
            return options;
        }
    }


    // Unity treats [PropertyAttribute] on container types (lists, arrays) as if the attribute is instead on each
    // element of the container.  We don't want that behavior for these attributes.  So unfortunately we have to work
    // around this by using a non-array / non-list container as the field type whenever the actual value is a list
    // or array type.  The following class is a therefore a dumb container of another value, which ensures that any
    // [PropertyAttribute] applied to it is not propagated to the contained value(s).

    public abstract class Container : Wrapper { }
    public abstract class Container<T> : Container // T is array or List of something, generally.
    {
        // Here's the actual value.  Both the OptionDrawer function and the NetworkOptions property know to edit/serialize
        // this value instead of the whole Container object.

        public T actualValue;

        public override object value { get { return actualValue; } }
    }

    // The actual fields must have non-generic types with the [Serializable] attribute.  So we declare explicit
    // instantiations of typical field types here.

    [Serializable] public class Strings : Container<string[]> { }
    [Serializable] public class Ints    : Container<int[]>    { }


    // Sometimes a field will need to have different values on Android vs iOS.  This class makes a simple template
    // for any type, which provides separate value entries for each platform.  At runtime the matching platform's
    // value is selected.

    public abstract class PlatformSpecific : Wrapper { }
    public abstract class PlatformSpecific<T> : PlatformSpecific
    {
        public T android;
        public T ios;

        public override object value
        {
            get { return Application.platform == RuntimePlatform.Android ? android : ios; }
        }
    }

    // Similarly with the Container instantiations above, we have to make non-generic types with the [Serializable]
    // attribute for each specific field type that is needed.

    [Serializable] public class PlatformSpecificString  : PlatformSpecific<string>   { }
    [Serializable] public class PlatformSpecificStrings : PlatformSpecific<string[]> { }


    // The above Container and PlatformSpecific classes share this class as base, to centralize how the value is
    // obtained during JSON serialization.

    public abstract class Wrapper
    {
        public abstract object value { get; }

        // While we're at it, override the ToString() property to help with passing the T value in the NetworkConfiguration
        // dictionary (which really only accepts string values).  Any type other than string is serialized as JSON.
        // (The platform-level adapter's initialize() function will have to know about this, of course.)

        public override string ToString()
        {
            var val = value;
            return val is string ? (string)val : MJ.Json.Serialize(val);
        }
    }


    // The following classes create the attributes:
    //    [Config.Optional]    -- mark a field as optional and to be added to the NetworkConfiguration dictionary.
    //    [Config.Required]    -- mark a field as required and to be added to the NetworkConfiguration dictionary.
    //    [Mediation.Optional] -- mark a field as optional and to be added to the MediationSettings dictionary.
    //    [Mediation.Required] -- mark a field as required and to be added to the MediationSettings dictionary.

    // We will override how Unity presents fields marked with these attributes, to include a toggle that indicates
    // whether to activate the field so that a value can be entered.  This value will be added to the appropriate
    // dictionary, with the field's name as the key (this name is overridable in the attribute).

    public abstract class SettingAttribute : PropertyAttribute
    {
        // The key to be used when adding the field's value to the appropriate dictionary.  If null (the default),
        // then the actual name of the field (variable) itself is the key.
        public string name;
        protected SettingAttribute(string name) { this.name = name; }
    }


    // Abstract attribute indicating that the field goes in the NetworkConfiguration dictionary.
    // It is only used to 1) provide a namespace for the Optional and Required subclasses, so they are distinct
    // from the similar ones in the Mediation class (below);  2)  Using the 'is' operator to check if either of
    // those attributes is a Config type vs a Mediation type, for selecting the correct dictionary at runtime.

    public abstract class Config : SettingAttribute
    {
        protected Config(string name = null) : base(name) { }

        public class Optional : Config
        {
            public Optional() { }  // Explicit default constructor prevents code completion from eagerly inserting '()'.
            public Optional(string name) : base(name) { }
        }

        public class Required : Config
        {
            public Required() { }
            public Required(string name) : base(name) { }
        }
    }


    // Abstract attribute indicating that the field goes in the MediationSettings dictionary.  See comment on Config
    // above for details.

    public abstract class Mediation : SettingAttribute
    {
        protected Mediation(string name = null) : base(name) { }

        public class Optional : Mediation
        {
            public Optional() { }
            public Optional(string name) : base(name) { }
        }

        public class Required : Mediation
        {
            public Required() { }
            public Required(string name) : base(name) { }
        }
    }


    // The state of each optional field's toggle is saved by adding the field name / key to this list.
    // The OptionDrawer class below manages its value as the user toggles the enabled states of various fields.
    [SerializeField]
    [HideInInspector]
    protected List<string> enabledOptions;


#if UNITY_EDITOR
    // Intercept how Unity draws the inspector for any fields marked with the above attributes.
    [CustomPropertyDrawer(typeof(SettingAttribute), useForChildren:true)]
    public class OptionDrawer : PropertyDrawer
    {
#if UNITY_2018_1_OR_NEWER
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            // Caching the inspector GUI causes weird scrolling artifacts with this drawer, so disable it.
            return false;
        }
#endif

        // If the field is optional and not selected for inclusion, force "include children" to false.
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Find the selectedOptions list for this object.
            var target = property.serializedObject.targetObject as MoPubNetworkConfig;
            var targetEnabledOptions = target.enabledOptions;

            // Get the key name that will be used in the dictionary.
            var attr = (SettingAttribute) attribute;
            var name = attr.name ?? property.name;

            // Is this field currently enabled?
            var isRequired = attr is Config.Required || attr is Mediation.Required;
            var isEnabled = isRequired || targetEnabledOptions != null && targetEnabledOptions.Contains(name);

            // Is this field one of our Wrapper types?
            var isContainer = fieldInfo.FieldType.IsSubclassOf(typeof(Container));
            //var isPlatformSpecific = !isContainer && fieldInfo.FieldType.IsSubclassOf(typeof(PlatformSpecific));

            // Make the foldout state match the enabled state so the user doesn't have to click two things to see
            // the fields of a container / collection / struct etc.
            property.isExpanded = isEnabled;

            // Also, for Container-derived types, only include the height of the inner 'value' field, since
            // that is all that OnGUI() below will draw.
            if (isContainer) {
                property = property.FindPropertyRelative("value");
                property.isExpanded = isEnabled;  // Also set the inner foldout.
            }

            return EditorGUI.GetPropertyHeight(property, label, isEnabled);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Find the selectedOptions list for this object.
            var target = property.serializedObject.targetObject as MoPubNetworkConfig;
            var targetEnabledOptions = target.enabledOptions;

            // Get the key name that will be used in the dictionary.
            var attr = (SettingAttribute) attribute;
            var name = attr.name ?? property.name;

            // Is this field currently enabled?  (And required?)
            var isRequired = attr is Config.Required || attr is Mediation.Required;
            var isEnabled = isRequired || targetEnabledOptions != null && targetEnabledOptions.Contains(name);

            // Is this field one of our Wrapper types?
            var isContainer = fieldInfo.FieldType.IsSubclassOf(typeof(Container));
            var isPlatformSpecific = !isContainer && fieldInfo.FieldType.IsSubclassOf(typeof(PlatformSpecific));

            // Don't draw the Container, only the value it contains (see Container class comment above).
            if (isContainer)
                property = property.FindPropertyRelative("value");

            // Space for the toggle.
            var isEnabledRect = position;
            isEnabledRect.width = EditorGUIUtility.singleLineHeight;

            // Draw the toggle and handle a change in its state.  In the case of Required fields, the toggle is still
            // drawn but it is disabled.
            using (var check = new EditorGUI.ChangeCheckScope()) {
                using (new EditorGUI.DisabledScope(isRequired))
                    isEnabled = EditorGUI.Toggle(isEnabledRect, GUIContent.none, isEnabled);
                if (check.changed) {
                    Undo.RecordObject(target, "Select Option");
                    if (targetEnabledOptions == null)
                        // First time any one of the toggles gets enabled, the list will be null.
                        targetEnabledOptions = target.enabledOptions = new List<string>();
                    if (isEnabled)
                        targetEnabledOptions.Add(name);
                    else
                        targetEnabledOptions.Remove(name);
                    if (isPlatformSpecific) {
                        // Automatically fold/unfold the PlatformSpecific values to match the enabled state.
                        // This is just a time saver when enabling / disabling the field.
                        var android = property.FindPropertyRelative("android");
                        var ios = property.FindPropertyRelative("ios");
                        if (android != null)
                            android.isExpanded = isEnabled;
                        if (ios != null)
                            ios.isExpanded = isEnabled;
                    }
                }
            }

            // If the type is one of our PlatformSpecific's, append "Per Platform" to the label so it is obvious even
            // if the foldout triangle is closed.
            var label2 = label;
            if (isPlatformSpecific) {
                label2 = new GUIContent(label);
                label2.text += " (Per Platform)";
            }

            // Now draw the actual field (label + value) next to the toggle.  Have to indent the label, and also disable
            // the value entry if the field is not enabled.
            using (new EditorGUI.IndentLevelScope(property.hasVisibleChildren ? 2 : 1))
            using (new EditorGUI.DisabledScope(!isEnabled))
                EditorGUI.PropertyField(position, property, label2, isEnabled);
        }
    }
#endif


    // Using reflection, find all fields marked with one of the optional/required attributes defined above,
    // and add entries for them to the correct dictionary (NetworkConfiguration or MediationSettings).

    protected void FillInMarkedFields(MoPub.MediatedNetwork options)
    {
        if (enabledOptions == null)
            enabledOptions = new List<string>();

        // Not just the NetworkConfig script's type, but its superclasses as well.
        for (var type = GetType(); type != typeof(MoPubNetworkConfig); type = type.BaseType)
            foreach (var fi in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                // Does the field have one of our attributes?
                var attr = fi.GetCustomAttributes(typeof(SettingAttribute), false).FirstOrDefault() as SettingAttribute;
                if (attr == null)
                    continue;
                // Get the name (key) for the field.
                var name = attr.name ?? fi.Name;
                // If it is optional, has it been enabled?
                var isOptional = attr is Config.Optional || attr is Mediation.Optional;
                if (isOptional && !enabledOptions.Contains(name))
                    continue;
                // Get the value and add to the appropriate dictionary.
                object value = fi.GetValue(this);
                if (attr is Config)
                    // Convert field value to a string.  (This is where the Wrapper.ToString() is used.)
                    options.NetworkConfiguration[name] = value.ToString();
                else {
                    if (value is Wrapper)
                        // Grab the inner value out of Wrapper types.
                        value = ((Wrapper) value).value;
                    options.MediationSettings[name] = value;
                }
            }
    }
}


/*
Here is an example implementation of this class.  This enables Unity projects to make use of a custom
Java or Objective-C network adapter (which should also be present in the project).

public class MyCustomNetworkConfig : NetworkConfig
{
    // Override the two class name properties to correspond to the Java/Objective-C class names that your
    // network implements to work with the Android/iOS MoPub SDK.

    // Note that for Android, the class names need to be fully qualified (including package name).
    // The names given here are example only.

    public override string AdapterConfigurationClassName
    {
        get { return Application.platform == RuntimePlatform.Android
                  ? "com.my.custom.MyCustomAdapterConfiguration"
                  : "MyCustomAdapterConfiguration"; }
    }

    public override string MediationSettingsClassName
    {
        get { return Application.platform == RuntimePlatform.Android
                  ? "com.my.custom.MyCustomGlobalMediationSettings"
                  : "MyCustomGlobalMediationSettings"; }
    }


    // Define fields for any adapter configuration settings you want passed through from Unity/C# to the
    // platform (Java/ObjC) adapter code.

     // Optional:  You can split fields into named sections using [Header]
    [Header("Network Configuration")]

    // Optional:  You can document the field with [Tooltip]
    [Tooltip("This is the api key for MyCustom network.")]
    [Config.Required] // or .Optional
    public string apiKey;


    [Header("Mediation Settings (Global)")]

    [Tooltip("This is the 'foo' parameter.  Who knows what it does?")]
    [Mediation.Optional] // or .Required
    public bool foo;


    // Highly Optional:  Override the 'NetworkOptions' property to compute a MediatedNetwork structure from your fields.
    // You *only* need to do this if you have special serialization logic that is not handled by our base class.

    public override MoPub.MediatedNetwork NetworkOptions
    {
        get {
            // Get the options using the base class property so it puts the class names in for you, and allocates
            // the dictionaries.  It also fills in any fields marked with [Config.Optional] (etc) attributes.
            var options = base.NetworkOptions;

            // Add other special case logic here.

            return options;
        }
    }
}
*/
