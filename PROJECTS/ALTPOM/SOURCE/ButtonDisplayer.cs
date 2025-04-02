public class ButtonDisplayer<T>
{
    public string PropertyToDisplay { get; set; }
    public T ObjectData { get; set; }

    public ButtonDisplayer(T objectData, string propertyName)
    {
        this.ObjectData = objectData;

        var property = typeof(T).GetProperty(propertyName);
        if (property == null)
        {
            throw new ArgumentException($"ButtonDisplayer.cs Property '{propertyName}' does not exist, type '{typeof(T).Name}");
        }

        this.PropertyToDisplay = property.GetValue(objectData)?.ToString();
    }
}