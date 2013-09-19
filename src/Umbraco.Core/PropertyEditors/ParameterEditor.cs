﻿using System;
using Newtonsoft.Json;
using Umbraco.Core.IO;

namespace Umbraco.Core.PropertyEditors
{
    /// <summary>
    /// Basic definition of a macro parameter editor
    /// </summary>
    public class ParameterEditor
    {

        private readonly ParameterEditorAttribute _attribute;

        /// <summary>
        /// The constructor will setup the property editor based on the attribute if one is found
        /// </summary>
        public ParameterEditor()             
        {
            //assign properties based on the attribute if it is found
            _attribute = GetType().GetCustomAttribute<ParameterEditorAttribute>(false);
            if (_attribute != null)
            {
                //set the id/name from the attribute
                Alias = _attribute.Alias;
                Name = _attribute.Name;
            }
        }

        /// <summary>
        /// These are assigned by default normally based on parameter editor attributes or manifest definitions,
        /// developers have the chance to override CreateValueEditor if they don't want to use the pre-defined instance
        /// </summary>
        internal ParameterValueEditor ManifestDefinedParameterValueEditor = null;

        /// <summary>
        /// The id  of the property editor
        /// </summary>
        [JsonProperty("alias", Required = Required.Always)]
        public string Alias { get; internal set; }

        /// <summary>
        /// The name of the property editor
        /// </summary>
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; internal set; }

        /// <summary>
        /// Creates a value editor instance
        /// </summary>
        /// <returns></returns>
        protected virtual ParameterValueEditor CreateValueEditor()
        {
            if (ManifestDefinedParameterValueEditor != null)
            {
                //detect if the view is a virtual path (in most cases, yes) then convert it
                if (ManifestDefinedParameterValueEditor.View.StartsWith("~/"))
                {
                    ManifestDefinedParameterValueEditor.View = IOHelper.ResolveUrl(ManifestDefinedParameterValueEditor.View);
                }
                return ManifestDefinedParameterValueEditor;
            }

            //create a new editor
            var editor = new ParameterValueEditor();

            if (_attribute.EditorView.IsNullOrWhiteSpace())
            {
                throw new NotImplementedException("This method must be implemented if a view is not explicitly set");
            }

            editor.View = _attribute.EditorView;
            return editor;
        }
    }
}