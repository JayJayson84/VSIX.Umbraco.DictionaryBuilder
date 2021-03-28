namespace System.ComponentModel.ExtendedAttributes
{
    /// <summary>
    /// Specifies the name of the category in which to group the propery or event when displayed in a <see cref="Windows.Forms.PropertyGrid"/> control set to categorized mode, and the order in which it should appear.
    /// </summary>
    internal sealed class SortableCategoryAttribute : CategoryAttribute
    {
        private const char NonPrintableChar = '\t';

        public SortableCategoryAttribute(string category, ushort categoryIndex, ushort categoryCount = 0)
            : base(category.PadLeft(category.Length + (categoryCount - categoryIndex), NonPrintableChar)) { }
    }
}
