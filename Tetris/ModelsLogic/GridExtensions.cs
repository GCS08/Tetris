namespace Tetris.ModelsLogic
{
    // This static class defines an **attached property** that lets us bind a collection of Views to a Grid.
    // Normally, Grid.Children is NOT bindable, so this is our MVVM-friendly workaround.
    public static class GridExtensions
    {
        // Define the attached BindableProperty called "ChildrenSource".
        // It will hold an IEnumerable<View> — basically, a list of BoxViews for the Tetris board.
        public static readonly BindableProperty ChildrenSourceProperty =
            BindableProperty.CreateAttached(
                "ChildrenSource",                // Name of the property
                typeof(IEnumerable<View>),       // Type of the property
                typeof(GridExtensions),          // Owner type
                null,                             // Default value
                propertyChanged: OnChildrenSourceChanged); // Called when the property value changes

        // Getter for the attached property. Allows XAML or ViewModel to read the current collection.
        public static IEnumerable<View> GetChildrenSource(BindableObject view)
            => (IEnumerable<View>)view.GetValue(ChildrenSourceProperty);

        // Setter for the attached property. Allows XAML or ViewModel to set the collection.
        public static void SetChildrenSource(BindableObject view, IEnumerable<View> value)
            => view.SetValue(ChildrenSourceProperty, value);

        // This method is automatically called whenever the ChildrenSource property changes.
        private static void OnChildrenSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Ensure the object this property is attached to is actually a Grid.
            if (bindable is not Grid grid)
                return;

            // Clear any existing children in the Grid.
            // This ensures that old BoxViews are removed before adding the new ones.
            grid.Children.Clear();

            // If the new value is a collection of Views (BoxViews in our case)
            if (newValue is IEnumerable<View> views)
            {
                int r = 0, c = 0;

                // Loop through each BoxView in the flat collection
                foreach (View v in views)
                {
                    // Set the row and column for the BoxView in the grid
                    Grid.SetRow(v, r);
                    Grid.SetColumn(v, c);

                    // Add the BoxView to the grid
                    grid.Children.Add(v);

                    // Move to the next column
                    c++;

                    // If we reached the last column, reset column to 0 and move to the next row
                    if (c >= grid.ColumnDefinitions.Count)
                    {
                        c = 0;
                        r++;
                    }
                }
            }
        }
    }
}
