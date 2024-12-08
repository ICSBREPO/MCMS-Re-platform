using System;
namespace mcms.Controls
{
    /// <summary>
    /// This class extends the behavior of the SearchableListView control to filter the ListViewItem based on text.
    /// </summary>
    public class SearchableLookup : SearchableListView
    {
        #region Method 

        /// <summary>
        /// Filtering the list view items based on the search text.
        /// </summary>
        /// <param name="obj">The list view item</param>
        /// <returns>Returns the filtered item</returns>
        public override bool FilterFromList(object obj)
        {
            if (base.FilterFromList(obj))
            {
                var taskInfo = obj as Models.MxDomain;
                if (taskInfo == null || string.IsNullOrEmpty(taskInfo.value) || string.IsNullOrEmpty(taskInfo.description))
                {
                    return false;
                }
                return taskInfo.value.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant())
                       || taskInfo.description.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant());
            }
            return false;
        }

        #endregion
    }
}
