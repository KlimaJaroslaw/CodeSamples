private class CodeSample
{
    public string Code { get; set; }
    public string SourceFile { get; set; }
    public string ProjectName { get; set; }
}

private List<CodeSample> GetCodeSamples()
{
    List<CodeSample> result = new List<CodeSample>();
    result.Add(new CodeSample() { Code = "Production Order UI", ProjectName = "ALTPOM", SourceFile = "ProductionOrder.html" });
    result.Add(new CodeSample() { Code = "Dynamic Button Form", ProjectName = "ALTPOM", SourceFile = "DynamicButtonForm.cs" });
    result.Add(new CodeSample() { Code = "Cooldowner", ProjectName = "ShipIt", SourceFile = "Cooldowner.cs" });
    result.Add(new CodeSample() { Code = "Asset Access", ProjectName = "ShipIt", SourceFile = "AssetAccess.cs" });
    result.Add(new CodeSample() { Code = "Abilities", ProjectName = "ShipIt", SourceFile = "AbilityMate.cs" });
    result.Add(new CodeSample() { Code = "Barcode Listener", ProjectName = "IMS20", SourceFile = "BarcodeListener.cs" });
    result.Add(new CodeSample() { Code = "APIHandler", ProjectName = "ASC3", SourceFile = "APIHandler.cs" });
    return result;
}

private void InitializeDBF()
{
    List<CodeSample> codeSamples = GetCodeSamples();
    DynamicButtonForm dbf = new DynamicButtonForm();
    dbf.Initilize<CodeSample>(codeSamples, nameof(CodeSample.Code), "Choose: Code Sample", itemWidth_: 300);
    if (dbf.ShowDialog() == DialogResult.OK)
    {
        CodeSample code = dbf.GetChoosen<CodeSample>();
        if (code != null)
        {
            Console.WriteLine($"{code?.Code} from {code?.ProjectName} in {code?.SourceFile}");
        }
    }
}