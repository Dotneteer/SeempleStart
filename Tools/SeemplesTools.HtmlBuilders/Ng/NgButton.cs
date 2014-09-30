using System;
using System.Collections.Generic;
using SeemplesTools.HtmlBuilders.Bs;
using SeemplesTools.HtmlBuilders.Infrastructure;

namespace SeemplesTools.HtmlBuilders.Ng
{
    public class NgButton : NgHtmlElement<NgButton>
    {
        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="value">Button value</param>
        /// <param name="theme">Button theme</param>
        public NgButton(string value, BsButtonTheme theme = BsButtonTheme.Default)
            : base(HtmlTag.Button)
        {
            InitInstance(value, theme);
        }

        /// <summary>
        /// Initializes a new instance with the specified extra attributes
        /// </summary>
        /// <param name="value">Button value</param>
        /// <param name="theme">Button theme</param>
        /// <param name="attribs">Attribute enumeration</param>
        public NgButton(string value, BsButtonTheme theme, IDictionary<string, object> attribs)
            : base(HtmlTag.Button, attribs)
        {
            InitInstance(value, theme);
        }

        /// <summary>
        /// Initializes a new instance with the specified extra attributes
        /// </summary>
        /// <param name="value">Button value</param>
        /// <param name="theme">Button theme</param>
        /// <param name="attribs">Attribute enumeration</param>
        public NgButton(string value, BsButtonTheme theme, params object[] attribs)
            : base(HtmlTag.Button, attribs)
        {
            InitInstance(value, theme);
        }

        /// <summary>
        /// Initializes the instance with the specified value and theme
        /// </summary>
        /// <param name="value"></param>
        /// <param name="theme"></param>
        private void InitInstance(string value, BsButtonTheme theme)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            Text = value;
            Theme = theme;
        }

        /// <summary>
        /// Gets or sets the value of the button
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets the theme of the value;
        /// </summary>
        public BsButtonTheme Theme { get; private set; }

        /// <summary>
        /// Gets the alignment of the button icon
        /// </summary>
        public BsIconAlignment IconAlignment { get; private set; }

        /// <summary>
        /// Gets the optional icon name associated with the button
        /// </summary>
        public string IconName { get; private set; }

        /// <summary>
        /// Sets the alignment of an optional button
        /// </summary>
        /// <param name="iconName">Name of the icon</param>
        /// <param name="align">Icon alignment</param>
        /// <returns>This instance</returns>
        public NgButton Icon(string iconName, BsIconAlignment align = BsIconAlignment.Left)
        {
            IconName = iconName;
            IconAlignment = align;
            return this;
        }
    
}
}