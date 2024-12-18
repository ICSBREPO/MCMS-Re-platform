namespace McmsApp.Controls
{
    /// <summary>
    /// This class extends the behavior of the SearchableListView control to filter the ListViewItem based on text.
    /// </summary>
    public class SearchableWorkorderList : SearchableListView
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
                var taskInfo = obj as Models.Workorder;
                var date = Convert.ToString(taskInfo.statusdate);
                if (taskInfo.fcprojectid == null)
                {
                    taskInfo.fcprojectid = " ";
                }
                if (taskInfo == null || string.IsNullOrEmpty(taskInfo.wonum) || string.IsNullOrEmpty(taskInfo.fcprojectid) || string.IsNullOrEmpty(taskInfo.description) || string.IsNullOrEmpty(date))
                {
                    return false;
                }
                return taskInfo.wonum.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant())
                       || taskInfo.fcprojectid.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant())
                       || taskInfo.status.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant())
                       || taskInfo.description.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant())
                       || date.ToUpperInvariant().Contains(this.SearchText.ToUpperInvariant());
            }
            return false;
        }

        #endregion
    }
}
